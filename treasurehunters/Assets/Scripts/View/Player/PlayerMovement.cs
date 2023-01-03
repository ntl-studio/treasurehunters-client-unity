using System;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool AcceptInput = false;

    private bool _isPlayingMovingAnimation;
    private Vector3 _destination;
    private Vector3 _direction;

    private static GameClient Game => GameClient.Instance();
    
    void Start()
    {
        Game.OnYourTurn += () => { AcceptInput = true; };

        Game.OnUpdatePlayerPosition += () =>
        {
            transform.position = new Vector3(Game.PlayerPosition.X, Game.PlayerPosition.Y);
        };

        Game.OnMakingMove += (actionResult) =>
        {
            if (actionResult)
            {
                Vector2Int shift = GameUtils.ActionToVector2(_lastAction);

                _destination = new Vector3(transform.position.x + shift.x, transform.position.y + shift.y, 0);

                _direction = (_destination - transform.position).normalized;
                Debug.Log($"set direction {_direction}");

                _isPlayingMovingAnimation = true;
                // GameUtils.UpdateRotation(Game.CurrentPlayerMoveStates[0].Direction, transform);

                AcceptInput = false;
            }
            else
            {
                Debug.Log("Could not move player");
            }

            _lastAction = PlayerAction.None;
        };
    }

    void Update()
    {
        if (AcceptInput)
            MovePlayer();
    }

    void FixedUpdate()
    {
        if (_isPlayingMovingAnimation)
        {
            const float movementSpeed = 5.0f;

            if (math.distancesq(transform.position, _destination) > 0.01f)
            {
                Debug.Assert(Math.Abs(_direction.x) > Mathf.Epsilon || 
                             Math.Abs(_direction.y) > Mathf.Epsilon || 
                             Math.Abs(_direction.z) > Mathf.Epsilon);

                transform.position += _direction * Time.deltaTime * movementSpeed;
            }
            else
            {
                _isPlayingMovingAnimation = false;
                transform.position = _destination;

                // tell them game we finished the move when animation finished playing
                Game.EndMove(); 
            }
        }
    }

    public void UpdateView()
    {
        transform.position = new Vector3(Game.PlayerPosition.X, Game.PlayerPosition.Y);
    }

    private PlayerAction _lastAction = PlayerAction.None;

    private void MovePlayer()
    {
        if (_isPlayingMovingAnimation)
            return;

        ActionDirection direction = GameUtils.GetActionDirection(transform.position);

        if (direction != ActionDirection.None)
        {
            _lastAction = new PlayerAction()
            {
                Direction = direction,
                Type = ActionType.Move
            };

            Game.PerformAction(_lastAction);
        }
    }
}