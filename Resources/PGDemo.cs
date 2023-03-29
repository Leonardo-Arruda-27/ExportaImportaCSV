using Npgsql;
using ProjetoLerCsv.Models;

namespace ProjetoLerCsv.Resources
{
    internal class PGDemo
    {
        public static void ConnectPostgress(List<PersonModel> personModels)
        {
            var cs = "Server=localhost;Port=5432;User Id=postgres;Password=12345;Database=Pessoa";

            using var con = new NpgsqlConnection(cs);
            con.Open();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            Console.WriteLine("Conectado\n");

            foreach (var personModel in personModels)
            {
                cmd.CommandText = $"INSERT INTO cliente(nome,cpf,idade) VALUES('{personModel.Name}','{personModel.Document}',{personModel.Idade})";
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine($"Total de dados inseridos: {personModels.Count}\n");
        }

    }
}
