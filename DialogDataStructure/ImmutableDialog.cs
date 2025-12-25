using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DialogDataStructure;

public class ImmutableDialog<TBranchBeginNode, TBranchNode>
    where TBranchBeginNode : ICloneable
    where TBranchNode : ICloneable
{
    public string? Name { get; }
    public ImmutableBranch<TBranchBeginNode, TBranchNode> Start { get; }

    public class ImmutableBranch<TBeginNode, TNode>
        where TBeginNode : ICloneable
        where TNode : ICloneable
    {
        public ImmutableBranch<TBeginNode, TNode>? Previous { get; private set; }
        public TBeginNode BeginNode { get; }
        public ImmutableList<TNode> Nodes { get; }
        public ImmutableList<ImmutableBranch<TBeginNode, TNode>> NextBranches { get; }

        public bool HasPrevious => Previous is not null;
        public bool Continues => NextBranches.Count > 0;

        public ImmutableBranch(Dialog<TBranchBeginNode, TBranchNode>.Branch<TBeginNode, TNode> branch)
        {
            BeginNode = branch.BeginNode;
            Nodes = branch.Nodes.ConvertAll(e => (TNode) e.Clone()).ToImmutableList();
            NextBranches = branch.NextBranches.ConvertAll(e => e.ToImmutableBranch()).ToImmutableList();
            NextBranches.ForEach(e => e.Previous = this);
        }

        public Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode> ToBranch()
        {
            Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode> branch = new();
            branch.BeginNode = BeginNode;
            branch.Nodes = Nodes.ConvertAll(e => (TNode) e.Clone()).ToList();
            branch.NextBranches = NextBranches.ConvertAll(e => e.ToBranch()).ToList();
            branch.NextBranches.ForEach(e => e.Previous = branch);
            return branch;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append(nameof(ImmutableBranch<TBeginNode, TNode>)).Append('{')
                .Append("Previous is not null = ").Append(Previous is not null).Append(", ")
                .Append("Texts = ").Append(Nodes).Append(", ")
                .Append("NextBranches.Count = ").Append(NextBranches.Count).Append('}')
                .ToString();
        }
    }

    public ImmutableDialog(Dialog<TBranchBeginNode, TBranchNode> dialog)
    {
        Name = dialog.Name;
        Start = dialog.Start.ToImmutableBranch();
    }

    public Dialog<TBranchBeginNode, TBranchNode> ToDialog() => new()
    {
        Name = Name,
        Start = Start.ToBranch()
    };

    public override string ToString()
    {
        return new StringBuilder()
            .Append(nameof(ImmutableDialog<TBranchBeginNode, TBranchNode>)).Append('{')
            .Append("Name = ").Append(Name).Append(", ")
            .Append("Start = ").Append(Start).Append('}')
            .ToString();
    }
}

public static class DialogExtensions
{
    public static ImmutableDialog<T1, T2> ToImmutableDialog<T1, T2>(this Dialog<T1, T2> dialog)
        where T1 : ICloneable
        where T2 : ICloneable
        => new(dialog);
    public static ImmutableDialog<T1, T2>.ImmutableBranch<T3, T4> ToImmutableBranch<T1, T2, T3, T4>(this Dialog<T1, T2>.Branch<T3, T4> branch)
        where T1 : ICloneable
        where T2 : ICloneable
        where T3 : ICloneable
        where T4 : ICloneable
        => new(branch);
}