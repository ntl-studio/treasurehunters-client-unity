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
    private List<GamesListItem> _gameListItems = new();

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

    private GamesListItem AddNewGameListItem()
    {
        var obj = Instantiate(GamesListItemPrefab, GameItemsParent);

        var gamesListItem = obj.GetComponent<GamesListItem>();
        Debug.Assert(gamesListItem);

        return gamesListItem;
    }

    private void UpdateGamesList(GamesJson games)
    {
        for (int gameId = 0; gameId < games.games.Length; ++gameId)
        {
            if (gameId >= _gameListItems.Count)
                _gameListItems.Add(AddNewGameListItem());

            var game = games.games[gameId];
            _gameListItems[gameId].GameId = game.id;
            _gameListItems[gameId].State = game.state;
            _gameListItems[gameId].NumberOfPlayers = game.playerscount.ToString();

            if (game.players.Any(p => p == Game.PlayerName))
                _gameListItems[gameId].AllowRejoin = true;
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
