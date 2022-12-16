using JsonObjects;
using UnityEngine;

public class GamesList : MonoBehaviour
{
    public GameObject GamesListItemPrefab;

    void Start()
    {
        Debug.Assert(GamesListItemPrefab);

        ServerConnection.Instance().UpdateGamesListAsync(UpdateGamesList);
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
            gamesListItem.GameState = game.state;
            // gamesListItem.NumberOfPlayers = game.number_of_players; // TODO: add to the API
        }
    }
}
