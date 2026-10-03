using System.Text.Json;

namespace DesafioTecnico.ConsoleApp.Common;

public static class JsonFileReader
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true,
        RespectNullableAnnotations = true
    };

    public static T LerArquivo<T>(string caminho)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caminho);
        using var arquivo = File.OpenRead(caminho);
        try
        {
            return JsonSerializer.Deserialize<T>(arquivo, Opcoes)
                ?? throw new InvalidDataException($"O arquivo '{Path.GetFileName(caminho)}' contém dados nulos.");
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"JSON inválido em '{Path.GetFileName(caminho)}'. Verifique a estrutura e os campos obrigatórios.", ex);
        }
    }
}
