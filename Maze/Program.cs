
namespace Maze
{
    internal class Program
    {

        static char[,] world;    
        enum Direction
        { 
          Up,
          Down,
          Left,
          Right        
        }
        static Direction direction;
        static void Main(string[] args)
        {
            GenerateWorld();      
            GameLoop();
        }

        static void GenerateWorld()
        {
            Random rnd = new Random();

            world = new char[1000, 1000];

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    char symbol = 'o';
                    world[i, j] = symbol;
                }
            }
        }

        static void Input()
        {
            //Direction direction = 0;
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
                    direction= Direction.Right;
                    break;
            }
           
        }
        static void DrawPLayerPosition(int x, int y)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(x, y);
            Console.Write("X");   
        }

        static void GameLoop()
        {
            int playerX = 30;
            int playerY = 20;
            int lastFrameTimeMs = Environment.TickCount;
            int targetFps = 30;
            int targetFrameTimeMs = 60 / targetFps;
            int frames = 0;
            double elapsedTime = 0;
            double fps = 0;
            double time = 0.19;
            const double cameraDistanceX = 0.4;
            const double cameraDistanceY = 0.5;
            double cameraX = playerX - Console.WindowWidth * 0.4;
            double cameraY = playerY - Console.WindowHeight * 0.4;

            while (true)
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

                Input();
                MovePlayer(ref playerX, ref playerY);
                cameraX = playerX - Console.WindowWidth * cameraDistanceX;
                cameraY = playerY - Console.WindowHeight * cameraDistanceY;
                DrawWorld( playerX, playerY);
                DrawPLayerPosition(playerX,playerY);
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write($"Frame rate: {fps:F2} fps ");
            }         
        }
        static void DrawWorld(int playerX, int playerY)
        {
            Console.Clear();
            char[,] buffer = new char[1000, 1000];
            ConsoleColor[,] colors = new ConsoleColor[1000, 1000];

            // Calculate the bounds of the visible area
            int startX = Math.Max(0, playerX - 5);
            int startY = Math.Max(0, playerY - 2);
            int endX = Math.Min(Console.WindowWidth - 1, playerX + 5);
            int endY = Math.Min(Console.WindowHeight - 1, playerY + 2);

            for (int i = startY; i <= endY; i++)
            {
                for (int j = startX; j <= endX; j++)
                {
                    buffer[i, j] = world[i, j];
                    ConsoleColor brown = (ConsoleColor)((int)ConsoleColor.DarkRed + (int)ConsoleColor.Green);
                    colors[i, j] = brown;         
                }
            }            

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
        static void MovePlayer(ref int x,ref int y)
        {
            int nextX = x;
            int nextY = y;

            switch (direction)
            {
                case Direction.Up:
                    y--;
                    break;
                case Direction.Left:
                    x--;
                    break;
                case Direction.Down:
                    y++;
                    break;
                case Direction.Right:
                    x++;
                    break;
            }
            
            //   x = Math.Max(0, Math.Min(x, Console.WindowWidth - 1));
            //    y = Math.Max(0, Math.Min(y, Console.WindowHeight - 1));
        }
    }
}