namespace DesafioTecnico.ConsoleApp.Models;

public sealed record DadosVendas
{
    public required List<Venda> Vendas { get; init; }
}
