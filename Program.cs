using System;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main()
    {
        Console.WriteLine("===== GERADOR DE RELATÓRIOS =====");
        Console.WriteLine();

        Console.Write("Título: ");
        string titulo = Console.ReadLine() ?? "";

        Console.Write("Responsável: ");
        string responsavel = Console.ReadLine() ?? "";

        Console.Write("Descrição: ");
        string descricao = Console.ReadLine() ?? "";

        Console.Write("Data de geração: ");
        string data = Console.ReadLine() ?? "";

        Console.WriteLine();
        Console.WriteLine("Escolha o formato:");
        Console.WriteLine("1 - PDF");
        Console.WriteLine("2 - CSV");
        Console.WriteLine("3 - JSON");
        Console.Write("Opção: ");

        string opcao = Console.ReadLine() ?? "";

        switch (opcao)
        {
            case "1":
                Console.WriteLine();
                Console.WriteLine("Gerando PDF...");

                string conteudoPdf =
                    "RELATÓRIO\n\n" +
                    "Título: " + titulo + "\n" +
                    "Responsável: " + responsavel + "\n" +
                    "Descrição: " + descricao + "\n" +
                    "Data: " + data;

                File.WriteAllText("relatorio.pdf", conteudoPdf);

                Console.WriteLine("Relatório \"" + titulo + "\" gerado em PDF com sucesso.");
                break;

            case "2":
                Console.WriteLine();
                Console.WriteLine("Gerando CSV...");

                string conteudoCsv =
                    "Título,Responsável,Descrição,Data\n" +
                    "\"" + titulo + "\",\"" + responsavel + "\",\"" + descricao + "\",\"" + data + "\"";

                File.WriteAllText("relatorio.csv", conteudoCsv);

                Console.WriteLine("Relatório \"" + titulo + "\" gerado em CSV com sucesso.");
                break;

            case "3":
                Console.WriteLine();
                Console.WriteLine("Gerando JSON...");

                var relatorio = new
                {
                    titulo = titulo,
                    responsavel = responsavel,
                    descricao = descricao,
                    data = data
                };

                string conteudoJson = JsonSerializer.Serialize(
                    relatorio,
                    new JsonSerializerOptions { WriteIndented = true }
                );

                File.WriteAllText("relatorio.json", conteudoJson);

                Console.WriteLine("Relatório \"" + titulo + "\" gerado em JSON com sucesso.");
                break;

            default:
                Console.WriteLine();
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
}