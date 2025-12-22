using System;
using System.Linq;

namespace DialogDataStructure;

public class DialogBuilder
{
    private readonly Dialog _dialog = new();

    public DialogBuilder(string? name = null)
    {
        _dialog.Name = name;
    }

    public static DialogBuilder Create(string? name = null) => new(name);

    public DialogBuilder StartBranch(Action<BranchBuilder> build)
    {
        var branchBuilder = new BranchBuilder(null);
        build(branchBuilder);
        _dialog.Start = branchBuilder.Build();
        return this;
    }

    public Dialog Build() => _dialog.Copy();
}

public class BranchBuilder(Dialog.Branch? previous)
{
    private readonly Dialog.Branch _branch = new()
    {
        Previous = previous
    };

    public BranchBuilder Text(string id, string content)
    {
        _branch.Texts.Add(new Dialog.Text(id, content));
        return this;
    }

    public BranchBuilder Text(Dialog.Text text)
    {
        _branch.Texts.Add(text.Copy());
        return this;
    }

    public BranchBuilder Texts(params Dialog.Text[] texts)
    {
        _branch.Texts.AddRange(texts.Select(e => e.Copy()));
        return this;
    }

    public BranchBuilder Branch(Action<BranchBuilder> build)
    {
        var childBuilder = new BranchBuilder(_branch);
        build(childBuilder);

        var child = childBuilder.Build();
        child.Previous = _branch;
        _branch.NextBranches.Add(child);

        return this;
    }

    public Dialog.Branch Build()
    {
        return _branch.Texts.Count == 0 ? throw new InvalidOperationException("Branch must contain at least one Text.") : _branch;
    }
}