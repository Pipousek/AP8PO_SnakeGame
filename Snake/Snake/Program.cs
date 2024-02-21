﻿using System;
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
        const int gameSpeed = 2000;
        const char drawingBlock = '■';
        Random randomNum;

        private int score;
        private bool gameOver;
        private Pixel head;
        private Direction movement;
        private Direction lastMovement;
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


                if (berryX == head.XPos && berryY == head.YPos)
                {
                    score++;
                    GenerateBerry();
                }


                DrawPixelInConsole(head.XPos, head.YPos, head.ScreenColor);
                if (bodyYPos.Count > 0)
                    DrawPixelInConsole(bodyXPos[bodyXPos.Count - 1], bodyYPos[bodyYPos.Count - 1], ConsoleColor.Green);

                var time = DateTime.Now;
                lastMovement = movement;
                while (true)
                {
                    InputHandler();
                    if (DateTime.Now.Subtract(time).TotalMilliseconds > gameSpeed) { break; }
                }
                
                InputHandler();
                UpdateSnakePosition(ref head, ref bodyXPos, ref bodyYPos, movement, score);
                CheckGameOver();
            }
            GameOverScreen();
        }

        private void InputHandler()
        {
            if (Console.KeyAvailable)
            {
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

            movement = Direction.Right;
            bodyXPos = new List<int>();
            bodyYPos = new List<int>();
            GenerateBerry();
            DrawBorders(screenWidth, screenHeight);
        }

        private void GenerateBerry()
        {
            berryX = randomNum.Next(1, screenWidth - 2);
            berryY = randomNum.Next(1, screenHeight - 2);
            DrawPixelInConsole(berryX, berryY, ConsoleColor.Cyan);
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
                DrawPixelInConsole(0, i, ConsoleColor.White);
                DrawPixelInConsole(width - 1, i, ConsoleColor.White);
            }
        }

        private void DrawPixelInConsole(int xPos, int yPos, ConsoleColor pixelColor)
        {
            Console.SetCursorPosition(xPos, yPos);
            Console.ForegroundColor = pixelColor;
            Console.Write(drawingBlock);
        }

        private void UpdateSnakePosition(ref Pixel head, ref List<int> bodyXPos, ref List<int> bodyYPos, Direction movement, int score)
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
        private enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}