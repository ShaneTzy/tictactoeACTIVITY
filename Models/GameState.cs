namespace TicTacToe.Models
{
    public class GameState
    {
        public GameBoard Board { get; set; }
        public int PlayerXWins { get; set; } = 0;
        public int PlayerOWins { get; set; } = 0;
        public int Draws { get; set; } = 0;

        public GameState()
        {
            Board = new GameBoard();
        }

        public void UpdateStats()
        {
            if (Board.IsGameOver)
            {
                if (Board.Winner == "X")
                {
                    PlayerXWins++;
                }
                else if (Board.Winner == "O")
                {
                    PlayerOWins++;
                }
                else
                {
                    Draws++;
                }
            }
        }
    }
}