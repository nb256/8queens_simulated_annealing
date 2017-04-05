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
            int totalTime = 0;

            for (int i = 0; i < 35; i++)
            {
                bool[,] board = initializeBoard(8); //initialize for 8x8 (8 queens)


                int time = applySimulatedAnnealingAlgorithmToTheBoard(board, 35.0, 0.0, 0.03, 35, 1.005);


                drawTheBoard(board);
                Console.WriteLine(") Time elapsed: " + time + " ms");

                totalTime += time;

                Thread.Sleep(300); //to see the board
                Console.Clear();
            }


            Console.WriteLine("Average Time: " + (double)totalTime / 35 + " ms ");

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

        static int applySimulatedAnnealingAlgorithmToTheBoard(bool[,] board, double initialTemp, double freezingTemp, double coolingFactor, double currentStabilizer, double stabilizingFactor)
        {
            int size = Convert.ToInt32(Math.Sqrt(board.Length));

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start(); // Begin timing.

            for (double temperature = initialTemp; (temperature > freezingTemp) && (calculateCollisions(board) != 0); temperature = temperature - coolingFactor)
            {
                Console.WriteLine("temp : " + temperature + ", Current Fitness: " + calculateCollisions(board));
                for (int k = 0; k < currentStabilizer; k++)
                {
                    //bool[,] temporaryBoard = board.Clone() as bool[,];
                    bool[,] temporaryBoard = new bool[size, size];
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            temporaryBoard[i, j] = board[i, j];
                        }
                    }

                    Random rnd = new Random();

                    //First make a random move
                    int randomRowIndex;
                    int randomColumnIndex;
                    do
                    {
                        randomRowIndex = rnd.Next(8);
                        randomColumnIndex = rnd.Next(8);
                    }
                    while (temporaryBoard[randomRowIndex, randomColumnIndex] == true && randomRowIndex == randomColumnIndex);
                    for (int j = 0; j < size; j++)
                    {
                        if (temporaryBoard[randomRowIndex, j] == true)
                        {
                            temporaryBoard[randomRowIndex, j] = false;
                        }
                    }
                    temporaryBoard[randomRowIndex, randomColumnIndex] = true;

                    //Then make a best move
                    int[,] successors = new int[size, size];

                    for (int i = 0; i < size; i++)
                    {
                        //find the queen of the row first
                        int indexOfQueen = -1;
                        for (int j = 0; j < size; j++)
                        {
                            if (temporaryBoard[i, j] == true)
                            {
                                indexOfQueen = j;
                                temporaryBoard[i, j] = false;
                            }
                        }
                        //try all the moves on the row and save collisions
                        for (int j = 0; j < size; j++)
                        {
                            if (j != indexOfQueen)
                            {
                                temporaryBoard[i, j] = true;
                                successors[i, j] = calculateCollisions(temporaryBoard);
                                temporaryBoard[i, j] = false;
                            }
                            else
                            {
                                successors[i, j] = 999; //to ignore old position
                            }
                        }
                        temporaryBoard[i, indexOfQueen] = true; //fixing the row to its first position
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
                        if (temporaryBoard[indexI, j] == true)
                        {
                            temporaryBoard[indexI, j] = false;
                        }
                    }
                    temporaryBoard[indexI, indexJ] = true;

                    //drawTheBoard(temporaryBoard);
                    //Thread.Sleep(100); //to follow steps


                    double delta = calculateCollisions(board) - calculateCollisions(temporaryBoard);
                    double probability = Math.Exp(delta / temperature);

                    double rand = rnd.NextDouble();
                    //Console.Clear();

                    if (delta > 0)
                    {
                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                board[i, j] = temporaryBoard[i, j];

                            }
                        }
                    }
                    else if (rand <= probability)
                    {
                        for (int i = 0; i < size; i++)
                        {
                            for (int j = 0; j < size; j++)
                            {
                                board[i, j] = temporaryBoard[i, j];
                            }
                        }
                    }

                }
                currentStabilizer = currentStabilizer * stabilizingFactor;
            }
            stopwatch.Stop();
            return stopwatch.Elapsed.Milliseconds;


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
