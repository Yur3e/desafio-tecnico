namespace DesafioTecnico.ConsoleApp.Models;

public sealed record Venda
{
    public required string Vendedor { get; init; }
    public required decimal Valor { get; init; }
}
