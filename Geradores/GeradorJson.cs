using System.Text.Json;
using System.Text.Encodings.Web;
using TdeGeradorRelatorios.Interfaces;
using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Geradores;

public class GeradorJson : IGeradorRelatorio
{
    public void Gerar(Relatorio relatorio, string caminhoDestino)
    {
        var opcoes = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var dados = new
        {
            titulo = relatorio.Titulo,
            responsavel = relatorio.Responsavel,
            descricao = relatorio.Descricao,
            data = relatorio.Data.ToString("dd/MM/yyyy HH:mm")
        };

        string conteudoJson = JsonSerializer.Serialize(dados, opcoes);

        File.WriteAllText(caminhoDestino, conteudoJson);
    }
}