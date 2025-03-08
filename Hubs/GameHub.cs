using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TicTacToe.Models;

namespace TicTacToe.Hubs
{
    public class GameHub : Hub
    {
        // Dictionary to store active games with their connection IDs
        private static Dictionary<string, string> _games = new Dictionary<string, string>();
        private static Dictionary<string, GameBoard> _gameBoards = new Dictionary<string, GameBoard>();

        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

            // Check if this game already exists
            if (!_games.ContainsKey(gameId))
            {
                _games[gameId] = Context.ConnectionId;
                _gameBoards[gameId] = new GameBoard();
                await Clients.Caller.SendAsync("PlayerAssigned", "X");
            }
            else
            {
                // Second player joins
                await Clients.Caller.SendAsync("PlayerAssigned", "O");
            }

            // Send current board state
            await Clients.Group(gameId).SendAsync("UpdateBoard", _gameBoards[gameId]);
        }

        public async Task MakeMove(string gameId, int position, string player)
        {
            if (_gameBoards.ContainsKey(gameId))
            {
                GameBoard board = _gameBoards[gameId];

                // Verify it's the player's turn
                if (board.CurrentPlayer == player && !board.IsGameOver)
                {
                    bool success = board.MakeMove(position);
                    if (success)
                    {
                        // Broadcast updated board to all clients in the game
                        await Clients.Group(gameId).SendAsync("UpdateBoard", board);

                        if (board.IsGameOver)
                        {
                            string message = board.Winner != null
                                ? $"Player {board.Winner} wins!"
                                : "Game ended in a draw!";
                            await Clients.Group(gameId).SendAsync("GameOver", message);
                        }
                    }
                }
            }
        }

        public async Task ResetGame(string gameId)
        {
            if (_gameBoards.ContainsKey(gameId))
            {
                _gameBoards[gameId].Reset();
                await Clients.Group(gameId).SendAsync("UpdateBoard", _gameBoards[gameId]);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Handle player disconnection
            string gameId = _games.FirstOrDefault(g => g.Value == Context.ConnectionId).Key;
            if (!string.IsNullOrEmpty(gameId))
            {
                await Clients.Group(gameId).SendAsync("PlayerDisconnected", "The other player has disconnected.");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}