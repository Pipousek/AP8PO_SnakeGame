using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
///█ ■
////https://www.youtube.com/watch?v=SGZgvMwjq2U
namespace Snake
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }
        private void Run() { 
            Console.WindowHeight = 16;
            Console.WindowWidth = 32;

            var screenWidth = Console.WindowWidth;
            var screenHeight = Console.WindowHeight;
            var randomNum = new Random();
            var score = 5;
            var gameOver = false;
            var head = new Pixel
            {
                XPos = screenWidth / 2,
                YPos = screenHeight / 2,
                ScreenColor = ConsoleColor.Red
            };

            var movement = "RIGHT";
            var bodyXPos = new List<int>();
            var bodyYPos = new List<int>();
            var berryX = randomNum.Next(0, screenWidth);
            var berryY = randomNum.Next(0, screenHeight);

            while (!gameOver)
            {
                Console.Clear();
                DrawBorders(screenWidth, screenHeight);

                if (head.XPos == screenWidth - 1 || head.XPos == 0 || head.YPos == screenHeight - 1 || head.YPos == 0)
                {
                    gameOver = true;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                if (berryX == head.XPos && berryY == head.YPos)
                {
                    score++;
                    berryX = randomNum.Next(1, screenWidth - 2);
                    berryY = randomNum.Next(1, screenHeight - 2);
                }

                for (var i = 0; i < bodyXPos.Count; i++)
                {
                    Console.SetCursorPosition(bodyXPos[i], bodyYPos[i]);
                    Console.Write("■");
                    if (bodyXPos[i] == head.XPos && bodyYPos[i] == head.YPos)
                    {
                        gameOver = true;
                    }
                }

                if (gameOver)
                {
                    break;
                }

                Console.SetCursorPosition(head.XPos, head.YPos);
                Console.ForegroundColor = head.ScreenColor;
                Console.Write("■");
                Console.SetCursorPosition(berryX, berryY);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("■");

                var time = DateTime.Now;
                var buttonPressed = false;

                while (true)
                {
                    if (DateTime.Now.Subtract(time).TotalMilliseconds > 500) { break; }
                    if (Console.KeyAvailable && !buttonPressed)
                    {
                        var key = Console.ReadKey(true);

                        switch (key.Key)
                        {
                            case ConsoleKey.UpArrow when movement != "DOWN":
                                movement = "UP";
                                buttonPressed = true;
                                break;
                            case ConsoleKey.DownArrow when movement != "UP":
                                movement = "DOWN";
                                buttonPressed = true;
                                break;
                            case ConsoleKey.LeftArrow when movement != "RIGHT":
                                movement = "LEFT";
                                buttonPressed = true;
                                break;
                            case ConsoleKey.RightArrow when movement != "LEFT":
                                movement = "RIGHT";
                                buttonPressed = true;
                                break;
                        }
                    }
                }

                UpdateSnakePosition(ref head, ref bodyXPos, ref bodyYPos, movement, score);
            }

            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
            Console.ReadLine(); // Keep the console window open
        }

        private void DrawBorders(int width, int height)
        {
            Console.ForegroundColor = ConsoleColor.White;
            for (var i = 0; i < width; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, height - 1);
                Console.Write("■");
            }

            for (var i = 0; i < height; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(width - 1, i);
                Console.Write("■");
            }
        }

        private void UpdateSnakePosition(ref Pixel head, ref List<int> bodyXPos, ref List<int> bodyYPos, string movement, int score)
        {
            bodyXPos.Add(head.XPos);
            bodyYPos.Add(head.YPos);

            switch (movement)
            {
                case "UP":
                    head.YPos--;
                    break;
                case "DOWN":
                    head.YPos++;
                    break;
                case "LEFT":
                    head.XPos--;
                    break;
                case "RIGHT":
                    head.XPos++;
                    break;
            }

            if (bodyXPos.Count <= score)
                return;

            bodyXPos.RemoveAt(0);
            bodyYPos.RemoveAt(0);
        }

        private class Pixel
        {
            public int XPos { get; set; }
            public int YPos { get; set; }
            public ConsoleColor ScreenColor { get; init; }
        }
    }
}