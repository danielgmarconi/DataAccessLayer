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
User _user = new User();
_user.Email = "user@teste.com";
_user.Senha = "test123";
_ISQLServerAdapter.Open();
var list = _ISQLServerAdapter.ExecuteReader<User>("StorageProcedureUserSelect", new User() { Id = 1 });
_ISQLServerAdapter.Close();
```

Execute a query and map it to a list of typed objects
-------------------------------------------------------

``` sql
create procedure [dbo].[spUserSelect]
(
	  @Id			bigint			= null
	, @Email		varchar(200)	= null
)
as
begin
	set nocount on
	select Id
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

``` csharp
public class Dog
{
    public int? Age { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public float? Weight { get; set; }

    public int IgnoredProperty { get { return 1; } }
}

var guid = Guid.NewGuid();
var dog = connection.Query<Dog>("select Age = @Age, Id = @Id", new { Age = (int?)null, Id = guid });

Assert.Equal(1,dog.Count());
Assert.Null(dog.First().Age);
Assert.Equal(guid, dog.First().Id);
```

Execute a query and map it to a list of dynamic objects
-------------------------------------------------------

This method will execute SQL and return a dynamic list.

Example usage:

```csharp
var rows = connection.Query("select 1 A, 2 B union all select 3, 4").AsList();

Assert.Equal(1, (int)rows[0].A);
Assert.Equal(2, (int)rows[0].B);
Assert.Equal(3, (int)rows[1].A);
Assert.Equal(4, (int)rows[1].B);
```

Execute a Command that returns no results
-----------------------------------------

Example usage:

```csharp
var count = connection.Execute(@"
  set nocount on
  create table #t(i int)
  set nocount off
  insert #t
  select @a a union all select @b
  set nocount on
  drop table #t", new {a=1, b=2 });
Assert.Equal(2, count);
```

Execute a Command multiple times
--------------------------------

The same signature also allows you to conveniently and efficiently execute a command multiple times (for example to bulk-load data)

Example usage:

```csharp
var count = connection.Execute(@"insert MyTable(colA, colB) values (@a, @b)",
    new[] { new { a=1, b=1 }, new { a=2, b=2 }, new { a=3, b=3 } }
  );
Assert.Equal(3, count); // 3 rows inserted: "1,1", "2,2" and "3,3"
```

Another example usage when you _already_ have an existing collection:
```csharp
var foos = new List<Foo>
{
    { new Foo { A = 1, B = 1 } }
    { new Foo { A = 2, B = 2 } }
    { new Foo { A = 3, B = 3 } }
};

var count = connection.Execute(@"insert MyTable(colA, colB) values (@a, @b)", foos);
Assert.Equal(foos.Count, count);
```

This works for any parameter that implements `IEnumerable<T>` for some T.
