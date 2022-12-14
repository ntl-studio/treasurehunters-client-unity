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
    }
    
    public void SkipAction()
    {
        Debug.Log("Skip turn");
        Game.StartNextTurn();
    }
}
