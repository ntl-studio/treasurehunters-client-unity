using System.Collections.Generic;

namespace TreasureHunters
{
    public class Game
    {
        public Board CurrentBoard => CurrentPlayer.Board;
        public Player CurrentPlayer => _players[_currentPlayer];

        private readonly List<Player> _players = new();
        private int _currentPlayer = 0;

        public Game()
        {
            var board = new Board("Level//level01.txt");

            var player = new Player()
            {
                Board = board,
                Position = GameUtils.FindPlayerPosition(board)
            };

            _players.Add(player);
        }
    }
}