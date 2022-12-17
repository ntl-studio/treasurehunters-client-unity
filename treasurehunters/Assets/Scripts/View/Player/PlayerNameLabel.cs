using TMPro;
using TreasureHunters;
using UnityEngine;

public class PlayerNameLabel : MonoBehaviour
{
    private TextMeshProUGUI _playerNameText;

    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        _playerNameText = GetComponent<TextMeshProUGUI>();

        UpdatePlayerName();
        Game.OnStartTurn += UpdatePlayerName;
        Game.OnEndMove += UpdatePlayerName;
    }

    void UpdatePlayerName()
    {
        _playerNameText.text = Game.PlayerName +
                              " (" + Game.Position.X + ", "
                              + Game.Position.Y + ")";
    }
}
