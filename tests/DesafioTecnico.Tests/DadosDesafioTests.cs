using DesafioTecnico.ConsoleApp.Common;
using DesafioTecnico.ConsoleApp.Models;
using DesafioTecnico.ConsoleApp.Services;

namespace DesafioTecnico.Tests;

public class DadosDesafioTests
{
    [Fact]
    public void CalculaComissoesDas36VendasOriginais()
    {
        var dados = JsonFileReader.LerArquivo<DadosVendas>(Path.Combine(AppContext.BaseDirectory, "Data", "vendas.json"));
        Assert.Equal(36, dados.Vendas.Count);
        var totais = new ComissaoService().CalcularComissoesPorVendedor(dados.Vendas);
        Assert.Equal(4, totais.Count);
        Assert.Equal(495.6770m, totais["João Silva"]);
        Assert.Equal(465.9495m, totais["Maria Souza"]);
        Assert.Equal(379.3715m, totais["Carlos Oliveira"]);
        Assert.Equal(404.9805m, totais["Ana Lima"]);
    }

    [Fact]
    public void MovimentaProdutosOriginaisComIdentificadorUnicoEntreProdutos()
    {
        var dados = JsonFileReader.LerArquivo<DadosEstoque>(Path.Combine(AppContext.BaseDirectory, "Data", "estoque.json"));
        Assert.Equal(new[] { 101, 102, 103, 104, 105 }, dados.Estoque.Select(p => p.CodigoProduto));
        Assert.Equal(new[] { 150, 75, 200, 320, 90 }, dados.Estoque.Select(p => p.Estoque));
        Assert.Equal(new[] { "Caneta Azul", "Caderno Universitário", "Borracha Branca", "Lápis Preto HB", "Marcador de Texto Amarelo" },
            dados.Estoque.Select(p => p.DescricaoProduto));
        var service = new EstoqueService(dados.Estoque);
        var entrada = service.Movimentar(101, TipoMovimentacao.Entrada, 10);
        Assert.Throws<InvalidOperationException>(() => service.Movimentar(102, TipoMovimentacao.Saida, 76));
        var saida = service.Movimentar(102, TipoMovimentacao.Saida, 5);
        Assert.Equal(160, entrada.EstoqueFinal);
        Assert.Equal(70, saida.EstoqueFinal);
        Assert.Equal(1L, entrada.Id);
        Assert.Equal(2L, saida.Id);
    }
}
