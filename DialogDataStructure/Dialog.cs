using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DialogDataStructure;

public class Dialog<TBranchBeginNode, TBranchNode> : ICloneable
    where TBranchBeginNode : ICloneable
    where TBranchNode : ICloneable
{
    public string? Name { get; set; }
    public Branch<TBranchBeginNode, TBranchNode> Start { get; set; }

    public class Branch<TBeginNode, TNode> : ICloneable, IJsonOnSerializing, IJsonOnSerialized
        where TBeginNode : ICloneable
        where TNode : ICloneable
    {
        [JsonIgnore] public Branch<TBeginNode, TNode>? Previous { get; set; }

        public TBeginNode? BeginNode { get; set; }
        public List<TNode> Nodes { get; set; } = [];
        public List<Branch<TBeginNode, TNode>> NextBranches { get; set; } = [];

        [JsonIgnore] public bool HasPrevious => Previous is not null;
        [JsonIgnore] public bool Continues => NextBranches.Count > 0;

        public void OnSerializing()
        {
            if (Nodes.Count == 0) Nodes = null!;
            if (NextBranches.Count == 0) NextBranches = null!;
        }

        public void OnSerialized()
        {
            Nodes ??= [];
            NextBranches ??= [];
        }

        public Branch()
        {
        }

        public Branch(Branch<TBeginNode, TNode> branch)
        {
            BeginNode = (TBeginNode?) branch.BeginNode?.Clone();
            Nodes = branch.Nodes.ConvertAll(e => (TNode) e.Clone());
            NextBranches = branch.NextBranches.ConvertAll(e => (Branch<TBeginNode, TNode>) e.Clone());
            NextBranches.ForEach(e => e.Previous = this);
        }

        public object Clone() => new Branch<TBeginNode, TNode>(this);
        public void LinkPrevious() => NextBranches.ForEach(e => { e.Previous = this; e.LinkPrevious(); });

        public override string ToString()
        {
            return new StringBuilder()
                .Append(nameof(Branch<TBeginNode, TNode>)).Append('{')
                .Append("Previous is not null = ").Append(Previous is not null).Append(", ")
                .Append("BeginNode = ").Append(BeginNode).Append(", ")
                .Append("Nodes = ").Append(Nodes).Append(", ")
                .Append("NextBranches.Count = ").Append(NextBranches.Count).Append('}')
                .ToString();
        }
    }

    public Dialog()
    {
    }

    public Dialog(Dialog<TBranchBeginNode, TBranchNode> dialog)
    {
        Name = dialog.Name;
        Start = (Branch<TBranchBeginNode, TBranchNode>) dialog.Start.Clone();
    }

    public object Clone() => new Dialog<TBranchBeginNode, TBranchNode>(this);

    public override string ToString()
    {
        return new StringBuilder()
            .Append(nameof(Dialog<TBranchBeginNode, TBranchNode>)).Append('{')
            .Append("Name = ").Append(Name).Append(", ")
            .Append("Start = ").Append(Start).Append('}')
            .ToString();
    }
}