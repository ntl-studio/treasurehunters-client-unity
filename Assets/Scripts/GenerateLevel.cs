using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{
    public GameObject _floorPrefab;
    public GameObject _wallPrefab;

    private List<List<GameObject>> _gameBoard = new List<List<GameObject>>();

    private const int BOARD_WIDTH = 10;
    private const int BOARD_HEIGHT = 10;
    private const int boardRealWidth = BOARD_WIDTH * 2 + 1;
    private const int boardRealHeight = BOARD_HEIGHT * 2 + 1;

    void Start()
    {
        Debug.Assert(_floorPrefab != null);
        Debug.Assert(_wallPrefab != null);

        for (int row = 0; row < boardRealHeight; ++row)
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
                        ((float)col - 1) / 2 + 0.5f, 
                        ((float)row - 1) / 2 + 0.5f, 
                        0);

                    var newObject = Instantiate(_floorPrefab, pos, new Quaternion());
                    newObject.transform.SetParent(transform);
                    newObject.name = row + " " + col + " cell";
                    rowList.Add(newObject);
                }

                // creating walls
                else
                {
                    var pos = new Vector3(col >> 1, row >> 1, 0);
                    var rot = new Quaternion().normalized;

                    string nameSuffix = " vertical";

                    if (row % 2 == 0)
                    {
                        rot *= Quaternion.Euler(0, 0, 270);
                        nameSuffix = " horizontal";
                    }

                    var newObject = Instantiate(_wallPrefab, pos, rot);
                    newObject.transform.SetParent(transform);
                    newObject.name = row + " " + col + " wall" + nameSuffix;
                    rowList.Add(newObject);
                }
            }

            _gameBoard.Add(rowList);
        }
    }
}