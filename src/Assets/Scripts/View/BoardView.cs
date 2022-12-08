using System;
using System.Collections.Generic;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;
using VContainer;
using Debug = UnityEngine.Debug;

using Position = TreasureHunters.Position;
using SM = NtlStudio.TreasureHunters.Model;

public class BoardView : MonoBehaviour
{
    public GameObject _floorPrefab;
    public Transform _floorParent;

    public GameObject _wallPrefab;
    public Transform _wallsParent;

    public GameObject _ceilingPrefab;
    public Transform _ceilingParent;

    public GameObject _cellLabelPrefab;
    public Transform _cellLabelsParent;

    private readonly List<List<GameObject>> _floorCells = new();
    private readonly List<List<WallView>> _wallCells = new();
    private readonly List<List<CeilingCell>> _ceilingCells = new();

    [Inject] void InjectGame(Game game) { _game = game; }
    private Game _game;

    void Start()
    {
        Debug.Assert(_floorPrefab);
        Debug.Assert(_floorParent);

        Debug.Assert(_wallPrefab);
        Debug.Assert(_wallsParent);

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
        for (var row = 0; row < Game.FieldHeight; ++row)
        {
            var floorRowList = new List<GameObject>();
            var wallsRowList = new List<WallView>();
            var ceilingRowList = new List<CeilingCell>();

            for (int col = 0; col < Game.FieldWidth; col++)
            {
                var pos = new Vector3(col, row, 0);

                // creating floor cells
                {
                    var floorCell = Instantiate(_floorPrefab, pos, new Quaternion(), _floorParent);
                    floorCell.name = row + " " + col + " floor";
                    floorRowList.Add(floorCell);
                }

                // creating walls
                {
                    var wallCell = Instantiate(_wallPrefab, pos, new Quaternion(), _wallsParent);
                    wallCell.name = row + " " + col + " wall";

                    var wallView = wallCell.GetComponent<WallView>();
                    Debug.Assert(wallView);

                    wallsRowList.Add(wallView);
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
                    cellLabel.GetComponent<CellLabel>().UpdateCellLabel(new Vector2Int(row, col));
                }
            }

            _floorCells.Add(floorRowList);
            _wallCells.Add(wallsRowList);
            _ceilingCells.Add(ceilingRowList);
        }
    }

    // Go through the Board (that is associated with the player session) and update sprites visibility:
    // 1) Walls on/off
    // 2) Fog of war
    // 3) Objects like grenades, treasure, etc
    public void UpdateBoard()
    {
        var board = _game.CurrentBoard;

        for (int row = 0; row < Game.FieldHeight; ++row)
        {
            for (int col = 0; col < Game.FieldWidth; ++col)
            {
                // TODO: fix the distance function 
                _ceilingCells[row][col].State = CeilingState.Visible;

                SM.FieldCell cell = _game.CurrentBoard[col, row];

                _wallCells[row][col].SetLeftWallVisible(false);
                _wallCells[row][col].SetBottomWallVisible(false);
                _wallCells[row][col].SetRightWallVisible(false);
                _wallCells[row][col].SetTopWallVisible(false);

                _wallCells[row][col].SetBottomWallVisible(cell.HasFlag(FieldCell.TopWall));
                _wallCells[row][col].SetLeftWallVisible(cell.HasFlag(FieldCell.LeftWall));

                if (row == Game.FieldHeight - 1)
                {
                    _wallCells[row][col].SetTopWallVisible(cell.HasFlag(FieldCell.BottomWall));
                }
                
                if (col == Game.FieldWidth - 1)
                {
                    _wallCells[row][col].SetRightWallVisible(cell.HasFlag(FieldCell.RightWall));
                }
            }
        }
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
                _ceilingCells[previousPosition.Y][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.Y - 2 >= 0)
                    _ceilingCells[previousPosition.Y - 2][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.Y + 2 < GameSettings.BoardRealHeight)
                    _ceilingCells[previousPosition.Y + 2][previousPosition.X - 2].EnableFogIfVisible();
            }
        }

        // moving left
        if (x < previousPosition.X)
        {
            if (previousPosition.X + 2 < GameSettings.BoardRealWidth)
            {
                _ceilingCells[previousPosition.Y][previousPosition.X + 2].EnableFogIfVisible();

                if (previousPosition.Y - 2 >= 0)
                    _ceilingCells[previousPosition.Y - 2][previousPosition.X + 2].EnableFogIfVisible();

                if (previousPosition.Y + 2 < GameSettings.BoardRealHeight)
                    _ceilingCells[previousPosition.Y + 2][previousPosition.X + 2].EnableFogIfVisible();
            }
        }

        // moving down
        if (y < previousPosition.Y)
        {
            if (previousPosition.Y + 2 < GameSettings.BoardRealHeight)
            {
                _ceilingCells[previousPosition.Y + 2][previousPosition.X].EnableFogIfVisible();

                if (previousPosition.X - 2 >= 0)
                    _ceilingCells[previousPosition.Y + 2][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.X + 2 < GameSettings.BoardRealWidth)
                    _ceilingCells[previousPosition.Y + 2][previousPosition.X + 2].EnableFogIfVisible();
            }
        }

        // moving up
        if (y > previousPosition.Y)
        {
            if (previousPosition.Y - 2 > 0)
            {
                _ceilingCells[previousPosition.Y - 2][previousPosition.X].EnableFogIfVisible();

                if (previousPosition.X - 2 >= 0)
                    _ceilingCells[previousPosition.Y - 2][previousPosition.X - 2].EnableFogIfVisible();

                if (previousPosition.X + 2 < GameSettings.BoardRealWidth)
                    _ceilingCells[previousPosition.Y - 2][previousPosition.X + 2].EnableFogIfVisible();
            }
        }
    }
}