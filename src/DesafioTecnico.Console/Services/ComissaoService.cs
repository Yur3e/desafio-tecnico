using DesafioTecnico.ConsoleApp.Models;

namespace DesafioTecnico.ConsoleApp.Services;

public sealed class ComissaoService
{
    private const decimal TaxaIntermediaria = 0.01m;
    private const decimal TaxaAlta = 0.05m;

    public decimal CalcularComissao(decimal valorVenda)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(valorVenda);
        var taxa = valorVenda < 100m ? 0m : valorVenda < 500m ? TaxaIntermediaria : TaxaAlta;
        return valorVenda * taxa;
    }

    public Dictionary<string, decimal> CalcularComissoesPorVendedor(IEnumerable<Venda> vendas)
    {
        ArgumentNullException.ThrowIfNull(vendas);
        var totais = new Dictionary<string, decimal>(StringComparer.Ordinal);
        foreach (var venda in vendas)
        {
            ArgumentNullException.ThrowIfNull(venda);
            ArgumentException.ThrowIfNullOrWhiteSpace(venda.Vendedor);
            var vendedor = venda.Vendedor.Trim();
            totais[vendedor] = totais.GetValueOrDefault(vendedor) + CalcularComissao(venda.Valor);
        }
        return totais;
    }
}
