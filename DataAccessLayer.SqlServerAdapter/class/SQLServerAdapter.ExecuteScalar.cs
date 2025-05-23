using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccessLayer.SqlServerAdapter;

public partial class SQLServerAdapter : ISQLServerAdapter
{
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
            return ExecuteScalar(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return ExecuteScalar(CommandType.StoredProcedure, spName);
        }
    }

    public async Task<object> ExecuteScalarAsync(CommandType commandType, string commandText) => await ExecuteScalarAsync(commandType, commandText, null);
    public async Task<object> ExecuteScalarAsync(CommandType commandType, string commandText, object objectValue)
    {
        if (connection == null) throw new ArgumentNullException("connection");
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, connection, null, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
        object retval = await cmd.ExecuteScalarAsync();
        cmd.Parameters.Clear();
        if (mustCloseConnection)
            connection.Close();
        return retval;
    }
    public async Task<object> ExecuteScalarAsync(string spName, object objectValue)
    {
        if (connection == null) throw new ArgumentNullException("connection");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return await ExecuteScalarAsync(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return await ExecuteScalarAsync(CommandType.StoredProcedure, spName);
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
            return ExecuteScalarTrans(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return ExecuteScalarTrans(CommandType.StoredProcedure, spName);
        }
    }

    public async Task<object> ExecuteScalarTransAsync(CommandType commandType, string commandText) => await ExecuteScalarTransAsync(commandType, commandText, null);
    public async Task<object> ExecuteScalarTransAsync(CommandType commandType, string commandText, object objectValue)
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        SqlCommand cmd = new SqlCommand();
        bool mustCloseConnection = false;
        PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, AssignParameterValues(objectValue), out mustCloseConnection);
        object retval = await cmd.ExecuteScalarAsync();
        cmd.Parameters.Clear();
        return retval;
    }
    public async Task<object> ExecuteScalarTransAsync(string spName, object objectValue)
    {
        if (transaction == null) throw new ArgumentNullException("transaction");
        if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
        if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
        if (objectValue != null)
        {
            return await ExecuteScalarTransAsync(CommandType.StoredProcedure, spName, objectValue);
        }
        else
        {
            return await ExecuteScalarTransAsync(CommandType.StoredProcedure, spName);
        }
    }
}