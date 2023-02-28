using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace iRacer.Tools.Telemetry;
public static class VariableInfoCollectionSerializer
{
    private static readonly JsonSerializerOptions _JsonSerializerOptions = CreateJsonSerializerOptions();

    public static string ToJson(VariableInfoCollection collection)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        return JsonSerializer.Serialize(collection, _JsonSerializerOptions);
    }

    public static void WriteJson(VariableInfoCollection collection, TextWriter textWriter)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (textWriter is null)
        {
            throw new ArgumentNullException(nameof(textWriter));
        }

        textWriter.Write(ToJson(collection));
    }

    public static Task WriteJsonAsync(VariableInfoCollection collection, TextWriter textWriter)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (textWriter is null)
        {
            throw new ArgumentNullException(nameof(textWriter));
        }

        return textWriter.WriteAsync(ToJson(collection));
    }

    private static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var enumConverter = new JsonStringEnumConverter();
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        serializerOptions.Converters.Add(enumConverter);

        return serializerOptions;
    }
}
