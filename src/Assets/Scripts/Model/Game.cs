using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;

using SM = NtlStudio.TreasureHunters.Model;

namespace TreasureHunters
{
    public class Game
    {
        private SM.GameState _gameState = new SM.GameState(Guid.NewGuid());

        public SM.GameField CurrentBoard => _gameState.GameField;

        public SM.VisibleArea CurrentVisibleArea =>
            _gameState.GetPlayerVisibleArea(_gameState.Players.ToList<SM.Player>()[_currentPlayer]);

        public Player CurrentPlayer => Players[_currentPlayer];

        public int PlayersCount => _gameState.Players.Count;

        public List<Player> Players = new List<Player>();
        private int _currentPlayer = 0;

        public void Init()
        {
            _gameState.RegisterPlayer("Player 1");
            _gameState.RegisterPlayer("Player 2");


            foreach (var p in _gameState.Players)
            {
                Players.Add(new Player(p));
            }

            Debug.Log("Game initialized successfully");
        }

        public delegate void GameEvent();

        public event GameEvent OnStartTurn;
        public event GameEvent OnEndTurn;
        public event GameEvent OnPlayerClicked;

        public bool IsCellVisible(int col, int row)
        {
            return Math.Abs(CurrentPlayer.Position.X - col) <= 1 && Math.Abs(CurrentPlayer.Position.Y - row) <= 1;
        }

        public void EndTurn()
        {
            OnEndTurn?.Invoke();
        }

        public void StartNextTurn()
        {
            _currentPlayer++;

            if (_currentPlayer >= _gameState.Players.Count)
                _currentPlayer = 0;

            OnStartTurn?.Invoke();
        }

        public void PlayerClicked()
        {
            OnPlayerClicked?.Invoke();
        }

    }
}