namespace DesafioTecnico.ConsoleApp.Services;

public sealed class JurosService
{
    private const decimal TaxaDiaria = 0.025m;

    public decimal CalcularJuros(decimal valor, DateOnly dataVencimento, DateOnly dataCalculo)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(valor);
        var diasEmAtraso = Math.Max(0, dataCalculo.DayNumber - dataVencimento.DayNumber);
        return valor * TaxaDiaria * diasEmAtraso;
    }
}
