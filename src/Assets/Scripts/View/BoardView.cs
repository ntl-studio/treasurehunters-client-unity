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

    private static Game _game => Game.Instance();

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

        _game.OnStartTurn += () => UpdateBoard();
        _game.OnEndTurn += () =>
        {
            UpdateBoard(true);
            UpdateBoard(false);
        };

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

                    wallView.SetLeftWallVisible(false);
                    wallView.SetBottomWallVisible(false);
                    wallView.SetRightWallVisible(false);
                    wallView.SetTopWallVisible(false);

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

    public void UpdateBoard(bool addFog = false)
    {
        var position = addFog ? _game.CurrentPlayerPreviousPosition() : _game.CurrentPlayer.Position;

        for (int row = 0; row < SM.VisibleArea.Width; ++row)
        {
            for (int col = 0; col < SM.VisibleArea.Height; ++col)
            {
                var fieldX = col + position.X - 1;
                var fieldY = row + position.Y - 1;

                if (fieldX is < 0 or >= SM.GameField.FieldWidth ||
                    fieldY is < 0 or >= SM.GameField.FieldHeight)
                {
                    continue;
                }

                if (!addFog)
                {
                    _wallCells[fieldY][fieldX].SetLeftWallVisible(false);
                    _wallCells[fieldY][fieldX].SetBottomWallVisible(false);
                    _wallCells[fieldY][fieldX].SetRightWallVisible(false);
                    _wallCells[fieldY][fieldX].SetTopWallVisible(false);

                    FieldCell cell = _game.CurrentBoard[fieldX, fieldY];

                    _wallCells[fieldY][fieldX].SetBottomWallVisible(cell.HasFlag(FieldCell.BottomWall));
                    _wallCells[fieldY][fieldX].SetLeftWallVisible(cell.HasFlag(FieldCell.LeftWall));

                    if (fieldY == SM.VisibleArea.Height || fieldY == Game.FieldHeight - 1)
                    {
                        _wallCells[fieldY][fieldX].SetTopWallVisible(cell.HasFlag(FieldCell.TopWall));
                    }

                    if (fieldX == SM.VisibleArea.Width || fieldX == Game.FieldWidth - 1)
                    {
                        _wallCells[fieldY][fieldX].SetRightWallVisible(cell.HasFlag(FieldCell.RightWall));
                    }

                    _ceilingCells[fieldY][fieldX].State = CeilingState.Visible;
                }
                else
                {
                    _ceilingCells[fieldY][fieldX].State = CeilingState.Fog;
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