using System;
using System.Collections.Generic;
using TreasureHunters;
using UnityEngine;
using VContainer;
using Debug = UnityEngine.Debug;

public class BoardView : MonoBehaviour
{
    public GameObject _floorPrefab;
    public GameObject _wallPrefab;

    public GameObject _ceilingPrefab;
    public Transform _ceilingParent;

    public GameObject _cellLabelPrefab;
    public Transform _cellLabelsParent;

    private readonly List<List<GameObject>> _gameBoard = new();
    private readonly List<List<CeilingCell>> _ceilingBoard = new();

    [Inject] void InjectGame(Game game) { _game = game; }
    private Game _game;

    void OnEnable()
    {
        _game.Init();
    }

    void Start()
    {
        Debug.Assert(_floorPrefab);
        Debug.Assert(_wallPrefab);

        Debug.Assert(_ceilingPrefab);
        Debug.Assert(_ceilingParent);

        Debug.Assert(_cellLabelPrefab);
        Debug.Assert(_cellLabelsParent);

        GenerateBoardSprites();

        _game.OnStartTurn += UpdateBoard;
        _game.OnEndTurn += UpdateBoard;

        UpdateBoard();
    }

    // Initializes all board sprites (floor tiles, walls, etc). By default the board will look like 
    // all walls are enabled.
    private void GenerateBoardSprites()
    {
        for (var row = 0; row < GameSettings.BoardRealHeight; ++row)
        {
            var boardRowList = new List<GameObject>();
            var ceilingRowList = new List<CeilingCell>();

            for (int col = 0; col < GameSettings.BoardRealWidth; col++)
            {
                //empty element (not a wall, not a cell)
                if (col % 2 == 0 && row % 2 == 0)
                {
                    boardRowList.Add(null);
                    ceilingRowList.Add(null);
                }
                // creating cells
                else if (col % 2 == 1 && row % 2 == 1)
                //else if (Board.IsWallCell(row, col))
                {
                    var pos = new Vector3(
                        ((float)col - 1) / 2,
                        ((float)row - 1) / 2,
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
                else if (Board.IsWallCell(row, col))
                {
                    var pos = new Vector3(
                        ((float)col - 1) / 2,
                        ((float)row - 1) / 2,
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

                    ceilingRowList.Add(null);
                }
                else
                {
                    throw new Exception($"Invalid board col ${col} and row ${row} values");
                }
            }

            _gameBoard.Add(boardRowList);
            _ceilingBoard.Add(ceilingRowList);
        }
    }

    // Go through the Board (that is associated with the player session) and update sprites visibility:
    // 1) Walls on/off
    // 2) Fog of war
    // 3) Objects like grenades, treasure, etc
    public void UpdateBoard()
    {
        var board = _game.CurrentBoard;

        for (int row = 0; row < GameSettings.BoardRealWidth; ++row)
        {
            for (int col = 0; col < GameSettings.BoardRealWidth; ++col)
            {
                if (!Board.IsValidCell(col, row))
                    continue;

                if (Board.IsFloorCell(row, col))
                {
                    _ceilingBoard[col][row].State = 
                        board.IsCellVisible(row, col) ? 
                            CeilingState.Visible : CeilingState.Hidden;
                }

                if (Board.IsWallCell(row, col))
                    _gameBoard[col][row].SetActive(board.IsWall(row, col));
            }
        }
    }

    // This is the code to "open" the map when player is moving, respecting the walls when
    // checking for visibility. Leaving it here in case we might need to use it again in the client.
    // Eventually this information should come from the server, and hopefully it will one day.
    public void SetMapVisibility(Position position, Position previousPosition)
    {
        var board = _game.CurrentBoard;
        var x = position.X;
        var y = position.Y;

        _ceilingBoard[y][x].State = CeilingState.Visible;

        // right cell
        if (x + 2 < GameSettings.BoardRealWidth && !board.IsWall(x + 1, y))
        {
            _ceilingBoard[y][x + 2].State = CeilingState.Visible;
        }

        // lower-right cell
        if (x + 2 < GameSettings.BoardRealWidth &&
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
            y + 2 < GameSettings.BoardRealHeight &&
            !board.IsWall(x, y + 1) &&
            !board.IsWall(x - 1, y) &&
            !board.IsWall(x - 2, y + 1) &&
            !board.IsWall(x - 1, y + 2))
        {
            _ceilingBoard[y + 2][x - 2].State = CeilingState.Visible;
        }

        // upper cell
        if (y + 2 < GameSettings.BoardRealHeight && !board.IsWall(x, y + 1))
        {
            _ceilingBoard[y + 2][x].State = CeilingState.Visible;
        }

        // upper-right cell
        if (x + 2 < GameSettings.BoardRealWidth &&
            y + 2 < GameSettings.BoardRealHeight &&
            !board.IsWall(x, y + 1) &&
            !board.IsWall(x + 1, y) &&
            !board.IsWall(x + 2, y + 1) &&
            !board.IsWall(x + 1, y + 2))
        {
            _ceilingBoard[y + 2][x + 2].State = CeilingState.Visible;
        }

        // enabling for of war for cells we came from 
        UpdateFogOfWar(position, previousPosition);
    }

    void UpdateFogOfWar(Position position, Position previousPosition)
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

                if (previousPosition.Y + 2 < GameSettings.BoardRealHeight)
                    _ceilingBoard[previousPosition.Y + 2][previousPosition.X - 2].EnableFogIfVisible();
            }
        }

        // moving left
        if (x < previousPosition.X)
        {
            if (previousPosition.X + 2 < GameSettings.BoardRealWidth)
            {
                _ceilingBoard[previousPosition.Y][previousPosition.X + 2].EnableFogIfVisible();

                if (previousPosition.Y - 2 >= 0)
                    _ceilingBoard[previousPosition.Y - 2][previousPosition.X + 2].EnableFogIfVisible();

                if (previousPosition.Y + 2 < GameSettings.BoardRealHeight)
                    _ceilingBoard[previousPosition.Y + 2][previousPosition.X + 2].EnableFogIfVisible();
            }
        }

        // moving down
        if (y < previousPosition.Y)
        {
            if (previousPosition.Y + 2 < GameSettings.BoardRealHeight)
            {
                _ceilingBoard[previousPosition.Y + 2][previousPosition.X].EnableFogIfVisible();

                if (previousPosition.X - 2 >= 0)
                    _ceilingBoard[previousPosition.Y + 2][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.X + 2 < GameSettings.BoardRealWidth)
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

                if (previousPosition.X + 2 < GameSettings.BoardRealWidth)
                    _ceilingBoard[previousPosition.Y - 2][previousPosition.X + 2].EnableFogIfVisible();
            }
        }
    }
}