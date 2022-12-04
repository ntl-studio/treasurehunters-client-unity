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
        UpdatePlayerName();
        _game.OnStartNextTurn += UpdatePlayerName;
    }

    void UpdatePlayerName()
    {
        PlayerNameText.text = _game.CurrentPlayer.Name +
                              " (" + _game.CurrentPlayer.Position.X + ", "
                              + _game.CurrentPlayer.Position.Y + ")";
    }
}
