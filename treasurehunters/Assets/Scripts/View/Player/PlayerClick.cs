using TreasureHunters;
using UnityEngine;

public class PlayerClick : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    private void OnMouseDown()
    {
        Debug.Log("Player clicked");
        if (Game.IsPlayerAlive)
            Game.ChoosePlayerAction();
        else
            Debug.Log("You are dead, so not actions are available");
    }
}
