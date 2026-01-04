using System;

namespace DialogDataStructure;

/// <summary>
/// Builder pattern for <see cref="Dialog{TBranchBeginNode,TBranchNode}"/>.
/// </summary>
/// 
/// <typeparam name="TBeginNode">
/// Type of the node that starts a branch.
/// Must implement <see cref="ICloneable"/>.
/// </typeparam>
/// 
/// <typeparam name="TNode">
/// Type of the dialog nodes inside branches.
/// Must implement <see cref="ICloneable"/>.
/// </typeparam>
public class DialogBuilder<TBeginNode, TNode>
    where TBeginNode : ICloneable
    where TNode : ICloneable
{
    private readonly Dialog<TBeginNode, TNode> _dialog = new();

    /// <summary>
    /// Initializes a new dialog builder with an optional dialog name.
    /// </summary>
    /// <param name="name">Optional dialog name.</param>
    public DialogBuilder(string? name = null)
    {
        _dialog.Name = name;
    }

    /// <summary>
    /// Syntax sugar. Calls <see cref="DialogBuilder{TBeginNode,TNode}"/> constructor.
    /// </summary>
    /// <param name="name">Optional dialog name.</param>
    public static DialogBuilder<TBeginNode, TNode> Create(string? name = null) => new(name);

    /// <summary>
    /// Defines the root branch of the dialog.
    /// </summary>
    /// <param name="build">
    /// A configuration action receiving a <see cref="BranchBuilder{TBeginNode,TNode}"/>
    /// used to define the branch.
    /// </param>
    /// <returns>
    /// This builder instance.
    /// </returns>
    public DialogBuilder<TBeginNode, TNode> StartBranch(Action<BranchBuilder<TBeginNode, TNode>> build)
    {
        var branchBuilder = new BranchBuilder<TBeginNode, TNode>(null);
        build(branchBuilder);
        _dialog.Start = branchBuilder.Build();
        return this;
    }

    /// <summary>
    /// Builds the final dialog instance.
    /// The returned dialog is a deep copy of the internally built structure.
    /// </summary>
    /// <returns>A completed dialog instance.</returns>
    public Dialog<TBeginNode, TNode> Build() => (Dialog<TBeginNode, TNode>) _dialog.Clone();
}

/// <summary>
/// Builder pattern for <see cref="Dialog{TBranchBeginNode,TBranchNode}.Branch"/>
/// </summary>
///
/// <param name="previous">
/// Reference to the parent branch. If <c>null</c>, this branch is the root branch.
/// </param>
/// 
/// <typeparam name="TBeginNode">
/// Type of the node that starts a branch.
/// Must implement <see cref="ICloneable"/>.
/// </typeparam>
/// 
/// <typeparam name="TNode">
/// Type of the dialog nodes inside branches.
/// Must implement <see cref="ICloneable"/>.
/// </typeparam>
public class BranchBuilder<TBeginNode, TNode>(Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode>? previous)
    where TBeginNode : ICloneable
    where TNode : ICloneable
{
    private readonly Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode> _branch = new()
    {
        Previous = previous
    };

    /// <summary>
    /// Sets the beginning node of this branch.
    /// </summary>
    /// <param name="beginNode">The beginning node.</param>
    /// <returns>This builder instance.</returns>
    public BranchBuilder<TBeginNode, TNode> BeginNode(TBeginNode beginNode)
    {
        _branch.BeginNode = beginNode;
        return this;
    }

    /// <summary>
    /// Adds dialog nodes to this branch.
    /// </summary>
    /// <param name="nodes">Array of dialog nodes to append.</param>
    /// <returns>This builder instance.</returns>
    public BranchBuilder<TBeginNode, TNode> Nodes(params TNode[] nodes)
    {
        _branch.Nodes.AddRange(nodes);
        return this;
    }

    /// <summary>
    /// Creates and links a child branch to this branch.
    /// </summary>
    /// <param name="build">
    /// A configuration action receiving a new branch builder instance.
    /// </param>
    /// <returns>This builder instance.</returns>
    public BranchBuilder<TBeginNode, TNode> Branch(Action<BranchBuilder<TBeginNode, TNode>> build)
    {
        var childBuilder = new BranchBuilder<TBeginNode, TNode>(_branch);
        build(childBuilder);

        var child = childBuilder.Build();
        child.Previous = _branch;
        _branch.NextBranches.Add(child);

        return this;
    }

    /// <summary>
    /// Builds the Branch.
    /// </summary>
    /// <returns>A branch.</returns>
    public Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode> Build() => _branch;
}