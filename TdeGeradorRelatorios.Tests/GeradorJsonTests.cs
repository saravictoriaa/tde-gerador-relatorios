using System.Text.Json;
using TdeGeradorRelatorios.Geradores;
using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Tests;

public class GeradorJsonTests : IDisposable
{
    private readonly string _caminhoArquivo;
    private readonly Relatorio _relatorio;

    public GeradorJsonTests()
    {
        _caminhoArquivo = Path.Combine(Path.GetTempPath(), $"relatorio_teste_{Guid.NewGuid()}.json");
        _relatorio = new Relatorio(
            Titulo: "Título Teste",
            Responsavel: "Responsável Teste",
            Descricao: "Descrição com acentuação",
            Data: new DateTime(2026, 9, 16, 14, 30, 0)
        );
    }

    public void Dispose()
    {
        if (File.Exists(_caminhoArquivo))
            File.Delete(_caminhoArquivo);
    }

    [Fact]
    public void Gerar_DeveCriarArquivoNoCaminhoEsperado()
    {
        var gerador = new GeradorJson();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        Assert.True(File.Exists(_caminhoArquivo));
    }

    [Fact]
    public void Gerar_DeveProuzirJsonValido()
    {
        var gerador = new GeradorJson();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        string conteudo = File.ReadAllText(_caminhoArquivo);
        // Deve ser possível parsear sem exceção
        using var doc = JsonDocument.Parse(conteudo);
        Assert.NotNull(doc);
    }

    [Fact]
    public void Gerar_DeveConterCamposEsperados()
    {
        var gerador = new GeradorJson();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        string conteudo = File.ReadAllText(_caminhoArquivo);
        using var doc = JsonDocument.Parse(conteudo);
        JsonElement root = doc.RootElement;

        Assert.True(root.TryGetProperty("titulo", out _), "Campo 'titulo' ausente");
        Assert.True(root.TryGetProperty("responsavel", out _), "Campo 'responsavel' ausente");
        Assert.True(root.TryGetProperty("descricao", out _), "Campo 'descricao' ausente");
        Assert.True(root.TryGetProperty("data", out _), "Campo 'data' ausente");
    }

    [Fact]
    public void Gerar_DevePreservarAcentuacaoSemEscapeUnicode()
    {
        var gerador = new GeradorJson();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        string conteudo = File.ReadAllText(_caminhoArquivo);
        // Acentuação deve aparecer literal, não como \uXXXX
        Assert.Contains("Título", conteudo);
        Assert.Contains("Responsável", conteudo);
        Assert.Contains("acentuação", conteudo);
        Assert.DoesNotContain("\\u00", conteudo);
    }

    [Fact]
    public void Gerar_DeveEscreverValoresCorretosDosCompos()
    {
        var gerador = new GeradorJson();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        string conteudo = File.ReadAllText(_caminhoArquivo);
        using var doc = JsonDocument.Parse(conteudo);
        JsonElement root = doc.RootElement;

        Assert.Equal("Título Teste", root.GetProperty("titulo").GetString());
        Assert.Equal("Responsável Teste", root.GetProperty("responsavel").GetString());
        Assert.Equal("Descrição com acentuação", root.GetProperty("descricao").GetString());
        Assert.Equal("16/09/2026 14:30", root.GetProperty("data").GetString());
    }
}
