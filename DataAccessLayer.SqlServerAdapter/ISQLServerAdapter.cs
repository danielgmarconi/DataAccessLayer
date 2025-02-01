using System.Data;

namespace DataAccessLayer.SqlServerAdapter;

public interface ISQLServerAdapter
{
    void Open();
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
    void Close();
    int ExecuteNonQuery(CommandType commandType, string commandText);
    int ExecuteNonQuery(CommandType commandType, string commandText, object objectValue);
    int ExecuteNonQuery(string spName, object objectValue);
    int ExecuteNonQueryTrans(CommandType commandType, string commandText);
    int ExecuteNonQueryTrans(CommandType commandType, string commandText, object objectValue);
    int ExecuteNonQueryTrans(string spName, object objectValue);
    object ExecuteScalar(CommandType commandType, string commandText);
    object ExecuteScalar(CommandType commandType, string commandText, object objectValue);
    object ExecuteScalar(string spName, object objectValue);
    object ExecuteScalarTrans(CommandType commandType, string commandText);
    object ExecuteScalarTrans(CommandType commandType, string commandText, object objectValue);
    object ExecuteScalarTrans(string spName, object objectValue);
    public List<T> ExecuteReader<T>(CommandType commandType, string commandText) where T : new();
    public List<T> ExecuteReader<T>(CommandType commandType, string commandText, object valor) where T : new();
    public List<T> ExecuteReader<T>(string spName, object valor) where T : new();
    public List<T> ExecuteReaderTrans<T>(CommandType commandType, string commandText) where T : new();
    public List<T> ExecuteReaderTrans<T>(CommandType commandType, string commandText, object valor) where T : new();
    public List<T> ExecuteReaderTrans<T>(string spName, object valor) where T : new();
}
