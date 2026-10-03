using DesafioTecnico.ConsoleApp.Models;
using DesafioTecnico.ConsoleApp.Services;

namespace DesafioTecnico.Tests;

public class ComissaoServiceTests
{
    private readonly ComissaoService service = new();

    [Theory]
    [InlineData("0", "0")]
    [InlineData("99.99", "0")]
    [InlineData("100", "1")]
    [InlineData("499.99", "4.9999")]
    [InlineData("500", "25")]
    [InlineData("1000", "50")]
    public void CalculaFaixas(string valor, string esperado)
    {
        var cultura = System.Globalization.CultureInfo.InvariantCulture;
        Assert.Equal(decimal.Parse(esperado, cultura), service.CalcularComissao(decimal.Parse(valor, cultura)));
    }

    [Fact]
    public void CalculaPorVendaAntesDeAgrupar()
    {
        var resultado = service.CalcularComissoesPorVendedor([
            new Venda { Vendedor = "Ana", Valor = 300m },
            new Venda { Vendedor = "Ana", Valor = 300m },
            new Venda { Vendedor = "João", Valor = 500m }]);
        Assert.Equal(6m, resultado["Ana"]);
        Assert.Equal(25m, resultado["João"]);
        Assert.Equal(2, resultado.Count);
    }

    [Fact]
    public void RejeitaEntradasInvalidas()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => service.CalcularComissao(-1m));
        Assert.Throws<ArgumentNullException>(() => service.CalcularComissoesPorVendedor(null!));
        Assert.Throws<ArgumentNullException>(() => service.CalcularComissoesPorVendedor([null!]));
        Assert.Throws<ArgumentException>(() => service.CalcularComissoesPorVendedor([new Venda { Vendedor = " ", Valor = 10m }]));
        Assert.Empty(service.CalcularComissoesPorVendedor([]));
    }
}
