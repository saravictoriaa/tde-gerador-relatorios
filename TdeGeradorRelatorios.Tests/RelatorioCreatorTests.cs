using TdeGeradorRelatorios.Factories;
using TdeGeradorRelatorios.Geradores;
using TdeGeradorRelatorios.Interfaces;
using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Tests;

/// <summary>
/// Testes do padrão Factory Method.
///
/// Mapeamento GoF:
///   Product          → IGeradorRelatorio
///   ConcreteProduct  → GeradorPdf, GeradorCsv, GeradorJson
///   Creator          → RelatorioCreator (classe abstrata com CriarGerador() + Gerar())
///   ConcreteCreator  → PdfCreator, CsvCreator, JsonCreator
///   Factory Method   → CriarGerador() — abstrato no Creator, implementado em cada ConcreteCreator
/// </summary>
public class RelatorioCreatorTests : IDisposable
{
    private readonly List<string> _arquivosTemporarios = [];

    public void Dispose()
    {
        foreach (var arquivo in _arquivosTemporarios)
        {
            if (File.Exists(arquivo))
                File.Delete(arquivo);
        }
    }

    private string CriarCaminhoTemporario(string extensao)
    {
        var caminho = Path.Combine(Path.GetTempPath(), $"relatorio_fm_{Guid.NewGuid()}.{extensao}");
        _arquivosTemporarios.Add(caminho);
        return caminho;
    }

    // -----------------------------------------------------------------------
    // Testes de CriarGerador() — verificam que cada ConcreteCreator
    // retorna o ConcreteProduct correto (vínculo central do Factory Method)
    // -----------------------------------------------------------------------

    [Fact]
    public void PdfCreator_CriarGerador_DeveRetornarGeradorPdf()
    {
        var creator = new PdfCreator();

        IGeradorRelatorio gerador = creator.CriarGerador();

        Assert.IsType<GeradorPdf>(gerador);
    }

    [Fact]
    public void CsvCreator_CriarGerador_DeveRetornarGeradorCsv()
    {
        var creator = new CsvCreator();

        IGeradorRelatorio gerador = creator.CriarGerador();

        Assert.IsType<GeradorCsv>(gerador);
    }

    [Fact]
    public void JsonCreator_CriarGerador_DeveRetornarGeradorJson()
    {
        var creator = new JsonCreator();

        IGeradorRelatorio gerador = creator.CriarGerador();

        Assert.IsType<GeradorJson>(gerador);
    }

    // -----------------------------------------------------------------------
    // Testes de Gerar() através do Creator — verificam que o Template Method
    // em RelatorioCreator.Gerar() delega corretamente para o gerador criado
    // -----------------------------------------------------------------------

    [Fact]
    public void PdfCreator_Gerar_DeveCriarArquivoPdf()
    {
        var creator = new PdfCreator();
        var relatorio = new Relatorio("Título", "Responsável", "Descrição", DateTime.Now);
        string caminho = CriarCaminhoTemporario("pdf");

        creator.Gerar(relatorio, caminho);

        Assert.True(File.Exists(caminho));
    }

    [Fact]
    public void CsvCreator_Gerar_DeveCriarArquivoCsv()
    {
        var creator = new CsvCreator();
        var relatorio = new Relatorio("Título", "Responsável", "Descrição", DateTime.Now);
        string caminho = CriarCaminhoTemporario("csv");

        creator.Gerar(relatorio, caminho);

        Assert.True(File.Exists(caminho));
    }

    [Fact]
    public void JsonCreator_Gerar_DeveCriarArquivoJson()
    {
        var creator = new JsonCreator();
        var relatorio = new Relatorio("Título", "Responsável", "Descrição", DateTime.Now);
        string caminho = CriarCaminhoTemporario("json");

        creator.Gerar(relatorio, caminho);

        Assert.True(File.Exists(caminho));
    }

    // -----------------------------------------------------------------------
    // Teste de polimorfismo (LSP) — qualquer ConcreteCreator pode ser
    // tratado como RelatorioCreator sem quebrar o contrato do cliente
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData("pdf")]
    [InlineData("csv")]
    [InlineData("json")]
    public void QualquerCreator_UsadoComoRelatorioCreator_DeveGerarArquivo(string extensao)
    {
        RelatorioCreator creator = extensao switch
        {
            "pdf" => new PdfCreator(),
            "csv" => new CsvCreator(),
            "json" => new JsonCreator(),
            _ => throw new ArgumentException()
        };

        var relatorio = new Relatorio("Título", "Responsável", "Descrição", DateTime.Now);
        string caminho = CriarCaminhoTemporario(extensao);

        // O cliente conhece apenas RelatorioCreator — não os tipos concretos
        creator.Gerar(relatorio, caminho);

        Assert.True(File.Exists(caminho));
    }
}
