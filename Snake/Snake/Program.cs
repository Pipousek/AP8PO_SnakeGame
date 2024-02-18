using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Snake
{
    class Program
    {
        const int screenHeight = 16;
        const int screenWidth = 32;
        const char drawingBlock = '■';
        Random randomNum;

        private int score;
        private bool gameOver;
        private Pixel head;
        private string movement;
        List<int> bodyXPos;
        List<int> bodyYPos;
        int berryX;
        int berryY;
        

        static void Main(string[] args)
        {
            new Program().Run();
        }
        private void Run()
        {
            SetupWindow();
            Init();

            while (!gameOver)
            {
                Console.Clear();
                DrawBorders(screenWidth, screenHeight);
                CheckGameOver();

                if (berryX == head.XPos && berryY == head.YPos)
                {
                    score++;
                    GenerateBerry();
                }

                for (var i = 0; i < bodyXPos.Count; i++)
                {
                    DrawPixelInConsole(bodyXPos[i], bodyYPos[i], ConsoleColor.Green);
                }

                if (gameOver)
                {
                    break;
                }
                DrawPixelInConsole(head.XPos, head.YPos, head.ScreenColor);
                DrawPixelInConsole(berryX, berryY, ConsoleColor.Cyan);

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
            GameOverScreen();
        }

        private void GameOverScreen()
        {
            Console.Clear();
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2);
            Console.WriteLine("Game over, Score: " + score);
            Console.SetCursorPosition(screenWidth / 5, screenHeight / 2 + 1);
            Console.ReadLine(); // Keep the console window open
        }
        private void SetupWindow()
        {
            Console.WindowHeight = screenHeight;
            Console.WindowWidth = screenWidth;
        }

        private void Init()
        {
            randomNum = new Random();
            score = 5;
            gameOver = false;
            head = new Pixel
            {
                XPos = screenWidth / 2,
                YPos = screenHeight / 2,
                ScreenColor = ConsoleColor.Red
            };

            movement = "RIGHT";
            bodyXPos = new List<int>();
            bodyYPos = new List<int>();
            GenerateBerry();
        }

        private void GenerateBerry()
        {
            berryX = randomNum.Next(1, screenWidth - 2);
            berryY = randomNum.Next(1, screenHeight - 2);
        }


        private void DrawBorders(int width, int height)
        {
            for (var i = 0; i < width; i++)
            {
                DrawPixelInConsole(i, 0, ConsoleColor.White);
                DrawPixelInConsole(i, height - 1, ConsoleColor.White);
            }

            for (var i = 0; i < height; i++)
            {
                DrawPixelInConsole(0, i , ConsoleColor.White);
                DrawPixelInConsole(width - 1, i, ConsoleColor.White);
            }
        }

        private void DrawPixelInConsole(int xPos, int yPos, ConsoleColor pixelColor)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.ForegroundColor = pixelColor;
            Console.Write(drawingBlock);
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
        private void CheckGameOver()
        {
            // Snake crash into wall
            if (head.XPos == screenWidth - 1 || head.XPos == 0 || head.YPos == screenHeight - 1 || head.YPos == 0)
            {
                gameOver = true;
                return;
            }
            // Snake crash into himself
            for (var i = 0; i < bodyXPos.Count; i++)
            {
                if (bodyXPos[i] == head.XPos && bodyYPos[i] == head.YPos)
                {
                    gameOver = true;
                    return;
                }
            }
        }
        private class Pixel
        {
            public int XPos { get; set; }
            public int YPos { get; set; }
            public ConsoleColor ScreenColor { get; init; }
        }
    }
}