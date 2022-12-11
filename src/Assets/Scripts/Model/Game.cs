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

        private SM.GameState _gameState = new SM.GameState(Guid.NewGuid());

        public const int FieldWidth = SM.GameField.FieldWidth;
        public const int FieldHeight = SM.GameField.FieldHeight;

        public SM.GameField CurrentBoard => _gameState.GameField;

        public SM.FieldCell CurrentPlayerFieldCell()
        {
            var pos = CurrentPlayer.Position;
            return _gameState.GameField[pos.X, pos.Y];
        }

        public Player CurrentPlayer => Players[_gameState.CurrentPlayerIndex];

        public List<SM.PlayerMoveState> CurrentPlayerMoveStates =>
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

        public bool MakeTurn(SM.PlayerAction playerAction)
        {
            return _gameState.PerformAction(playerAction);
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
            _gameState.EndTurn();
            OnStartTurn?.Invoke();
        }

        public void PlayerClicked()
        {
            OnPlayerClicked?.Invoke();
        }
    }
}