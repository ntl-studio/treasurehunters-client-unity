using TMPro;
using TreasureHunters;
using UnityEngine;
using VContainer;

public class PlayerName : MonoBehaviour
{
    public TextMeshProUGUI PlayerNameText;

    private Game _game;

    [Inject]
    void InjectGame(Game game)
    {
        _game = game;
    }

    void Start()
    {
        Debug.Log(_game.CurrentPlayer.Name);
        PlayerNameText.text = _game.CurrentPlayer.Name;
    }
}
