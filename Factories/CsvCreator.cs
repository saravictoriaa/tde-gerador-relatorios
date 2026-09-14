using TdeGeradorRelatorios.Geradores;
using TdeGeradorRelatorios.Interfaces;

namespace TdeGeradorRelatorios.Factories;

public class CsvCreator : RelatorioCreator
{
    public override IGeradorRelatorio CriarGerador()
    {
        return new GeradorCsv();
    }
}