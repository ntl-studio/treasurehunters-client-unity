using TreasureHunters;
using UnityEngine;

public class PlayerClick : MonoBehaviour
{
    private static Game _game => Game.Instance();

    private void OnMouseDown()
    {
        Debug.Log("Player clicked");
        _game.PlayerClicked();
    }
}
