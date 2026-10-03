using System.Globalization;
using System.Text;
using DesafioTecnico.ConsoleApp;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
return new AplicacaoConsole().Executar();
