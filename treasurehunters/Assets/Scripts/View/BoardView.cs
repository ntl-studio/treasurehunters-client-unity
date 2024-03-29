using System.Collections.Generic;
using NtlStudio.TreasureHunters.Common;
using TreasureHunters;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Position = TreasureHunters.Position;

class PlayerBoardView
{
    public FieldCell[,] Board = new FieldCell[GameSettings.FieldWidth, GameSettings.FieldHeight];
    public bool[,] Visited = new bool[GameSettings.FieldWidth, GameSettings.FieldHeight];

    public PlayerBoardView()
    {
        for (int x = 0; x < GameSettings.FieldWidth; ++x)
        {
            for (int y = 0; y < GameSettings.FieldHeight; ++y)
            {
                Board[x, y] = FieldCell.Empty;
                Visited[x, y] = false;
            }
        }
    }
}

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

    public GameObject _treasure;

    public Enemies _enemies;

    private readonly PlayerBoardView _playerBoard = new();

    private readonly List<List<WallView>> _wallCells = new();
    private readonly List<List<CeilingCell>> _ceilingCells = new();

    private static GameClient Game => GameClient.Instance();

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

        Debug.Assert(_enemies);

        GenerateBoardSprites();

        Game.OnUpdateVisibleArea += () =>
        {
            FogVisitedAreas(Game.PreviousPosition);
            UpdatePlayerVisibility();
        };

        Game.OnUpdateTreasurePosition_Debug += () =>
        {
            UpdateTreasurePosition(Game.TreasurePosition_Debug);
        };

        Game.OnShowTreasureEvent += isVisible =>
        {
            _treasure.SetActive(isVisible);
        };
    }

    // Initializes all board sprites (floor tiles, walls, etc). By default the board will look like 
    // all walls are enabled.
    private void GenerateBoardSprites()
    {
        for (var row = 0; row < GameClient.FieldHeight; ++row)
        {
            var wallsRowList = new List<WallView>();
            var ceilingRowList = new List<CeilingCell>();

            for (int col = 0; col < GameClient.FieldWidth; col++)
            {
                var pos = new Vector3(col, row);

                // creating floor cells
                {
                    var floorCell = Instantiate(_floorPrefab, pos, new Quaternion(), _floorParent);
                    floorCell.name = row + " " + col + " floor";
                }

                // creating walls
                {
                    var wallCell = Instantiate(_wallPrefab, pos, new Quaternion(), _wallsParent);
                    wallCell.name = row + " " + col + " wall";

                    var wallView = wallCell.GetComponent<WallView>();
                    Debug.Assert(wallView);

                    wallView.SetWallsVisibility(FieldCell.Empty);

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

            _wallCells.Add(wallsRowList);
            _ceilingCells.Add(ceilingRowList);
        }
    }

    public void FogVisitedAreas(Position position)
    {
        for (int x = 0; x < GameSettings.VisibleAreaWidth; ++x)
        {
            for (int y = 0; y < GameSettings.VisibleAreaHeight; ++y)
            {
                var fieldX = x + position.X - 1;
                var fieldY = y + position.Y - 1;

                if (fieldX is >= 0 and < GameSettings.FieldWidth &&
                    fieldY is >= 0 and < GameSettings.FieldHeight)
                {
                    if (_playerBoard.Visited[fieldX, fieldY])
                        _ceilingCells[fieldY][fieldX].State = CeilingState.Fog;
                }
            }
        }
    }

    public void UpdatePlayerVisibility()
    {
        _enemies.ClearEnemies();

        var position = Game.PlayerPosition;
        var visibleArea = Game.CurrentVisibleArea();

        if (!Game.IsTreasureAlwaysVisible)
            _treasure.SetActive(false);

        for (int x = 0; x < GameSettings.VisibleAreaWidth; ++x)
        {
            for (int y = 0; y < GameSettings.VisibleAreaHeight; ++y)
            {
                var fieldX = x + position.X - 1;
                var fieldY = y + position.Y - 1;

                if (fieldX is < 0 or >= GameSettings.FieldWidth ||
                    fieldY is < 0 or >= GameSettings.FieldHeight)
                {
                    continue;
                }

                var cell = visibleArea[x, y];
                if (cell.HasFlag(FieldCell.Invisible))
                    continue;

                if (cell.HasTreasure())
                {
                    UpdateTreasurePosition(fieldX, fieldY);
                    _treasure.SetActive(true);
                }

                if (cell.HasEnemy())
                {
                    _enemies.ShowEnemy(fieldX, fieldY);
                }

                _wallCells[fieldY][fieldX].SetWallsVisibility(cell);
                _ceilingCells[fieldY][fieldX].State = CeilingState.Visible;

                _playerBoard.Board[fieldX, fieldY] = cell;
                _playerBoard.Visited[fieldX, fieldY] = true;
            }
        }
    }

    void UpdateTreasurePosition(Position pos)
    {
        UpdateTreasurePosition(pos.X, pos.Y);
    }

    void UpdateTreasurePosition(int x, int y)
    {
        _treasure.transform.position = new Vector3(x, y);
    }
}