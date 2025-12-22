namespace DialogDataStructure;

internal static class Program
{
    private static Dialog.Text PlayerText(string content) => new("Player", content);
    private static Dialog.Text NpcText(string content) => new("Npc", content);

    private static void Main()
    {
        var dialog = DialogBuilder
            .Create("Mega super duper dialog")
            .StartBranch(b => b
                .Texts(
                    NpcText("Hello adventurer!"),
                    NpcText("I have a puzzle for you."),
                    NpcText("What is 1 + 1 equal to?")
                )
                .Branch(b2 => b2
                    .Texts(
                        PlayerText("It's 1!"),
                        NpcText("This is not boolean algebra. It's 2.")
                    )
                )
                .Branch(b2 => b2
                    .Texts(
                        PlayerText("It's 2!"),
                        NpcText("You are right!"),
                        NpcText("You are the smartest JavaScript user I have ever encountered.")
                    )
                )
                .Branch(b2 => b2
                    .Texts(
                        PlayerText("It's 3!"),
                        NpcText("No dumbass. It's 2.")
                    )
                )
                .Branch(b2 => b2
                    .Texts(
                        PlayerText("It's 4!"),
                        NpcText("No dumbass. It's 2."),
                        PlayerText("Am I really this dumb?"),
                        NpcText("Seems so...")
                    )
                )
            )
            .Build();


        Util.ConsoleRunDialog(dialog);
    }
}