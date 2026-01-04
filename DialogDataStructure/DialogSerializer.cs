using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DialogDataStructure;

public static class DialogSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        IndentSize = 2,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    public static Dialog<T1, T2> Load<T1, T2>(string filepath)
        where T1 : ICloneable, new()
        where T2 : ICloneable, new()
    {
        var d = filepath.EndsWith(".json") ? JsonSerializer.Deserialize<Dialog<T1, T2>>(File.ReadAllText(filepath))! :
                    throw new Exception("Unknown file format");
        d.Start.LinkPrevious();
        return d;
    }

    public static void Save<T1, T2>(Dialog<T1, T2> d, string filepath)
        where T1 : ICloneable
        where T2 : ICloneable
    {
        if (filepath.EndsWith(".json"))
            File.WriteAllText(filepath, JsonSerializer.Serialize(d, Options));
        else
            throw new Exception("Unknown file format");
    }
}