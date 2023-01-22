using System;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using Unity.Mathematics;
using UnityEngine;
using Position = TreasureHunters.Position;

public class PlayerMovement : MonoBehaviour
{
    public PlayerAnimation PlayerAnimation;

    [SerializeField]
    private float _speed = 5f;

    private bool _acceptInput = false;
    private bool _enableAcceptInput = false;

    private bool _isPlayingMovingAnimation;
    private Vector3 _destination;
    private Vector3 _direction;

    private GameObject _playerCameraObject;
    private Camera _playerCamera;

    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        var cameras = GameObject.FindGameObjectsWithTag("PlayerCamera");
        Debug.Assert(cameras.Length == 1);
        _playerCameraObject = cameras[0];
        Debug.Assert(_playerCameraObject);
        _playerCamera = _playerCameraObject.GetComponent<Camera>();
        Debug.Assert(_playerCamera);

        Game.OnYourTurn += () => { _acceptInput = true; };

        Game.OnWaitingForTurn += () => { _acceptInput = false; };

        // This is to double check that the player position on the client and player position on the
        // server match
        Game.OnEndMoveAnimation += () =>
        {
            var clientPos = new Position((int)transform.position.x, (int)transform.position.y);
            if (Game.PlayerPosition != clientPos)
            {
                Debug.LogWarning($"Client position {clientPos} and server position {Game.PlayerPosition} do not match, " +
                                 "forcing client position update the update");
                transform.position = new Vector3(Game.PlayerPosition.X, Game.PlayerPosition.Y);
            }
        };

        Game.OnUpdatePlayerPosition += () =>
        {
            if (!_isPlayingMovingAnimation)
            {
                transform.position = new Vector3(Game.PlayerPosition.X, Game.PlayerPosition.Y);

                UpdatePlayerCameraPosition();
            }
        };

        Game.OnPerformActionClient += (result) =>
        {
            switch (_lastAction.Type)
            {
                case ActionType.Move:
                    if (result)
                    {
                        Vector2Int shift = GameUtils.ActionDirectionToVector2(_lastAction.Direction);

                        _destination = new Vector3(
                            transform.position.x + shift.x, transform.position.y + shift.y, 0);

                        _direction = (_destination - transform.position).normalized;
                        Debug.Log($"set direction {_direction}");

                        _isPlayingMovingAnimation = true;
                        MoveAnimation();

                        GameUtils.UpdateRotation(_lastAction.Direction, transform);
                    }
                    break;
                case ActionType.FireGun:
                    GameUtils.UpdateRotation(_lastAction.Direction, transform);
                    PlayerAnimation.PlayShootAnimation();
                    Game.ClientActionType = ClientActionType.Move;
                    break;
            }

            _lastAction = PlayerAction.None;
        };

        Game.OnPlayerDied += () => _acceptInput = false;

        Game.OnGameViewClick += (clickWorldPosition) =>
        {
            if (!_acceptInput || _isPlayingMovingAnimation) 
                return;

            ActionDirection direction = ActionDirection.None;
            var playerPosition = transform.position;

            var shiftX = (int)math.round(clickWorldPosition.x - playerPosition.x);
            var shiftY = (int)math.round(clickWorldPosition.y - playerPosition.y);

            if ((math.abs(shiftX) == 1) ^ (math.abs(shiftY) == 1))
            {
                if (math.abs(shiftX) > math.abs(shiftY))
                    direction = shiftX > 0 ? ActionDirection.Right : ActionDirection.Left;
                else
                    direction = shiftY > 0 ? ActionDirection.Up : ActionDirection.Down;
            }

            HandleDirection(direction);
        };
    }

    void Update()
    {
        if (Game.State == GameClientState.Finished && !_isPlayingMovingAnimation)
            return;

        if (_isPlayingMovingAnimation)
        {
            var delta = math.distancesq(transform.position, _destination);
            if (delta > 0.01f)
            {
                Debug.Assert(Math.Abs(_direction.x) > Mathf.Epsilon || 
                             Math.Abs(_direction.y) > Mathf.Epsilon || 
                             Math.Abs(_direction.z) > Mathf.Epsilon);

                transform.position += _direction * Time.deltaTime * _speed;
            }
            else
            {
                _isPlayingMovingAnimation = false;
                transform.position = _destination;
                IdleAnimation();

                // tell the game we finished the move when animation finished playing
                Game.EndMoveAnimation(); 
            }

            UpdatePlayerCameraPosition();
        }
        else if (_acceptInput)
        {
            HandleKeyBoardInput();
        }
    }

    void LateUpdate()
    {
        if (_enableAcceptInput)
        {
            _enableAcceptInput = false;
            _acceptInput = true;
        }
    }

    private void MoveAnimation()
    {
        PlayerAnimation.PlayMoveAnimation();
    }
    
    private void IdleAnimation()
    {
        PlayerAnimation.PlayIdleAnimation();
    }

    public void UpdateView()
    {
        transform.position = new Vector3(Game.PlayerPosition.X, Game.PlayerPosition.Y);
    }

                // Type = _actionType == EActionType.Move ? ActionType.Move : ActionType.FireGun
    private PlayerAction _lastAction = PlayerAction.None;

    private void HandleKeyBoardInput()
    {
        ActionDirection direction = GetActionDirectionFromKeyboard(transform.position);
        HandleDirection(direction);
    }

    private void HandleDirection(ActionDirection direction)
    {
        if (direction != ActionDirection.None)
        {
            _lastAction = new PlayerAction()
            {
                Direction = direction,
                Type = Game.ClientActionType == ClientActionType.Move ? ActionType.Move : ActionType.FireGun
            };

            Game.PerformAction(_lastAction);
        }
    }

    public ActionDirection GetActionDirectionFromKeyboard(Vector3 playerPosition)
    {
        ActionDirection direction = ActionDirection.None;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            direction = ActionDirection.Left;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            direction = ActionDirection.Up;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            direction = ActionDirection.Right;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            direction = ActionDirection.Down;

        return direction;
    }

    private void UpdatePlayerCameraPosition()
    {
        _playerCameraObject.transform.position =
            new Vector3(
                transform.position.x,
                transform.position.y,
                -1);
    }
}