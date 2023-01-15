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

        Game.OnWaitingForTurn += () =>
        {
            Message.text = Game.IsPlayerAlive 
                ? $"Waiting for turn.\nCurrent player: {Game.CurrentPlayerName}" 
                : $"You died, sorry.\nCurrent player: {Game.CurrentPlayerName}";
        };

        Game.OnUpdateCurrentPlayerName += () =>
        {
            Message.text = Game.IsPlayerAlive
                ? $"Waiting for turn.\nCurrent player: {Game.CurrentPlayerName}"
                : $"You died, sorry.\nCurrent player: {Game.CurrentPlayerName}";
        };

        Game.OnYourTurn += () => { Message.text = "Your turn!"; };

        Game.OnUpdateWinner += () =>
        {
            Message.text = Game.PlayerName == Game.WinnerName 
                ? "You won!" 
                : $"You lost, the winner is {Game.WinnerName}.\nSorry, but you suck at this game.";
        };

        Game.OnPlayerDied += () => 
        { 
            Message.text = $"You died, sorry.\n Current player: {Game.CurrentPlayerName}";
        };
    }
}
