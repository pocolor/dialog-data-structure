using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DialogDataStructure;

public class ImmutableDialog
{
    public string? Name { get; }
    public ImmutableBranch Start { get; }

    public record ImmutableText(string Id, string Content)
    {
        public ImmutableText(Dialog.Text text) : this(text.Id, text.Content)
        {
        }

        public ImmutableText(ImmutableText text) // copy constructor
        {
            Id = text.Id;
            Content = text.Content;
        }

        public ImmutableText Copy() => new(this);
        public Dialog.Text ToText() => new(Id, Content);
        public override string ToString() => $"{nameof(ImmutableText)}(Id = \"{Id}\", Content = \"{Content})\"";
    }

    public class ImmutableBranch : IEnumerable<ImmutableText>
    {
        public ImmutableBranch? Previous { get; private set; }
        public ImmutableList<ImmutableText> Texts { get; }
        public ImmutableList<ImmutableBranch> NextBranches { get; }

        public bool HasPrevious => Previous is not null;
        public bool Continues => NextBranches.Count > 0;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<ImmutableText> GetEnumerator() => Texts.GetEnumerator();

        public ImmutableBranch(Dialog.Branch branch)
        {
            Texts = branch.Texts.ConvertAll(e => e.ToImmutableText()).ToImmutableList();
            NextBranches = branch.NextBranches.ConvertAll(e => e.ToImmutableBranch()).ToImmutableList();
            NextBranches.ForEach(e => e.Previous = this);
        }

        public ImmutableBranch(ImmutableBranch branch) // copy constructor
        {
            Texts = branch.Texts.ConvertAll(e => e.Copy());
            NextBranches = branch.NextBranches.ConvertAll(e => e.Copy());
            NextBranches.ForEach(e => e.Previous = this);
        }

        public ImmutableBranch Copy() => new(this);

        public Dialog.Branch ToBranch()
        {
            Dialog.Branch branch = new();
            branch.Texts = Texts.ConvertAll(e => e.ToText()).ToList();
            branch.NextBranches = NextBranches.ConvertAll(e => e.ToBranch()).ToList();
            branch.NextBranches.ForEach(e => e.Previous = branch);
            return branch;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append(nameof(ImmutableBranch)).Append('{')
                .Append("Previous is not null = ").Append(Previous is not null).Append(", ")
                .Append("Texts = ").Append(Texts).Append(", ")
                .Append("NextBranches.Count = ").Append(NextBranches.Count).Append('}')
                .ToString();
        }
    }

    public ImmutableDialog(Dialog dialog)
    {
        Name = dialog.Name;
        Start = dialog.Start.ToImmutableBranch();
    }

    public ImmutableDialog(ImmutableDialog dialog) // copy constructor
    {
        Name = dialog.Name;
        Start = dialog.Start.Copy();
    }

    public ImmutableDialog Copy() => new(this);

    public Dialog ToDialog() => new()
    {
        Name = Name,
        Start = Start.ToBranch()
    };

    public override string ToString()
    {
        return new StringBuilder()
            .Append(nameof(ImmutableDialog)).Append('{')
            .Append("Name = ").Append(Name).Append(", ")
            .Append("Start = ").Append(Start).Append('}')
            .ToString();
    }
}

public static class DialogExtensions
{
    public static ImmutableDialog ToImmutableDialog(this Dialog dialog) => new(dialog);
    public static ImmutableDialog.ImmutableBranch ToImmutableBranch(this Dialog.Branch branch) => new(branch);
    public static ImmutableDialog.ImmutableText ToImmutableText(this Dialog.Text text) => new(text);
}