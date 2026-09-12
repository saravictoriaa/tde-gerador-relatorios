using System;
using System.IO;
using System.Text.Json;
using System.Globalization;
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

        string titulo = LerCampoObrigatorio("Título");
        string responsavel = LerCampoObrigatorio("Responsável");
        string descricao = LerCampoObrigatorio("Descrição");

        DateTime data = DateTime.Now;
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
                    "Título;Responsável;Descrição;Data\n" +
                    "\"" + titulo + "\";\"" +
                    responsavel + "\";\"" +
                    descricao + "\";\"" +
                    data.ToString("dd/MM/yyyy HH:mm:ss") + "\"";

                File.WriteAllText(
                    Path.Combine("output", "relatorio.csv"),
                    conteudoCsv,
                    new System.Text.UTF8Encoding(true)
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
                    data = data.ToString("dd/MM/yyyy HH:mm")
                };

                string conteudoJson = JsonSerializer.Serialize(relatorio, 
                    new JsonSerializerOptions{
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