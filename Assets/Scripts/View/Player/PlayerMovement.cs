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
            _boardPosition = value;
            transform.position = new Vector3(
                (_boardPosition.X - 1.0f) / 2.0f,
                (_boardPosition.Y - 1.0f) / 2.0f
            );
        }
    }

    private Position _boardPosition;

    private bool _isMoving = false;
    private Vector3 _destination;
    private Vector3 _direction;

    private BoardView _boardView;
    [Inject]
    public void Construct(BoardView boardView)
    {
        _boardView = boardView;
    }

    private Game _game;
    [Inject]
    private void InjectGame(Game game)
    {
        _game = game;
    }

    void Start()
    {
        UpdatePosition();
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

    public void UpdatePosition()
    {
        BoardPosition = _game.CurrentPlayer.Position;
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

        if (shiftY != 0 || shiftX != 0)
        {
            if (((0 == shiftX && 1 == math.abs(shiftY)) ||
                 (1 == math.abs(shiftX) && 0 == shiftY)) &&
                !board.IsWall(_boardPosition.X + shiftX, _boardPosition.Y + shiftY))
            {
                _destination = new Vector3(
                    transform.position.x + shiftX,
                    transform.position.y + shiftY,
                    0);

                _direction = (_destination - transform.position).normalized;
                _isMoving = true;

                var prevPosition = _boardPosition;

                _boardPosition.X += 2 * shiftX;
                _boardPosition.Y += 2 * shiftY;

                _game.CurrentPlayer.Position = _boardPosition;

                _boardView.UpdateBoard(_game.CurrentBoard);
            }
        }
    }
}