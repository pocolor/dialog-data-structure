using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace DialogDataStructure;

public static class DialogSerializer
{
    public static Dialog Load(string filepath)
    {
        var d = filepath.EndsWith(".json") ? JsonSerializer.Deserialize<Dialog>(File.ReadAllText(filepath))! :
                    filepath.EndsWith(".xml") ? (Dialog) new XmlSerializer(typeof(Dialog)).Deserialize(File.OpenRead(filepath))! :
                    throw new Exception("Unknown file format");
        d.Start.LinkPrevious();
        return d;
    }

    public static void Save(Dialog d, string filepath)
    {
        if (filepath.EndsWith(".json"))
            File.WriteAllText(filepath, JsonSerializer.Serialize(d, new JsonSerializerOptions { WriteIndented = true, IndentSize = 2}));
        else if (filepath.EndsWith(".xml"))
            new XmlSerializer(typeof(Dialog)).Serialize(File.OpenWrite(filepath), d);
        else
            throw new Exception("Unknown file format");
    }
}