namespace Maze
{
    internal class Program
    {
        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

        static char[,] world;

        int playerX;
        int playerY;

        static void Main(string[] args)
        {
            GenerateWorld();


        }

        static void GenerateWorld()
        {
            Random rnd = new Random();

            world = new char[Console.WindowHeight, Console.WindowWidth];
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                for (int j = 0; j < Console.WindowHeight; j++)
                {
                    char symbol = '.';

                    world[i, j] = symbol;
                }
            }
        }

        static void Input()
        {


        }
        static void DrawPLayerPosition(int x, int y)
        {



        }

        static void GameLoop()
        {



        }
    }
}