using System.Collections.Generic;
using TreasureHunters;
using UnityEngine;
using VContainer;

public class GenerateLevel : MonoBehaviour
{
    public GameObject _floorPrefab;
    public GameObject _wallPrefab;

    public GameObject _ceilingPrefab;
    public Transform _ceilingParent;

    public GameObject _cellLabelPrefab;
    public Transform _cellLabelsParent;

    private readonly List<List<GameObject>> _gameBoard = new();
    private readonly List<List<CeilingCell>> _ceilingBoard = new();

    private Game _game;

    [Inject]
    void InjectGame(Game game)
    {
        _game = game;
    }

    void Start()
    {
        Debug.Assert(_floorPrefab);
        Debug.Assert(_wallPrefab);

        Debug.Assert(_ceilingPrefab);
        Debug.Assert(_ceilingParent);

        Debug.Assert(_cellLabelPrefab);
        Debug.Assert(_cellLabelsParent);

        GenerateBoardSprites(_game.Board);

        var playerPosition = _game.Player.Position;
        SetMapVisibility(playerPosition, playerPosition);
    }

    private void GenerateBoardSprites(Board board)
    {
        for (var row = 0; row < Board.BoardRealHeight; ++row)
        {
            var boardRowList = new List<GameObject>();
            var ceilingRowList = new List<CeilingCell>();

            for (int col = 0; col < Board.BoardRealWidth; col++)
            {
                // empty element (not a wall, not a cell)
                if (col % 2 == 0 && row % 2 == 0)
                {
                    boardRowList.Add(null);
                    ceilingRowList.Add(null);
                }

                // creating cells
                else if (col % 2 == 1 && row % 2 == 1)
                {
                    var pos = new Vector3(
                        ((float) col - 1) / 2,
                        ((float) row - 1) / 2,
                        0);

                    // creating floor cells
                    {
                        var floorCell = Instantiate(_floorPrefab, pos, new Quaternion(), transform);
                        floorCell.name = row + " " + col + " cell";
                        boardRowList.Add(floorCell);
                    }

                    // creating ceiling
                    {
                        var ceilingCell = Instantiate(_ceilingPrefab, pos, new Quaternion(), _ceilingParent);
                        ceilingCell.name = row + " " + col + " ceiling";
                        ceilingRowList.Add(ceilingCell.GetComponent<CeilingCell>());
                    }

                    // create debug labels
                    {
                        var cellLabel = Instantiate(_cellLabelPrefab, pos, new Quaternion(), _cellLabelsParent);
                        cellLabel.name = row + " " + col + " label";
                        cellLabel.GetComponent<CellLabel>().UpdateCellLabel(new Vector2Int(row / 2, col / 2));
                    }
                }

                // creating walls
                else
                {
                    var pos = new Vector3(
                        ((float) col - 1) / 2,
                        ((float) row - 1) / 2,
                        0);

                    var rot = new Quaternion().normalized;

                    string nameSuffix = " vertical";

                    if (row % 2 == 0)
                    {
                        rot *= Quaternion.Euler(0, 0, 270);
                        pos.x -= 0.5f;
                        nameSuffix = " horizontal";
                    }
                    else
                    {
                        pos.y -= 0.5f;
                    }

                    var newObject = Instantiate(_wallPrefab, pos, rot, transform);
                    newObject.name = row + " " + col + " wall" + nameSuffix;
                    boardRowList.Add(newObject);

                    if (!board.IsWall(col, row))
                        newObject.SetActive(false);

                    ceilingRowList.Add(null);
                }
            }

            _gameBoard.Add(boardRowList);
            _ceilingBoard.Add(ceilingRowList);
        }
    }

    public void SetMapVisibility(Position position, Position previousPosition)
    {
        var board = _game.Board;
        var x = position.X;
        var y = position.Y;

        _ceilingBoard[y][x].State = CeilingState.Visible;

        // right cell
        if (x + 2 < Board.BoardRealWidth && !board.IsWall(x + 1, y))
        {
            _ceilingBoard[y][x + 2].State = CeilingState.Visible;
        }

        // lower-right cell
        if (x + 2 < Board.BoardRealWidth &&
            y - 2 >= 0 &&
            !board.IsWall(x, y - 1) &&
            !board.IsWall(x + 1, y) &&
            !board.IsWall(x + 2, y - 1) &&
            !board.IsWall(x + 1, y - 2))
        {
            _ceilingBoard[y - 2][x + 2].State = CeilingState.Visible;
        }

        // lower cell
        if (y - 2 >= 0 && !board.IsWall(x, y - 1))
        {
            _ceilingBoard[y - 2][x].State = CeilingState.Visible;
        }

        // lower-left cell
        if (x - 2 >= 0 &&
            y - 2 >= 0 &&
            !board.IsWall(x, y - 1) &&
            !board.IsWall(x - 1, y) &&
            !board.IsWall(x - 2, y - 1) &&
            !board.IsWall(x - 1, y - 2))
        {
            _ceilingBoard[y - 2][x - 2].State = CeilingState.Visible;
        }

        // left cell
        if (x - 2 >= 0 && !board.IsWall(x - 1, y))
        {
            _ceilingBoard[y][x - 2].State = CeilingState.Visible;
        }

        // upper-left cell
        if (x - 2 >= 0 &&
            y + 2 < Board.BoardRealHeight &&
            !board.IsWall(x, y + 1) &&
            !board.IsWall(x - 1, y) &&
            !board.IsWall(x - 2, y + 1) &&
            !board.IsWall(x - 1, y + 2))
        {
            _ceilingBoard[y + 2][x - 2].State = CeilingState.Visible;
        }

        // upper cell
        if (y + 2 < Board.BoardRealHeight && !board.IsWall(x, y + 1))
        {
            _ceilingBoard[y + 2][x].State = CeilingState.Visible;
        }

        // upper-right cell
        if (x + 2 < Board.BoardRealWidth &&
            y + 2 < Board.BoardRealHeight &&
            !board.IsWall(x, y + 1) &&
            !board.IsWall(x + 1, y) &&
            !board.IsWall(x + 2, y + 1) &&
            !board.IsWall(x + 1, y + 2))
        {
            _ceilingBoard[y + 2][x + 2].State = CeilingState.Visible;
        }

        // enabling for of war for cells we came from 
        UpdateForOfWar(position, previousPosition);
    }

    void UpdateForOfWar(Position position, Position previousPosition)
    {
        if (position == previousPosition)
            return;

        var x = position.X;
        var y = position.Y;

        // moving right
        if (x > previousPosition.X)
        {
            if (previousPosition.X - 2 >= 0)
            {
                _ceilingBoard[previousPosition.Y][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.Y - 2 >= 0)
                    _ceilingBoard[previousPosition.Y - 2][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.Y + 2 < Board.BoardRealHeight)
                    _ceilingBoard[previousPosition.Y + 2][previousPosition.X - 2].EnableFogIfVisible();
            }
        }

        // moving left
        if (x < previousPosition.X)
        {
            if (previousPosition.X + 2 < Board.BoardRealWidth)
            {
                _ceilingBoard[previousPosition.Y][previousPosition.X + 2].EnableFogIfVisible();

                if (previousPosition.Y - 2 >= 0)
                    _ceilingBoard[previousPosition.Y - 2][previousPosition.X + 2].EnableFogIfVisible();

                if (previousPosition.Y + 2 < Board.BoardRealHeight)
                    _ceilingBoard[previousPosition.Y + 2][previousPosition.X + 2].EnableFogIfVisible();
            }
        }

        // moving down
        if (y < previousPosition.Y)
        {
            if (previousPosition.Y + 2 < Board.BoardRealHeight)
            {
                _ceilingBoard[previousPosition.Y + 2][previousPosition.X].EnableFogIfVisible();

                if (previousPosition.X - 2 >= 0)
                    _ceilingBoard[previousPosition.Y + 2][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.X + 2 < Board.BoardRealWidth)
                    _ceilingBoard[previousPosition.Y + 2][previousPosition.X + 2].EnableFogIfVisible();
            }
        }

        // moving up
        if (y > previousPosition.Y)
        {
            if (previousPosition.Y - 2 > 0)
            {
                _ceilingBoard[previousPosition.Y - 2][previousPosition.X].EnableFogIfVisible();

                if (previousPosition.X - 2 >= 0)
                    _ceilingBoard[previousPosition.Y - 2][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.X + 2 < Board.BoardRealWidth)
                    _ceilingBoard[previousPosition.Y - 2][previousPosition.X + 2].EnableFogIfVisible();
            }
        }
    }
}