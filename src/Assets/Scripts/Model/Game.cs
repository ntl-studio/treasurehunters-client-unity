using System;
using System.Collections.Generic;
using UnityEngine;

using SM = NtlStudio.TreasureHunters.Model;

namespace TreasureHunters
{
    public enum PlayerAction
    {
        MoveRight,
        MoveDown,
        MoveLeft,
        MoveUp,
        SkipTurn,
        ThrowGrenade,
        FireGun,
        None
    }
    public class Game
    {
        private SM.GameState _gameState = new SM.GameState(Guid.NewGuid());

        public const int FieldWidth = SM.GameField.FieldWidth;
        public const int FieldHeight = SM.GameField.FieldHeight;

        public SM.GameField CurrentBoard => _gameState.GameField;

        public SM.FieldCell CurrentPlayerFieldCell()
        {
            var pos = CurrentPlayer.Position;
            return _gameState.GameField[pos.X, pos.Y];
        }

        private int _currentPlayer = 0;
        public Player CurrentPlayer => Players[_currentPlayer];

        public int PlayersCount => _gameState.Players.Count;

        public List<Player> Players = new List<Player>();

        public bool MakeTurn(PlayerAction playerAction)
        {
            var pos = CurrentPlayer.Position;

            Vector2Int shift = GameUtils.ActionToVector2(playerAction);
            pos.X += shift.x;
            pos.Y += shift.y;

            CurrentPlayer.Position = pos;

            return true;
        }

        public Game()
        {
            _gameState.RegisterPlayer("Player 1");
            _gameState.RegisterPlayer("Player 2");

            foreach (var p in _gameState.Players)
            {
                Players.Add(new Player(p));
            }

            Players[0].Color = UnityEngine.Color.yellow;
            Players[1].Color = UnityEngine.Color.blue;

            Debug.Log("Game initialized successfully");
        }

        public delegate void GameEvent();

        public event GameEvent OnStartTurn;
        public event GameEvent OnEndTurn;
        public event GameEvent OnPlayerClicked;

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