using System.Collections.Generic;

namespace TreasureHunters
{
    public class Game
    {
        public Board CurrentBoard => CurrentPlayer.Board;
        public Player CurrentPlayer => Players[_currentPlayer];
        public int PlayersCount => GameSettings.PlayersCount;

        public readonly List<Player> Players = new();
        private int _currentPlayer = 0;

        public delegate void TurnEventHandler();
        public event TurnEventHandler OnEndTurn;
        public event TurnEventHandler OnStartNextTurn;

        public void EndTurn()
        {
            OnEndTurn?.Invoke();
        }

        public void StartNextTurn()
        {
            _currentPlayer++;

            if (_currentPlayer >= GameSettings.PlayersCount)
                _currentPlayer = 0;

            OnStartNextTurn?.Invoke();
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
                Players.Add(player);
            }

            {
                var board = new Board("Level//level01.txt");
                var player = new Player()
                {
                    Name = "Player 2",
                    Board = board,
                    Position = GameUtils.FindPlayerPosition(board)
                };
                Players.Add(player);
            }
        }
    }
}