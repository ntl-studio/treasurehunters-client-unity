using TMPro;
using TreasureHunters;
using UnityEngine;

public class ServerNameInput : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        gameObject.GetComponent<TMP_InputField>().text = Game.ServerName;
    }

    public void UpdateServerName()
    {
        Game.ServerName = gameObject.GetComponent<TMP_InputField>().text;
    }
}
