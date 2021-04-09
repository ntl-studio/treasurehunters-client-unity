using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public GameObject _floorPrefab;
    public GameObject _wallPrefab;

    public List<string[]> _board = new List<string[]>();

    public TextAsset _levelFileName;

    private List<List<GameObject>> _gameBoard = new List<List<GameObject>>();

    private const int BOARD_WIDTH = 10;
    private const int BOARD_HEIGHT = 10;
    private const int boardRealWidth = BOARD_WIDTH * 2 + 1;
    private const int boardRealHeight = BOARD_HEIGHT * 2 + 1;

    void Start()
    {
        Debug.Assert(_floorPrefab != null);
        Debug.Assert(_wallPrefab != null);
        Debug.Assert(_levelFileName != null);

        var levelLines = ReadLevel();

        GenerateBoard(levelLines);

        var playerPosition = FindPlayerPosition(_board);
        UpdatePlayerPosition(playerPosition);
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

        // Debug.Log(board.Count, BOARD_HEIGHT);
        Debug.Assert(_board.Count == boardRealHeight);
        Debug.Assert(_board[0].Length == boardRealWidth);

        for (var row = 0; row < boardRealHeight; ++row)
        {
            List<GameObject> rowList = new List<GameObject>();

            for (int col = 0; col < boardRealWidth; col++)
            {
                // empty element (not a wall, not a cell)
                if (col % 2 == 0 && row % 2 == 0)
                    rowList.Add(null);

                // creating cells
                else if (col % 2 == 1 && row % 2 == 1)
                {
                    var pos = new Vector3(
                        ((float) col - 1) / 2,
                        ((float) row - 1) / 2,
                        0);

                    var newObject = Instantiate(_floorPrefab, pos, new Quaternion());
                    newObject.transform.SetParent(transform);
                    newObject.name = row + " " + col + " cell";
                    rowList.Add(newObject);
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

                    var newObject = Instantiate(_wallPrefab, pos, rot);
                    newObject.transform.SetParent(transform);
                    newObject.name = row + " " + col + " wall" + nameSuffix;
                    rowList.Add(newObject);

                    if (_board[row][col] != "w")
                        newObject.SetActive(false);
                }
            }

            _gameBoard.Add(rowList);
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
    }
}