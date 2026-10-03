using System.Globalization;
using DesafioTecnico.ConsoleApp.Common;
using DesafioTecnico.ConsoleApp.Models;
using DesafioTecnico.ConsoleApp.Services;

namespace DesafioTecnico.ConsoleApp;

public sealed class AplicacaoConsole
{
    private readonly ComissaoService comissoes = new();
    private readonly JurosService juros = new();
    private EstoqueService? estoque;

    public int Executar()
    {
        while (true)
        {
            Console.WriteLine("\n=== DESAFIO TÉCNICO ===\n1 - Calcular comissões\n2 - Movimentar estoque\n3 - Calcular juros\n0 - Sair");
            Console.Write("Escolha uma opção: ");
            var opcao = Console.ReadLine();
            if (opcao is null or "0") return 0;
            try
            {
                switch (opcao)
                {
                    case "1": ExibirComissoes(); break;
                    case "2": MenuEstoque(); break;
                    case "3": ExibirJuros(); break;
                    default: Console.WriteLine("Opção inválida."); break;
                }
            }
            catch (EndOfStreamException) { return 0; }
            catch (FileNotFoundException) { Console.WriteLine("Arquivo de dados não encontrado na pasta Data."); }
            catch (DirectoryNotFoundException) { Console.WriteLine("Pasta Data não encontrada."); }
            catch (InvalidDataException ex) { Console.WriteLine(ex.Message); }
            catch (IOException) { Console.WriteLine("Não foi possível ler o arquivo de dados."); }
            catch (UnauthorizedAccessException) { Console.WriteLine("Sem permissão para ler o arquivo de dados."); }
            catch (ArgumentException) { Console.WriteLine("Dados inválidos: verifique nomes, valores, códigos e quantidades."); }
            catch (OverflowException) { Console.WriteLine("O valor excede o limite numérico permitido."); }
        }
    }

    private static string Caminho(string nome) => Path.Combine(AppContext.BaseDirectory, "Data", nome);

    private void ExibirComissoes()
    {
        var vendas = JsonFileReader.LerArquivo<DadosVendas>(Caminho("vendas.json")).Vendas;
        var resultado = comissoes.CalcularComissoesPorVendedor(vendas);
        Console.WriteLine("\n=== COMISSÕES ===");
        if (resultado.Count == 0) Console.WriteLine("Nenhuma venda cadastrada.");
        foreach (var (vendedor, total) in resultado.OrderBy(item => item.Key))
            Console.WriteLine($"{vendedor,-22} | Comissão: {total:C2}");
    }

    private void MenuEstoque()
    {
        estoque ??= new EstoqueService(JsonFileReader.LerArquivo<DadosEstoque>(Caminho("estoque.json")).Estoque);
        while (true)
        {
            Console.WriteLine("\n=== ESTOQUE ===\n1 - Listar produtos\n2 - Registrar entrada\n3 - Registrar saída\n4 - Voltar");
            Console.Write("Escolha uma opção: ");
            var opcao = LerLinha();
            if (opcao == "4") return;
            if (opcao is not ("1" or "2" or "3"))
            {
                Console.WriteLine("Opção inválida.");
                continue;
            }
            foreach (var produto in estoque.ListarProdutos())
                Console.WriteLine($"{produto.CodigoProduto} - {produto.DescricaoProduto} | Estoque: {produto.Estoque}");
            if (opcao == "1") continue;
            var codigo = LerInteiroPositivo("Código do produto: ");
            var quantidade = LerInteiroPositivo("Quantidade: ");
            try
            {
                var movimento = estoque.Movimentar(codigo,
                    opcao == "2" ? TipoMovimentacao.Entrada : TipoMovimentacao.Saida, quantidade);
                var produto = estoque.ListarProdutos().Single(p => p.CodigoProduto == codigo);
                Console.WriteLine($"Movimentação registrada.\nID: {movimento.Id}\nProduto: {produto.DescricaoProduto}\nTipo: {(movimento.Tipo == TipoMovimentacao.Entrada ? "Entrada" : "Saída")}\nQuantidade: {movimento.Quantidade}\nEstoque final: {movimento.EstoqueFinal}");
            }
            catch (KeyNotFoundException ex) { Console.WriteLine(ex.Message); }
            catch (InvalidOperationException ex) { Console.WriteLine(ex.Message); }
            catch (OverflowException) { Console.WriteLine("A entrada ultrapassa o estoque máximo permitido."); }
        }
    }

    private void ExibirJuros()
    {
        decimal valor;
        while (true)
        {
            Console.Write("Valor original (ex.: 1000,50, sem separador de milhar): ");
            if (decimal.TryParse(LerLinha(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.CurrentCulture, out valor) && valor > 0) break;
            Console.WriteLine("Valor inválido. Informe um valor positivo usando vírgula para os centavos.");
        }
        DateOnly vencimento;
        while (true)
        {
            Console.Write("Data de vencimento (dd/MM/aaaa): ");
            if (DateOnly.TryParseExact(LerLinha(), "dd/MM/yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out vencimento)) break;
            Console.WriteLine("Data inválida. Use dd/MM/aaaa.");
        }
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var totalJuros = juros.CalcularJuros(valor, vencimento, hoje);
        var total = valor + totalJuros;
        Console.WriteLine($"Data do cálculo: {hoje:dd/MM/yyyy}\nValor original: {valor:C2}\nJuros: {totalJuros:C2}\nValor atualizado: {total:C2}");
    }

    private static string LerLinha() => Console.ReadLine()?.Trim() ?? throw new EndOfStreamException();

    private static int LerInteiroPositivo(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            if (int.TryParse(LerLinha(), out var valor) && valor > 0) return valor;
            Console.WriteLine("Informe um número inteiro maior que zero.");
        }
    }
}
