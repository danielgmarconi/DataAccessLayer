Data Access Layer for .Net
========================================

Description
-------------
Data Access Layer facilitates access to the database in a performant and simplified way, with simple call construction, using dependency injection.

Packages DataAccessLayer.SqlServerAdapter
--------

Package Purposes:
* ExecuteNonQuery
  * Execution without returning information used for Update and Delete.
* ExecuteScalar
  * Execution with return of a single piece of information, used for Insert with return of the ID.
* ExecuteReader
  * Execution with multiple data return, used to return a Select.

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
var list = _ISQLServerAdapter.ExecuteReader<User>("StorageProcedureUserSelect", new User() { Id = 1 });
_ISQLServerAdapter.Close();
```

