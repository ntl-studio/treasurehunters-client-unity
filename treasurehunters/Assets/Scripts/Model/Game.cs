using NtlStudio.TreasureHunters.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

using SM = NtlStudio.TreasureHunters.Model;

namespace TreasureHunters
{
    public class Game
    {
        private static Game _instance;

        public static Game Instance()
        {
            return _instance ??= new Game();
        }

        private Game()
        {
            _gameState.RegisterPlayer("Player 1");
            _gameState.RegisterPlayer("Player 2");

            int index = 0;
            foreach (var p in _gameState.Players)
            {
                Players.Add(new Player(this, index++));
            }

            Players[0].Color = Color.yellow;
            Players[1].Color = Color.blue;

            Debug.Log("Game initialized successfully");
        }

        private readonly GameState _gameState = new(Guid.NewGuid());

        public const int FieldWidth = GameField.FieldWidth;
        public const int FieldHeight = GameField.FieldHeight;

        public Player CurrentPlayer => Players[_gameState.CurrentPlayerIndex];

        public Position CurrentPlayerPreviousPosition()
        {
            var pos = CurrentPlayerMoveStates[0].Position;
            return new Position(pos.X, pos.Y);
        }

        public int CurrentPlayerId => _gameState.CurrentPlayerIndex;

        public VisibleArea CurrentVisibleArea()
        {
            var field = _gameState.GameField;
            var player = _gameState.Players[_gameState.CurrentPlayerIndex];
            return field.GetVisibleArea(player.Position);
        }

        public bool CurrentPlayerHasTreasure => _gameState.Players[CurrentPlayerId].HasTreasure;

        public List<PlayerMoveState> CurrentPlayerMoveStates =>
            _gameState.Players[_gameState.CurrentPlayerIndex].PlayerMoveStates;

        public Position PlayerPosition(int playerIndex)
        {
            var pos = _gameState.Players[playerIndex].Position;
            return new Position(pos.X, pos.Y);
        }
        public string PlayerName(int playerIndex)
        {
            return _gameState.Players[playerIndex].Name;
        }

        public int PlayersCount => _gameState.Players.Count;

        public List<Player> Players = new();

        public bool MakeTurn(PlayerAction playerAction)
        {
            return _gameState.PerformAction(playerAction);
        }

        public delegate void GameEvent();
        public event GameEvent OnStartTurn;
        public event GameEvent OnEndMove;
        public event GameEvent OnPlayerClicked;

        public void EndMove()
        {
            OnEndMove?.Invoke();
        }

        public void StartNextTurn()
        {
            _gameState.EndTurn();
            OnStartTurn?.Invoke();
        }

        public void PlayerClicked()
        {
            OnPlayerClicked?.Invoke();
        }

        public delegate void ShowTreasureEvent(bool isVisible);
        public event ShowTreasureEvent OnShowTreasureEvent;

        public void ShowTreasure(bool isVisible)
        {
            IsTreasureAlwaysVisible = isVisible;
            OnShowTreasureEvent?.Invoke(isVisible);
        }

        public bool IsTreasureAlwaysVisible { get; private set; }

        public Position TreasurePosition()
        {
            var pos = _gameState.TreasurePosition;
            return new Position(pos.X, pos.Y);
        }
    }
}