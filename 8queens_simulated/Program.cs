using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _8queens_simulated
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> tablo = new List<string>();
            int totalTime = 0;
            int totalMoves = 0;
            int totalRestarts = 0;

            for (int i = 0; i < 35; i++)
            {
                bool[,] board = initializeBoard(8); //initialize for 8x8 (8 queens)

                int Moves = 0;
                int restartCount = 0;
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start(); // Begin timing.

                while (calculateCollisions(board) > 0)
                {
                    applyHillClimbingAlgorithmToTheBoard(board);
                    Moves++;
                    if (Moves > 20) //if it stucks at the local minimum
                    {
                        board = initializeBoard(8); //random new board
                        Moves = 0;
                        restartCount++;
                    }
                }
                stopwatch.Stop();
                drawTheBoard(board);
                Console.WriteLine(i + 1 + ") Total Moves: " + Moves + ", Restart Count: " + restartCount + ", Time elapsed: {0} ms", stopwatch.Elapsed.Milliseconds);
                tablo.Add(i + 1 + ") Total Moves: " + Moves + ", Restart Count: " + restartCount + ", Time elapsed:" + stopwatch.Elapsed.Milliseconds + " ms");

                totalMoves += Moves;
                totalTime += stopwatch.Elapsed.Milliseconds;
                totalRestarts += restartCount;

                Thread.Sleep(300); //to see the board
                Console.Clear();
            }

            foreach (String istatistik in tablo)
            {
                Console.WriteLine(istatistik);
            }
            Console.WriteLine("++Average Moves: " + totalMoves / 35 + ", Average Restarts: " + totalRestarts / 35 + ", Average Time: " + totalTime / 35 + " ms ");

            Console.ReadKey();
        }

        // returns 2d array with specified size (if size=4, 2d array is 4 x 4 and there are 4 queens)
        static bool[,] initializeBoard(int size)
        {
            bool[,] board = new bool[size, size];
            Random rnd = new Random();


            for (int i = 0; i < size; i++)
            {
                int randomSquare = rnd.Next(8); //placing queen to a random square in the row
                for (int j = 0; j < size; j++)
                {
                    if (j != randomSquare)
                        board[i, j] = false;
                    else
                        board[i, j] = true;
                }
            }

            return board;
        }

        static int calculateCollisions(bool[,] board)
        {
            int size = Convert.ToInt32(Math.Sqrt(board.Length)); //if the board is 8x8, board.Length will be 64
            int totalCollisions = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (board[i, j] == true) // if there is a queen on that square
                    {
                        //for (int z = j + 1; z < size; z++) //check the row //not necessary with this put-a-queen-to-each-row initialization
                        //{
                        //    if (board[i, z] == true)
                        //    {
                        //        totalCollisions++;
                        //        break;
                        //    }
                        //}

                        for (int z = i + 1; z < size; z++) //check the column
                        {
                            if (board[z, j] == true)
                            {
                                totalCollisions++;
                                //break; //if indirect attacking does not count
                            }
                        }

                        for (int z = 1; i + z < size && j + z < size; z++) //check the bottom-right diagonal
                        {
                            if (board[i + z, j + z] == true)
                            {
                                totalCollisions++;
                                //break;
                            }
                        }

                        for (int z = 1; i - z >= 0 && j + z < size; z++) //check the top-right diagonal
                        {
                            if (board[i - z, j + z] == true)
                            {
                                totalCollisions++;
                                //break;
                            }
                        }

                    }
                }
            }
            return totalCollisions;

        }

        static void applyHillClimbingAlgorithmToTheBoard(bool[,] board)
        {
            int size = Convert.ToInt32(Math.Sqrt(board.Length));
            int[,] successors = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                //find the queen of the row first
                int indexOfQueen = -1;
                for (int j = 0; j < size; j++)
                {
                    if (board[i, j] == true)
                    {
                        indexOfQueen = j;
                        board[i, j] = false;
                    }
                }
                //try all the moves on the row and save collisions
                for (int j = 0; j < size; j++)
                {
                    if (j != indexOfQueen)
                    {
                        board[i, j] = true;
                        successors[i, j] = calculateCollisions(board);
                        board[i, j] = false;
                    }
                    else
                    {
                        successors[i, j] = 999; //to ignore old position
                    }
                }
                board[i, indexOfQueen] = true; //fixing the row to its first position
            }

            //select lowest value of successors
            int min = 998;
            int indexI = -1;
            int indexJ = -1;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (successors[i, j] < min)
                    {
                        min = successors[i, j];
                        indexI = i;
                        indexJ = j;
                    }
                }
            }

            //make the move, remove queen of the row first and put the new one
            for (int j = 0; j < size; j++)
            {
                if (board[indexI, j] == true)
                {
                    board[indexI, j] = false;
                }
            }
            board[indexI, indexJ] = true;


            drawTheBoard(board);
            Console.WriteLine("Total Collisions: " + calculateCollisions(board));
            //Thread.Sleep(100); //to follow steps
            Console.Clear();
        }

        static void drawTheBoard(bool[,] board)
        {
            int size = Convert.ToInt32(Math.Sqrt(board.Length)); //if the board is 8x8, board.Length will be 64

            for (int k = 0; k < size; k++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (board[k, j] == true)
                        Console.Write("O  ");
                    else
                        Console.Write("-  ");
                }
                Console.WriteLine();
            }
        }


    }
}
