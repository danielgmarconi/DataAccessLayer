namespace DataAccessLayer.SqlServerAdapter.Test
{
    using Microsoft.Extensions.DependencyInjection;
    using System.Data;

    public class DataAccessLayerTest
    {
        public class Usuario
        {
            public Int64? Id { get; set; }
            public string Email { get; set; }
            public string Senha { get; set; }
            public string ChaveSenha { get; set; }
        }
        private const string _connectionstring = "Data Source=localhost;Initial Catalog=TestePratico; User Id=sa;Password=;Connect Timeout=120;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true";
        [Fact]
        public void TransTeste()
        {
            var services = new ServiceCollection();

            services.AddScoped<ISQLServerAdapter>(p => { return new SQLServerAdapter(_connectionstring); });
           
            var v = services.BuildServiceProvider().GetService<ISQLServerAdapter>();


            Usuario obj = new Usuario();
            obj.Email = "danile@xpto.com";
            obj.Senha = "xxxxx";
            obj.ChaveSenha = "bla";
            v.Open();
            v.BeginTransaction();
            v.ExecuteNonQueryTrans("spUsuarioInsert", obj);
            v.ExecuteNonQueryTrans(CommandType.Text, "update Usuario set Email = 'a@a.com', Senha = 'a', ChaveSenha = 'b' where id = 10012");
            //var list = v.ExecuteReader<Usuario>("spUsuarioSelect", new Usuario() { Id = 10012 });
            v.CommitTransaction();
            v.Close();
            
            //v.ExecuteNonQuery("spUsuarioInsert", obj);

        }
    }
}