using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DataAccessLayer.SqlServerAdapter;

public class SQLServerAdapter : ISQLServerAdapter
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

    public int ExecuteNonQuery(CommandType commandType, string commandText) => ExecuteNonQuery(commandType, commandText, null);
    public int ExecuteNonQuery(CommandType commandType, string commandText, object objectValue)
    {
        if (connection == null) throw new ArgumentNullException("connection");
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, connection, null, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
        int retval = cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
        if (mustCloseConnection)
            connection.Close();
        return retval;
    }
    public int ExecuteNonQuery(string spName, object objectValue)
    {
        if (connection == null) throw new ArgumentNullException("connection");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, spName);
        }
    }

    public int ExecuteNonQueryTrans(CommandType commandType, string commandText) => ExecuteNonQueryTrans(commandType, commandText, null);
    public int ExecuteNonQueryTrans(CommandType commandType, string commandText, object objectValue)
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
        int retval = cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
        return retval;
    }
    public int ExecuteNonQueryTrans(string spName, object objectValue)
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return ExecuteNonQueryTrans(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return ExecuteNonQueryTrans(CommandType.StoredProcedure, spName);
        }
    }

    public object ExecuteScalar(CommandType commandType, string commandText) => ExecuteScalar(commandType, commandText, null);
    public object ExecuteScalar(CommandType commandType, string commandText, object objectValue)
    {
        if (connection == null) throw new ArgumentNullException("connection");
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, connection, null, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
        object retval = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        if (mustCloseConnection)
            connection.Close();
        return retval;
    }
    public object ExecuteScalar(string spName, object objectValue)
    {
        if (connection == null) throw new ArgumentNullException("connection");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return ExecuteScalar(CommandType.StoredProcedure, spName, AssignParameterValues(objectValue));
        }
        else
        {
            return ExecuteScalar(CommandType.StoredProcedure, spName);
        }
    }

    public object ExecuteScalarTrans(CommandType commandType, string commandText) => ExecuteScalarTrans(commandType, commandText, null);
    public object ExecuteScalarTrans(CommandType commandType, string commandText, object objectValue)
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
        object retval = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        return retval;
    }
    public object ExecuteScalarTrans(string spName, object objectValue)
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return ExecuteScalarTrans(CommandType.StoredProcedure, spName, AssignParameterValues(objectValue));
        }
        else
        {
            return ExecuteScalarTrans(CommandType.StoredProcedure, spName);
        }
    }

    private List<T> ExecuteReader<T>(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, object objectValue) where T : new()
    {
        List<T> ret = new List<T>();
        if (connection == null) throw new ArgumentNullException("connection");
        bool mustCloseConnection = false;
        SqlCommand cmd = new SqlCommand();
        try
        {
            PrepareCommand(cmd, connection, transaction, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
            SqlDataReader dataReader;
            dataReader = cmd.ExecuteReader();
            bool canClear = true;
            foreach (SqlParameter commandParameter in cmd.Parameters)
                if (commandParameter.Direction != ParameterDirection.Input)
                    canClear = false;
            if (canClear)
                cmd.Parameters.Clear();
            while (dataReader.Read())
            {
                ret.Add(SetObject<T>(dataReader));
            }
            dataReader.Close();
            return ret;
        }
        catch
        {
            if (mustCloseConnection)
                connection.Close();
            throw;
        }
    }
    public List<T> ExecuteReader<T>(CommandType commandType, string commandText) where T : new() => ExecuteReader<T>(commandType, commandText, null);
    public List<T> ExecuteReader<T>(CommandType commandType, string commandText, object objectValue) where T : new()
    {
        if (connection == null) throw new ArgumentNullException("connection");
        return ExecuteReader<T>(connection, null, commandType, commandText, objectValue);
    }
    public List<T> ExecuteReader<T>(string spName, object objectValue) where T : new()
    {
        if (connection == null) throw new ArgumentNullException("connection");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return ExecuteReader<T>(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return ExecuteReader<T>(CommandType.StoredProcedure, spName);
        }
    }
    public List<T> ExecuteReaderTrans<T>(CommandType commandType, string commandText) where T : new() => ExecuteReaderTrans<T>(commandType, commandText, null);
    public List<T> ExecuteReaderTrans<T>(CommandType commandType, string commandText, object objectValue) where T : new()
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        return ExecuteReader<T>(transaction.Connection, transaction, commandType, commandText, objectValue);
    }
    public List<T> ExecuteReaderTrans<T>(string spName, object objectValue) where T : new()
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {

            return ExecuteReaderTrans<T>(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return ExecuteReaderTrans<T>(CommandType.StoredProcedure, spName);
        }
    }


    private async Task<List<T>> ExecuteReaderAsync<T>(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, object objectValue) where T : new()
    {
        List<T> ret = new List<T>();
        if (connection == null) throw new ArgumentNullException("connection");
        bool mustCloseConnection = false;
        SqlCommand cmd = new SqlCommand();
        try
        {
            PrepareCommand(cmd, connection, transaction, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
            using (SqlDataReader dataReader = await cmd.ExecuteReaderAsync())
            {
                bool canClear = true;
                foreach (SqlParameter commandParameter in cmd.Parameters)
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                if (canClear)
                    cmd.Parameters.Clear();
                while (await dataReader.ReadAsync())
                {
                    ret.Add(SetObject<T>(dataReader));
                }
            }         
        }
        catch
        {
            if (mustCloseConnection)
                connection.Close();
            throw;
        }
        return ret;
    }
    public Task<List<T>> ExecuteReaderAsync<T>(CommandType commandType, string commandText) where T : new() => ExecuteReaderAsync<T>(commandType, commandText, null);
    public Task<List<T>> ExecuteReaderAsync<T>(CommandType commandType, string commandText, object objectValue) where T : new()
    {
        if (connection == null) throw new ArgumentNullException("connection");
        return ExecuteReaderAsync<T>(connection, null, commandType, commandText, objectValue);
    }
    public Task<List<T>> ExecuteReaderAsync<T>(string spName, object objectValue) where T : new()
    {
        if (connection == null) throw new ArgumentNullException("connection");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return ExecuteReaderAsync<T>(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return ExecuteReaderAsync<T>(CommandType.StoredProcedure, spName);
        }
    }
    public Task<List<T>> ExecuteReaderTransAsync<T>(CommandType commandType, string commandText) where T : new() => ExecuteReaderTransAsync<T>(commandType, commandText, null);
    public Task<List<T>> ExecuteReaderTransAsync<T>(CommandType commandType, string commandText, object objectValue) where T : new()
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        return ExecuteReaderAsync<T>(transaction.Connection, transaction, commandType, commandText, objectValue);
    }
    public Task<List<T>> ExecuteReaderTransAsync<T>(string spName, object objectValue) where T : new()
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {

            return ExecuteReaderTransAsync<T>(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return ExecuteReaderTransAsync<T>(CommandType.StoredProcedure, spName);
        }
    }
}
