using System.Collections.Generic;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;
using Debug = UnityEngine.Debug;
using SM = NtlStudio.TreasureHunters.Model;

class PlayerBoardView
{
    public FieldCell[,] Board = new FieldCell[SM.GameField.FieldWidth, SM.GameField.FieldHeight];
    public bool[,] Visited = new bool[SM.GameField.FieldWidth, SM.GameField.FieldHeight];

    public PlayerBoardView()
    {
        for (int x = 0; x < SM.GameField.FieldWidth; ++x)
        {
            for (int y = 0; y < SM.GameField.FieldHeight; ++y)
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

    private List<PlayerBoardView> _playerBoards = new();

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

        for (int i = 0; i < _game.PlayersCount; ++i)
        {
            _playerBoards.Add(new PlayerBoardView());
        }

        GenerateBoardSprites();

        _game.OnStartTurn += () =>
        {
            UpdateBoardAfterPreviousTurn();
            UpdatePlayerVisibility();
        };
        _game.OnEndTurn += () =>
        {
            FogVisitedAreas();
            UpdatePlayerVisibility();
        };

        _game.OnShowTreasureEvent += (bool isVisible) =>
        {
            _treasure.SetActive(isVisible);
            var pos = _game.TreasurePosition();
            _treasure.transform.position = new Vector3(pos.X, pos.Y);
        };

        UpdatePlayerVisibility();
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
                var pos = new Vector3(col, row);

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

            _floorCells.Add(floorRowList);
            _wallCells.Add(wallsRowList);
            _ceilingCells.Add(ceilingRowList);
        }
    }

    // Hides all cells visible to the previous player
    // Enables all cells visible to the current player, including areas visited before
    // (covered in fog of war)
    public void UpdateBoardAfterPreviousTurn()
    {
        int playerId = _game.CurrentPlayerId;

        for (int x = 0; x < SM.GameField.FieldWidth; ++x)
        {
            for (int y = 0; y < SM.GameField.FieldHeight; ++y)
            {
                if (_playerBoards[playerId].Visited[x, y])
                {
                    FieldCell cell = _playerBoards[playerId].Board[x, y];
                    _wallCells[y][x].SetWallsVisibility(cell);
                    _ceilingCells[y][x].State = CeilingState.Fog;
                }
                else
                {
                    _ceilingCells[y][x].State = CeilingState.Hidden;
                }
            }
        }
    }

    public void FogVisitedAreas()
    {
        var position = _game.CurrentPlayerPreviousPosition();

        for (int x = 0; x < SM.VisibleArea.Width; ++x)
        {
            for (int y = 0; y < SM.VisibleArea.Height; ++y)
            {
                var fieldX = x + position.X - 1;
                var fieldY = y + position.Y - 1;

                if (fieldX is >= 0 and < SM.GameField.FieldWidth &&
                    fieldY is >= 0 and < SM.GameField.FieldHeight)
                {
                    if (_playerBoards[_game.CurrentPlayerId].Visited[fieldX, fieldY])
                        _ceilingCells[fieldY][fieldX].State = CeilingState.Fog;
                }
            }
        }
    }

    public void UpdatePlayerVisibility()
    {
        var position = _game.CurrentPlayer.Position;
        var visibleArea = _game.CurrentVisibleArea();

        if (!_game.IsTreasureAlwaysVisible)
            _treasure.SetActive(false);

        for (int x = 0; x < SM.VisibleArea.Width; ++x)
        {
            for (int y = 0; y < SM.VisibleArea.Height; ++y)
            {
                var fieldX = x + position.X - 1;
                var fieldY = y + position.Y - 1;

                if (fieldX is < 0 or >= SM.GameField.FieldWidth ||
                    fieldY is < 0 or >= SM.GameField.FieldHeight)
                {
                    continue;
                }

                var cell = visibleArea[x, y];
                if (cell.HasFlag(FieldCell.Invisible))
                    continue;

                if (cell.HasFlag(FieldCell.Treasure))
                {
                    _treasure.transform.position = new Vector3(fieldX, fieldY, 0);
                    _treasure.SetActive(true);
                }    

                _wallCells[fieldY][fieldX].SetWallsVisibility(cell);
                _ceilingCells[fieldY][fieldX].State = CeilingState.Visible;

                _playerBoards[_game.CurrentPlayerId].Board[fieldX, fieldY] = cell;
                _playerBoards[_game.CurrentPlayerId].Visited[fieldX, fieldY] = true;
            }
        }
    }
}