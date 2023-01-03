using TreasureHunters;
using UnityEngine;

public class PlayerClick : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    private void OnMouseDown()
    {
        Debug.Log("Player clicked");
        Game.ChoosePlayerAction();
    }
}
