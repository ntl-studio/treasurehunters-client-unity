using NtlStudio.TreasureHunters.Model;
using TMPro;
using TreasureHunters;
using UnityEngine;

public class NextTurn : MonoBehaviour
{
    public GameObject NextTurnPanel;
    public TextMeshProUGUI Message;

    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Debug.Assert(NextTurnPanel);
        Debug.Assert(Message);

        NextTurnPanel.SetActive(false);
        Game.OnEndMove += () =>
        {
            NextTurnPanel.SetActive(true);
            if (Game.State == GameClientState.GameOver)
            {
                Message.text = "You won";
            }
        };
    }

    void Update()
    {
        if (Game.State == GameClientState.YourTurn &&
            (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            NextTurnPanel.SetActive(false);
            Game.StartNextTurn();
        }
    }
}
