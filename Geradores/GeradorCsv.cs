using System.Text;
using TdeGeradorRelatorios.Interfaces;
using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Geradores;

public class GeradorCsv : IGeradorRelatorio
{
    public void Gerar(Relatorio relatorio, string caminhoDestino)
    {
        string conteudoCsv =
            "Título;Responsável;Descrição;Data" + Environment.NewLine +
            $"{EscaparCampo(relatorio.Titulo)};" +
            $"{EscaparCampo(relatorio.Responsavel)};" +
            $"{EscaparCampo(relatorio.Descricao)};" +
            $"{EscaparCampo(relatorio.Data.ToString("dd/MM/yyyy HH:mm:ss"))}";

        File.WriteAllText(
            caminhoDestino,
            conteudoCsv,
            new UTF8Encoding(true)
        );
    }

    private static string EscaparCampo(string valor)
    {
        return $"\"{valor.Replace("\"", "\"\"")}\"";
    }
}