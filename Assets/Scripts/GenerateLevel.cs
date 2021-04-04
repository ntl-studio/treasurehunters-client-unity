using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEditor.UIElements;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public Sprite _cellSprite;
    public Sprite _wallSprite;

    private const int BOARD_WIDTH = 10;
    private const int BOARD_HEIGHT = 10;
    private const int boardRealWidth = BOARD_WIDTH * 2 + 1;
    private const int boardRealHeight = BOARD_HEIGHT * 2 + 1;

    private List<List<GameObject>> _gameBoard = new List<List<GameObject>>();

    void Start()
    {
        Debug.Assert(_cellSprite != null, "CellSprite not initialized");
        Debug.Assert(_wallSprite != null, "WallSprite not initialized");

        GameObject cellObject = new GameObject("Cell Sprite");
        {
            SpriteRenderer cellSpriteRenderer = cellObject.AddComponent<SpriteRenderer>();
            cellSpriteRenderer.sprite = _cellSprite;
        }

        GameObject wallObject = new GameObject("Wall Sprite");
        {
            SpriteRenderer wallSpriteRenderer = wallObject.AddComponent<SpriteRenderer>();
            wallSpriteRenderer.sprite = _wallSprite;
        }

        for (int row = 0; row < boardRealHeight; ++row)
        {
            List<GameObject> rowList = new List<GameObject>();

            for (int col = 0; col < boardRealWidth; col++)
            {
                if (col % 2 == 0 && row % 2 == 0)
                { 
                    rowList.Add(null);
                }
                // board cells are always on the event positions
                else if (col % 2 == 1 && row % 2 == 1)
                {
                    // var pos = cellObject.transform.position + new Vector3(col % 2 + 1, row % 2 + 1, 0);
                    var pos = cellObject.transform.position + new Vector3(col + 1, row + 1, 0);
                    var newObject = Instantiate(cellObject, pos, cellObject.transform.rotation);
                    rowList.Add(newObject);
                }
                else
                {
                    // var pos = wallObject.transform.position + new Vector3(col % 2 + 1, row % 2 + 1, 0);
                    var pos = wallObject.transform.position + new Vector3(col + 1, row + 1, 0);
                    var rot = wallObject.transform.rotation;

                    if (row % 2 == 0)
                    {
                        rot *= Quaternion.Euler(0, 0, 90);
                    }

                    var newObject = Instantiate(wallObject, pos, rot);
                    rowList.Add(newObject);
                }
            }

            _gameBoard.Add(rowList);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}