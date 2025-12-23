namespace DialogDataStructure;

internal static class Program
{
    private static void Main()
    {
        Util.ConsoleRunDialog(ExampleDialogBuilder());
        Util.ConsoleRunDialog(ExampleDialogDsl());
        Util.ConsoleRunDialog(DialogSerializer.Load("../../../dialogs/dialog.json"));
    }

    private static Dialog ExampleDialogBuilder()
    {
        return DialogBuilder
            .Create("Example dialog cat")
            .StartBranch(start => start
                .Texts(
                    new Dialog.Text("Cat", "Meow"),
                    new Dialog.Text("Person", "Hey! You like my cat?")
                )
                .Branch(branch => branch
                    .Texts(
                        new Dialog.Text("You", "No."),
                        new Dialog.Text("You", "*runs away*")
                    )
                )
                .Branch(branch => branch
                    .Texts(
                        new Dialog.Text("You", "Yes.")
                    )
                )
                .Branch(branch => branch
                    .Texts(
                        new Dialog.Text("You", "Maybe.")
                    )
                )
            ).Build();
    }

    private static Dialog ExampleDialogDsl()
    {
        return DialogDsl
            .Dialog("Example dialog dog")
            .Start(
                DialogDsl.Branch()
                    .Texts(
                        ("Dog", "Bark"),
                        ("Person", "Hey! You like my dog?")
                    )
                    .Then(
                        DialogDsl.Branch().Texts(
                            ("You", "No."),
                            ("You", "*runs away*")
                        ),
                        DialogDsl.Branch().Texts(
                            ("You", "Yes.")
                        ),
                        DialogDsl.Branch().Texts(
                            ("You", "Maybe.")
                        )
                    )
            );
    }
}