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

        private const string PlayerNameKey = "PlayerName";

        private GameClient()
        {
            if (PlayerPrefs.HasKey(PlayerNameKey))
                _playerName = PlayerPrefs.GetString(PlayerNameKey);
        }

        private GameClientState _state;
        public GameClientState State {
            set
            {
                var oldState = _state;
                _state = value;

                if (oldState != value)
                {
                    Debug.Log($"State: {_state}");

                    switch (_state)
                    {
                        case GameClientState.NotConnected:
                        case GameClientState.WaitingForGameStart:
                            break;
                        case GameClientState.WaitingForTurn:
                            if (oldState == GameClientState.WaitingForGameStart)
                                StartGame();
                            break;
                        case GameClientState.YourTurn:
                            if (oldState == GameClientState.WaitingForGameStart)
                                StartGame();
                            break;
                        case GameClientState.GameOver:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            get => _state;
        }

        public delegate void GameEvent();

        public event GameEvent OnJoinGame;      // NotConnected -> WaitingForGameStart
        public event GameEvent OnGameStarted;   // WaitingForGameStater -> WaitingForTurn
        public event GameEvent OnStartTurn;     // WaitingForTurn -> YourTurn
        public event GameEvent OnEndMove;       // YourTurn -> WaitingForTurn
        public event GameEvent OnPlayerClicked; // no state change

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

        public int CurrentPlayerId => _game.CurrentPlayerIndex;

        public VisibleArea CurrentVisibleArea()
        {
            var field = _game.GameField;
            var player = _game.Players[_game.CurrentPlayerIndex];
            return field.GetVisibleArea(player.Position);
        }

        public bool CurrentPlayerHasTreasure => _game.Players[CurrentPlayerId].HasTreasure;

        public List<PlayerMoveState> CurrentPlayerMoveStates =>
            _game.PlayerMoveStates[_game.CurrentPlayerIndex];

        public Position PlayerPosition(int playerIndex)
        {
            var pos = _game.Players[playerIndex].Position;
            return new Position(pos.X, pos.Y);
        } 

        public string PlayerNameOld(int playerIndex)
        {
            return _game.Players[playerIndex].Name;
        }

        public int PlayersCount => _game.Players.Count;

        public List<Player> Players = new();

        public bool MakeTurn(PlayerAction playerAction)
        {
            return _game.PerformAction(playerAction);
        }

        public string GameId
        {
            get
            {
                Debug.Assert(_gameId.Length != 0);
                return _gameId;
            }
        }

        private string _gameId;

        private string _playerName;

        public string PlayerName
        {
            set
            {
                _playerName = value;
                PlayerPrefs.SetString(PlayerNameKey, _playerName);

            }
            get => _playerName;
        }

        private string _sessionId;
        public Position Position;
        public Position PreviousPosition;

        public void JoinGame(string gameId, string playerName, string sessionId)
        {
            _gameId = gameId;
            PlayerName = playerName;
            _sessionId = sessionId;

            State = GameClientState.WaitingForGameStart;

            OnJoinGame?.Invoke();
        }

        public void EndMove()
        {
            OnEndMove?.Invoke();
        }

        public void StartTurn()
        {
            _game.EndTurn();
            OnStartTurn?.Invoke();
        }

        public void PlayerClicked()
        {
            OnPlayerClicked?.Invoke();
        }

        public void StartGame()
        {
            Debug.Log("Starting the game");
            OnGameStarted?.Invoke();
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