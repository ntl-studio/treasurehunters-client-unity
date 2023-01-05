using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using JsonObjects;
using TreasureHunters;
using UnityEngine;

using NtlStudio.TreasureHunters.Model;

using CurrentPlayerDataJson = JsonObjects.DataJson<JsonObjects.CurrentPlayerJson>;
using PlayersMoveHistoryDataJson = JsonObjects.DataJson<JsonObjects.PlayersMoveStatesJson>;
using NewGameDataJson = JsonObjects.DataJson<JsonObjects.NewGameJson>;
using DeleteGameDataJson = JsonObjects.DataJson<string>;
using GameDataJson = JsonObjects.DataJson<JsonObjects.GameJson>;
using GameStateDataJson = JsonObjects.DataJson<JsonObjects.GameStateJson>;
using GamesDataJson = JsonObjects.DataJson<JsonObjects.GamesJson>;
using PlayerActionResultDataJson = JsonObjects.DataJson<JsonObjects.PlayerActionResult>;
using PlayerInfoDataJson = JsonObjects.DataJson<JsonObjects.PlayerInfoJson>;
using PlayersDataJson =  JsonObjects.DataJson<JsonObjects.PlayersJson>;
using TreasurePositionDataJson = JsonObjects.DataJson<JsonObjects.TreasurePositionJson>;
using WinnerNameDataJson = JsonObjects.DataJson<string>;

public class ServerConnection : MonoBehaviour
{
    private static ServerConnection _instance;
    private static GameClient Game => GameClient.Instance();
    private string ServerAddress => Game.ServerName;
    private enum RequestType { Get, Post, Put }

    private static readonly HttpClient _httpClient = new();

    public static ServerConnection Instance()
    {
        if (!_instance)
        { 
            var connectionObject = GameObject.Find("ServerConnection");

            Debug.Assert(connectionObject);

            _instance = connectionObject.GetComponent<ServerConnection>();
            Debug.Assert(_instance);
        }

        return _instance;
    }

    public async void CreateGameAsync(Action gameListCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/create?privateGame=false";
        await WebRequestAsync<NewGameDataJson>(uri, RequestType.Post);
        gameListCallback();
    }

    public async Task<GamesJson> UpdateGamesListAsync()
    {
        string uri = $"https://{ServerAddress}/api/v1/games";
        var gamesList = await WebRequestAsync<GamesDataJson>(uri, RequestType.Get);
        return gamesList.data;
    }

    public delegate void JoinGameCallbackAction(bool joined, string gameId, string sessionId);

    public async void JoinGameAsync(string gameId, JoinGameCallbackAction joinGameCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/players/{Game.PlayerName}";
        var playerData = await WebRequestAsync<PlayersDataJson>(uri, RequestType.Put);
        joinGameCallback(playerData.successful, gameId, playerData.data.sessionid);
    }

    public async void GetTreasurePositionAsync(string gameId, Action<int, int> getTreasurePositionCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/treasureposition_debug";
        var treasureData = await WebRequestAsync<TreasurePositionDataJson>(uri, RequestType.Get);
        getTreasurePositionCallback(treasureData.data.x, treasureData.data.y);
    }

    public async void GetGameStateAsync(string gameId, Action<string, int> gameStateCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}";
        var gameData = await WebRequestAsync<GameDataJson>(uri, RequestType.Get);
        gameStateCallback(gameData.data.state, gameData.data.playerscount);
    }

    public async void StartGameAsync(string gameId, Action startGameCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/start";
        await WebRequestAsync<GameStateDataJson>(uri, RequestType.Put);
        startGameCallback();
    }

    public async void DeleteGameAsync(string gameId, Action deleteGameCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/delete";
        await WebRequestAsync<DeleteGameDataJson>(uri, RequestType.Post);
        deleteGameCallback();
    }

    public async void GetCurrentPlayerAsync(string gameId, Action<string, string> currentPlayerCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/currentplayer";
        var currentPlayerJson = await WebRequestAsync<CurrentPlayerDataJson>(uri, RequestType.Get);
        currentPlayerCallback(currentPlayerJson.data.name, currentPlayerJson.data.gamestate);
    }

    public async void GetPlayerInfoAsync(string gameId, string playerName, Action<int, int, int[]> getPlayerInfoCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/players/{playerName}";
        var playerInfo = await WebRequestAsync<PlayerInfoDataJson>(uri, RequestType.Get);
        getPlayerInfoCallback(
            playerInfo.data.x,
            playerInfo.data.y,
            playerInfo.data.visiblearea
        );
    }

    public async void PerformActionAsync(string gameId, string playerName, PlayerAction playerAction, 
        Action<bool, bool, string> performActionCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/performaction/player/{playerName}/action/{playerAction.Type}/{playerAction.Direction}";
        var playerActionResult = await WebRequestAsync<PlayerActionResultDataJson>(uri, RequestType.Put);
        performActionCallback(
            playerActionResult.successful,
            playerActionResult.data.hastreasure,
            playerActionResult.data.state // game state
        );
    }

    public async void GetWinnerAsync(string gameId, Action<string> getWinnerCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/getwinner";
        var winnerResult= await WebRequestAsync<WinnerNameDataJson>(uri, RequestType.Get);
        getWinnerCallback(winnerResult.data);
    }

    public async void GetMovesHistoryAsync(string gameId, Action<List<PlayerMovesDetails>> movesHistoryCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/playersmovestates";
        var playersMoveStates = await WebRequestAsync<PlayersMoveHistoryDataJson>(uri, RequestType.Get);

        List<PlayerMovesDetails> playerMoveStates = new();

        foreach (var player in playersMoveStates.data.players)
        {
            PlayerMovesDetails details = new PlayerMovesDetails
            {
                PlayerName = player.player
            };

            Debug.Assert(player.actionstates != null);

            foreach (var playerAction in player.actionstates)
            {
                details.Moves.Add(new PlayerActionState()
                {
                    Position = playerAction.position,

                    Action = new PlayerAction
                    {
                        Direction = playerAction.direction,
                        Type = playerAction.type
                    },

                    FieldCell = playerAction.cell
                });
            }

            playerMoveStates.Add(details);
        }

        movesHistoryCallback(playerMoveStates);
    }

    private async Task<T> WebRequestAsync<T>(string uri, RequestType requestType)
    {
        HttpResponseMessage response;

        switch (requestType)
        {
            case RequestType.Get:
                response = await _httpClient.GetAsync(uri);
                break;
            case RequestType.Post:
                response = await _httpClient.PostAsync(uri, null);
                break;
            case RequestType.Put:
                response = await _httpClient.PutAsync(uri, null);
                break;
            default:
                throw new Exception($"Not supported request type {requestType}");
        }

        if (!response.IsSuccessStatusCode)
            throw new Exception($"{requestType} request {uri} failed with error: {response.StatusCode}");

        var jsonText = await response.Content.ReadAsStringAsync();

        var data = JsonUtility.FromJson<T>(jsonText);
        if (data == null)
        {
            Debug.Log($"Could not read data form the json: {jsonText}");
            Debug.Assert(true);
        }
        else
            Debug.Log($"Getting data: {jsonText}");

        return data;
    }
}
