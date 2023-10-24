using System.Data.Common;
using System.Diagnostics.SymbolStore;
using static System.Formats.Asn1.AsnWriter;

internal class Program
{
    public class ConnectFour
    {
        public const int ROWS = 8;
        public const int COLS = 8;
        public const int WINDOW_SIZE = 4;
        public int AI_DEPTH = 7;

        const int PlayerTurn = 0;
        public const int AITurn = 1;

        const int playerPiece = 1;
        const int AIPiece = 2;
        const int EmptyPiece = 0;

        private string intToPiece(int code)
        {
            if (playerPiece == code) return "X";
            if (AIPiece == code) return "O";
            return " ";
        }

        private bool checkWinner()
        {
            const int XinARow = WINDOW_SIZE;

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    int cellValue = board[i, j];
                    if (cellValue == 0)
                        continue;

                    if (j + XinARow <= COLS)
                    {
                        bool isHorizontalMatch = true;
                        for (int k = 1; k < XinARow; k++)
                        {
                            if (board[i, j + k] != cellValue)
                            {
                                isHorizontalMatch = false;
                                break;
                            }
                        }
                        if (isHorizontalMatch)
                            return true;
                    }

                    if (i + XinARow <= ROWS)
                    {
                        bool isVerticalMatch = true;
                        for (int k = 1; k < XinARow; k++)
                        {
                            if (board[i + k, j] != cellValue)
                            {
                                isVerticalMatch = false;
                                break;
                            }
                        }
                        if (isVerticalMatch)
                            return true;
                    }

                    if (j + XinARow <= COLS && i + XinARow <= ROWS)
                    {
                        bool isDiagonalMatch = true;
                        for (int k = 1; k < XinARow; k++)
                        {
                            if (board[i + k, j + k] != cellValue)
                            {
                                isDiagonalMatch = false;
                                break;
                            }
                        }
                        if (isDiagonalMatch)
                            return true;
                    }

                    if (j - XinARow >= 0 && i + XinARow <= ROWS)
                    {
                        bool isDiagonalMatch = true;
                        for (int k = 1; k < XinARow; k++)
                        {
                            if (board[i + k, j - k] != cellValue)
                            {
                                isDiagonalMatch = false;
                                break;
                            }
                        }
                        if (isDiagonalMatch)
                            return true;
                    }
                }
            }

            return false;
        }

        private bool isWinnerMove(int[,] map, int piece)
        {
            const int XinARow = WINDOW_SIZE;

            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    int cellValue = piece;
                    if (cellValue == 0)
                        continue;

                    if (j + XinARow <= COLS)
                    {
                        bool isHorizontalMatch = true;
                        for (int k = 1; k < XinARow; k++)
                        {
                            if (map[i, j + k] != cellValue)
                            {
                                isHorizontalMatch = false;
                                break;
                            }
                        }
                        if (isHorizontalMatch)
                            return true;
                    }

                    if (i + XinARow <= ROWS)
                    {
                        bool isVerticalMatch = true;
                        for (int k = 1; k < XinARow; k++)
                        {
                            if (map[i + k, j] != cellValue)
                            {
                                isVerticalMatch = false;
                                break;
                            }
                        }
                        if (isVerticalMatch)
                            return true;
                    }

                    if (j + XinARow <= COLS && i + XinARow <= ROWS)
                    {
                        bool isDiagonalMatch = true;
                        for (int k = 1; k < XinARow; k++)
                        {
                            if (map[i + k, j + k] != cellValue)
                            {
                                isDiagonalMatch = false;
                                break;
                            }
                        }
                        if (isDiagonalMatch)
                            return true;
                    }

                    if (j - XinARow >= 0 && i + XinARow <= ROWS)
                    {
                        bool isDiagonalMatch = true;
                        for (int k = 1; k < XinARow; k++)
                        {
                            if (map[i + k, j - k] != cellValue)
                            {
                                isDiagonalMatch = false;
                                break;
                            }
                        }
                        if (isDiagonalMatch)
                            return true;
                    }
                }
            }

            return false;
        }

        private int[,] board = new int[ROWS, COLS];


        public ConnectFour() { }

        private void resetBoard()
        {
            board = new int[ROWS, COLS];
        }
        private bool validDrop(int column)
        {
            if (column < 0 || column >= COLS) return false;

            return board[0, column] == 0;
        }

        private int[,] dropPiece(int[,] map,int column, int ch)
        {
            int row = 0;
            while (row + 1 < ROWS && map[row + 1, column] == 0)
            {
                row++;
            }
            map[row, column] = ch;
            return map;
        }
        private void makeAMove(int column, int ch)
        {
            if (!validDrop(column)) return;
            int row = 0;
            while (row + 1 < ROWS && board[row + 1, column] == 0)
            {
                row++;
            }
            board[row, column] = ch;
            printBoard();
            if (checkWinner())
            {
                Console.WriteLine(ch == 1 ? "You wins" : "AI wins");
                resetBoard();
            }
        }

        private int evalWindow(int[] window,int piece)
        {
            
            int score = 0;
            int emptyCnt = window.Count(item => item == EmptyPiece);
            int currCnt = window.Count(item => item == piece);
            int oppCnt = WINDOW_SIZE - currCnt - emptyCnt;
            //Console.WriteLine($"AI: {currCnt} -> OP:{oppCnt} -> MT:{emptyCnt}");
            if (currCnt == 4) score += 100000000;
            else if (currCnt == 3 && emptyCnt == 1) score += 6500;
            else if (currCnt == 2 && emptyCnt == 2) score += 9;

            if (oppCnt == 4) score -= 10000;
            else if (oppCnt == 3 && emptyCnt == 1) score -= 10000;
            else if (oppCnt == 2 && emptyCnt == 2) score -= 800;
            else if (oppCnt == 2 && emptyCnt == 2) score -= 1;

            return score;
        }

        private int ScorePosition(int[,] map,int piece)
        {
            const int WINDOW_LENGTH = WINDOW_SIZE;
            const int centerMux = 9;

            int score = 0;

            // Center
            int[] centerArray = new int[ROWS];
            for (int i = 0; i < ROWS; i++)
            { centerArray[i] = map[i, COLS / 2]; }
            score += centerArray.Count(item => item == piece) * centerMux;

            // Aim Low
            for(int r = 0; r < ROWS; r++)
            {
                for(int c = 0; c < COLS; c++)
                {
                    if (map[r, c] == 0) score += ROWS - r;
                }
            }
            

            // Horizontal
            for (int r = ROWS-1; r>=0; r--) {
                int[] rowArray = new int[map.GetLength(1)];
                for (int i = 0; i < map.GetLength(1); i++)
                {rowArray[i] = map[r, i];}
                for(int c = 0; c < COLS - WINDOW_LENGTH; c++)
                {
                    int[] window = new int[WINDOW_LENGTH];
                    for (int i = 0; i < WINDOW_LENGTH; i++)
                    {
                        window[i] = rowArray[c + i];
                    }
                    score += evalWindow(window, piece);
                }
            }
            // veritcal
            for (int c = 0; c < COLS; c++)
            {
                int[] columnArray = new int[map.GetLength(0)];
                for (int r = 0; r < map.GetLength(0); r++)
                {
                    columnArray[r] = map[r, c];
                }
                for (int r = 0; r < ROWS - WINDOW_LENGTH; r++)
                {
                    int[] window = new int[WINDOW_LENGTH];
                    for (int i = 0; i < WINDOW_LENGTH; i++)
                    {
                        window[i] = columnArray[r + i];
                    }
                    score += evalWindow(window, piece);
                }
            }

            // pos diagonal
            for (int r = 0; r < ROWS - WINDOW_LENGTH; r++)
            {
                for (int c = 0; c < COLS - WINDOW_LENGTH; c++)
                {
                    int[] window = new int[WINDOW_LENGTH];
                    for (int i = 0; i < WINDOW_LENGTH; i++)
                    {
                        window[i] = map[r + i, c + i];
                    }
                    score += evalWindow(window, piece);
                }
            }

            // neg diagonal
            for (int r = 0; r < ROWS - WINDOW_LENGTH; r++)
            {
                for (int c = 0; c < COLS - WINDOW_LENGTH; c++)
                {
                    int[] window = new int[WINDOW_LENGTH];
                    for (int i = 0; i < WINDOW_LENGTH; i++)
                    {
                        window[i] = map[r + (WINDOW_LENGTH - 1) - i, c + i];
                    }
                    score += evalWindow(window, piece);
                }
            }





            //Console.WriteLine($"The score was {score}");

            return score;
        }

        private int pick_best_move(int[,] map,int piece)
        {
            
            int[] validLocations = getValidLocations(map);
            Random r = new Random();
            int bestScore = int.MinValue, bestCol = validLocations[r.Next(validLocations.Length)];
            foreach (int col in validLocations)
            {
                int currRow = getNextOpenRow(map, col);
                int[,] tmpBoard = (int[,])map.Clone();
                tmpBoard = dropPiece(tmpBoard,col, piece);
                int score = ScorePosition(tmpBoard, piece);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCol = col;
                }
            }

            return bestCol;
        }

        private int[] minimax(int[,] map, int depth, bool isMaximingPlayer, int alpha = int.MinValue, int beta = int.MaxValue)
        {
            int[] validLocations = getValidLocations(map);
            bool is_terminal_node = isTerminalNode(map);

            Random r = new Random();
            int rndValidPos = validLocations[r.Next(validLocations.Length)];

            if (depth == 0 || is_terminal_node)
            {
                if (is_terminal_node)
                {
                    if (isWinnerMove(map, AIPiece)) return new int[] { rndValidPos, int.MaxValue };
                    else if (isWinnerMove(map, playerPiece)) return new int[] { rndValidPos, int.MinValue };
                    else return new int[] { rndValidPos, 0 };
                }
                else // depth is zero
                {
                    return new int[] { rndValidPos, ScorePosition(map, AIPiece) };
                }
            }


            if (isMaximingPlayer)
            {
                int value = int.MinValue;
                int column = rndValidPos; // Initialize with an Random valid

                foreach (int col in validLocations)
                {
                    int row = getNextOpenRow(map, col);
                    int[,] mapClone = (int[,])map.Clone();
                    mapClone = dropPiece(mapClone, col, AIPiece);
                    int newscore = minimax(mapClone, depth - 1, false, alpha, beta)[1];

                    if (newscore > value)
                    {
                        value = newscore;
                        column = col;
                    }

                    // Alpha-beta pruning
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }

                return new int[] { column, value };
            }
            else
            {
                int value = int.MaxValue;
                int column = rndValidPos; // Initialize with an Random valid

                foreach (int col in validLocations)
                {
                    int row = getNextOpenRow(map, col);
                    int[,] mapClone = (int[,])map.Clone();
                    mapClone = dropPiece(mapClone, col, playerPiece);
                    int newscore = minimax(mapClone, depth - 1, true, alpha, beta)[1];

                    if (newscore < value)
                    {
                        column = col;
                        value = newscore;
                    }

                    // Alpha-beta pruning
                    beta = Math.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }

                //Console.WriteLine($"{column},{value}");

                return new int[] { column, value };
            }
        }

        private bool isTerminalNode(int[,] map)
        {
            return isWinnerMove(map, playerPiece) || isWinnerMove(map,AIPiece) || getValidLocations(map).Length == 0;
        }
        private int getNextOpenRow(int[,] map, int col)
        {
            int currRow = 0;
            while (currRow + 1 < ROWS && map[currRow + 1, col] != 0) currRow++;
            return currRow;
        }

        private int[] getValidLocations(int[,] map)
        {
            List<int> locations = new List<int>();
            for (int i = 0; i < COLS; i++)
                if (map[0, i] == 0) locations.Add(i);
            return locations.ToArray();
        }

        public void AIMakeDecision()
        {
            //int AIMove = pick_best_move(board, AIPiece);
            int AIMove = minimax(board, AI_DEPTH, true)[0];
            Console.WriteLine($"AI Choose {AIMove}");
            makeAMove(AIMove, AIPiece);
        }

        public void PlayerMakeDecision(int column)
        {
            makeAMove(column, playerPiece);
        }

        private void printBoard()
        {
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    Console.Write(intToPiece(board[i, j]) + " ");
                }
                Console.WriteLine();
            }
        }

        private bool isGameOver()
        {
            return isTerminalNode(board);
        }

        public int[,] getBoard()
        {
            return board;
        }
    }
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        ConnectFour game = new ConnectFour();

        /*game.printBoard();
        Console.WriteLine("_________");
        game.dropPiece(0, 2);
        game.printBoard();
        Console.WriteLine("_________");*/

        while (true)
        {
            Console.WriteLine("Player Move: ");
            int input = Convert.ToInt32(Console.ReadLine());
            game.PlayerMakeDecision(input);
            Console.WriteLine("AI Move: ");
            game.AIMakeDecision();
        }


    }
}