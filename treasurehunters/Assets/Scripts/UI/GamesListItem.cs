using Position = TreasureHunters.Position;
using TMPro;
using TreasureHunters;
using UnityEngine.UI;
using UnityEngine;

public class GamesListItem : MonoBehaviour
{
    public TextMeshProUGUI GameIdText;
    public TextMeshProUGUI NumberOfPlayersText;
    public TextMeshProUGUI GameStateText;
    public TextMeshProUGUI JoinButtonText;

    public Button JoinGameButton;
    public Button StartGameButton;
    public Button DeleteGameButton;

    private GamesList _gamesList; 

    void Start()
    {
        Debug.Assert(GameIdText);
        Debug.Assert(NumberOfPlayersText);
        Debug.Assert(GameStateText);
        Debug.Assert(JoinButtonText);

        Debug.Assert(JoinGameButton);
        JoinGameButton.GetComponent<Button>().onClick.AddListener(JoinGameAsync);

        Debug.Assert(StartGameButton);
        StartGameButton.GetComponent<Button>().onClick.AddListener(StartGameAsync);

        Debug.Assert(DeleteGameButton);
        DeleteGameButton.GetComponent<Button>().onClick.AddListener(DeleteGame);

        _gamesList = gameObject.GetComponentInParent<GamesList>();
        Debug.Assert(_gamesList);

        if (AllowRejoin)
            JoinButtonText.text = "Rejoin";
    }

    public string GameId 
    { 
        set => GameIdText.text = value;
        get => GameIdText.text;
    }
    public string NumberOfPlayers { set => NumberOfPlayersText.text = value; }

    public string State
    {
        set
        {
            GameStateText.text = value;

            if (value == "Finished")
                JoinGameButton.interactable = false;

            if (value is "Running" or "Finished")
                StartGameButton.interactable = false;
        }
    }

    public bool AllowRejoin;

    private GameClient Game => GameClient.Instance();

    public async void JoinGameAsync()
    {
        var gameId = GameIdText.text;

        // If we join the game for the first time (player name is not in the players list)
        if (!AllowRejoin)
        {
            var joinGameData = await ServerConnection.Instance().JoinGameAsync(gameId);

            if (joinGameData.successful)
            {
                Debug.Log($"Joined to the game {gameId}");
                Game.JoinGame(gameId, joinGameData.data.sessionid);
            }
            else
                Debug.Log("Did not join");
        }
        else
        {
            Game.JoinGame(gameId, "");
        }
    }

    public async void StartGameAsync()
    {
        await ServerConnection.Instance().StartGameAsync(GameId);
        Debug.Log($"Game {GameId} started");
        StartGameButton.interactable = false;
    }

    public void DeleteGame()
    {
        _gamesList.DeleteGameAsync(GameId);
    }
}
