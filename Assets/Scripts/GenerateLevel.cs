using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public GameObject _floorPrefab;
    public GameObject _wallPrefab;

    public GameObject _ceilingPrefab;
    public Transform _ceilingParent;

    public GameObject _cellLabelPrefab;
    public Transform _cellLabelsParent;

    public List<string[]> _board = new List<string[]>();

    public TextAsset _levelFileName;

    private List<List<GameObject>> _gameBoard = new List<List<GameObject>>();
    private List<List<GameObject>> _ceilingBoard = new List<List<GameObject>>();

    private const int BOARD_WIDTH = 10;
    private const int BOARD_HEIGHT = 10;
    private const int boardRealWidth = BOARD_WIDTH * 2 + 1;
    private const int boardRealHeight = BOARD_HEIGHT * 2 + 1;

    void Start()
    {
        Debug.Assert(_floorPrefab);
        Debug.Assert(_wallPrefab);
        Debug.Assert(_levelFileName);

        Debug.Assert(_ceilingPrefab);
        Debug.Assert(_ceilingParent);

        Debug.Assert(_cellLabelPrefab);
        Debug.Assert(_cellLabelsParent);

        var levelLines = ReadLevel();

        GenerateBoard(levelLines);

        var playerPosition = FindPlayerPosition(_board);
        UpdatePlayerPosition(playerPosition);

        _ceilingBoard[playerPosition.y][playerPosition.x].SetActive(false);
    }

    List<string> ReadLevel()
    {
        var levelFile = _levelFileName.text;

        var rawLines = new List<string>();
        rawLines.AddRange(levelFile.Split("\n"[0]));

        var cleanLines = new List<string>();

        foreach (var line in rawLines)
        {
            var commentPos = line.IndexOf("//", StringComparison.Ordinal);

            string newLine = line;
            if (commentPos >= 0)
                newLine = newLine.Substring(0, commentPos);
            newLine = newLine.Trim();

            if (newLine.Length > 0)
                cleanLines.Add(newLine);
        }

        cleanLines.Reverse();

        return cleanLines;
    }

    private void GenerateBoard(List<string> lines)
    {
        foreach (var line in lines)
        {
            var characters = line.Split(' ');
            _board.Add(characters);
        }

        Debug.Assert(_board.Count == boardRealHeight);
        Debug.Assert(_board[0].Length == boardRealWidth);

        for (var row = 0; row < boardRealHeight; ++row)
        {
            List<GameObject> boardRowList = new List<GameObject>();
            List<GameObject> ceilingRowList = new List<GameObject>();

            for (int col = 0; col < boardRealWidth; col++)
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
                        ceilingRowList.Add(ceilingCell);
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

                    if (_board[row][col] != "w")
                        newObject.SetActive(false);

                    ceilingRowList.Add(null);
                }
            }

            _gameBoard.Add(boardRowList);
            _ceilingBoard.Add(ceilingRowList);
        }
    }

    Vector2Int FindPlayerPosition(List<string[]> board)
    {
        for (var row = 0; row < board.Count; ++row)
        {
            for (var col = 0; col < board[row].Length; ++col)
            {
                if (board[row][col] == "P")
                {
                    return new Vector2Int(col, row);
                }
            }
        }

        Debug.Assert(false, "Did not find player position on the board");
        return new Vector2Int(-1, -1);
    }

    void UpdatePlayerPosition(Vector2Int position)
    {
        var player = GameObject.Find("Player");
        Debug.Assert(player);

        var playerMovement = player.GetComponent<PlayerMovement>();
        Debug.Assert(playerMovement);

        playerMovement.BoardPosition = position;

        SetMapVisibility(position, true);
    }

    public void SetMapVisibility(Vector2Int position, bool isVisible)
    {
        // right cell
        if (position.x + 2 < boardRealWidth && !isWall(_board[position.y][position.x + 1]))
        {
            _ceilingBoard[position.y][position.x + 2].SetActive(false);
        }

        // lower-right cell
        if (position.x + 2 < boardRealWidth &&
            position.y - 2 >= 0 &&
            !isWall(_board[position.y - 1][position.x]) &&
            !isWall(_board[position.y][position.x + 1]) &&
            !isWall(_board[position.y - 1][position.x + 2]) &&
            !isWall(_board[position.y - 2][position.x + 1]))
        {
            _ceilingBoard[position.y - 2][position.x + 2].SetActive(false);
        }

        // lower cell
        if (position.y - 2 >= 0 && !isWall(_board[position.y - 1][position.x]))
        {
            _ceilingBoard[position.y - 2][position.x].SetActive(false);
        }

        // lower-left cell
        if (position.x - 2 >= 0 &&
            position.y - 2 >= 0 &&
            !isWall(_board[position.y - 1][position.x]) &&
            !isWall(_board[position.y][position.x - 1]) &&
            !isWall(_board[position.y - 1][position.x - 2]) &&
            !isWall(_board[position.y - 2][position.x - 1]))
        {
            _ceilingBoard[position.y - 2][position.x - 2].SetActive(false);
        }

        // left cell
        if (position.x - 2 >= 0 && !isWall(_board[position.y][position.x - 1]))
        {
            _ceilingBoard[position.y][position.x - 2].SetActive(false);
        }

        // upper-left cell
        if (position.x - 2 >= 0 &&
            position.y + 2 < boardRealHeight &&
            !isWall(_board[position.y + 1][position.x]) &&
            !isWall(_board[position.y][position.x - 1]) &&
            !isWall(_board[position.y + 1][position.x - 2]) &&
            !isWall(_board[position.y + 2][position.x - 1]))
        {
            _ceilingBoard[position.y + 2][position.x - 2].SetActive(false);
        }

        // upper cell
        if (position.y + 2 < boardRealHeight && !isWall(_board[position.y + 1][position.x]))
        {
            _ceilingBoard[position.y + 2][position.x].SetActive(false);
        }

        // upper-right cell
        if (position.x + 2 < boardRealWidth &&
            position.y + 2 < boardRealHeight &&
            !isWall(_board[position.y + 1][position.x]) &&
            !isWall(_board[position.y][position.x + 1]) &&
            !isWall(_board[position.y + 1][position.x + 2]) &&
            !isWall(_board[position.y + 2][position.x + 1]))
        {
            _ceilingBoard[position.y + 2][position.x + 2].SetActive(false);
        }
    }

    bool isWall(string cellCode)
    {
        return cellCode == "w";
    }
}