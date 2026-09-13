namespace TdeGeradorRelatorios.Models;

public record Relatorio(
    string Titulo,
    string Responsavel,
    string Descricao,
    DateTime Data
);

