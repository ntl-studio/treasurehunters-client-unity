using NtlStudio.TreasureHunters.Common;
using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionStateView : MonoBehaviour
{
    public Image RightWallImage;
    public Image DownWallImage;
    public Image LeftWallImage;
    public Image UpWallImage;

    public GameObject PlayerImage;
    public GameObject SkullImage;

    public Transform PlayerDirectionTransform;

    void Start()
    {
        Debug.Assert(PlayerDirectionTransform);

        Debug.Assert(RightWallImage);
        Debug.Assert(DownWallImage);
        Debug.Assert(LeftWallImage);
        Debug.Assert(UpWallImage);

        Debug.Assert(PlayerImage);
        Debug.Assert(SkullImage);
    }

    public void SetWallsVisibility(PlayerActionState playerActionState)
    {
        PlayerImage.SetActive(false);
        SkullImage.SetActive(false);
        UpdateWalls(FieldCell.Empty);

        if (playerActionState.Action.Type == ActionType.Move)
        {
            UpdateWalls(playerActionState.FieldCell);
            PlayerImage.SetActive(true);
            GameUtils.UpdateRotation(playerActionState.Action.Direction, PlayerDirectionTransform);
        }
        else if (playerActionState.Action.Type == ActionType.Die)
        {
            SkullImage.SetActive(true);
        }
    }

    protected void UpdateWalls(FieldCell cell)
    {
        RightWallImage.gameObject.SetActive(cell.HasFlag(FieldCell.RightWall));
        DownWallImage.gameObject.SetActive(cell.HasFlag(FieldCell.BottomWall));
        LeftWallImage.gameObject.SetActive(cell.HasFlag(FieldCell.LeftWall));
        UpWallImage.gameObject.SetActive(cell.HasFlag(FieldCell.TopWall));
    }
}
