namespace DesafioTecnico.ConsoleApp.Models;

public enum TipoMovimentacao { Entrada, Saida }

public sealed record MovimentacaoEstoque(long Id, int CodigoProduto, TipoMovimentacao Tipo,
    int Quantidade, DateTime Data, int EstoqueFinal);
