using Minesweeper.Models;

public class MineSweeper
{
    public static void Main(String[] args)
    {
        Grid grid = new Grid(8);
        grid.GenerateNew();

        while (true)
        {
            System.Console.Clear();
            if (grid.ErrorMessage != String.Empty)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(grid.ErrorMessage);
                System.Console.ForegroundColor = ConsoleColor.White;
                grid.ErrorMessage = String.Empty;
            }
            grid.DrawGame();
            string answer = System.Console.ReadLine()!;
            grid.Prompt(answer);
        }
    }
}