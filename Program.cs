using System;
using System.IO;
using System.Text.Json;
using TdeGeradorRelatorios.Models;
using TdeGeradorRelatorios.Geradores;

namespace TdeGeradorRelatorios;

class Program
{
    static void Main()
    {

        Console.WriteLine("===== GERADOR DE RELATÓRIOS =====");
        Console.WriteLine();

        string titulo = LerCampoObrigatorio("Título");
        string responsavel = LerCampoObrigatorio("Responsável");
        string descricao = LerCampoObrigatorio("Descrição");

        DateTime data = DateTime.Now;

        var relatorio = new Relatorio(
            titulo,
            responsavel,
            descricao,
            data
        );

        Console.WriteLine($"Data de geração: {data:dd/MM/yyyy HH:mm}");

        string opcao;

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Escolha o formato:");
            Console.WriteLine("1 - PDF");
            Console.WriteLine("2 - CSV");
            Console.WriteLine("3 - JSON");
            Console.Write("Opção: ");

            opcao = Console.ReadLine() ?? "";

            if (opcao == "1" || opcao == "2" || opcao == "3")
            {
                break;
            }

            Console.Clear();
            Console.WriteLine("===== GERADOR DE RELATÓRIOS =====");
            Console.WriteLine();
            Console.WriteLine("Opção inválida. Escolha 1, 2 ou 3.");
        }

        Directory.CreateDirectory("output");

        switch (opcao)
        {
            case "1":
                Console.WriteLine();
                Console.WriteLine("Gerando PDF...");

                string caminhoPdf = Path.Combine("output", "relatorio.pdf");

                var geradorPdf = new GeradorPdf();

                geradorPdf.Gerar(relatorio, caminhoPdf);

                Console.WriteLine(
                    $"Relatório \"{titulo}\" gerado em PDF com sucesso."
                );

                break;

            case "2":
                Console.WriteLine();
                Console.WriteLine("Gerando CSV...");

                string caminhoCsv = Path.Combine("output", "relatorio.csv");

                var geradorCsv = new GeradorCsv();

                geradorCsv.Gerar(relatorio, caminhoCsv);

                Console.WriteLine("Relatório \"" + titulo + "\" gerado em CSV com sucesso.");
                break;

            case "3":
                Console.WriteLine();
                Console.WriteLine("Gerando JSON...");

                var dadosJson = new
                {
                    titulo = titulo,
                    responsavel = responsavel,
                    descricao = descricao,
                    data = data.ToString("dd/MM/yyyy HH:mm")
                };

                string conteudoJson = JsonSerializer.Serialize(dadosJson,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    }
                 );

                File.WriteAllText(
                    Path.Combine("output", "relatorio.json"),
                    conteudoJson
                );

                Console.WriteLine("Relatório \"" + titulo + "\" gerado em JSON com sucesso.");
                break;

        }
    }

    static string LerCampoObrigatorio(string nomeCampo)
    {
        while (true)
        {
            Console.Write($"{nomeCampo}: ");
            string valor = Console.ReadLine() ?? "";

            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor;
            }

            Console.WriteLine($"{nomeCampo} é obrigatório.");
        }
    }
}