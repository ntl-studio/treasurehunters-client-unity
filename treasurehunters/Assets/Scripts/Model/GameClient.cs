using NtlStudio.TreasureHunters.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreasureHunters
{
    public class GameClient
    {
        private static GameClient _instance;

        public static GameClient Instance()
        {
            return _instance ??= new GameClient();
        }

        private GameClient()
        {
            _game.RegisterPlayer("Player 1");
            _game.RegisterPlayer("Player 2");

            int index = 0;
            foreach (var p in _game.Players)
            {
                Players.Add(new Player(this, index++));
            }

            Players[0].Color = Color.yellow;
            Players[1].Color = Color.blue;

            Debug.Log("Game initialized successfully");
        }

        private readonly Game _game = new(Guid.NewGuid());

        public const int FieldWidth = GameField.FieldWidth;
        public const int FieldHeight = GameField.FieldHeight;

        public Player CurrentPlayer => Players[_game.CurrentPlayerIndex];

        public Position CurrentPlayerPreviousPosition()
        {
            var pos = CurrentPlayerMoveStates[0].Position;
            return new Position(pos.X, pos.Y);
        }

        public int CurrentPlayerId => _game.CurrentPlayerIndex;

        public VisibleArea CurrentVisibleArea()
        {
            var field = _game.GameField;
            var player = _game.Players[_game.CurrentPlayerIndex];
            return field.GetVisibleArea(player.Position);
        }

        public bool CurrentPlayerHasTreasure => _game.Players[CurrentPlayerId].HasTreasure;

        public List<PlayerMoveState> CurrentPlayerMoveStates =>
            _game.Players[_game.CurrentPlayerIndex].PlayerMoveStates;

        public Position PlayerPosition(int playerIndex)
        {
            var pos = _game.Players[playerIndex].Position;
            return new Position(pos.X, pos.Y);
        }
        public string PlayerName(int playerIndex)
        {
            return _game.Players[playerIndex].Name;
        }

        public int PlayersCount => _game.Players.Count;

        public List<Player> Players = new();

        public GameState State => _game.State;

        public bool MakeTurn(PlayerAction playerAction)
        {
            return _game.PerformAction(playerAction);
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
            _game.EndTurn();
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
            var pos = _game.TreasurePosition;
            return new Position(pos.X, pos.Y);
        }
    }
}