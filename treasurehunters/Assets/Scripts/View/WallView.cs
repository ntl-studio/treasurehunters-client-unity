using NtlStudio.TreasureHunters.Common;
using UnityEngine;

public class WallView : MonoBehaviour
{
    public GameObject RightWall;
    public GameObject BottomWall;
    public GameObject LeftWall;
    public GameObject TopWall;

    void Start()
    {
        Debug.Assert(RightWall);
        Debug.Assert(BottomWall);
        Debug.Assert(LeftWall);
        Debug.Assert(TopWall);
    }

    public void SetWallsVisibility(FieldCell cell)
    {
        LeftWall.SetActive(cell.HasFlag(FieldCell.LeftWall));
        BottomWall.SetActive(cell.HasFlag(FieldCell.BottomWall));
        RightWall.SetActive(cell.HasFlag(FieldCell.RightWall));
        TopWall.SetActive(cell.HasFlag(FieldCell.TopWall));
    }
}
