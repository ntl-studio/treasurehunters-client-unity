using NtlStudio.TreasureHunters.Model;
using System;
using System.Collections.Generic;
using UnityEngine;
using static NtlStudio.TreasureHunters.Model.ActionType;

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

        PlayerDied,             // This player is dead, so game is "over", but technically not Finished as 
                                // other can still play

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

        public int HistorySize => Game.ActionsHistorySize;

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
                        case GameClientState.PlayerDied:
                            IsPlayerAlive = false;
                            OnPlayerDied?.Invoke();
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
        public event GameEventBool OnPerformActionClient;
        public event GameEventBool OnPerformActionServer;
        public event GameEvent OnEndMove;
        public event GameEvent OnEndMoveAnimation;
        public event GameEvent OnGameFinished;
        public event GameEvent OnPlayerDied;

        public event GameEvent OnUpdateVisibleArea;
        public event GameEvent OnUpdatePlayerPosition;
        public event GameEvent OnUpdateBullets;
        public event GameEvent OnUpdatePlayersMoveHistory;

        public event GameEvent OnUpdatePlayerName;
        public event GameEvent OnUpdateServerName;
        public event GameEvent OnUpdateWinner;
        public event GameEventBool OnUpdatePlayerHasTreasure;
        public event GameEvent OnUpdateTreasurePosition_Debug;

        // current player is the player who's turn is now, and it is not necessarily player who runs the client
        public event GameEvent OnUpdateCurrentPlayerName;

        public event GameEvent OnChoosePlayerAction;
        public event GameEvent OnChoosePlayerActionCancel;

        // when the player chooses to fire gun from the menu
        public event GameEvent OnStartFiringGun;

        private void AddCallbacksDebug()
        {
            OnJoined += () => Debug.Log("OnJoined");
            OnWaitingForStart += () => Debug.Log("OnWaitingForStart");
            OnYourTurn += () => Debug.Log("OnYourTurn");
            OnEndMoveAnimation += () => Debug.Log("OnEndMoveAnimation");

            OnUpdateVisibleArea += () => Debug.Log("OnUpdateVisibleArea");
            OnUpdatePlayerPosition += () => Debug.Log("OnUpdatePlayerPosition");
        }

        public const int FieldWidth = GameField.FieldWidth;
        public const int FieldHeight = GameField.FieldHeight;

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
            set
            {
                PreviousPosition = _playerPosition;
                _playerPosition = value;
                OnUpdatePlayerPosition?.Invoke();
            }
            get => _playerPosition;
        }

        private int _bullets;

        public int Bullets
        {
            set
            {
                _bullets = value;
                OnUpdateBullets?.Invoke();
            }
            get => _bullets;
        }

        public int PlayersCount = -1;

        public Dictionary<string, Position> Enemies = new();

        public async void PerformAction(PlayerAction playerAction)
        {
            var result = PerformActionClient(playerAction);
            OnPerformActionClient?.Invoke(result);

            var actionResult = await ServerConnection.Instance().PerformActionAsync(GameId, PlayerName, playerAction);

            if (actionResult.data.state == "Finished")
                State = GameClientState.Finished;

            PlayerHasTreasure = actionResult.data.hastreasure;

            if (!actionResult.successful)
                Debug.Log($"Perform Action failed: {actionResult.errormessage}");

            OnPerformActionServer?.Invoke(actionResult.successful);

            if (actionResult.successful && playerAction.Type is Move or Skip)
                OnEndMove?.Invoke();
        }

        bool PerformActionClient(PlayerAction playerAction)
        {
            if (playerAction.Type is not Move) 
                return false;

            var dir = playerAction.Direction;

            return
                (dir == ActionDirection.Right && !_visibleArea[1, 1].HasFlag(FieldCell.RightWall)) ||
                (dir == ActionDirection.Down && !_visibleArea[1, 1].HasFlag(FieldCell.BottomWall)) ||
                (dir == ActionDirection.Left && !_visibleArea[1, 1].HasFlag(FieldCell.LeftWall)) ||
                (dir == ActionDirection.Up && !_visibleArea[1, 1].HasFlag(FieldCell.TopWall));
        }

        private string _gameId;
        public string GameId
        {
            get
            {
                Debug.Assert(_gameId.Length != 0);
                return _gameId;
            }
        }

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

        private string _winnerName;
        public string WinnerName
        {
            set
            {
                _winnerName = value;
                OnUpdateWinner?.Invoke();
            }
            get => _winnerName;
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

        public void EndMoveAnimation()
        {
            OnEndMoveAnimation?.Invoke();
        }

        public void StartTurn()
        {
            OnYourTurn?.Invoke();
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

        public bool IsPlayerAlive { get; private set; } = true;

        public void StartFiringGun() { OnStartFiringGun?.Invoke(); }
        public void ChoosePlayerAction() { OnChoosePlayerAction?.Invoke(); }
        public void ChoosePlayerActionCancel() { OnChoosePlayerActionCancel?.Invoke(); }
    }
}