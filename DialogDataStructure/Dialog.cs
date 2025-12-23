using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DialogDataStructure;

public class Dialog
{
    public string? Name { get; set; }

    public Branch Start { get; set; } = null!;

    public record Text
    {
        public string Id { get; set; } = "";

        public string Content { get; set; } = "";

        public Text()
        {
        }

        public Text(string id, string content)
        {
            Id = id;
            Content = content;
        }

        public Text(Text text)
        {
            Id = text.Id;
            Content = text.Content;
        }

        public Text Copy() => new(this);
        public override string ToString() => $"{nameof(Text)}(Id = \"{Id}\", Content = \"{Content})\"";
    }

    public class Branch
    {
        [JsonIgnore] public Branch? Previous { get; set; }

        public List<Text> Texts { get; set; } = [];

        public List<Branch> NextBranches { get; set; } = [];

        [JsonIgnore] public bool HasPrevious => Previous is not null;

        [JsonIgnore] public bool Continues => NextBranches.Count > 0;

        public void LinkPrevious() => NextBranches.ForEach(e => { e.Previous = this; e.LinkPrevious(); });

        public Branch()
        {
        }

        public Branch(Branch branch)
        {
            Texts = branch.Texts.ConvertAll(e => e.Copy());
            NextBranches = branch.NextBranches.ConvertAll(e => e.Copy());
            NextBranches.ForEach(e => e.Previous = this);
        }

        public Branch Copy() => new(this);

        public override string ToString()
        {
            return new StringBuilder()
                .Append(nameof(Branch)).Append('{')
                .Append("Previous is not null = ").Append(Previous is not null).Append(", ")
                .Append("Texts = ").Append(Texts).Append(", ")
                .Append("NextBranches.Count = ").Append(NextBranches.Count).Append('}')
                .ToString();
        }
    }

    public Dialog()
    {
    }

    public Dialog(Dialog dialog)
    {
        Name = dialog.Name;
        Start = dialog.Start.Copy();
    }

    public Dialog Copy() => new(this);

    public override string ToString()
    {
        return new StringBuilder()
            .Append(nameof(Dialog)).Append('{')
            .Append("Name = ").Append(Name).Append(", ")
            .Append("Start = ").Append(Start).Append('}')
            .ToString();
    }
}