using TdeGeradorRelatorios.Interfaces;
using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Factories;

public abstract class RelatorioCreator
{
    public abstract IGeradorRelatorio CriarGerador();

    public void Gerar(Relatorio relatorio, string caminhoDestino)
    {
        var gerador = CriarGerador();
        gerador.Gerar(relatorio, caminhoDestino);
    }
}