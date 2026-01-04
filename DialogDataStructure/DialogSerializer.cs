using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DialogDataStructure;

public static class DialogSerializer
{
    /// <summary>
    /// Default JsonSerializationsOptions for <c>Save()</c> method.
    /// </summary>
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        IndentSize = 2,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    /// <summary>
    /// Loads a dialog from a file.
    /// </summary>
    /// <param name="filepath">Filepath to the JSON file.</param>
    /// <typeparam name="T1">First generic type of the Dialog. Must have an empty constructor.</typeparam>
    /// <typeparam name="T2">Second generic type of the Dialog. Must have an empty constructor.</typeparam>
    /// <returns>A dialog</returns>
    /// <exception cref="Exception">If the file's extension is something else than <c>.json</c>.</exception>
    public static Dialog<T1, T2> Load<T1, T2>(string filepath)
        where T1 : ICloneable, new()
        where T2 : ICloneable, new()
    {
        var d = filepath.EndsWith(".json") ? JsonSerializer.Deserialize<Dialog<T1, T2>>(File.ReadAllText(filepath))! :
                    throw new Exception("Unknown file format");
        d.Start.LinkPrevious();
        return d;
    }

    /// <summary>
    /// Saves a dialog to a file.
    /// </summary>
    /// <param name="d">A dialog to save.</param>
    /// <param name="filepath">Filepath to the JSON file.</param>
    /// <typeparam name="T1">First generic type of the Dialog.</typeparam>
    /// <typeparam name="T2">Second generic type of the Dialog.</typeparam>
    /// <exception cref="Exception">If the file's extension is something else than <c>.json</c>.</exception>
    public static void Save<T1, T2>(Dialog<T1, T2> d, string filepath, JsonSerializerOptions? options = null)
        where T1 : ICloneable
        where T2 : ICloneable
    {
        options ??= Options;
        
        if (filepath.EndsWith(".json"))
            File.WriteAllText(filepath, JsonSerializer.Serialize(d, options));
        else
            throw new Exception("Unknown file format");
    }
}