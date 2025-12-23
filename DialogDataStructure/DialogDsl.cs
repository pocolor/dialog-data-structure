using System;
using System.Collections.Generic;
using System.Linq;

namespace DialogDataStructure;

public static class DialogDsl
{
    public static DialogNode Dialog(string? name = null) => new(name);

    public static BranchNode Branch() => new();
}

public sealed class DialogNode(string? name)
{
    private BranchNode? _start;

    public Dialog Start(BranchNode start)
    {
        _start = start;
        if (_start is null)
            throw new InvalidOperationException("Dialog must have a start branch.");

        return new Dialog
        {
            Name = name,
            Start = _start.Build(null)
        };
    }
}

public sealed class BranchNode
{
    private readonly List<Dialog.Text> _texts = [];
    private readonly List<BranchNode> _next = [];

    public BranchNode Texts(params Tuple<string, string>[] texts)
    {
        _texts.AddRange(texts.Select(e => new Dialog.Text(e.Item1, e.Item2)));
        return this;
    }

    public BranchNode Texts(params Dialog.Text[] texts)
    {
        _texts.AddRange(texts.Select(e => e.Copy()));
        return this;
    }

    public BranchNode Then(params BranchNode[] branches)
    {
        _next.AddRange(branches);
        return this;
    }

    public Dialog.Branch Build(Dialog.Branch? previous)
    {
        if (_texts.Count == 0)
            throw new InvalidOperationException("Each branch must contain at least one Text.");

        var branch = new Dialog.Branch
        {
            Previous = previous,
            Texts = _texts.ConvertAll(t => t.Copy())
        };
        branch.NextBranches.AddRange(_next.ConvertAll(t => t.Build(branch)));

        return branch;
    }
}