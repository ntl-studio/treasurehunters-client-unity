using NtlStudio.TreasureHunters.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreasureHunters
{
    public enum GameClientState
    {
        NotConnected,           // Opened the app, the list of games is available

        Joined, 

        WaitingForStart,    // Joined one of the games in the list, the list is still visible

        WaitingForTurn,         // The game has started (you started it or someone else),
                                // you are waiting for other players to make a move

        YourTurn,               // It is your turn, you can move the player

        MakingMove,

        Finished                // Someone won the game (it could be you)
    }

    public class GameClient
    {
        private static GameClient _instance;

        public static GameClient Instance()
        {
            return _instance ??= new GameClient();
        }

        private const string PlayerNameKey = "PlayerName";
        private const string ServerNameKey = "ServerName";

        private GameClient()
        {
            if (PlayerPrefs.HasKey(PlayerNameKey))
                _playerName = PlayerPrefs.GetString(PlayerNameKey);

            if (PlayerPrefs.HasKey(ServerNameKey))
                _serverName = PlayerPrefs.GetString(ServerNameKey);

            AddCallbacksDebug();
        }

        private GameClientState _state = GameClientState.NotConnected;
        public GameClientState State
        {
            set
            {
                var oldState = _state;
                _state = value;

                if (oldState != value)
                {
                    Debug.Log($"State changed from {oldState} to {_state}");

                    switch (_state)
                    {
                        case GameClientState.NotConnected:
                            break;
                        case GameClientState.Joined:
                            OnJoined?.Invoke();
                            break;
                        case GameClientState.WaitingForStart:
                            OnWaitingForStart?.Invoke();
                            break;
                        case GameClientState.WaitingForTurn:
                            OnWaitingForTurn?.Invoke();
                            break;
                        case GameClientState.YourTurn:
                            OnYourTurn?.Invoke();
                            break;
                        case GameClientState.MakingMove:
                            Debug.LogError("Should not be assigning MakingMove state directory");
                            break;
                        case GameClientState.Finished:
                            OnGameFinished?.Invoke();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            get => _state;
        }

        public delegate void GameEvent();
        public delegate void GameEventBool(bool value);

        public event GameEvent OnJoined;
        public event GameEvent OnWaitingForStart;
        public event GameEvent OnWaitingForTurn;
        public event GameEvent OnYourTurn;
        public event GameEvent OnGameFinished;
        public event GameEventBool OnMakingMove;

        public event GameEvent OnEndMove;

        public event GameEvent OnPlayerClicked;
        public event GameEvent OnUpdateVisibleArea;
        public event GameEvent OnUpdatePlayerPosition;
        public event GameEvent OnUpdatePlayersMoveHistory;

        public event GameEvent OnUpdatePlayerName;
        public event GameEvent OnUpdateServerName;
        public event GameEventBool OnUpdatePlayerHasTreasure;
        public event GameEvent OnUpdateTreasurePosition_Debug;

        // current player is the player who's turn is now, and it is not necessarily player who runs the client
        public event GameEvent OnUpdateCurrentPlayerName; 

        private void AddCallbacksDebug()
        {
            OnJoined += () => Debug.Log("OnJoined");
            OnWaitingForStart += () => Debug.Log("OnWaitingForStart");
            OnYourTurn += () => Debug.Log("OnYourTurn");
            OnEndMove += () => Debug.Log("OnEndMove");

            OnPlayerClicked += () => Debug.Log("OnPlayerClicked");

            OnUpdateVisibleArea += () => Debug.Log("OnUpdateVisibleArea");
            OnUpdatePlayerPosition += () => Debug.Log("OnUpdatePlayerPosition");
        }

        private readonly Game _game = new(Guid.NewGuid());

        public const int FieldWidth = GameField.FieldWidth;
        public const int FieldHeight = GameField.FieldHeight;

        public int CurrentPlayerId => _game.CurrentPlayerIndex;


        private string _currentPlayerName;

        public string CurrentPlayerName
        {
            set
            {
                _currentPlayerName = value; 
                OnUpdateCurrentPlayerName?.Invoke();
            }
            get => _currentPlayerName;
        }

        private VisibleArea _visibleArea;

        public void SetVisibleArea(int[] cells)
        {
            FieldCell[,] fieldCells = new FieldCell[VisibleArea.Width, VisibleArea.Height];

            for (int x = 0; x < VisibleArea.Width; ++x)
            {
                for (int y = 0; y < VisibleArea.Height; ++y)
                    fieldCells[x, y] = (FieldCell)cells[y * 3 + x];
            }
            _visibleArea = new VisibleArea(fieldCells);

            OnUpdateVisibleArea?.Invoke();
        }

        public VisibleArea CurrentVisibleArea()
        {
            Debug.Assert(_visibleArea != null);
            return _visibleArea;
        }

        private List<PlayerMovesDetails> _playersMovesHistory;

        public List<PlayerMovesDetails> PlayersMovesHistory
        {
            set
            {
                _playersMovesHistory = value;
                OnUpdatePlayersMoveHistory?.Invoke();
            }
            get => _playersMovesHistory;
        }

        protected bool _playerHasTreasure;
        public bool PlayerHasTreasure
        {
            set
            {
                _playerHasTreasure = value;
                OnUpdatePlayerHasTreasure?.Invoke(_playerHasTreasure);
            }
            get => _playerHasTreasure;
        }

        private Position _playerPosition;
        public Position PlayerPosition
        {
            get => _playerPosition;
            set
            {
                PreviousPosition = _playerPosition;
                _playerPosition = value;
                OnUpdatePlayerPosition?.Invoke();
            }
        }

        public int PlayersCount = -1;

        public Dictionary<string, Position> Enemies = new();

        public void PerformAction(PlayerAction playerAction)
        {
            ServerConnection.Instance().PerformActionAsync(
                GameId, PlayerName, playerAction.ToString(),
                (actionResult, hasTreasure, gameState) =>
                {
                    if (gameState == "Finished")
                        State = GameClientState.Finished;

                    PlayerHasTreasure = hasTreasure;
                    OnMakingMove?.Invoke(actionResult);
                });
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
                OnUpdatePlayerName?.Invoke();
            }
            get => _playerName;
        }

        private string _serverName;
        public string ServerName
        {
            set
            {
                _serverName = value;
                PlayerPrefs.SetString(ServerNameKey, _serverName);
                OnUpdateServerName?.Invoke();
            }
            get => _serverName;
        }

        private string _sessionId;

        public Position PreviousPosition;

        private bool _isStarted;
        public void JoinGame(string gameId, string sessionId)
        {
            Debug.Log($"Joined game {gameId}");
            _gameId = gameId;
            _sessionId = sessionId;

            State = GameClientState.Joined;
        }

        public void EndMove()
        {
            OnEndMove?.Invoke();
        }

        public void StartTurn()
        {
            _game.EndTurn();
            OnYourTurn?.Invoke();
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

        protected Position _treasurePosition_Debug;
        public Position TreasurePosition_Debug
        {
            set
            {
                _treasurePosition_Debug = value;
                OnUpdateTreasurePosition_Debug?.Invoke();
            }
            get => _treasurePosition_Debug;
        }
    }
}