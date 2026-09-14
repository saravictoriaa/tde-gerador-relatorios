using TdeGeradorRelatorios.Geradores;
using TdeGeradorRelatorios.Interfaces;

namespace TdeGeradorRelatorios.Factories;

public class JsonCreator : RelatorioCreator
{
    public override IGeradorRelatorio CriarGerador()
    {
        return new GeradorJson();
    }
}