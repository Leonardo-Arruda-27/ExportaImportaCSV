using ProjetoLerCsv.Models;
using System.Text;
using ProjetoLerCsv.Resources;

namespace ProjetoLerCsv
{
    class Program
    {
        static void Main(string[] args)
        {
            int opcao;
            var peopleList = new List<PersonModel>();

            Console.WriteLine("Olá, Seja Bem Vindo ao Importa/Exporta CSV");

            do
            {
                Console.WriteLine("\n================================================");
                Console.WriteLine("\nEscolha uma opção: \n"
                    + "1 - Importar CSV e mostrar infos na tela\n"
                    + "2 - Inserir infos no banco de dados\n"
                    + "3 - Exportar CSV\n"
                    + "0 - Finalizar Sessão\n");
                opcao = int.Parse(Console.ReadLine());

                switch (opcao)
                {
                    case 1:
                        ImportCsv(peopleList);
                        break;
                    case 2:
                        SaveData(peopleList);
                        break;
                    case 3:
                        ExportCsv(peopleList, @"C:\Users\leo5d\Documents\Estudos\C#\Projetos\ProjetoLerCsv\", "pessoas");
                        break;
                    case 0:
                        Console.WriteLine("Finalizando programa.\nAté logo!");
                        break;
                    default:
                        Console.WriteLine("Opção invalida!");
                        break;
                }
            } while (opcao != 0);
        }

        //Mapeando as colunas do arquivo CSV exportado
        private static (int, int, int) SetColumnsIndex(string[] columns)
        {
            int indexName = -1;
            int indexDocument = -1;
            int indexIdade = -1;

            for (int i = 0; i < columns.Length; i++)
            {
                if (string.IsNullOrEmpty(columns[i]))
                    continue;

                if (columns[i].ToLower() == "nome")
                    indexName = i;

                if (columns[i].ToLower() == "cpf")
                    indexDocument = i;

                if (columns[i].ToLower() == "idade")
                    indexIdade = i;
            }
            return (indexName, indexDocument, indexIdade);
        }

        //Pegando os dados importados do CSV
        private static List<PersonModel> BuildPeopleList(StreamReader reader, int indexName, int indexDocument, int indexIdade)
        {
            string line;
            var people = new List<PersonModel>();
            PersonModel personModel;

            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(";");
                personModel = new PersonModel();

                if (indexName != -1)
                    personModel.Name = values[indexName];

                if (indexDocument != -1)
                    personModel.Document = values[indexDocument];

                if (indexIdade != -1)
                    personModel.Idade = int.Parse(values[indexIdade]);

                people.Add(personModel);
            }
            return people;
        }

        //Exporta o arquivo CSV
        private static void ExportCsv<T>(List<T> genericList, string basePath, string fileName)
        {
            if (genericList.Count != 0)
            {
                var sb = new StringBuilder();
                //var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var finalPath = Path.Combine(basePath, fileName + ".csv");
                var header = "";
                var info = typeof(T).GetProperties();

                if (!File.Exists(finalPath))
                {
                    var file = File.Create(finalPath);
                    file.Close();

                    foreach (var prop in typeof(T).GetProperties())
                    {
                        header += prop.Name + "; ";
                    }

                    header = header.Substring(0, header.Length - 2);
                    sb.AppendLine(header);
                    TextWriter sw = new StreamWriter(finalPath, true);
                    sw.Write(sb.ToString());
                    sw.Close();
                }

                foreach (var obj in genericList)
                {
                    sb = new StringBuilder();
                    var line = "";

                    foreach (var prop in info)
                    {
                        line += prop.GetValue(obj, null) + "; ";
                    }

                    line = line.Substring(0, line.Length - 2);
                    sb.AppendLine(line);
                    TextWriter sw = new StreamWriter(finalPath, true);
                    sw.Write(sb.ToString());
                    sw.Close();
                }
                Console.WriteLine($"O arquivo {fileName}.csv foi gerado com sucesso\n");
            }
            else
            {
                Console.WriteLine("Não foi possivél gerar o CSV por falta de dados!");
            }
        }

        private static void ImportCsv(List<PersonModel> peopleList)
        {
            Console.WriteLine("================== IMPORTA CSV =================\n");

            var path = @"C:\Users\leo5d\Documents\Estudos\C#\Projetos\ProjetoLerCsv\Cópia de pessoas.csv";
            var reader = new StreamReader(File.OpenRead(path));

            var line = reader.ReadLine();
            var columns = line.Split(';');
            (int indexName, int indexDocument, int indexIdade) = SetColumnsIndex(columns);
            var people = BuildPeopleList(reader, indexName, indexDocument, indexIdade);

            foreach (var person in people)
            {
                Console.WriteLine($"Nome: {person.Name} | CPF: {person.Document} | Idade:{person.Idade}");
                peopleList.Add(person);
            }
        }

        private static void SaveData(List<PersonModel> peopleList)
        {
            Console.WriteLine("===== MANDANDO AS INFOS PARA O BANCO DE DADOS =====\n");
            PGDemo.ConnectPostgress(peopleList);
        }
    }
}