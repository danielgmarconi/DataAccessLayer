using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DataAccessLayer.SqlServerAdapter;

public partial class SQLServerAdapter : ISQLServerAdapter
{
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
