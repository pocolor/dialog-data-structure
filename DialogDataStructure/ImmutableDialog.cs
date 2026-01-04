using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace DialogDataStructure;

/// <summary>Immutable tree-like data structure for holding dialogs.
/// Consists of branches. Each branch has a beginning node,
/// a list of dialog nodes and references to subsequent branches.
/// </summary>
/// 
/// <typeparam name="TBranchBeginNode">
/// Type of the node that starts a branch.
/// Must implement <see cref="ICloneable"/>.
/// </typeparam>
/// 
/// <typeparam name="TBranchNode">
/// Type of the dialog nodes inside branches.
/// Must implement <see cref="ICloneable"/>.
/// </typeparam>
public class ImmutableDialog<TBranchBeginNode, TBranchNode>
    where TBranchBeginNode : ICloneable
    where TBranchNode : ICloneable
{
    /// <summary>
    /// Optional name of the dialog.
    /// </summary>
    public string? Name { get; }
    
    /// <summary>
    /// Root branch from which the dialog starts.
    /// </summary>
    public ImmutableBranch<TBranchBeginNode, TBranchNode> Start { get; }

    /// <summary>
    /// Represents a single dialog branch consisting of a beginning node,
    /// list of dialog nodes and references to subsequent branches.
    /// </summary>
    public class ImmutableBranch<TBeginNode, TNode>
        where TBeginNode : ICloneable
        where TNode : ICloneable
    {
        /// <summary>
        /// Reference to the previous branch in the dialog structure if any.
        /// </summary>
        public ImmutableBranch<TBeginNode, TNode>? Previous { get; private set; }
        
        /// <summary>
        /// An entry point to a branch. Usually used in choosing of a following branch in a dialog.
        /// </summary>
        public TBeginNode BeginNode { get; }
        
        /// <summary>
        /// List of dialog nodes. Can be empty.
        /// </summary>
        public ImmutableList<TNode> Nodes { get; }
        
        /// <summary>
        /// List of subsequent branches. Can be empty if the branch doesn't continue.
        /// </summary>
        public ImmutableList<ImmutableBranch<TBeginNode, TNode>> NextBranches { get; }

        /// <summary>
        /// Checks if the branch has a previous branch (a parent).
        /// </summary>
        public bool HasPrevious => Previous is not null;
        
        /// <summary>
        /// Checks if the branch has any subsequent branches (children).
        /// </summary>
        public bool Continues => NextBranches.Count > 0;

        /// <summary>
        /// Converts Dialog.Branch to ImmutableDialog.ImmutableBranch. Performs a deep copy.
        /// </summary>
        /// <param name="branch">The branch to be copied/converted.</param>
        public ImmutableBranch(Dialog<TBranchBeginNode, TBranchNode>.Branch<TBeginNode, TNode> branch)
        {
            BeginNode = branch.BeginNode;
            Nodes = branch.Nodes.ConvertAll(e => (TNode) e.Clone()).ToImmutableList();
            NextBranches = branch.NextBranches.ConvertAll(e => e.ToImmutableBranch()).ToImmutableList();
            NextBranches.ForEach(e => e.Previous = this);
        }

        /// <summary>
        /// Converts ImmutableBranch to a mutable Branch.
        /// </summary>
        /// <returns>Mutable Branch.</returns>
        public Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode> ToBranch()
        {
            Dialog<TBeginNode, TNode>.Branch<TBeginNode, TNode> branch = new();
            branch.BeginNode = BeginNode;
            branch.Nodes = Nodes.ConvertAll(e => (TNode) e.Clone()).ToList();
            branch.NextBranches = NextBranches.ConvertAll(e => e.ToBranch()).ToList();
            branch.NextBranches.ForEach(e => e.Previous = branch);
            return branch;
        }

        /// <inheritdoc/>
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

    /// <summary>
    /// Converts Dialog to ImmutableDialog. Performs a deep copy.
    /// </summary>
    /// <param name="dialog">The dialog to be copied/converted.</param>
    public ImmutableDialog(Dialog<TBranchBeginNode, TBranchNode> dialog)
    {
        Name = dialog.Name;
        Start = dialog.Start.ToImmutableBranch();
    }

    /// <summary>
    /// Converts ImmutableDialog to a mutable Dialog.
    /// </summary>
    /// <returns>Mutable Dialog.</returns>
    public Dialog<TBranchBeginNode, TBranchNode> ToDialog() => new()
    {
        Name = Name,
        Start = Start.ToBranch()
    };

    /// <inheritdoc/>
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
    /// <summary>
    /// Converts a mutable Dialog to an ImmutableDialog.
    /// </summary>
    /// <returns>ImmutableDialog</returns>
    public static ImmutableDialog<T1, T2> ToImmutableDialog<T1, T2>(this Dialog<T1, T2> dialog)
        where T1 : ICloneable
        where T2 : ICloneable
        => new(dialog);
    
    /// <summary>
    /// Converts mutable Branch to an ImmutableBranch.
    /// </summary>
    /// <returns>ImmutableBranch</returns>
    public static ImmutableDialog<T1, T2>.ImmutableBranch<T3, T4> ToImmutableBranch<T1, T2, T3, T4>(this Dialog<T1, T2>.Branch<T3, T4> branch)
        where T1 : ICloneable
        where T2 : ICloneable
        where T3 : ICloneable
        where T4 : ICloneable
        => new(branch);
}