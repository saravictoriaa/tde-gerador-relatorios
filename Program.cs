using System;
using System.IO;
using System.Text.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;

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

        Directory.CreateDirectory("output");

        switch (opcao)
        {
            case "1":
                Console.WriteLine();
                Console.WriteLine("Gerando PDF...");

                GlobalFontSettings.FontResolver = new FontResolver();

                var documento = new PdfDocument();
                documento.Info.Title = titulo;

                var pagina = documento.AddPage();
                var grafico = XGraphics.FromPdfPage(pagina);

                var fonteTitulo = new XFont("Arial", 20, XFontStyle.Bold);
                var fonteTexto = new XFont("Arial", 12, XFontStyle.Regular);

                grafico.DrawString(
                    titulo,
                    fonteTitulo,
                    XBrushes.Black,
                    new XRect(40, 40, pagina.Width - 80, 40),
                    XStringFormats.TopLeft
                );

                grafico.DrawString(
                    "Responsável: " + responsavel,
                    fonteTexto,
                    XBrushes.Black,
                    new XRect(40, 100, pagina.Width - 80, 30),
                    XStringFormats.TopLeft
                );

                grafico.DrawString(
                    "Descrição: " + descricao,
                    fonteTexto,
                    XBrushes.Black,
                    new XRect(40, 140, pagina.Width - 80, 60),
                    XStringFormats.TopLeft
                );

                grafico.DrawString(
                    "Data: " + data,
                    fonteTexto,
                    XBrushes.Black,
                    new XRect(40, 220, pagina.Width - 80, 30),
                    XStringFormats.TopLeft
                );

                string caminhoPdf = Path.Combine("output", "relatorio.pdf");

                documento.Save(caminhoPdf);

                Console.WriteLine("Relatório \"" + titulo + "\" gerado em PDF com sucesso.");
                break;

            case "2":
                Console.WriteLine();
                Console.WriteLine("Gerando CSV...");

                string conteudoCsv =
                    "Título,Responsável,Descrição,Data\n" +
                    "\"" + titulo + "\",\"" + responsavel + "\",\"" + descricao + "\",\"" + data + "\"";

                File.WriteAllText(
                    Path.Combine("output", "relatorio.csv"),
                    conteudoCsv
                );

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

                File.WriteAllText(
                    Path.Combine("output", "relatorio.json"),
                    conteudoJson
                );

                Console.WriteLine("Relatório \"" + titulo + "\" gerado em JSON com sucesso.");
                break;

            default:
                Console.WriteLine();
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
}