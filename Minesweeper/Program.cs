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
            grid.ShowErrorMessage();
            grid.DrawGame();
            string answer = System.Console.ReadLine()!;
            grid.Prompt(answer);
        }
    }
}