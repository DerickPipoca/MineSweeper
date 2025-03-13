using System.Numerics;

namespace Minesweeper.Models
{
    public class Grid
    {
        private readonly int BombAmount = 0;
        private List<Vector2> VerifiedPlaces = [];
        private List<Vector2> AvoidBombPlaces = [];
        private bool FirstPlay = true;
        private bool Playing = true;
        private bool Won = false;
        private bool Defeat = false;

        public string ErrorMessage = String.Empty;
        public int Size { get; set; }
        public List<Land> Lands { get; set; }

        public Grid(int size)
        {
            Size = size;
            BombAmount = (int)MathF.Pow(size, 2) / 8;
            Lands = [];
        }

        public void GenerateNew()
        {
            Lands = [];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    var land = new Land(new Vector2(j, i));
                    Lands.Add(land);
                }
            }
        }

        public void DrawGame()
        {
            System.Console.BackgroundColor = ConsoleColor.DarkMagenta;
            System.Console.Write("  ");
            for (int i = 1; i <= Size; i++)
            {
                string freeSpace = "";
                if (i < 10) freeSpace = " ";
                if (i % 2 == 0)
                {
                    System.Console.BackgroundColor = ConsoleColor.DarkMagenta;
                }
                else
                {
                    System.Console.BackgroundColor = ConsoleColor.Magenta;
                }
                System.Console.Write(i + freeSpace);
                System.Console.BackgroundColor = ConsoleColor.Black;
            }
            System.Console.Write($" {BombAmount - MathF.Min(Lands.FindAll(e => e.Flag).Count, BombAmount)} Bomba(s) Restante");
            System.Console.WriteLine();
            for (int i = 0; i < Size; i++)
            {
                string freeSpace = " ";
                if (i + 1 >= 10) freeSpace = "";
                if (i % 2 != 0)
                {
                    System.Console.BackgroundColor = ConsoleColor.DarkMagenta;
                }
                else
                {
                    System.Console.BackgroundColor = ConsoleColor.Magenta;
                }
                System.Console.Write((i + 1) + freeSpace);
                System.Console.BackgroundColor = ConsoleColor.Black;
                for (int j = 0; j < Size; j++)
                {
                    var land = Lands.FirstOrDefault(x => x.Coordinate == new Vector2(j, i));
                    if (land != null)
                    {
                        land.DrawSelf(this);
                    }
                }
                System.Console.WriteLine();
            }
            string message = "Para verificar campo: '2,7'\nPara colocar bandeira: F2,2";
            if (Defeat)
                message = "Você perdeu!";
            if (Won)
                message = "Você venceu!!!!";
            System.Console.WriteLine(message);
        }

        public void Prompt(string answer)
        {
            try
            {
                if (Playing)
                {
                    if (answer[0] != 'F'
                    && answer[0] != 'f')
                    {
                        string[] prompt = answer.Split(',');
                        int[] convPrompt = [int.Parse(prompt[0]) - 1, int.Parse(prompt[1]) - 1];
                        Vector2 coordinates = new(convPrompt[0], convPrompt[1]);
                        if (FirstPlay)
                            AddBombs(coordinates);
                        FirstPlay = false;
                        ProcessPromptAnswer(coordinates);
                    }
                    else
                    {
                        answer = answer.Substring(1);
                        string[] prompt = answer.Split(',');
                        int[] convPrompt = [int.Parse(prompt[0]) - 1, int.Parse(prompt[1]) - 1];
                        Vector2 coordinates = new(convPrompt[0], convPrompt[1]);
                        if (IsValidPlace(coordinates))
                        {
                            var land = Lands.FirstOrDefault(x => x.Coordinate == coordinates);

                            if (land != null
                            && !land.Revealed)
                            {
                                land.Flag = !land.Flag;
                            }
                        }
                    }
                    if (!Defeat && IsGameWon())
                    {
                        Win();
                    }
                    VerifiedPlaces = [];
                }
                else
                {
                    string[] prompt = answer.Split(',');
                    int[] convPrompt = [int.Parse(prompt[0]) - 1, int.Parse(prompt[1]) - 1];
                    Vector2 coordinates = new(convPrompt[0], convPrompt[1]);
                    Won = false;
                    Defeat = false;
                    GenerateNew();
                    AddBombs(coordinates);
                    ProcessPromptAnswer(coordinates);
                    Playing = true;
                }
            }
            catch (Exception)
            {
                ErrorMessage = $"ERRO: '{answer}' não é um comando válido!";
            }

        }

        public int VerifyBombsSides(Vector2 coordinate)
        {
            int amount = 0;
            if (BombPlaced(coordinate + new Vector2(-1, -1))) amount++;
            if (BombPlaced(coordinate + new Vector2(0, -1))) amount++;
            if (BombPlaced(coordinate + new Vector2(1, -1))) amount++;

            if (BombPlaced(coordinate + new Vector2(-1, 0))) amount++;
            if (BombPlaced(coordinate + new Vector2(1, 0))) amount++;

            if (BombPlaced(coordinate + new Vector2(-1, 1))) amount++;
            if (BombPlaced(coordinate + new Vector2(0, 1))) amount++;
            if (BombPlaced(coordinate + new Vector2(1, 1))) amount++;

            return amount;
        }

        public bool BombPlaced(Vector2 coordinate)
        {
            if (IsValidPlace(coordinate))
            {
                var land = Lands.FirstOrDefault(x => x.Coordinate == coordinate);

                if (land == null)
                {
                    throw new Exception("Land not found!");
                }
                return land.Bomb;
            }
            return false;
        }

        private void AddBombAvoidList(Vector2 coord)
        {
            if (IsValidPlace(coord))
                AvoidBombPlaces.Add(coord);
        }

        private void CreateBombsAvoidList(Vector2 coords)
        {

            AvoidBombPlaces = [];

            var coord = new Vector2(coords.X - 1, coords.Y - 1);

            AddBombAvoidList(new Vector2(coords.X - 1, coords.Y - 1));
            AddBombAvoidList(new Vector2(coords.X, coords.Y - 1));
            AddBombAvoidList(new Vector2(coords.X + 1, coords.Y - 1));
            AddBombAvoidList(new Vector2(coords.X - 1, coords.Y));
            AddBombAvoidList(new Vector2(coords.X, coords.Y));
            AddBombAvoidList(new Vector2(coords.X + 1, coords.Y));
            AddBombAvoidList(new Vector2(coords.X - 1, coords.Y + 1));
            AddBombAvoidList(new Vector2(coords.X, coords.Y + 1));
            AddBombAvoidList(new Vector2(coords.X + 1, coords.Y + 1));
        }

        private void AddBombs(Vector2 coords)
        {
            CreateBombsAvoidList(coords);

            int bombs = 0;
            do
            {
                var rnd = new Random();
                int x = rnd.Next(0, Size - 1);
                rnd = new Random();
                int y = rnd.Next(0, Size - 1);

                var land = Lands.FirstOrDefault(l => l.Coordinate == new Vector2(x, y));

                if (land != null
                && land.Bomb == false
                && !AvoidBombPlaces.Contains(land.Coordinate))
                {
                    land.Bomb = true;
                    bombs++;
                }
            } while (bombs < BombAmount);
        }

        private void Show8Sides(Vector2 coordinate)
        {
            ShowSide(coordinate + new Vector2(-1, -1));
            ShowSide(coordinate + new Vector2(0, -1));
            ShowSide(coordinate + new Vector2(1, -1));
            ShowSide(coordinate + new Vector2(-1, 0));
            ShowSide(coordinate + new Vector2(1, 0));
            ShowSide(coordinate + new Vector2(-1, 1));
            ShowSide(coordinate + new Vector2(0, 1));
            ShowSide(coordinate + new Vector2(1, 1));
        }

        private bool IsGameWon()
        {
            List<Land> lands = [];
            lands.AddRange(Lands.FindAll(e => !e.Bomb && e.Revealed == true));
            if (lands.Count >= (Size * Size) - BombAmount)
                return true;
            return false;
        }

        private void ShowSide(Vector2 coordinate)
        {
            if (IsValidPlace(coordinate)
            && !VerifiedPlaces.Contains(coordinate))
            {
                var land = Lands.FirstOrDefault(x => x.Coordinate == coordinate);
                if (land != null)
                {
                    land.Revealed = true;
                    if (VerifyBombsSides(land.Coordinate) == 0)
                    {
                        VerifiedPlaces.Add(land.Coordinate);
                        Show8Sides(coordinate);
                    }
                }
            }
        }

        private bool IsValidPlace(Vector2 coordinate)
        {
            if (coordinate.X >= Size
            || coordinate.Y >= Size
            || coordinate.X < 0
            || coordinate.Y < 0)
            {
                return false;
            }
            return true;
        }

        private void Defeated()
        {
            Playing = false;
            Defeat = true;
            foreach (var land in Lands)
            {
                land.Revealed = true;
            }
        }

        private void Win()
        {
            Playing = false;
            Won = true;
            foreach (var land in Lands)
            {
                land.Revealed = true;
            }
        }

        private void ProcessPromptAnswer(Vector2 coordinates)
        {
            if (IsValidPlace(coordinates))
            {
                var land = Lands.FirstOrDefault(x => x.Coordinate == coordinates);

                if (land != null)
                {
                    if (land.Bomb)
                    {
                        Defeated();
                    }
                    ShowSide(coordinates);
                }
            }
        }
    }
}