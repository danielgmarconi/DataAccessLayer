using ConsoleTeste;
using DataAccessLayer.SqlServerAdapter;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

const string _connectionstring = "Data Source=localhost;Initial Catalog=TestePratico; User Id=sa;Password=;Connect Timeout=120;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true";
var services = new ServiceCollection();
services.AddScoped<ISQLServerAdapter>(p => { return new SQLServerAdapter(_connectionstring); });

var v = services.BuildServiceProvider().GetService<ISQLServerAdapter>();


Usuario obj = new Usuario();
obj.Email = "danile1@xpto.com";
obj.Senha = "xxxxx1";
obj.ChaveSenha = "bla1";
v.Open();
v.BeginTransaction();
v.ExecuteNonQueryTrans("spUsuarioInsert", obj);
var list = v.ExecuteReaderTrans<Usuario>("spUsuarioSelect", new Usuario() { Id = 10012 });
v.ExecuteNonQueryTrans(CommandType.Text, "update Usuario set Email = 'a12@a.com', Senha = 'a12', ChaveSenha = 'b12' where id = 10012");
v.CommitTransaction();
v.Close();

//v.ExecuteNonQuery("spUsuarioInsert", obj);