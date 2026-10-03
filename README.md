# Desafio Técnico

Aplicação de console em C#/.NET 10 com três funções: cálculo de comissões por vendedor, controle de entradas e saídas de estoque e cálculo de juros simples por atraso.

## Executar

Com o SDK .NET 10 instalado, execute na raiz do projeto. A restauração dos pacotes requer acesso ao NuGet:

```sh
dotnet restore
dotnet build --no-restore
dotnet run --project src/DesafioTecnico.Console
```

Escolha uma das opções do menu para acessar cada função ou digite `0` no menu principal para sair. Informe valores monetários com vírgula decimal, sem símbolo de moeda nem separador de milhar (ex.: `1000,50`), e datas no formato `dd/MM/aaaa`.

## Testes e formatação

```sh
dotnet test
dotnet format --verify-no-changes
```

Os testes usam xUnit e verificam os cálculos de comissão e juros, as movimentações de estoque e a leitura dos arquivos JSON. Também cobrem entradas inválidas, estoque insuficiente e limites numéricos.

## Organização

- `src/DesafioTecnico.Console/Models`: dados de vendas, produtos e movimentações.
- `src/DesafioTecnico.Console/Services`: regras de negócio independentes da interface.
- `src/DesafioTecnico.Console/Common`: leitura com `System.Text.Json`.
- `src/DesafioTecnico.Console/Data`: dados originais do desafio copiados para build e publicação.
- `src/DesafioTecnico.Console/AplicacaoConsole.cs`: menus, entradas e mensagens.
- `tests/DesafioTecnico.Tests`: testes automatizados.

## Dados

Os arquivos JSON contêm as 36 vendas e os 5 produtos (códigos 101 a 105) de `desafio_dev.docx`. Eles ficam na pasta `Data` e usam a seguinte estrutura:

```json
{ "vendas": [{ "vendedor": "João Silva", "valor": 1200.50 }] }
```

```json
{ "estoque": [{ "codigoProduto": 101, "descricaoProduto": "Caneta Azul", "estoque": 150 }] }
```

Todos esses campos são obrigatórios. Nomes de propriedades não distinguem maiúsculas e minúsculas. Valores numéricos no JSON usam ponto decimal. Códigos de produto devem ser positivos e únicos; descrição e vendedor não podem estar vazios. Coleções vazias são aceitas.

## Funcionamento

- A comissão é calculada **por venda** e depois somada por vendedor: abaixo de R$ 100, 0%; de R$ 100 até menos de R$ 500, 1%; a partir de R$ 500, 5%. Vendas com valor zero são aceitas; valores negativos são rejeitados.
- Vendedores são agrupados pelo nome após remover espaços nas extremidades, distinguindo maiúsculas e minúsculas.
- Os valores monetários usam `decimal`, sem arredondamentos intermediários, e são exibidos com duas casas decimais no formato brasileiro.
- Os juros são **simples, de 2,5% ao dia**, sobre o valor original. O cálculo considera a data local da máquina. Não há juros quando o vencimento é hoje ou uma data futura, e o valor original deve ser positivo.
- O estoque e o histórico ficam **em memória durante a execução**. Ao reiniciar o programa, o estoque volta aos valores do JSON e o histórico é apagado.
- Cada movimentação registra ID sequencial, produto, tipo, quantidade, data UTC e saldo final. Os IDs começam em 1, são compartilhados entre todos os produtos e reiniciam com o programa.
- Movimentações exigem quantidades positivas. Saídas acima do saldo e entradas que ultrapassem `int.MaxValue` são rejeitadas, sem alterar o estoque nem gerar histórico.
- Produtos e registros são imutáveis; as alterações de estoque passam pelo serviço.

Com os dados originais, as comissões exibidas são: Ana Lima R$ 404,98; Carlos Oliveira R$ 379,37; João Silva R$ 495,68; Maria Souza R$ 465,95.
