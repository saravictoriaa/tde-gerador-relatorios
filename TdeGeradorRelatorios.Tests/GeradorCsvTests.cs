using System.Text;
using TdeGeradorRelatorios.Geradores;
using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Tests;

public class GeradorCsvTests : IDisposable
{
    private readonly string _caminhoArquivo;
    private readonly Relatorio _relatorio;

    public GeradorCsvTests()
    {
        _caminhoArquivo = Path.Combine(Path.GetTempPath(), $"relatorio_teste_{Guid.NewGuid()}.csv");
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
        var gerador = new GeradorCsv();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        Assert.True(File.Exists(_caminhoArquivo));
    }

    [Fact]
    public void Gerar_DeveTerCabecalhoCorreto()
    {
        var gerador = new GeradorCsv();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        string[] linhas = File.ReadAllLines(_caminhoArquivo, new UTF8Encoding(true));
        Assert.Equal("Título;Responsável;Descrição;Data", linhas[0]);
    }

    [Fact]
    public void Gerar_DeveUsarSeparadorPontoEVirgula()
    {
        var gerador = new GeradorCsv();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        string conteudo = File.ReadAllText(_caminhoArquivo, new UTF8Encoding(true));
        Assert.Contains(";", conteudo);
        // A linha de dados deve ter exatamente 3 separadores
        string[] linhas = conteudo.Split(Environment.NewLine);
        int separadores = linhas[1].Count(c => c == ';');
        Assert.Equal(3, separadores);
    }

    [Fact]
    public void Gerar_DeveUsarEncodingUtf8ComBom()
    {
        var gerador = new GeradorCsv();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        byte[] bytes = File.ReadAllBytes(_caminhoArquivo);
        // UTF-8 BOM: 0xEF, 0xBB, 0xBF
        Assert.True(bytes.Length >= 3);
        Assert.Equal(0xEF, bytes[0]);
        Assert.Equal(0xBB, bytes[1]);
        Assert.Equal(0xBF, bytes[2]);
    }

    [Fact]
    public void Gerar_DevePreservarAcentuacao()
    {
        var gerador = new GeradorCsv();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        string conteudo = File.ReadAllText(_caminhoArquivo, new UTF8Encoding(true));
        Assert.Contains("Título", conteudo);
        Assert.Contains("Responsável", conteudo);
        Assert.Contains("Descrição", conteudo);
        Assert.Contains("acentuação", conteudo);
    }

    [Fact]
    public void Gerar_DeveEscreverValoresDoRelatorioNaLinhaDeData()
    {
        var gerador = new GeradorCsv();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        string[] linhas = File.ReadAllLines(_caminhoArquivo, new UTF8Encoding(true));
        string linhaDados = linhas[1];
        Assert.Contains("Título Teste", linhaDados);
        Assert.Contains("Responsável Teste", linhaDados);
        Assert.Contains("Descrição com acentuação", linhaDados);
        Assert.Contains("16/09/2026", linhaDados);
    }

    [Fact]
    public void Gerar_DeveEscaparAspasDuplasInternasNosValores()
    {
        var relatorioComAspas = new Relatorio(
            Titulo: "Título com \"aspas\" internas",
            Responsavel: "Responsável",
            Descricao: "Descrição",
            Data: DateTime.Now
        );
        var gerador = new GeradorCsv();

        gerador.Gerar(relatorioComAspas, _caminhoArquivo);

        string conteudo = File.ReadAllText(_caminhoArquivo, new UTF8Encoding(true));
        // Aspas duplas internas devem ser dobradas (padrão CSV)
        Assert.Contains("\"\"aspas\"\"", conteudo);
    }
}
