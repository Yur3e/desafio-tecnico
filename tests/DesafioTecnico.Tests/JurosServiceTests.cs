using DesafioTecnico.ConsoleApp.Services;

namespace DesafioTecnico.Tests;

public class JurosServiceTests
{
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 25)]
    [InlineData(10, 250)]
    public void CalculaJurosSimples(int dias, int esperado)
    {
        var vencimento = new DateOnly(2026, 1, 1);
        Assert.Equal((decimal)esperado, new JurosService().CalcularJuros(1000m, vencimento, vencimento.AddDays(dias)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejeitaValorInvalido(int valor) => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new JurosService().CalcularJuros(valor, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 2)));

    [Fact]
    public void ConsideraDiaBissexto() => Assert.Equal(5m,
        new JurosService().CalcularJuros(100m, new DateOnly(2024, 2, 28), new DateOnly(2024, 3, 1)));
}
