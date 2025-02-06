using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DataAccessLayer.SqlServerAdapter;

public partial class SQLServerAdapter : ISQLServerAdapter
{

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

    public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText) => await ExecuteNonQueryAsync(commandType, commandText, null);
    public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, object objectValue)
    {
        if (connection == null) throw new ArgumentNullException("connection");
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, connection, null, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
        int retval = await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();
        if (mustCloseConnection)
            connection.Close();
        return retval;
    }
    public async Task<int> ExecuteNonQueryAsync(string spName, object objectValue)
    {
        if (connection == null) throw new ArgumentNullException("connection");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return await ExecuteNonQueryAsync(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return await ExecuteNonQueryAsync(CommandType.StoredProcedure, spName);
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

    public async Task<int> ExecuteNonQueryTransAsync(CommandType commandType, string commandText) => await ExecuteNonQueryTransAsync(commandType, commandText, null);
    public async Task<int> ExecuteNonQueryTransAsync(CommandType commandType, string commandText, object objectValue)
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
        int retval = await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();
        return retval;
    }
    public async Task<int> ExecuteNonQueryTransAsync(string spName, object objectValue)
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return await ExecuteNonQueryTransAsync(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return await ExecuteNonQueryTransAsync(CommandType.StoredProcedure, spName);
        }
    }

}
