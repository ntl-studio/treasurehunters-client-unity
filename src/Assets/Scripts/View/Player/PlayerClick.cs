using TreasureHunters;
using UnityEngine;
using VContainer;

public class PlayerClick : MonoBehaviour
{
    [Inject] void InjectGame(Game game) { _game = game; }
    private Game _game;

    private void OnMouseDown()
    {
        Debug.Log("Player clicked");
        _game.PlayerClicked();
    }
}