using TreasureHunters;
using UnityEngine;

public class PlayerClick : MonoBehaviour
{
    private static Game Game => Game.Instance();

    private void OnMouseDown()
    {
        Debug.Log("Player clicked");
        Game.PlayerClicked();
    }
}
