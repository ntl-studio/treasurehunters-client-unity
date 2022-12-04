using System.Collections;
using TreasureHunters;
using Unity.Mathematics;
using UnityEngine;
using VContainer;

public class PlayerMovement : MonoBehaviour
{
    public Position BoardPosition
    {
        set
        {
            _game.CurrentPlayer.Position = value;
            transform.position = new Vector3(
                (value.X - 1.0f) / 2.0f,
                (value.Y - 1.0f) / 2.0f
            );
        }
    }

    private bool _isMoving = false;
    private Vector3 _destination;
    private Vector3 _direction;

    [Inject]
    private void InjectGame(Game game)
    {
        _game = game;
    }
    private Game _game;

    private 
    
    void Start()
    {
        UpdateView();
    }

    void Update()
    {
        MovePlayer();
    }

    void FixedUpdate()
    {
        if (_isMoving)
        {
            const float MOVEMENT_SPEED = 5.0f;

            if (math.distancesq(transform.position, _destination) > 0.01f)
            {
                transform.position += _direction * Time.deltaTime * MOVEMENT_SPEED;
            }
            else
            {
                _isMoving = false;
                transform.position = _destination;


                // tell them game we finished the move when animation finished playing
                _game.EndTurn(); 
            }
        }
    }

    public void UpdateView()
    {
        BoardPosition = _game.CurrentPlayer.Position;
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        switch (_game.CurrentPlayer.MoveDirection)
        {
            case Player.EMoveDirection.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case Player.EMoveDirection.Down:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Player.EMoveDirection.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Player.EMoveDirection.Up:
                break;
        }
    }

    private void MovePlayer()
    {
        if (_isMoving)
            return;

        int shiftX = 0;
        int shiftY = 0;

        // left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            var playerPos = transform.position;
            var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (math.abs(clickPos.x - playerPos.x) < 1.5f &&
                math.abs(clickPos.y - playerPos.y) < 1.5f)
            {
                shiftX = (int) math.round(clickPos.x - playerPos.x);
                shiftY = (int) math.round(clickPos.y - playerPos.y);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            shiftX = -1;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            shiftY = 1;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            shiftX = 1;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            shiftY = -1;

        var board = _game.CurrentBoard;
        var pos = _game.CurrentPlayer.Position;

        if (shiftY != 0 || shiftX != 0)
        {
            if (((0 == shiftX && 1 == math.abs(shiftY)) ||
                 (1 == math.abs(shiftX) && 0 == shiftY)) &&
                !board.IsWall(pos.X + shiftX, pos.Y + shiftY))
            {
                _destination = new Vector3(
                    transform.position.x + shiftX,
                    transform.position.y + shiftY,
                    0);

                _direction = (_destination - transform.position).normalized;
                _isMoving = true;

                pos.X += 2 * shiftX;
                pos.Y += 2 * shiftY;

                _game.CurrentPlayer.Position = pos;

                UpdateRotation();
            }
        }
    }
}