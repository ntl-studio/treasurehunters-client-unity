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

    public Transform PlayerDirectionTransform;

    void Start()
    {
        Debug.Assert(PlayerDirectionTransform);
    }

    public void SetWallsVisibility(SM.PlayerMoveState playerState)
    {
        RightWallImage.gameObject.SetActive(playerState.FieldCell.HasFlag(SM.FieldCell.RightWall));
        DownWallImage.gameObject.SetActive(playerState.FieldCell.HasFlag(SM.FieldCell.BottomWall));
        LeftWallImage.gameObject.SetActive(playerState.FieldCell.HasFlag(SM.FieldCell.LeftWall));
        UpWallImage.gameObject.SetActive(playerState.FieldCell.HasFlag(SM.FieldCell.TopWall));

        GameUtils.UpdateRotation(playerState.Direction, PlayerDirectionTransform);
    }
}
