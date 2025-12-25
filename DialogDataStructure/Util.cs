using System;
using System.Diagnostics;
using System.Threading;

namespace DialogDataStructure;

public static class Util
{
    public struct NameText(string name = "", string text = "") : ICloneable
    {
        public string Name { get; set; } = name;
        public string Text { get; set; } = text;

        public object Clone() => new NameText(Name, Text);
    }
    
    /*******************************************************************/
    
    public static void ConsoleWriteBranchNodes(Dialog<string, NameText>.Branch<string, NameText> branch, double timeScale = 0)
    {
        Thread.Sleep(1000);
        foreach (var node in branch.Nodes)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(node.Name);
            Console.ResetColor();
            Console.Write($": {node.Text}\n");
            Thread.Sleep(1000 * (int)Math.Ceiling(Math.Log(node.Text.Length) * timeScale));
        }
    }

    public static void ConsoleWriteBranchChoices(Dialog<string, NameText>.Branch<string, NameText> branch)
    {
        for (var i = 0; i < branch.NextBranches.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{i + 1}]");
            Console.ResetColor();
            Console.Write($" {branch.NextBranches[i].BeginNode}\n");
        }
    }

    public static void ConsoleReadBranchChoice(Dialog<string, NameText>.Branch<string, NameText> branch, out int choice)
    {
        Debug.Assert(branch.NextBranches.Count < 10, "for more than 9 choices implement a different reading technique");
        Debug.Assert(branch.NextBranches.Count > 0);
        
        while (true)
        {
            var i = Console.ReadKey(true).KeyChar - '1';
            if (i < 0 || i >= branch.NextBranches.Count) continue;
            choice = i;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{char.ConvertFromUtf32(i + '1')}]: {branch.NextBranches[i].BeginNode}");
            Console.ResetColor();
            break;
        }
    }

    public static void ConsoleRunDialog(Dialog<string, NameText> dialog, double timeScale = 0.1)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(dialog.Name ?? "Unnamed Dialog");
        Console.ResetColor();

        var curr = dialog.Start;
        while (true)
        {
            ConsoleWriteBranchNodes(curr, timeScale);
            if (!curr.Continues) break;
            ConsoleWriteBranchChoices(curr);
            ConsoleReadBranchChoice(curr, out var choice);
            curr = curr.NextBranches[choice];
        }
    }
}