using System.Collections.Generic;
using System.Linq;
using JsonObjects;
using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

public class GamesList : MonoBehaviour
{
    public GameObject GamesListItemPrefab;
    public Transform GameItemsParent;

    public Button CreateGameButton;
    public Button RefreshGameButton;

    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Debug.Assert(GamesListItemPrefab);
        Debug.Assert(GameItemsParent);

        Debug.Assert(CreateGameButton != null);
        CreateGameButton?.onClick.AddListener(CreateGameAsync);

        Debug.Assert(RefreshGameButton != null);
        RefreshGameButton?.onClick.AddListener(UpdateGamesListAsync);

        if (!string.IsNullOrEmpty(Game.ServerName))
            UpdateGamesListAsync();

        Game.OnUpdatePlayerName += UpdateGamesListAsync;
        Game.OnWaitingForTurn += () => gameObject.SetActive(false);
    }

    public async void CreateGameAsync()
    {
        await ServerConnection.Instance().CreateGameAsync();
        UpdateGamesListAsync();
    }

    private List<GameObject> _games = new List<GameObject>();

    public async void UpdateGamesListAsync()
    {
        var games = await ServerConnection.Instance().GetGamesListAsync();
        UpdateGamesList(games);
    }

    private void UpdateGamesList(GamesJson games)
    {
        foreach (var game in _games)
        {
            Destroy(game);
        }

        _games.Clear();

        foreach (var game in games.games)
        {
            var obj = Instantiate(GamesListItemPrefab, GameItemsParent);

            var gamesListItem = obj.GetComponent<GamesListItem>();
            Debug.Assert(gamesListItem);

            gamesListItem.GameId = game.id;
            gamesListItem.State = game.state;
            gamesListItem.NumberOfPlayers = game.playerscount.ToString();

            if (game.players.Any(p => p == Game.PlayerName))
                gamesListItem.AllowRejoin = true;

            _games.Add(obj);
        }
    }

    public async void DeleteGameAsync(string gameId)
    {
        await ServerConnection.Instance().DeleteGameAsync(gameId);

        var game = _games.First(x => x.GetComponent<GamesListItem>().GameId == gameId);
        Destroy(game);
        _games.Remove(game);

        Debug.Log($"Game {gameId} deleted");

        UpdateGamesListAsync();
    }
}
