using System.Collections.Generic;
using VContainer;

namespace TreasureHunters
{
    public class Game
    {
        public Board CurrentBoard => CurrentPlayer.Board;
        public Player CurrentPlayer => _players[_currentPlayer];

        private readonly List<Player> _players = new();
        private int _currentPlayer = 0;
        private const int PlayersCount = 2;

        public void EndTurn()
        {
            // Player ends his turn 

            // The game switches to the next player and updates the board view

            _currentPlayer++;

            if (_currentPlayer >= PlayersCount)
                _currentPlayer = 0;

        }

        public Game()
        {
            {
                var board = new Board("Level//level01.txt");
                var player = new Player()
                {
                    Name = "Player 1",
                    Board = board,
                    Position = GameUtils.FindPlayerPosition(board)
                };
                _players.Add(player);
            }

            {
                var board = new Board("Level//level01.txt");
                var player = new Player()
                {
                    Name = "Player2",
                    Board = board,
                    Position = GameUtils.FindPlayerPosition(board)
                };
                _players.Add(player);
            }
        }
    }
}