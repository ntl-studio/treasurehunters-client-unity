using TMPro;
using TreasureHunters;
using UnityEngine;

public class PlayerName : MonoBehaviour
{
    private TextMeshProUGUI PlayerNameText;

    private static Game _game => Game.Instance();

    void Start()
    {
        Debug.Assert(_game != null);

        PlayerNameText = GetComponent<TextMeshProUGUI>();

        UpdatePlayerName();
        _game.OnStartTurn += UpdatePlayerName;
        _game.OnEndMove += UpdatePlayerName;
    }

    void UpdatePlayerName()
    {
        PlayerNameText.text = _game.CurrentPlayer.Name +
                              " (" + _game.CurrentPlayer.Position.X + ", "
                              + _game.CurrentPlayer.Position.Y + ")";
    }
}
