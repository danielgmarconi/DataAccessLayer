using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DataAccessLayer.SqlServerAdapter;

public partial class SQLServerAdapter : ISQLServerAdapter
{
    private readonly string connectionstring;
    private SqlConnection connection;
    private SqlTransaction transaction;
    public SQLServerAdapter(string _connectionstring) 
    {
        connectionstring = _connectionstring;
    }
    public void Open() 
    {
        if (connection == null)
            connection = new SqlConnection(connectionstring);
        if (connection != null && connection.State != System.Data.ConnectionState.Open)
        {
            connection.Close();
            connection.Open();
        }
    }
    public void BeginTransaction() => transaction = connection.BeginTransaction();
    public void CommitTransaction() => transaction.Commit();
    public void RollbackTransaction() => transaction.Rollback();
    public void Close() => connection.Close();
    private void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
    {
        if (command == null) throw new ArgumentNullException("command");
        if (commandParameters != null)
            foreach (SqlParameter p in commandParameters)
                if (p != null)
                {
                    if ((p.Direction == ParameterDirection.InputOutput ||
                        p.Direction == ParameterDirection.Input) &&
                        (p.Value == null))
                    {
                        p.Value = DBNull.Value;
                    }
                    command.Parameters.Add(p);
                }
    }
    private SqlParameter[] AssignParameterValues(object objectValue)
    {
        if (objectValue == null)
            return new SqlParameter[0];

        SqlParameter[] param;
        int numParam = 0, posParam = 0;
        PropertyInfo[] propr = objectValue.GetType().GetProperties();

        for (int a = 0; a < propr.Length; a++)
            if ((objectValue.GetType().GetProperty(propr[a].Name).GetValue(objectValue, null) != null) && !propr[a].PropertyType.Namespace.Equals("System.Collections.Generic"))
                numParam++;

        if (numParam.Equals(0))
            return new SqlParameter[0];

        param = new SqlParameter[numParam];

        for (int a = 0; a < propr.Length; a++)
        {
            object objeto = objectValue.GetType().GetProperty(propr[a].Name).GetValue(objectValue, null);

            if ((objeto != null) && !propr[a].PropertyType.Namespace.Equals("System.Collections.Generic"))
            {
                param[posParam] = new SqlParameter("@" + propr[a].Name, objeto);
                posParam++;
            }
        }
        return param;
    }
    private void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
    {
        if (command == null) throw new ArgumentNullException("command");
        if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");
        if (connection.State != ConnectionState.Open)
        {
            mustCloseConnection = true;
            connection.Open();
        }
        else
        {
            mustCloseConnection = false;
        }

        command.Connection = connection;
        command.CommandText = commandText;
        command.CommandTimeout = connection.ConnectionTimeout;
        if (transaction != null)
        {
            if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            command.Transaction = transaction;
        }
        command.CommandType = commandType;
        if (commandParameters != null)
        {
            AttachParameters(command, commandParameters);
        }
        return;
    }
    private static T SetObject<T>(SqlDataReader dr) where T : new()
    {
        T item = new T();
        var Properties = item.GetType().GetProperties();
        for (int a = 0; a < dr.VisibleFieldCount; a++)
        {
            if (!dr.IsDBNull(a))
            {
                var result = (from i in Properties where i.Name.ToLower() == dr.GetName(a).ToLower() select i.Name).ToList();
                if (result.Count != 0)
                    item.GetType().GetProperty(result[0]).SetValue(item, dr[a]);
            }
        }
        return item;
    }
}
