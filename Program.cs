using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SnakeGame
{
    class GameEngine
    {
        // some public ints and strings to use later in the code
        static string recordFilePath = @"X:\Snake Game\Snake Game\record.txt";

        static char snakeHeadFacing = 'N';
        static char snakeNewHeadFacing;
        static int scoreSnake = 0;
        static int scoreSnakeNew = 0;
        static int recordSnake = int.Parse(File.ReadAllText(recordFilePath));
        static int gameSpeed = 180;
        static int height = 16;
        static int width = 16;
        static int centerY = height / 2 - 1;
        static int centerX = width / 2 - 1;
        static List<(int Y, int X)> coordinates = new List<(int Y, int X)>
        {
            (centerY, centerX),
            (centerY + 1, centerX),
            (centerY + 2, centerX)
        };

        /*╔══════════════════════════════════════════════════════╗
          ║ function to create 2d array, aka coordinate field    ║
          ║ it creates 2d array with heigh and width and         ║
          ║ replaces characters to empty squares...              ║
          ╚══════════════════════════════════════════════════════╝*/
        public static string[,] CreateGameFieldFunction(int height, int width)
        {
            string[,] gameField = new string[height, width];
            for (int row=0; row < height; row++)
            {
                for (int col=0; col < width; col++)
                {
                    if (row == 0 && col == 0) // top left
                        gameField[row, col] = "\u2554";
                    else if (row == 0 && col == width - 1) // top right
                        gameField[row, col] = "\u2557";
                    else if (row == height - 1 && col == 0) // bottom left
                        gameField[row, col] = "\u255A";
                    else if (row == height - 1 && col == width - 1) // bottom right
                        gameField[row, col] = "\u255D";
                    else if (row == 0 || row == height - 1) // top and bottom
                        gameField[row, col] = "\u2550";
                    else if (col == 0 || col == width - 1) // right and left
                        gameField[row, col] = "\u2551";
                    else
                        gameField[row, col] = " "; // inside

                    if (row == 3 && col == width - 1)
                        gameField[row, col] = "\u2551           Snake Game \u264C";
                    if (row == 5 && col == width - 1)
                        gameField[row, col] = "\u2551           Score: " + scoreSnake;
                    if (row == 7 && col == width - 1)
                        gameField[row, col] = "\u2551           Record: " + recordSnake;
                    if (row == 9 && col == width - 1)
                        gameField[row, col] = "\u2551           Game Speed: " + gameSpeed;
                }
            }
            return gameField;
        }

        // function to render the game field from function above
        public static void RenderGameFieldFunction(int height, int width, string[,] gameField)
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if ((row == 0 || row == height - 1) && col < width - 1)
                        Console.Write(gameField[row, col] + "\u2550");
                    else
                        Console.Write(gameField[row, col] + " ");

                    if (row == 5 && col == width - 1)
                        gameField[row, col] = "\u2551           Score: " + scoreSnake;
                }
                Console.WriteLine();
            }
        }

        // background thread / like function in background that checks on key pressed

        static void CheckInput()
        {
            while (true)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.W:
                        if (snakeHeadFacing != 'S')
                            snakeNewHeadFacing = 'N'; 
                            continue;   
                    case ConsoleKey.S:
                        if (snakeHeadFacing != 'N')
                            snakeNewHeadFacing = 'S'; 
                            continue;
                    case ConsoleKey.A:
                        if (snakeHeadFacing != 'E')
                            snakeNewHeadFacing = 'W'; 
                            continue;
                    case ConsoleKey.D:
                        if (snakeHeadFacing != 'W')
                            snakeNewHeadFacing = 'E'; 
                            continue;
                }
            }
        }

        // function to render snake

        public static void renderSnake()
        {
            
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            string[,] gameField = CreateGameFieldFunction(height, width);
            bool snakeAlive = true;

            var headCoordinate = coordinates[0];

            

            // foreach (var coordinate in coordinates {Console.WriteLine(coordinate);}
            
            /*coordinates of snake head and tails

            List<(int Y, int X)> coordinates = new List<(int Y, int X)>();
            coordinates.Add((centerY, centerX));
            coordinates.Add((centerY + 1, centerX));
            coordinates.Add((centerY + 2, centerX));

            var headCoordinate = coordinates[0];
            gameField[headCoordinate.Y, headCoordinate.X] = "\u25A1";*/
                

            

            foreach (var (Y, X) in coordinates)
            {
                gameField[Y, X] = "\u25A1";
            }
            
            RenderGameFieldFunction(height, width, gameField);
            
            // background thread to check if keys are pressed

            Thread inputThread = new Thread(CheckInput);
            inputThread.IsBackground = true;
            inputThread.Start();

            Random rnd = new Random();
            var foorCoordinateX = rnd.Next(1, width - 1);
            var foorCoordinateY = rnd.Next(1, height - 1);
            gameField[foorCoordinateY, foorCoordinateX] = "*";

            // basically game engine loop or frame gen

            while (snakeAlive)
            {
                Thread.Sleep(gameSpeed);
                if (snakeNewHeadFacing == '\0')
                {
                    continue;
                }

                Console.Clear();


                // if your snake's next movements covers the apple you get +1 to score and new apple

                if (gameField[foorCoordinateY, foorCoordinateX] == "\u25A1")
                {
                    while (true)
                    {
                        foorCoordinateX = rnd.Next(1, width - 1);
                        foorCoordinateY = rnd.Next(1, height - 1);
                        if (gameField[foorCoordinateY, foorCoordinateX] == " ")
                        {
                            scoreSnakeNew++;
                            gameField[foorCoordinateY, foorCoordinateX] = "*";
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }   
                }
                
                
                var newHeadCoordinate = headCoordinate;

                // based on snake facing you get movement

                if (snakeNewHeadFacing == 'N')
                {
                    newHeadCoordinate = (newHeadCoordinate.Y - 1, newHeadCoordinate.X);
                }
                else if (snakeNewHeadFacing == 'S')
                {
                    newHeadCoordinate = (newHeadCoordinate.Y + 1, newHeadCoordinate.X);
                }
                else if (snakeNewHeadFacing == 'W')
                {
                    newHeadCoordinate = (newHeadCoordinate.Y, newHeadCoordinate.X - 1);
                }
                else
                {
                    newHeadCoordinate = (newHeadCoordinate.Y, newHeadCoordinate.X + 1);
                }

                snakeHeadFacing = snakeNewHeadFacing;


                // if your snake hits border == death

                if (!(gameField[newHeadCoordinate.Y, newHeadCoordinate.X] == " " || gameField[newHeadCoordinate.Y, newHeadCoordinate.X] == "*"))
                {
                    snakeAlive = false; //exits the loop coz snake is dead
                }

                // inserts new coordinates for head
                coordinates.Insert(0, (newHeadCoordinate.Y, newHeadCoordinate.X));
                gameField[newHeadCoordinate.Y, newHeadCoordinate.X] = "\u25A1";
                headCoordinate = newHeadCoordinate;

                // clears the tail and removes it from coordinates

                if (scoreSnakeNew == scoreSnake)
                {
                    var endSnakeTail = coordinates[coordinates.Count - 1];
                    gameField[endSnakeTail.Y, endSnakeTail.X] = " ";
                    coordinates.RemoveAt(coordinates.Count - 1);
                }

                scoreSnake = scoreSnakeNew;

                /*foreach (var coord in coordinates)
                {
                    gameField[coord.Y, coord.X] = "\u25A1";
                }

                headCoordinate = newHeadCoordinate;*/

                

                RenderGameFieldFunction(height, width, gameField);
                Thread.Sleep(gameSpeed);
            }

            //deathscreen
            Console.Clear();
            if (scoreSnake > recordSnake)
            {
                File.WriteAllText(recordFilePath, scoreSnake.ToString());
            }
            Console.WriteLine("noob");
        }
    }
}