using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TicTacToe.Models;
using System.IO;

namespace TicTacToe.Controllers
{
    public class GameController : Controller
    {
        private const string GameSessionKey = "GameState";

        // GET: Game
        public IActionResult Index()
        {
            string viewPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "Game", "Index.cshtml");

            if (!System.IO.File.Exists(viewPath))
            {
                return Content("View not found: " + viewPath);
            }

            GameState gameState = GetGameState();
            return View(gameState);
        }

        // POST: Game/MakeMove
        [HttpPost]
        public IActionResult MakeMove(int position)
        {
            GameState gameState = GetGameState();
            bool moveSuccess = gameState.Board.MakeMove(position);

            if (gameState.Board.IsGameOver)
            {
                gameState.UpdateStats();
            }

            SaveGameState(gameState);

            return Json(new
            {
                success = moveSuccess,
                board = gameState.Board.Board,
                currentPlayer = gameState.Board.CurrentPlayer,
                isGameOver = gameState.Board.IsGameOver,
                winner = gameState.Board.Winner,
                playerXWins = gameState.PlayerXWins,
                playerOWins = gameState.PlayerOWins,
                draws = gameState.Draws
            });
        }

        // For AI mode
        [HttpPost]
        public IActionResult MakeAIMove()
        {
            GameState gameState = GetGameState();

            // Check if it's AI's turn (O) and game is not over
            if (gameState.Board.CurrentPlayer == "O" && !gameState.Board.IsGameOver)
            {
                AIPlayer ai = new AIPlayer();
                int aiMove = ai.GetNextMove(gameState.Board);

                if (aiMove >= 0)
                {
                    gameState.Board.MakeMove(aiMove);

                    if (gameState.Board.IsGameOver)
                    {
                        gameState.UpdateStats();
                    }

                    SaveGameState(gameState);
                }
            }

            return Json(new
            {
                board = gameState.Board.Board,
                currentPlayer = gameState.Board.CurrentPlayer,
                isGameOver = gameState.Board.IsGameOver,
                winner = gameState.Board.Winner,
                playerXWins = gameState.PlayerXWins,
                playerOWins = gameState.PlayerOWins,
                draws = gameState.Draws
            });
        }

        // POST: Game/Reset
        [HttpPost]
        public IActionResult Reset()
        {
            GameState gameState = GetGameState();
            gameState.Board.Reset();
            SaveGameState(gameState);

            return Json(new
            {
                board = gameState.Board.Board,
                currentPlayer = gameState.Board.CurrentPlayer,
                isGameOver = gameState.Board.IsGameOver,
                winner = gameState.Board.Winner
            });
        }

        // POST: Game/ResetStats
        [HttpPost]
        public IActionResult ResetStats()
        {
            GameState gameState = new GameState();
            SaveGameState(gameState);

            return Json(new
            {
                playerXWins = gameState.PlayerXWins,
                playerOWins = gameState.PlayerOWins,
                draws = gameState.Draws
            });
        }

        private GameState GetGameState()
        {
            string gameStateJson = HttpContext.Session.GetString(GameSessionKey);
            if (string.IsNullOrEmpty(gameStateJson))
            {
                GameState newGameState = new GameState();
                SaveGameState(newGameState);
                return newGameState;
            }
            return JsonConvert.DeserializeObject<GameState>(gameStateJson);
        }

        private void SaveGameState(GameState gameState)
        {
            string gameStateJson = JsonConvert.SerializeObject(gameState);
            HttpContext.Session.SetString(GameSessionKey, gameStateJson);
        }
    }
}
