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

        Game.OnWaitingForStart += () =>
        {
            NextTurnPanel.SetActive(true);
            Message.text = "Waiting for the game start";
        };

        Game.OnJoined += () => { Message.text = "You joined the game"; };

        Game.OnWaitingForTurn += () => { Message.text = $"Waiting for turn.\n Current player: {Game.CurrentPlayerName}"; };
        Game.OnUpdateCurrentPlayerName += () => { Message.text = $"Waiting for turn.\n Current player: {Game.CurrentPlayerName}"; };

        Game.OnYourTurn += () => { Message.text = "Your turn!"; };
    }

    void Update()
    {
        if (Game.State == GameClientState.YourTurn &&
            (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            NextTurnPanel.SetActive(false);
            Game.StartTurn();
        }
    }
}
