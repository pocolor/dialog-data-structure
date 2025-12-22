using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace DialogDataStructure;

public static class Util
{
    public static Dialog.Text NewText(string id, string content) => new(id, content);
    public static Dialog.Branch NewBranch(List<Dialog.Text> texts) => NewBranch(texts, []);
    public static Dialog.Branch NewBranch(List<Dialog.Text> texts, List<Dialog.Branch> nextBranches) =>
        new(){Texts = texts, NextBranches = nextBranches};
    public static Dialog NewDialog(string name, Dialog.Branch start) => new(){Name = name, Start = start};
    
    /*******************************************************************/
    
    public static void ConsoleWriteBranchTexts(Dialog.Branch branch, double timeScale = 0)
    {
        Thread.Sleep(1000);
        foreach (var t in branch.Texts)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(t.Id);
            Console.ResetColor();
            Console.Write($": {t.Content}\n");
            Thread.Sleep(1000 * (int)Math.Ceiling(Math.Log(t.Content.Length) * timeScale));
        }
    }

    public static void ConsoleWriteBranchChoices(Dialog.Branch branch)
    {
        for (var i = 0; i < branch.NextBranches.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{i + 1}]");
            Console.ResetColor();
            Console.Write($" {branch.NextBranches[i].Texts[0].Content}\n");
        }
    }

    public static void ConsoleReadBranchChoice(Dialog.Branch branch, out int choice)
    {
        Debug.Assert(branch.NextBranches.Count < 10, "for more than 9 choices implement a different reading technique");
        Debug.Assert(branch.NextBranches.Count > 0);
        
        while (true)
        {
            var c = Console.ReadKey(true).KeyChar - '1';
            if (c < 0 || c >= branch.NextBranches.Count) continue;
            choice = c;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{char.ConvertFromUtf32(c + '1')}]");
            Console.ResetColor();
            break;
        }
    }

    public static void ConsoleRunDialog(Dialog dialog, double timeScale = 0.1)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(dialog.Name ?? "Unnamed Dialog");
        Console.ResetColor();

        var curr = dialog.Start;
        while (true)
        {
            ConsoleWriteBranchTexts(curr, timeScale);
            if (!curr.Continues) break;
            ConsoleWriteBranchChoices(curr);
            ConsoleReadBranchChoice(curr, out var choice);
            curr = curr.NextBranches[choice];
        }
    }
    
    /*******************************************************************/

    public static void ConsoleWriteBranchTexts(ImmutableDialog.ImmutableBranch branch, double timeScale = 0)
    {
        Thread.Sleep(1000);
        foreach (var t in branch.Texts)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(t.Id);
            Console.ResetColor();
            Console.Write($": {t.Content}\n");
            Thread.Sleep(1000 * (int)Math.Ceiling(Math.Log(t.Content.Length) * timeScale));
        }
    }

    public static void ConsoleWriteBranchChoices(ImmutableDialog.ImmutableBranch branch)
    {
        for (var i = 0; i < branch.NextBranches.Count; i++)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{i + 1}]");
            Console.ResetColor();
            Console.Write($" {branch.NextBranches[i].Texts[0].Content}\n");
        }
    }

    public static void ConsoleReadBranchChoice(ImmutableDialog.ImmutableBranch branch, out int choice)
    {
        Debug.Assert(branch.NextBranches.Count < 10, "for more than 9 choices implement a different reading technique");
        Debug.Assert(branch.NextBranches.Count > 0);
        
        while (true)
        {
            var c = Console.ReadKey(true).KeyChar - '1';
            if (c < 0 || c >= branch.NextBranches.Count) continue;
            choice = c;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{char.ConvertFromUtf32(c + '1')}]");
            Console.ResetColor();
            return;
        }
    }

    public static void ConsoleRunDialog(ImmutableDialog dialog, double timeScale = 0.1)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(dialog.Name ?? "Unnamed Dialog");
        Console.ResetColor();

        var curr = dialog.Start;
        while (true)
        {
            ConsoleWriteBranchTexts(curr, timeScale);
            if (!curr.Continues) break;
            ConsoleWriteBranchChoices(curr);
            ConsoleReadBranchChoice(curr, out var choice);
            curr = curr.NextBranches[choice];
        }
    }
}