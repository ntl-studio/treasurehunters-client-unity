using TreasureHunters;
using UnityEngine;
using VContainer;

public class PlayerActions : MonoBehaviour
{
    //private Game _game;
    //[Inject]
    //private void InjectGame(Game game)
    //{
    //    _game = game;
    //}

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
        //_game.EndTurn();
    }
}
