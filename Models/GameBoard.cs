namespace TicTacToe.Models
{
    public class GameBoard
    {
        public string[] Board { get; set; } = new string[9];
        public string CurrentPlayer { get; set; } = "X";
        public string Winner { get; set; } = null;
        public bool IsGameOver { get; set; } = false;
        public int MovesCount { get; set; } = 0;

        public GameBoard()
        {
            // Initialize empty board
            for (int i = 0; i < 9; i++)
            {
                Board[i] = string.Empty;
            }
        }

        public bool MakeMove(int position)
        {
            // Check if position is valid and empty
            if (position < 0 || position >= 9 || !string.IsNullOrEmpty(Board[position]) || IsGameOver)
            {
                return false;
            }

            // Make the move
            Board[position] = CurrentPlayer;
            MovesCount++;

            // Check if the current move results in a win
            if (CheckForWin())
            {
                Winner = CurrentPlayer;
                IsGameOver = true;
                return true;
            }

            // Check for draw
            if (MovesCount >= 9)
            {
                IsGameOver = true;
                return true;
            }

            // Switch player
            CurrentPlayer = CurrentPlayer == "X" ? "O" : "X";
            return true;
        }

        public bool CheckForWin()
        {
            // Check rows
            for (int i = 0; i < 9; i += 3)
            {
                if (!string.IsNullOrEmpty(Board[i]) && Board[i] == Board[i + 1] && Board[i] == Board[i + 2])
                {
                    return true;
                }
            }

            // Check columns
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(Board[i]) && Board[i] == Board[i + 3] && Board[i] == Board[i + 6])
                {
                    return true;
                }
            }

            // Check diagonals
            if (!string.IsNullOrEmpty(Board[0]) && Board[0] == Board[4] && Board[0] == Board[8])
            {
                return true;
            }

            if (!string.IsNullOrEmpty(Board[2]) && Board[2] == Board[4] && Board[2] == Board[6])
            {
                return true;
            }

            return false;
        }

        public void Reset()
        {
            for (int i = 0; i < 9; i++)
            {
                Board[i] = string.Empty;
            }
            CurrentPlayer = "X";
            Winner = null;
            IsGameOver = false;
            MovesCount = 0;
        }
    }
}