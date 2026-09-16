using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Interfaces;

public interface IGeradorRelatorio
{
    void Gerar(Relatorio relatorio, string caminhoDestino);
}
