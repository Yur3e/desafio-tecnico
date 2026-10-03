using DesafioTecnico.ConsoleApp.Models;
using DesafioTecnico.ConsoleApp.Services;

namespace DesafioTecnico.Tests;

public class EstoqueServiceTests
{
    private static Produto Produto(int estoque = 10) => new()
    { CodigoProduto = 1, DescricaoProduto = "Caneta", Estoque = estoque };

    [Theory]
    [InlineData(TipoMovimentacao.Entrada, 5, 15)]
    [InlineData(TipoMovimentacao.Saida, 5, 5)]
    [InlineData(TipoMovimentacao.Saida, 10, 0)]
    public void AtualizaEstoqueERegistraMovimento(TipoMovimentacao tipo, int quantidade, int esperado)
    {
        var service = new EstoqueService([Produto()]);
        var resultado = service.Movimentar(1, tipo, quantidade);
        Assert.Equal(esperado, resultado.EstoqueFinal);
        Assert.Equal(esperado, Assert.Single(service.ListarProdutos()).Estoque);
        Assert.Equal(resultado, Assert.Single(service.ListarMovimentacoes()));
        Assert.True(resultado.Id > 0);
        Assert.Equal(1, resultado.CodigoProduto);
        Assert.Equal(tipo, resultado.Tipo);
        Assert.Equal(quantidade, resultado.Quantidade);
    }

    [Fact]
    public void RejeitaSaidaSemAlterarEstado()
    {
        var service = new EstoqueService([Produto()]);
        Assert.Throws<InvalidOperationException>(() => service.Movimentar(1, TipoMovimentacao.Saida, 11));
        Assert.Equal(10, Assert.Single(service.ListarProdutos()).Estoque);
        Assert.Empty(service.ListarMovimentacoes());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejeitaQuantidadeInvalida(int quantidade)
    {
        var service = new EstoqueService([Produto()]);
        Assert.Throws<ArgumentOutOfRangeException>(() => service.Movimentar(1, TipoMovimentacao.Entrada, quantidade));
        Assert.Equal(10, Assert.Single(service.ListarProdutos()).Estoque);
        Assert.Empty(service.ListarMovimentacoes());
    }

    [Fact]
    public void RejeitaProdutoETipoInvalidos()
    {
        var service = new EstoqueService([Produto()]);
        Assert.Throws<KeyNotFoundException>(() => service.Movimentar(2, TipoMovimentacao.Entrada, 1));
        Assert.Throws<ArgumentException>(() => service.Movimentar(1, (TipoMovimentacao)99, 1));
        Assert.Equal(10, Assert.Single(service.ListarProdutos()).Estoque);
        Assert.Empty(service.ListarMovimentacoes());
    }

    [Fact]
    public void RejeitaOverflowSemAlterarEstado()
    {
        var service = new EstoqueService([Produto(int.MaxValue)]);
        Assert.Throws<OverflowException>(() => service.Movimentar(1, TipoMovimentacao.Entrada, 1));
        Assert.Equal(int.MaxValue, Assert.Single(service.ListarProdutos()).Estoque);
        Assert.Empty(service.ListarMovimentacoes());
    }

    [Fact]
    public void MantemMovimentacoesSucessivasComIdsDistintos()
    {
        var original = Produto();
        var service = new EstoqueService([original]);
        var entrada = service.Movimentar(1, TipoMovimentacao.Entrada, 5);
        var saida = service.Movimentar(1, TipoMovimentacao.Saida, 2);
        Assert.Equal(13, saida.EstoqueFinal);
        Assert.Equal(1L, entrada.Id);
        Assert.Equal(2L, saida.Id);
        Assert.Equal(2, service.ListarMovimentacoes().Count);
        Assert.Equal(10, original.Estoque);
    }

    [Fact]
    public void ValidaCadastroInicial()
    {
        Assert.Throws<ArgumentNullException>(() => new EstoqueService(null!));
        Assert.Throws<ArgumentNullException>(() => new EstoqueService([null!]));
        Assert.Throws<ArgumentException>(() => new EstoqueService([Produto(), Produto()]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EstoqueService([Produto(-1)]));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EstoqueService([Produto() with { CodigoProduto = 0 }]));
        Assert.Throws<ArgumentException>(() => new EstoqueService([Produto() with { DescricaoProduto = " " }]));
    }
}
