namespace Snake
{
    internal class Program
    {
        private const int ScreenHeight = 16;
        private const int ScreenWidth = 32;
        private const int GameSpeed = 500;  // Bigger number = slower game
        private const char DrawingBlock = '■';

        private int score;
        private bool endGame;
        private Pixel berry;
        private Random randomNum;
        private Pixel head;
        private Direction movement;
        private Direction lastMovement;
        private List<int> bodyXPos;
        private List<int> bodyYPos;

        private static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            SetupWindow();
            Init();

            while (!endGame)
            {
                DrawPixelInConsole(head.XPos, head.YPos, head.PixelColor);
                
                if (bodyYPos.Count > 0)
                    DrawPixelInConsole(bodyXPos[^1], bodyYPos[^1], ConsoleColor.Green);

                var time = DateTime.Now;
                lastMovement = movement;
                while (true)
                {
                    InputHandler();
                    if (DateTime.Now.Subtract(time).TotalMilliseconds > GameSpeed) { break; }
                }
                
                UpdateSnakePosition(ref head, ref bodyXPos, ref bodyYPos, movement);
                CheckGameOver();
            }
            GameOverScreen();
        }

        private void InputHandler()
        {
            if (!Console.KeyAvailable) return;
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow when lastMovement != Direction.Down:
                    movement = Direction.Up;
                    break;
                case ConsoleKey.DownArrow when lastMovement != Direction.Up:
                    movement = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow when lastMovement != Direction.Right:
                    movement = Direction.Left;
                    break;
                case ConsoleKey.RightArrow when lastMovement != Direction.Left:
                    movement = Direction.Right;
                    break;
            }
        }

        private void GameOverScreen()
        {
            Console.Clear();
            Console.SetCursorPosition(ScreenWidth / 5, ScreenHeight / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(ScreenWidth / 5, ScreenHeight / 2 + 1);
            Console.ReadLine(); // Keep the console window open
        }

        private void SetupWindow()
        {
            Console.WindowHeight = ScreenHeight;
            Console.WindowWidth = ScreenWidth;
        }

        private void Init()
        {
            randomNum = new Random();
            score = 5;
            endGame = false;
            head = new Pixel
            {
                XPos = ScreenWidth / 2,
                YPos = ScreenHeight / 2,
                PixelColor = ConsoleColor.Red
            };
            berry = new Pixel
            {
                PixelColor = ConsoleColor.Cyan
            };

            movement = Direction.Right;
            bodyXPos = new List<int>();
            bodyYPos = new List<int>();
            GenerateBerry();
            DrawBorders(ScreenWidth, ScreenHeight);
        }

        private void GenerateBerry()
        {
            bool isBerryOnSnake = true;
            while (isBerryOnSnake)
            {
                berry.XPos = randomNum.Next(1, ScreenWidth - 1);
                berry.YPos = randomNum.Next(1, ScreenHeight - 1);
        
                // Check if the berry position overlaps with any snake body position or head
                isBerryOnSnake = false;
                if (berry.XPos == head.XPos && berry.YPos == head.YPos)
                {
                    isBerryOnSnake = true;
                    continue;
                }
                
                for (int i = 0; i < bodyXPos.Count; i++)
                {
                    if (berry.XPos == bodyXPos[i] && berry.YPos == bodyYPos[i])
                    {
                        isBerryOnSnake = true;
                        break;
                    }
                }
            }
            DrawPixelInConsole(berry.XPos, berry.YPos, berry.PixelColor);
        }

        private void DrawBorders(int width, int height)
        {
            ConsoleColor borderColor = ConsoleColor.White;
            for (var i = 0; i < width; i++)
            {
                DrawPixelInConsole(i, 0, borderColor);
                DrawPixelInConsole(i, height - 1, borderColor);
            }

            for (var i = 0; i < height; i++)
            {
                DrawPixelInConsole(0, i, borderColor);
                DrawPixelInConsole(width - 1, i, borderColor);
            }
        }

        private void DrawPixelInConsole(int xPos, int yPos, ConsoleColor pixelColor)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.ForegroundColor = pixelColor;
            Console.Write(DrawingBlock);
        }

        private void UpdateSnakePosition(ref Pixel head, ref List<int> bodyXPos, ref List<int> bodyYPos, Direction movement)
        {
            bodyXPos.Add(head.XPos);
            bodyYPos.Add(head.YPos);

            switch (movement)
            {
                case Direction.Up:
                    head.YPos--;
                    break;
                case Direction.Down:
                    head.YPos++;
                    break;
                case Direction.Left:
                    head.XPos--;
                    break;
                case Direction.Right:
                    head.XPos++;
                    break;
            }

            if (berry.XPos == head.XPos && berry.YPos == head.YPos)
            {
                score++;
                CheckGameWin();
                GenerateBerry();
            }
            
            if (bodyXPos.Count <= score)
                return;
            
            Console.SetCursorPosition(bodyXPos[0], bodyYPos[0]);
            Console.Write(" ");
            bodyXPos.RemoveAt(0);
            bodyYPos.RemoveAt(0);
        }

        private void CheckGameOver()
        {
            // Snake crash into wall
            if (head.XPos == ScreenWidth - 1 || head.XPos == 0 || head.YPos == ScreenHeight - 1 || head.YPos == 0)
            {
                endGame = true;
                return;
            }
            
            // Snake crash into himself
            for (var i = 0; i < bodyXPos.Count; i++)
            {
                if (bodyXPos[i] == head.XPos && bodyYPos[i] == head.YPos)
                {
                    endGame = true;
                    return;
                }
            }
        }

        private void CheckGameWin()
        {
            int maxBerriesGenerated = (ScreenWidth - 2) * (ScreenHeight - 2) - (score + 1);
            if (maxBerriesGenerated != 0) return;
            
            endGame = true;
            Console.Clear();
            Console.SetCursorPosition(ScreenWidth / 5, ScreenHeight / 2);
            Console.WriteLine("You WIN, Score: " + score);
            Console.SetCursorPosition(ScreenWidth / 5, ScreenHeight / 2 + 1);
            Console.ReadLine(); // Keep the console window open
        }

        private class Pixel
        {
            public int XPos { get; set; }
            public int YPos { get; set; }
            public ConsoleColor PixelColor { get; init; }
        }

        private enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}