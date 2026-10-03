using DesafioTecnico.ConsoleApp.Common;
using DesafioTecnico.ConsoleApp.Models;

namespace DesafioTecnico.Tests;

public class JsonFileReaderTests
{
    [Theory]
    [InlineData("{")]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("[]")]
    [InlineData("{\"vendas\":null}")]
    [InlineData("{\"vendas\":[{\"vendedor\":\"Ana\"}]}")]
    [InlineData("{\"vendas\":[{\"vendedor\":\"Ana\",\"valor\":\"abc\"}]}")]
    public void RejeitaJsonInvalido(string conteudo)
    {
        var caminho = Path.GetTempFileName();
        try
        {
            File.WriteAllText(caminho, conteudo);
            Assert.Throws<InvalidDataException>(() => JsonFileReader.LerArquivo<DadosVendas>(caminho));
        }
        finally { File.Delete(caminho); }
    }

    [Fact]
    public void LeJsonComNomesSemDistinguirMaiusculas()
    {
        var caminho = Path.GetTempFileName();
        try
        {
            File.WriteAllText(caminho, "{\"VENDAS\":[{\"vendedor\":\"João\",\"VALOR\":499.99}]}");
            var venda = Assert.Single(JsonFileReader.LerArquivo<DadosVendas>(caminho).Vendas);
            Assert.Equal("João", venda.Vendedor);
            Assert.Equal(499.99m, venda.Valor);
        }
        finally { File.Delete(caminho); }
    }

    [Fact]
    public void InformaArquivoAusente() => Assert.Throws<FileNotFoundException>(() =>
        JsonFileReader.LerArquivo<DadosVendas>(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json")));
}
