using TdeGeradorRelatorios.Geradores;
using TdeGeradorRelatorios.Interfaces;

namespace TdeGeradorRelatorios.Factories;

public class PdfCreator : RelatorioCreator
{
    public override IGeradorRelatorio CriarGerador()
    {
        return new GeradorPdf();
    }
}