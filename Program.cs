using System;
using System.IO;
using TdeGeradorRelatorios.Models;
using TdeGeradorRelatorios.Factories;

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

        RelatorioCreator creator;

        switch (opcao)
        {
            case "1":
                    Console.WriteLine();
                    Console.WriteLine("Gerando PDF...");
                    creator = new PdfCreator();
                    break;

            case "2":
            {
                    Console.WriteLine();
                    Console.WriteLine("Gerando CSV...");
                    creator = new CsvCreator();
                    break;
            }

            case "3":
            {
                    Console.WriteLine();
                    Console.WriteLine("Gerando JSON...");
                    creator = new JsonCreator();
                    break;
            }

            default:
                throw new InvalidOperationException("Opção inválida.");

        }

        string extensao = opcao switch
        {
            "1" => "pdf",
            "2" => "csv",
            "3" => "json",
            _ => throw new InvalidOperationException()
        };

        string caminhoDestino = Path.Combine("output", $"relatorio.{extensao}");

        creator.Gerar(relatorio, caminhoDestino);

        Console.WriteLine($"Relatório \"{titulo}\" gerado com sucesso.");

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