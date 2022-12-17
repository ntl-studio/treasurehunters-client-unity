using System.Linq;
using JsonObjects;
using TreasureHunters;
using UnityEngine;

public class GamesList : MonoBehaviour
{
    public GameObject GamesListItemPrefab;
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Debug.Assert(GamesListItemPrefab);

        ServerConnection.Instance().UpdateGamesListAsync(UpdateGamesList);

        Game.OnJoinGame += () => gameObject.SetActive(false);
        Game.OnGameStarted += () => gameObject.SetActive(false);
    }

    public void UpdateGamesList(GamesJson games)
    {
        var pos = transform.position;
        foreach (var game in games.games)
        {
            var obj = Instantiate(GamesListItemPrefab, transform);
            obj.transform.position = pos;
            pos.y -= 120;

            var gamesListItem = obj.GetComponent<GamesListItem>();
            Debug.Assert(gamesListItem);

            gamesListItem.GameId = game.id;
            gamesListItem.State = game.state;
            gamesListItem.NumberOfPlayers = game.playerscount.ToString();

            if (game.players.Any(p => p == Game.PlayerName))
                gamesListItem.AllowRejoin = true;
        }
    }
}
