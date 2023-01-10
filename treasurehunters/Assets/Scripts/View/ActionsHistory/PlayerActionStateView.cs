using System;
using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

using SM = NtlStudio.TreasureHunters.Model;

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

    public void SetWallsVisibility(SM.PlayerActionState playerActionState)
    {
        PlayerImage.SetActive(false);
        SkullImage.SetActive(false);
        UpdateWalls(SM.FieldCell.Empty);

        if (playerActionState.Action.Type == SM.ActionType.Move)
        {
            UpdateWalls(playerActionState.FieldCell);
            PlayerImage.SetActive(true);
            GameUtils.UpdateRotation(playerActionState.Action.Direction, PlayerDirectionTransform);
        }
        else if (playerActionState.Action.Type == SM.ActionType.Die)
        {
            SkullImage.SetActive(true);
        }
    }

    protected void UpdateWalls(SM.FieldCell cell)
    {
        RightWallImage.gameObject.SetActive(cell.HasFlag(SM.FieldCell.RightWall));
        DownWallImage.gameObject.SetActive(cell.HasFlag(SM.FieldCell.BottomWall));
        LeftWallImage.gameObject.SetActive(cell.HasFlag(SM.FieldCell.LeftWall));
        UpWallImage.gameObject.SetActive(cell.HasFlag(SM.FieldCell.TopWall));
    }
}
