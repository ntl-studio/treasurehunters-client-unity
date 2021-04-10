using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector2Int BoardPosition
    {
        set
        {
            _boardPosition = value;
            transform.position = new Vector3(
                (_boardPosition.x - 1.0f) / 2.0f,
                (_boardPosition.y - 1.0f) / 2.0f
            );
        }
    }

    private Vector2Int _boardPosition;

    private bool _isMoving = false;
    private Vector3 _destination;
    private Vector3 _direction;
    private List<string[]> _board;

    void Start()
    {
        var board = GameObject.Find("Board");
        Debug.Assert(board);

        var levelGenerator = board.GetComponent<GenerateLevel>();
        Debug.Assert(levelGenerator);

        _board = levelGenerator._board;

        BoardPosition = new Vector2Int(
            2 * (int) math.round(transform.position.x) + 1,
            2 * (int) math.round(transform.position.y) + 1
        );
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
            }
        }
    }

    private void MovePlayer()
    {
        if (_isMoving)
            return;

        int shiftX = 0;
        int shiftY = 0;

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

        if (shiftY != 0 || shiftX != 0)
        {
            if (((0 == shiftX && 1 == math.abs(shiftY)) ||
                 (1 == math.abs(shiftX) && 0 == shiftY)) &&
                (_board[_boardPosition.y + shiftY][_boardPosition.x + shiftX] != "w"))
            {
                _destination = new Vector3(
                    transform.position.x + shiftX,
                    transform.position.y + shiftY,
                    0);

                _direction = (_destination - transform.position).normalized;
                _isMoving = true;

                _boardPosition.x += 2 * shiftX;
                _boardPosition.y += 2 * shiftY;

                UpdateMapVisibility(_boardPosition);
            }
        }
    }

    private void UpdateMapVisibility(Vector2Int boardPosition)
    {
        var board = GameObject.Find("Board");
        Debug.Assert(board);

        var levelGenerator = board.GetComponent<GenerateLevel>();
        Debug.Assert(levelGenerator);

        levelGenerator.SetMapVisibility(boardPosition, true);
    }
}