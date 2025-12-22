using System;

namespace DialogDataStructure;

internal static class Program
{
    private static Dialog.Text PlayerText(string content) => new("Player", content);
    private static Dialog.Text NpcText(string content) => new("Npc", content);
    
    private static void Main()
    {
        const string npc = "Npc";
        const string player = "Player";
        
        Dialog dialog = new()
        {
            Name = "Mega super duper dialog",
            Start = new Dialog.Branch
            {
                Texts =
                {
                    NpcText("Hello adventurer!"),
                    NpcText("I have a puzzle for you."),
                    NpcText("What is 1 + 1 equal to?")
                },
                NextBranches =
                {
                    new Dialog.Branch
                    {
                        Texts = [
                            new Dialog.Text(player, "It's 1!"),
                            new Dialog.Text(npc, "This is not boolean algebra. It's 2.")
                        ]
                    },
                    new Dialog.Branch
                    {
                        Texts = [
                            new Dialog.Text(player, "It's 2!"),
                            new Dialog.Text(npc, "You are right!"),
                            new Dialog.Text(npc, "You are the smartest JavaScript user I have ever encountered.")
                        ]
                    },
                    new Dialog.Branch
                    {
                        Texts = [
                            new Dialog.Text(player, "It's 3!"),
                            new Dialog.Text(npc, "No dumbass. It's 2.")
                        ]
                    },
                    new Dialog.Branch
                    {
                        Texts = [
                            new Dialog.Text(player, "It's 4!"),
                            new Dialog.Text(npc, "No dumbass. It's 2."),
                            new Dialog.Text(player, "Am I really this dumb?"),
                            new Dialog.Text(npc, "Seems so...")
                        ]
                    }
                }
            }
        };

        Util.ConsoleRunDialog(dialog);
        Console.WriteLine("---------------------------------------------------------------");
        Util.ConsoleRunDialog(dialog.ToImmutableDialog());
    }
}