using System;

namespace DialogDataStructure;

public class DialogBuilder<TBeginNode, TNode>
    where TBeginNode : ICloneable
    where TNode : ICloneable
{
    private readonly Dialog<TBeginNode, TNode> _dialog = new();

    public DialogBuilder(string? name = null)
    {
        _dialog.Name = name;
    }

    public static DialogBuilder<TBeginNode, TNode> Create(string? name = null) => new(name);

    public DialogBuilder<TBeginNode, TNode> StartBranch(Action<BranchBuilder<TBeginNode, TNode>> build)
    {
        var branchBuilder = new BranchBuilder<TBeginNode, TNode>(null);
        build(branchBuilder);
        _dialog.Start = branchBuilder.Build();
        return this;
    }

    public Dialog<TBeginNode, TNode> Build() => (Dialog<TBeginNode, TNode>) _dialog.Clone();
}

public class BranchBuilder<TBeginNode, TNode>(Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode>? previous)
    where TBeginNode : ICloneable
    where TNode : ICloneable
{
    private readonly Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode> _branch = new()
    {
        Previous = previous
    };

    public BranchBuilder<TBeginNode, TNode> BeginNode(TBeginNode beginNode)
    {
        _branch.BeginNode = beginNode;
        return this;
    }

    public BranchBuilder<TBeginNode, TNode> Nodes(params TNode[] nodes)
    {
        _branch.Nodes.AddRange(nodes);
        return this;
    }

    public BranchBuilder<TBeginNode, TNode> Branch(Action<BranchBuilder<TBeginNode, TNode>> build)
    {
        var childBuilder = new BranchBuilder<TBeginNode, TNode>(_branch);
        build(childBuilder);

        var child = childBuilder.Build();
        child.Previous = _branch;
        _branch.NextBranches.Add(child);

        return this;
    }

    public Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode> Build()
    {
        return _branch.Nodes.Count == 0 ? throw new InvalidOperationException("Branch must contain at least one Text.") : _branch;
    }
}