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
        Game.OnYourTurn += UpdatePlayerName;
        Game.OnEndMoveAnimation += UpdatePlayerName;
        Game.OnUpdateBullets += UpdatePlayerName;
    }

    void UpdatePlayerName()
    {
        _playerNameText.text =
            $"{Game.PlayerName} ({Game.PlayerPosition.X},{Game.PlayerPosition.Y})\n" +
            $"Bullets: {Game.Bullets}";
    }
}
