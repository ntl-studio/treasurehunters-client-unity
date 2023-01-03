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

    public void SetWallsVisibility(SM.PlayerActionState playerActionState)
    {
        RightWallImage.gameObject.SetActive(playerActionState.FieldCell.HasFlag(SM.FieldCell.RightWall));
        DownWallImage.gameObject.SetActive(playerActionState.FieldCell.HasFlag(SM.FieldCell.BottomWall));
        LeftWallImage.gameObject.SetActive(playerActionState.FieldCell.HasFlag(SM.FieldCell.LeftWall));
        UpWallImage.gameObject.SetActive(playerActionState.FieldCell.HasFlag(SM.FieldCell.TopWall));

        GameUtils.UpdateRotation(playerActionState.Action, PlayerDirectionTransform);
    }
}
