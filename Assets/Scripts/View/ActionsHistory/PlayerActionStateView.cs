using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

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

    public void SetWallsVisibility(PlayerActionState playerState)
    {
        RightWallImage.gameObject.SetActive(playerState.IsRightWall);
        DownWallImage.gameObject.SetActive(playerState.IsDownWall);
        LeftWallImage.gameObject.SetActive(playerState.IsLeftWall);
        UpWallImage.gameObject.SetActive(playerState.IsUpWall);

        PlayerMovement.UpdateRotation(playerState.MoveDirection, PlayerDirectionTransform);
    }
}
