Data Access Layer for .Net
========================================

Description
-------------
Data Access Layer facilitates access to the database in a performant and simplified way, with simple call construction, using dependency injection.

Packages DataAccessLayer.SqlServerAdapter
--------

Package Purposes:
* ExecuteNonQuery, ExecuteNonQueryAsync, ExecuteNonQueryTrans and ExecuteNonQueryTransAsync
  * Execution without returning information used for Update and Delete.
* ExecuteScalar, ExecuteScalarAsync, ExecuteScalarTrans and ExecuteScalarTransAsync
  * Execution with return of a single piece of information, used for Insert with return of the ID.
* ExecuteReader, ExecuteReaderAsync, ExecuteReaderTrans and ExecuteReaderTransAsync
  * Execution with multiple data return, used to return a Select.
 
Library methods
--------
* Non-asynchronous method
  * No transaction
    * ExecuteNonQuery
      * int ExecuteNonQuery(CommandType commandType, string commandText)
      * int ExecuteNonQuery(CommandType commandType, string commandText, object objectValue)
      * int ExecuteNonQuery(string spName, object objectValue)
    * ExecuteScalar
      * object ExecuteScalar(CommandType commandType, string commandText)
      * object ExecuteScalar(CommandType commandType, string commandText, object objectValue)
      * object ExecuteScalar(string spName, object objectValue)
    * ExecuteReader
      * List<T> ExecuteReader<T>(CommandType commandType, string commandText)
      * List<T> ExecuteReader<T>(CommandType commandType, string commandText, object objectValue)
      * List<T> ExecuteReader<T>(string spName, object objectValue)
  * With transaction
    * ExecuteNonQuery
      * int ExecuteNonQueryTrans(CommandType commandType, string commandText)
      * int ExecuteNonQueryTrans(CommandType commandType, string commandText, object objectValue)
      * int ExecuteNonQueryTrans(string spName, object objectValue)
    * ExecuteScalar
      * object ExecuteScalarTrans(CommandType commandType, string commandText)
      * object ExecuteScalarTrans(CommandType commandType, string commandText, object objectValue)
      * object ExecuteScalarTrans(string spName, object objectValue)
    * ExecuteReader
      * List<T> ExecuteReaderTrans<T>(CommandType commandType, string commandText)
      * List<T> ExecuteReaderTrans<T>(CommandType commandType, string commandText, object objectValue)
      * List<T> ExecuteReaderTrans<T>(string spName, object objectValue)
* Asynchronous methods
  * No transaction
    * ExecuteNonQuery
      * int ExecuteNonQueryAsync(CommandType commandType, string commandText)
      * int ExecuteNonQueryAsync(CommandType commandType, string commandText, object objectValue)
      * int ExecuteNonQueryAsync(string spName, object objectValue)
    * ExecuteScalar
      * object ExecuteScalarAsync(CommandType commandType, string commandText)
      * object ExecuteScalarAsync(CommandType commandType, string commandText, object objectValue)
      * object ExecuteScalarAsync(string spName, object objectValue)
    * ExecuteReader
      * List<T> ExecuteReaderAsync<T>(CommandType commandType, string commandText)
      * List<T> ExecuteReaderAsync<T>(CommandType commandType, string commandText, object objectValue)
      * List<T> ExecuteReaderAsync<T>(string spName, object objectValue)
  * With transaction
    * ExecuteNonQuery
      * int ExecuteNonQueryTransAsync(CommandType commandType, string commandText)
      * int ExecuteNonQueryTransAsync(CommandType commandType, string commandText, object objectValue)
      * int ExecuteNonQueryTransAsync(string spName, object objectValue)
    * ExecuteScalar
      * object ExecuteScalarTransAsync(CommandType commandType, string commandText)
      * object ExecuteScalarTransAsync(CommandType commandType, string commandText, object objectValue)
      * object ExecuteScalarTransAsync(string spName, object objectValue)
    * ExecuteReader
      * List<T> ExecuteReaderTransAsync<T>(CommandType commandType, string commandText)
      * List<T> ExecuteReaderTransAsync<T>(CommandType commandType, string commandText, object objectValue)
      * List<T> ExecuteReaderTransAsync<T>(string spName, object objectValue)

Features
--------
Data Access Layer is a NuGet library that you can add in to your project that will enhance your ADO.NET.

``` csharp
const string _connectionstring = "";
var services = new ServiceCollection();
services.AddScoped<ISQLServerAdapter>(p => { return new SQLServerAdapter(_connectionstring); });
var _ISQLServerAdapter = services.BuildServiceProvider().GetService<ISQLServerAdapter>();
_ISQLServerAdapter.Open();
var list = _ISQLServerAdapter.ExecuteReader<User>("StorageProcedureUserSelect", new User() { Id = 1 });
_ISQLServerAdapter.Close();
```

Sample code
-------------------------------------------------------

Procedure:

``` sql
create procedure spUserSelect
(
	  @Id			bigint		= null
	, @Email		varchar(200)	= null
)
as
begin
	set nocount on
	select 	 Id
		, Email
		 , Senha
		 , ChaveSenha
	from 
		User with(nolock) 
	where
		(@Id is null or Id=@Id)
	and
		(@Email is null or Email = @Email)
	order by
		  Email
	asc
end
```
C Sharp
``` csharp
public class User
{
        public Int64? Id { get; set; }
        public string Email { get; set; }
	public string Senha { get; set; }
}

const string _connectionstring = "";
var services = new ServiceCollection();
services.AddScoped<ISQLServerAdapter>(p => { return new SQLServerAdapter(_connectionstring); });
var _ISQLServerAdapter = services.BuildServiceProvider().GetService<ISQLServerAdapter>();
_ISQLServerAdapter.Open();
var list = _ISQLServerAdapter.ExecuteReader<User>("spUserSelect", new User() { Id = 1 });
_ISQLServerAdapter.Close();
```

