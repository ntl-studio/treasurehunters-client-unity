using TMPro;
using TreasureHunters;
using UnityEngine;
using VContainer;

public class PlayerName : MonoBehaviour
{
    private TextMeshProUGUI PlayerNameText;

    [Inject] void InjectGame(Game game) { _game = game; }
    private Game _game;

    void Start()
    {
        PlayerNameText = GetComponent<TextMeshProUGUI>();

        UpdatePlayerName();
        _game.OnStartTurn += UpdatePlayerName;
        _game.OnEndTurn += UpdatePlayerName;
    }

    void UpdatePlayerName()
    {
        PlayerNameText.text = _game.CurrentPlayer.Name +
                              " (" + _game.CurrentPlayer.Position.X + ", "
                              + _game.CurrentPlayer.Position.Y + ")";
    }
}
