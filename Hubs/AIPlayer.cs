namespace TicTacToe.Models
{
    public class AIPlayer
    {
        private readonly Random _random = new Random();

        public int GetNextMove(GameBoard board)
        {
            // Check if AI can win in the next move
            for (int i = 0; i < 9; i++)
            {
                if (string.IsNullOrEmpty(board.Board[i]))
                {
                    // Temporarily make this move
                    board.Board[i] = "O";

                    // Check if this move would result in a win
                    if (board.CheckForWin())
                    {
                        // Undo the move and return this position
                        board.Board[i] = string.Empty;
                        return i;
                    }

                    // Undo the move
                    board.Board[i] = string.Empty;
                }
            }

            // Check if player can win in the next move and block
            for (int i = 0; i < 9; i++)
            {
                if (string.IsNullOrEmpty(board.Board[i]))
                {
                    // Temporarily make this move for the player
                    board.Board[i] = "X";

                    // Check if this move would result in a win for the player
                    if (board.CheckForWin())
                    {
                        // Undo the move and return this position to block
                        board.Board[i] = string.Empty;
                        return i;
                    }

                    // Undo the move
                    board.Board[i] = string.Empty;
                }
            }

            // Try to take the center
            if (string.IsNullOrEmpty(board.Board[4]))
            {
                return 4;
            }

            // Try to take the corners
            var corners = new List<int> { 0, 2, 6, 8 };
            var availableCorners = corners.Where(i => string.IsNullOrEmpty(board.Board[i])).ToList();

            if (availableCorners.Any())
            {
                return availableCorners[_random.Next(availableCorners.Count)];
            }

            // Take any available space
            var availableSpaces = Enumerable.Range(0, 9)
                .Where(i => string.IsNullOrEmpty(board.Board[i]))
                .ToList();

            if (availableSpaces.Any())
            {
                return availableSpaces[_random.Next(availableSpaces.Count)];
            }

            // If no moves available, return -1
            return -1;
        }
    }
}