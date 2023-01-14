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

    private static GameClient Game => GameClient.Instance();

    private EActionType _actionType;

    private enum EActionType
    {
        Move, // Default mode
        Gun
    }
    
    void Start()
    {
        Game.OnYourTurn += () =>
        {
            _actionType = EActionType.Move;
            _acceptInput = true;
        };

        Game.OnWaitingForTurn += () =>
        {
            _acceptInput = false;
        };

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
                transform.position = new Vector3(Game.PlayerPosition.X, Game.PlayerPosition.Y);
        };

        Game.OnPerformActionServer += (_) =>
        {
            _actionType = EActionType.Move;
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
                    break;
            }

            _lastAction = PlayerAction.None;
        };

        Game.OnStartFiringGun += () =>
        {
            _actionType = EActionType.Gun;
            _enableAcceptInput = true;
        };

        Game.OnChoosePlayerAction += () => _acceptInput = false;
        Game.OnChoosePlayerActionCancel += () => _enableAcceptInput = true;

        Game.OnPlayerDied += () => _acceptInput = false;
    }

    void Update()
    {
        if (Game.State == GameClientState.Finished && !_isPlayingMovingAnimation)
            return;

        if (_acceptInput)
            HandleInput();

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

    private PlayerAction _lastAction = PlayerAction.None;

    private void HandleInput()
    {
        if (_isPlayingMovingAnimation)
            return;

        ActionDirection direction = GameUtils.GetActionDirection(transform.position);

        if (direction != ActionDirection.None)
        {
            _lastAction = new PlayerAction()
            {
                Direction = direction,
                Type = _actionType == EActionType.Move ? ActionType.Move : ActionType.FireGun
            };

            Game.PerformAction(_lastAction); 
        }
    }
}