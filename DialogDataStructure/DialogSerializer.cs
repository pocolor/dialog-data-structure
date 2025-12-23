using System;
using System.IO;
using System.Text.Json;

namespace DialogDataStructure;

public static class DialogSerializer
{
    public static Dialog Load(string filepath)
    {
        var d = filepath.EndsWith(".json") ? JsonSerializer.Deserialize<Dialog>(File.ReadAllText(filepath))! :
                    throw new Exception("Unknown file format");
        d.Start.LinkPrevious();
        return d;
    }

    public static void Save(Dialog d, string filepath)
    {
        if (filepath.EndsWith(".json"))
            File.WriteAllText(filepath, JsonSerializer.Serialize(d, new JsonSerializerOptions { WriteIndented = true, IndentSize = 2}));
        else
            throw new Exception("Unknown file format");
    }
}