using System;
using NtlStudio.TreasureHunters.Model;
using TMPro;
using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

public class GamesListItem : MonoBehaviour
{
    public TextMeshProUGUI GameIdText;
    public TextMeshProUGUI NumberOfPlayersText;
    public TextMeshProUGUI GameStateText;
    public TextMeshProUGUI JoinButtonText;
    public GameObject StartGameButton;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(GameIdText);
        Debug.Assert(NumberOfPlayersText);
        Debug.Assert(GameStateText);
        Debug.Assert(JoinButtonText);
        Debug.Assert(StartGameButton);

        if (AllowRejoin)
            JoinButtonText.text = "Rejoin";
    }

    public string GameId 
    { 
        set => GameIdText.text = value;
        get => GameIdText.text;
    }
    public string NumberOfPlayers { set => NumberOfPlayersText.text = value; }

    public string State { set => GameStateText.text = value; }

    public bool AllowRejoin;

    private GameClient Game => GameClient.Instance();

    public void JoinGame()
    {
        // If we join the game for the first time (player name is not in the players list)
        if (!AllowRejoin)
        {
            ServerConnection.Instance().JoinGameAsync(GameIdText.text,
                (isJoined, gameId, playersCount, sessionId) =>
                {
                    if (isJoined)
                    {
                        Debug.Log($"Joined to the game {gameId}");
                        Game.JoinGame(gameId, playersCount, sessionId);
                    }
                    else
                        Debug.Log("Did not join");
                });
        }
        else
        {
            ServerConnection.Instance().GetGameStateAsync(GameIdText.text, (state, playersCount) =>
            {
                if (state == GameState.NotStarted.ToString())
                    Game.JoinGame(GameIdText.text, playersCount, "", started: false);
                else if (state == GameState.Running.ToString())
                    Game.JoinGame(GameIdText.text, playersCount, "", started: true);
                else
                    throw new Exception($"Game state {state} not supported");

            });
        }
    }

    public void StartGame()
    {
        ServerConnection.Instance()
            .StartGameAsync(GameId, () =>
            {
                Debug.Log($"Game {GameId} started");
                StartGameButton.GetComponent<Button>().interactable = false;
            });
    }
}
