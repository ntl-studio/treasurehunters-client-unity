using TreasureHunters;
using UnityEngine;
using VContainer;

public class PlayerActions : MonoBehaviour
{
    [Inject] private void InjectGame(Game game) { _game = game; }
    private Game _game;

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
        _game.EndTurn();
    }
}
