using System.Numerics;

namespace Minesweeper.Models
{
    public class Land
    {
        public Vector2 Coordinate { get; set; }
        public bool Revealed { get; set; }
        public bool Flag { get; set; }
        public bool Bomb { get; set; }

        public Land(Vector2 coordinate, bool bomb = false)
        {
            Coordinate = coordinate;
            Flag = false;
            Revealed = false;
            Bomb = bomb;
        }

        public void DrawSelf(Grid grid)
        {
            string icon = " ";
            ConsoleColor color = ConsoleColor.DarkGray;
            if ((Coordinate.Y - Coordinate.X) % 2 == 0)
            {
                color = ConsoleColor.Gray;
            }
            if (Revealed)
            {
                color = ConsoleColor.Green;
                if (Bomb)
                {
                    icon = "X";
                    color = ConsoleColor.Red;
                }
                else
                {
                    int bombsAmount = grid.VerifyBombsSides(Coordinate);
                    if (bombsAmount == 0)
                    {
                        icon = "■";
                    }
                    else
                    {
                        color = ConsoleColor.DarkYellow;
                        icon = bombsAmount.ToString();

                    }
                }
            }
            else
            {
                if (Flag)
                {
                    color = ConsoleColor.DarkBlue;
                    icon = "■";
                }
            }
            System.Console.BackgroundColor = color;
            string write = icon + " ";
            System.Console.Write(write);
            System.Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}