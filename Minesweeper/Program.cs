using Minesweeper.Models;

public class MineSweeper
{
    public static void Main(String[] args)
    {
        Grid grid = new();

        while (true)
        {
            System.Console.Clear();
            grid.GameInitialization();
            grid.ShowErrorMessage();
            grid.DrawGame();
            string answer = System.Console.ReadLine()!;
            grid.Prompt(answer);
        }
    }
}