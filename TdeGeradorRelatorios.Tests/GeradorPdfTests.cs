using TdeGeradorRelatorios.Geradores;
using TdeGeradorRelatorios.Models;

namespace TdeGeradorRelatorios.Tests;

public class GeradorPdfTests : IDisposable
{
    private readonly string _caminhoArquivo;
    private readonly Relatorio _relatorio;

    public GeradorPdfTests()
    {
        _caminhoArquivo = Path.Combine(Path.GetTempPath(), $"relatorio_teste_{Guid.NewGuid()}.pdf");
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
        var gerador = new GeradorPdf();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        Assert.True(File.Exists(_caminhoArquivo));
    }

    [Fact]
    public void Gerar_ArquivoNaoDeveEstarVazio()
    {
        var gerador = new GeradorPdf();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        var info = new FileInfo(_caminhoArquivo);
        Assert.True(info.Length > 0, "O arquivo PDF gerado está vazio.");
    }

    [Fact]
    public void Gerar_ArquivoDeveConterAssinaturaPdf()
    {
        var gerador = new GeradorPdf();

        gerador.Gerar(_relatorio, _caminhoArquivo);

        // PDFs válidos começam com a assinatura %PDF-
        byte[] primeirosBytes = new byte[5];
        using var stream = File.OpenRead(_caminhoArquivo);
        int bytesLidos = stream.Read(primeirosBytes, 0, 5);

        Assert.Equal(5, bytesLidos);
        string assinatura = System.Text.Encoding.ASCII.GetString(primeirosBytes);
        Assert.Equal("%PDF-", assinatura);
    }
}
