using TMPro;
using UnityEngine;

public class GamesListItem : MonoBehaviour
{
    public TextMeshProUGUI GameIdText;
    public TextMeshProUGUI NumberOfPlayersText;
    public TextMeshProUGUI GameStateText;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(GameIdText);
        Debug.Assert(NumberOfPlayersText);
        Debug.Assert(GameStateText);
    }

    public string GameId { set => GameIdText.text = value; }
    public string NumberOfPlayers { set => NumberOfPlayersText.text = value; }
    public string GameState { set => GameStateText.text = value; }

    public void JoinGame()
    {
        ServerConnection.Instance().JoinGameAsync(GameIdText.text, "Player 1",
            (isJoined) =>
            {
                if (isJoined)
                    Debug.Log("Joined");
                else
                    Debug.Log("Did not join");
            });
    }
}
