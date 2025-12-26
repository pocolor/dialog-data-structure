namespace DialogDataStructure;

internal static class Program
{
    private static void Main()
    {
        var dialog = ExampleDialogBuilder();
        DialogSerializer.Save(dialog, "../../../dialogs/dialog.tmp.json");
        
        Util.ConsoleRunDialog(dialog);
        Util.ConsoleRunDialog(DialogSerializer.Load<Util.SingleText, Util.NameText>("../../../dialogs/dialog.json"));
    }

    private static Dialog<Util.SingleText, Util.NameText> ExampleDialogBuilder()
    {
        return DialogBuilder<Util.SingleText, Util.NameText>
            .Create("Example dialog cat")
            .StartBranch(start => start
                .Nodes(
                    new Util.NameText("Cat", "*meow*"),
                    new Util.NameText("Person", "Hey! You like my cat?")
                )
                .Branch(branch => branch
                    .BeginNode("No")
                    .Nodes(
                        new Util.NameText("You", "I like dogs.")
                    )
                )
                .Branch(branch => branch
                    .BeginNode("Maybe")
                    .Nodes(
                        new Util.NameText("You", "I don't really like cats.")
                    )
                )
                .Branch(branch => branch
                    .BeginNode("Yes")
                    .Nodes(
                        new Util.NameText("You", "I'm a normal person so I do like cats.")
                    )
                )
            ).Build();
    }
}