using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TdeGeradorRelatorios.Interfaces;
using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Geradores;

public class GeradorPdf : IGeradorRelatorio
{
    public void Gerar(Relatorio relatorio, string caminhoDestino)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    column.Item()
                        .Text("RELATÓRIO")
                        .FontSize(20)
                        .Bold();

                    column.Item()
                        .Text($"Título: {relatorio.Titulo}");

                    column.Item()
                        .Text($"Responsável: {relatorio.Responsavel}");

                    column.Item()
                        .Text($"Descrição: {relatorio.Descricao}");

                    column.Item()
                        .Text($"Data: {relatorio.Data:dd/MM/yyyy HH:mm:ss}");
                });
            });
        })
        .GeneratePdf(caminhoDestino);
    }
}