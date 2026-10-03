using DesafioTecnico.ConsoleApp.Models;

namespace DesafioTecnico.ConsoleApp.Services;

public sealed class EstoqueService
{
    private readonly Dictionary<int, Produto> produtos = new();
    private readonly List<MovimentacaoEstoque> historico = [];

    public EstoqueService(IEnumerable<Produto> produtosIniciais)
    {
        ArgumentNullException.ThrowIfNull(produtosIniciais);
        foreach (var produto in produtosIniciais)
        {
            ArgumentNullException.ThrowIfNull(produto);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(produto.CodigoProduto);
            ArgumentException.ThrowIfNullOrWhiteSpace(produto.DescricaoProduto);
            ArgumentOutOfRangeException.ThrowIfNegative(produto.Estoque);
            if (!produtos.TryAdd(produto.CodigoProduto, produto))
                throw new ArgumentException("Existem códigos de produto duplicados.", nameof(produtosIniciais));
        }
    }

    public IReadOnlyList<Produto> ListarProdutos() => produtos.Values.OrderBy(p => p.CodigoProduto).ToArray();
    public IReadOnlyList<MovimentacaoEstoque> ListarMovimentacoes() => historico.ToArray();

    public MovimentacaoEstoque Movimentar(int codigoProduto, TipoMovimentacao tipo, int quantidade)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantidade);
        if (!Enum.IsDefined(tipo))
            throw new ArgumentException("Tipo de movimentação inválido.", nameof(tipo));
        if (!produtos.TryGetValue(codigoProduto, out var produto))
            throw new KeyNotFoundException("Produto não encontrado.");
        if (tipo == TipoMovimentacao.Saida && quantidade > produto.Estoque)
            throw new InvalidOperationException("Estoque insuficiente para essa saída.");
        var estoqueFinal = tipo == TipoMovimentacao.Entrada
            ? checked(produto.Estoque + quantidade) : produto.Estoque - quantidade;
        var movimentacao = new MovimentacaoEstoque((long)historico.Count + 1, codigoProduto, tipo,
            quantidade, DateTime.UtcNow, estoqueFinal);
        produtos[codigoProduto] = produto with { Estoque = estoqueFinal };
        historico.Add(movimentacao);
        return movimentacao;
    }
}
