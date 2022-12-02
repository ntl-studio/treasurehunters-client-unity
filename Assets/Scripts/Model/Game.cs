using System;
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

        public delegate void EndTurnEventHandler();
        public event EndTurnEventHandler OnEndTurn;

        public void EndTurn()
        {
            _currentPlayer++;

            if (_currentPlayer >= PlayersCount)
                _currentPlayer = 0;

            OnEndTurn?.Invoke();
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
                    Name = "Player 2",
                    Board = board,
                    Position = GameUtils.FindPlayerPosition(board)
                };
                _players.Add(player);
            }
        }
    }
}