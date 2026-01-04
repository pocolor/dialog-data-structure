using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DialogDataStructure;

/// <summary>Tree-like data structure for holding dialogs.
/// Consists of branches. Each branch has a beginning node,
/// a list of dialog nodes and references to subsequent branches.
/// Supports cloning and JSON (de)serialization helpers.
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
public class Dialog<TBranchBeginNode, TBranchNode> : ICloneable
    where TBranchBeginNode : ICloneable
    where TBranchNode : ICloneable
{
    /// <summary>
    /// Optional name of the dialog.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Root branch from which the dialog starts.
    /// </summary>
    public Branch<TBranchBeginNode, TBranchNode> Start { get; set; }

    /// <summary>
    /// Represents a single dialog branch consisting of a beginning node,
    /// list of dialog nodes and references to subsequent branches.
    /// Supports cloning and JSON (de)serialization helpers.
    /// </summary>
    public class Branch<TBeginNode, TNode> : ICloneable, IJsonOnSerializing, IJsonOnSerialized
        where TBeginNode : ICloneable
        where TNode : ICloneable
    {
        /// <summary>
        /// Reference to the previous branch in the dialog structure if any.
        /// </summary>
        [JsonIgnore] public Branch<TBeginNode, TNode>? Previous { get; set; }

        /// <summary>
        /// An entry point to a branch. Usually used in choosing of a following branch in a dialog.
        /// </summary>
        public TBeginNode? BeginNode { get; set; }
        
        /// <summary>
        /// List of dialog nodes. Can be empty.
        /// </summary>
        public List<TNode> Nodes { get; set; } = [];
        
        /// <summary>
        /// List of subsequent branches. Can be empty if the branch doesn't continue.
        /// </summary>
        public List<Branch<TBeginNode, TNode>> NextBranches { get; set; } = [];

        /// <summary>
        /// Checks if the branch has a previous branch (a parent).
        /// </summary>
        [JsonIgnore] public bool HasPrevious => Previous is not null;
        
        /// <summary>
        /// Checks if the branch has any subsequent branches (children).
        /// </summary>
        [JsonIgnore] public bool Continues => NextBranches.Count > 0;

        /// <summary>
        /// Called before JSON serialization. Temporally sets <c>Nodes</c> and <c>NextBranches</c>
        /// to null if empty so these fields can be ignored during serialization if empty.
        /// </summary>
        public void OnSerializing()
        {
            if (Nodes.Count == 0) Nodes = null!;
            if (NextBranches.Count == 0) NextBranches = null!;
        }

        /// <summary>
        /// Called after JSON serialization. Initializes lists if null.
        /// </summary>
        public void OnSerialized()
        {
            Nodes ??= [];
            NextBranches ??= [];
        }

        /// <summary>
        /// Empty constructor. Is required for JSON deserialization.
        /// </summary>
        public Branch()
        {
        }

        /// <summary>
        /// Copy constructor. Creates a deep copy of a branch.
        /// Works by recursively copying subsequent branches.
        /// Doesn't check for cycles - error may be thrown.
        /// </summary>
        /// <param name="branch">
        /// The branch to be copied.
        /// </param>
        public Branch(Branch<TBeginNode, TNode> branch)
        {
            BeginNode = (TBeginNode?) branch.BeginNode?.Clone();
            Nodes = branch.Nodes.ConvertAll(e => (TNode) e.Clone());
            NextBranches = branch.NextBranches.ConvertAll(e => (Branch<TBeginNode, TNode>) e.Clone());
            NextBranches.ForEach(e => e.Previous = this);
        }

        /// <inheritdoc/>
        public object Clone() => new Branch<TBeginNode, TNode>(this);
        
        /// <summary>
        /// Recursively links all child branches to its parent branch as their previous reference.
        /// </summary>
        public void LinkPrevious() => NextBranches.ForEach(e => { e.Previous = this; e.LinkPrevious(); });

        /// <inheritdoc/>
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

    /// <summary>
    /// Empty constructor. Is required for JSON deserialization.
    /// </summary>
    public Dialog()
    {
    }

    /// <summary>
    /// Copy constructor. Creates a deep copy of the dialog. Calls Branch's copy constructor.
    /// See Branch's copy constructor. (See tag doesn't work for some reason.)
    /// </summary>
    /// <param name="dialog">
    /// The dialog to be copied.
    /// </param>
    
    // <see cref="Branch{TBeginNode,TNode}.#ctor(Branch{TBeginNode,TNode})"/>
    // <see cref="Dialog{TBranchBeginNode,TBranchNode}.Branch{TBranchBeginNode,TBranchNode}.#ctor(Dialog{TBranchBeginNode,TBranchNode}.Branch{TBranchBeginNode,TBranchNode})"/>
    // <see cref="DialogDataStructure.Dialog{TBranchBeginNode,TBranchNode}.Branch{TBranchBeginNode,TBranchNode}.#ctor(DialogDataStructure.Dialog{TBranchBeginNode,TBranchNode}.Branch{TBranchBeginNode,TBranchNode})"/>
    // <see cref="Branch{TBeginNode,TNode}.Branch(Branch{TBeginNode,TNode})"/>
    // <see cref="Dialog{TBranchBeginNode,TBranchNode}.Branch{TBranchBeginNode,TBranchNode}.Branch(Dialog{TBranchBeginNode,TBranchNode}.Branch{TBranchBeginNode,TBranchNode})"/>
    // <see cref="DialogDataStructure.Dialog{TBranchBeginNode,TBranchNode}.Branch{TBranchBeginNode,TBranchNode}.Branch(DialogDataStructure.Dialog{TBranchBeginNode,TBranchNode}.Branch{TBranchBeginNode,TBranchNode})"/>
    // none of these work for some reason
    public Dialog(Dialog<TBranchBeginNode, TBranchNode> dialog)
    {
        Name = dialog.Name;
        Start = new Branch<TBranchBeginNode, TBranchNode>(dialog.Start);
    }

    /// <inheritdoc/>
    public object Clone() => new Dialog<TBranchBeginNode, TBranchNode>(this);

    /// <inheritdoc/>
    public override string ToString()
    {
        return new StringBuilder()
            .Append(nameof(Dialog<TBranchBeginNode, TBranchNode>)).Append('{')
            .Append("Name = ").Append(Name).Append(", ")
            .Append("Start = ").Append(Start).Append('}')
            .ToString();
    }
}