using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    public void GrenadeAction()
    {
        Debug.Log("Throw grenade");
    }

    public void GunAction()
    {
        Debug.Log("Shoot gun");

        // enable mouse & keyboardhandling

        // check for direction and call server API with action perform command
        // ServerConnection.Instance().PerformActionAsync
    }
    
    public void SkipAction()
    {
        Debug.Log("Skip turn");
        Game.PerformAction(new PlayerAction { Type = ActionType.Skip });
    }
}
