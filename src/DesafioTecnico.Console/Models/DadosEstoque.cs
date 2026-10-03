namespace DesafioTecnico.ConsoleApp.Models;

public sealed record DadosEstoque
{
    public required List<Produto> Estoque { get; init; }
}
