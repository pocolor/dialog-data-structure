using System;
using System.Diagnostics;
using System.Threading;

namespace DialogDataStructure;

public static class Util
{
    public struct SingleText(string text = "") : ICloneable
    {
        public string Text { get; set; } = text;

        public object Clone() => new SingleText(Text);
        public static implicit operator SingleText(string text) => new(text);
        public static implicit operator string(SingleText text) => text.Text;
        public override string ToString() => Text;
    }
    
    public struct NameText(string name = "", string text = "") : ICloneable
    {
        public string Name { get; set; } = name;
        public string Text { get; set; } = text;

        public object Clone() => new NameText(Name, Text);
        public override string ToString() => $"{Name}: {Text}";
    }
    
    /*******************************************************************/
    
    public static void ConsoleWriteBranchNodes<T1, T2>(Dialog<T1, T2>.Branch<T1, T2> branch, int sleepMillis = 0, ConsoleColor? color = null)
        where T1 : ICloneable
        where T2 : ICloneable
    {
        Console.ForegroundColor = color ?? Console.ForegroundColor;
        Thread.Sleep(sleepMillis);
        foreach (var node in branch.Nodes)
        {
            Console.WriteLine(node);
            Thread.Sleep(sleepMillis);
        }
        Console.ResetColor();
    }

    public static void ConsoleWriteBranchChoices<T1, T2>(Dialog<T1, T2>.Branch<T1, T2> branch, ConsoleColor? color = null)
        where T1 : ICloneable
        where T2 : ICloneable
    {
        Console.ForegroundColor = color ?? Console.ForegroundColor;
        for (var i = 0; i < branch.NextBranches.Count; i++)
            Console.WriteLine($"[{i + 1}] {branch.NextBranches[i].BeginNode}");
        
        Console.ResetColor();
    }

    public static void ConsoleReadBranchChoice<T1, T2>(Dialog<T1, T2>.Branch<T1, T2> branch, out int choice, ConsoleColor? color = null)
        where T1 : ICloneable
        where T2 : ICloneable
    {
        Debug.Assert(branch.NextBranches.Count < 10, "for more than 9 choices implement a different reading technique");
        Debug.Assert(branch.NextBranches.Count > 0);
        
        while (true)
        {
            var i = Console.ReadKey(true).KeyChar - '1';
            if (i < 0 || i >= branch.NextBranches.Count) continue;
            choice = i;
            
            Console.ForegroundColor = color ?? Console.ForegroundColor;
            Console.WriteLine($"[{char.ConvertFromUtf32(i + '1')}]: {branch.NextBranches[i].BeginNode}");
            Console.ResetColor();
            break;
        }
    }

    public static void ConsoleRunDialog<T1, T2>(Dialog<T1, T2> dialog, int sleepMillis = 500)
        where T1 : ICloneable
        where T2 : ICloneable
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(dialog.Name ?? "Unnamed Dialog");
        Console.ResetColor();

        var curr = dialog.Start;
        while (true)
        {
            ConsoleWriteBranchNodes(curr, sleepMillis);
            if (!curr.Continues) break;
            ConsoleWriteBranchChoices(curr, ConsoleColor.Green);
            ConsoleReadBranchChoice(curr, out var choice, ConsoleColor.Cyan);
            curr = curr.NextBranches[choice];
        }
    }
}