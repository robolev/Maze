
namespace Maze
{
    static class Program
    {

        static char[,] world;
        static char[,] buffer = new char[Console.WindowHeight, Console.WindowWidth];
        static ConsoleColor[,] colors = new ConsoleColor[Console.WindowHeight, Console.WindowWidth];
        static int startX = 0;
        static int startY = 0;
        static bool isFinished = false;

        static bool isPlayerWantToDig = false;

        static double time = 0.19;
        static double elapsedTime = 0;
        static double fps = 0;
        static int lastFrameTimeMs = Environment.TickCount;
        static int targetFps = 30;
        static int targetFrameTimeMs = 1000 / targetFps;
        static int frames = 0;
        enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Dig,
        }
        static Direction direction;
        static void Main(string[] args)
        {
            GenerateWorld();
            PlaceFinish();
            GameLoop();
        }

        static void GenerateWorld()
        {
            world = GenerateMaze(Console.WindowWidth, Console.WindowHeight);
            for (int y = 0; y < world.GetLength(0); y++)
            {
                for (int x = 0; x < world.GetLength(1); x++)
                {
                    Console.Write(world[y, x]);

                }
                Console.WriteLine();
            }
            Console.ReadKey();
            Console.Clear();
        }

        static void DrawPlayerPosition(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x, y);
            Console.Write("X");
        }

        static void GameLoop()
        {
            int playerX = startX;
            int playerY = startY;
            ColoringWorld();

            while (!isFinished)
            {                              
                UpdateFPSCount();

                Update(playerX,playerY);
                DrawPlayerPosition(playerX, playerY);
                
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Input();
                MovePlayer(ref playerX, ref playerY);
           
            }
        } 


        static void UpdateFPSCount()
        {           
            elapsedTime += time;
            frames++;
            if (elapsedTime >= 1)
            {
               fps = frames / elapsedTime;
               elapsedTime = 0;
               frames = 0;
            }

            int elapsedMs = Environment.TickCount - lastFrameTimeMs;
            if (elapsedMs < targetFrameTimeMs)
            {
               int delayMs = targetFrameTimeMs - elapsedMs;
               Thread.Sleep(delayMs);
               elapsedMs += delayMs;
            }

            lastFrameTimeMs = Environment.TickCount;
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.Write($"Frame rate: {fps:F2} fps ");
        }

        static void Input()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            switch (keyInfo.Key)
            {
                case ConsoleKey.W:
                    direction = Direction.Up;
                    break;
                case ConsoleKey.A:
                    direction = Direction.Left;
                    break;
                case ConsoleKey.S:
                    direction = Direction.Down;
                    break;
                case ConsoleKey.D:
                    direction = Direction.Right;
                    break;
                case ConsoleKey.E:
                    isPlayerWantToDig = true;
                    break;
            }
        }

        static void ColoringWorld()
        {           
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                for (int j = 0; j < Console.WindowWidth; j++)
                {
                    buffer[i, j] = world[i, j];
                    ConsoleColor brown = (ConsoleColor)((int)ConsoleColor.DarkRed + (int)ConsoleColor.Green);
                    colors[i, j] = brown;
                    Console.ForegroundColor = colors[i, j];
                }
            }

        }
        static void Update(int playerX, int playerY)
        {
            int startX = Math.Max(0, playerX - 5);
            int startY = Math.Max(0, playerY - 2);
            int endX = Math.Min(Console.WindowWidth - 1, playerX + 5);
            int endY = Math.Min(Console.WindowHeight - 1, playerY + 2);

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = startY; i <= endY; i++)
            {
                for (int j = startX; j <= endX; j++)
                {

                    Console.ForegroundColor = colors[i, j];
                    Console.SetCursorPosition(j, i);
                    Console.Out.Write(buffer[i, j]);
                }
            }

        }

        static void MovePlayer(ref int x, ref int y)
        {
            (int newX, int newY) = CalculateNewPlayerCoords(x, y);

            char currentChar = world[newY, newX];

            if (currentChar == 'o')
            {

                var hasDigged = Dig(newX, newY);

                if (!hasDigged)
                    return;
            }

            if (currentChar == 'F')
                Win();

            x = newX;
            y = newY;
        }
        static (int, int) CalculateNewPlayerCoords(int x, int y)
        {
            int nextX = x;
            int nextY = y;

            switch (direction)
            {
                case Direction.Up:
                    nextY--;
                    break;

                case Direction.Left:
                    nextX--;
                    break;

                case Direction.Down:
                    nextY++;
                    break;

                case Direction.Right:
                    nextX++;
                    break;
            }

            return (nextX, nextY);
        }
        static char[,] GenerateMaze(int width, int height)
        {
            char[,] maze = GenerateBlankField(height, width);

            Random random = new Random();
            startX = random.Next(1, width - 1);
            startY = random.Next(1, height - 1);
            maze[startY, startX] = ' ';

            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((startX, startY));

            while (stack.Count > 0)
            {

                (int x, int y) = stack.Pop();

                List<(int, int)> neighbors = new List<(int, int)>();

                if (x > 1)
                    neighbors.Add((x - 2, y));

                if (y > 1)
                    neighbors.Add((x, y - 2));

                if (x < width - 4)
                    neighbors.Add((x + 2, y));

                if (y < height - 4)
                    neighbors.Add((x, y + 2));


                neighbors.Shuffle(random);


                bool makePath = false;
                foreach ((int nx, int ny) in neighbors)
                {

                    if (maze[ny, nx] == 'o')
                    {

                        maze[(y + ny) / 2, (x + nx) / 2] = ' ';
                        maze[(y + ny) / 2, (x + nx) / 2 + 1] = ' ';
                        maze[(y + ny) / 2, (x + nx) / 2 - 1] = ' ';
                        maze[ny, nx] = ' ';

                        stack.Push((nx, ny));

                        makePath = true;
                        break;
                    }
                }

                if (!makePath)
                {

                    continue;
                }
            }

            return maze;
        }
        static char[,] GenerateBlankField(int height,int width)
        {
            char[,] maze = new char[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    maze[y, x] = 'o';
                }
            }

            return maze;
        }
        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        static bool Dig(int x, int y)
        {
            if (isPlayerWantToDig)
            {
                buffer[y, x] = ' ';
                world[y, x] = buffer[y, x];
                isPlayerWantToDig = false;
                return true;
            }

            return false;
        }
        static void FindFarthestPoint(int playerX, int playerY, out int farX, out int farY)
        {
            Random random = new Random();

            farX = playerX;
            farY = playerY;

            int maxDistance = 0;

            for (int i = 0; i < world.GetLength(0); i++)
            {
                for (int j = 0; j < world.GetLength(1); j++)
                {
                    if (world[i, j] == ' ')
                    {
                        int distance = Math.Abs(playerX - j) + Math.Abs(playerY - i);
                        if (distance > maxDistance)
                        {
                            maxDistance = distance;
                            farX = j;
                            farY = i;
                        }
                        else if (distance == maxDistance)
                        {
                            if (random.Next(2) == 0)
                            {
                                farX = j;
                                farY = i;
                            }
                        }
                    }
                }
            }
        }
        static void PlaceFinish()
        {
            int farthestX;
            int farthestY;
            FindFarthestPoint(startX, startY, out farthestX, out farthestY);
            world[farthestY, farthestX] = 'F';
        }
        static void Win()
        {
            Console.Clear();
            Console.WriteLine("WinWinWinWinWin   WinWinWinWinWinWinWinWinW     inWinWinWinWinWin");
            Console.WriteLine("WinWinWinWin     WinWinWinWinWinWinWinWin       WinWinWin");
            Console.WriteLine("WinWinWinWin      nWinWinWinWinWinWin            WinWinWin");
            Console.WriteLine("WinWinWinWin       WinWinWinWinWin              Win");
            Console.WriteLine("WinWinWin          WinWinWinWin                  Win");
            Console.WriteLine("   WinWin           WinWinWinWin                  Win");
            Console.WriteLine("   WinWin             WinWinWin                  Win");
            Console.WriteLine("   WinWin                                       Win");
            Console.WriteLine("    WinWin       WinWin            WinWin        Win");
            Console.WriteLine("    WinWin       W   in            W   in        Win");
            Console.WriteLine("    WinWin       WinWin            WinWin        Win");
            Console.WriteLine("     WinWin                                    Win");
            Console.WriteLine("      WinWin               WinWin               Win");
            Console.WriteLine("        WinWin                W                   Win");
            Console.WriteLine("         WinWin            W  W  W                  Win");
            Console.WriteLine("           WinWin           W   W                     Win");
            Console.WriteLine("      Win     WinWin                                    Win");
            Console.WriteLine("   Wi    wi     WinWin                                    Win");
            Console.WriteLine("   Wi    Win    Win                                        Win");
            Console.WriteLine("Wi        WinWinWin                                         Win");
            Console.WriteLine("Wi                                                        Win");
            Console.WriteLine("Wi                                                       Win");
            Console.WriteLine(" WinWin                                                 Win");
            isFinished = true;
            Console.ReadKey();
        }
    }
}