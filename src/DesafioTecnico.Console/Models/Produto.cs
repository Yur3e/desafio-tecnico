namespace DesafioTecnico.ConsoleApp.Models;

public sealed record Produto
{
    public required int CodigoProduto { get; init; }
    public required string DescricaoProduto { get; init; }
    public required int Estoque { get; init; }
}
