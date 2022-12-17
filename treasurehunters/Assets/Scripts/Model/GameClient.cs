using NtlStudio.TreasureHunters.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreasureHunters
{
    public enum GameClientState
    {
        NotConnected,           // Opened the app, the list of games is available

        WaitingForGameStart,    // Joined one of the games in the list, the list is still visible

        WaitingForTurn,         // The game has started (you started it or someone else),
                                // you are waiting for other players to make a move

        YourTurn,               // It is your turn, you can move the player

        GameOver                // Someone won the game (it could be you)
    }

    public class GameClient
    {
        private static GameClient _instance;

        public static GameClient Instance()
        {
            return _instance ??= new GameClient();
        }

        public GameClientState State = GameClientState.NotConnected;

        private readonly Game _game = new(Guid.NewGuid());

        public const int FieldWidth = GameField.FieldWidth;
        public const int FieldHeight = GameField.FieldHeight;

        private Player _player;
        public Player CurrentPlayer
        {
            get
            {
                Debug.Assert(_player != null);
                return Players[_game.CurrentPlayerIndex];
            }
        }

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

        public bool MakeTurn(PlayerAction playerAction)
        {
            return _game.PerformAction(playerAction);
        }

        public delegate void GameEvent();

        public event GameEvent OnJoinGame;
        public event GameEvent OnGameStarted;
        public event GameEvent OnStartTurn;
        public event GameEvent OnEndMove;
        public event GameEvent OnPlayerClicked;

        public string GameId
        {
            get
            {
                Debug.Assert(_gameId.Length != 0);
                return _gameId;
            }
        }

        private string _gameId;
        public string _playerName;
        private string _sessionId;

        public bool WaitingForTurn;

        public void JoinGame(string gameId, string playerName, string sessionId)
        {
            _gameId = gameId;
            _playerName = playerName;
            _sessionId = sessionId;

            WaitingForTurn = true;

            OnJoinGame?.Invoke();
        }

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

        public void StartGame() { OnGameStarted?.Invoke(); }

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