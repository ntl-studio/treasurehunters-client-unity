using TMPro;
using TreasureHunters;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        gameObject.GetComponent<TMP_InputField>().text = Game.PlayerName;
    }

    public void UpdatePlayerName()
    {
        Game.PlayerName = gameObject.GetComponent<TMP_InputField>().text;
    }
}
