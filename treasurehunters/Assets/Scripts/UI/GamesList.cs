using System.Collections.Generic;
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

        UpdateGamesList();

        Game.OnUpdatePlayerName += UpdateGamesList;
        Game.OnJoined += () => gameObject.SetActive(false);
    }


    public void CreateGame()
    {
        ServerConnection.Instance().CreateGameAsync(UpdateGamesList);
    }

    private List<GameObject> _games = new List<GameObject>();

    public void UpdateGamesList()
    {
        ServerConnection.Instance().UpdateGamesListAsync(UpdateGamesList);
    }

    public void UpdateGamesList(GamesJson games)
    {
        foreach (var game in _games)
        {
            Destroy(game);
        }

        _games.Clear();

        var pos = transform.position;
        pos.y += 300;

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
            gamesListItem.GamesList = this;

            if (game.players.Any(p => p == Game.PlayerName))
                gamesListItem.AllowRejoin = true;

            _games.Add(obj);
        }
    }

    public void DeleteGame(string gameId)
    {
        ServerConnection.Instance().DeleteGameAsync(gameId, () =>
        {
            var game = _games.First(x => x.GetComponent<GamesListItem>().GameId == gameId);
            Destroy(game);
            _games.Remove(game);

            Debug.Log($"Game {gameId} deleted");

            UpdateGamesList();
        });
    }
}
