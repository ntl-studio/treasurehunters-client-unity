using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using UnityEngine;

using JsonObjects;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;

public class ServerConnection : MonoBehaviour
{
    private static ServerConnection _instance;
    private static GameClient Game => GameClient.Instance();
    private string ServerAddress => Game.ServerName;
    private enum RequestType { Get, Post, Put }
    private static readonly HttpClient HttpClient = new();

    public static ServerConnection Instance()
    {
        if (_instance) 
            return _instance;

        var connectionObject = GameObject.Find("ServerConnection");
        Debug.Assert(connectionObject);

        _instance = connectionObject.GetComponent<ServerConnection>();
        Debug.Assert(_instance);

        return _instance;
    }

    public async Task CreateGameAsync()
    {
        string uri = $"https://{ServerAddress}/api/v1/games/create?privateGame=false";
        await WebRequestAsync<NewGameDataJson>(uri, RequestType.Post);
    }

    public async Task<GamesJson> GetGamesListAsync()
    {
        string uri = $"https://{ServerAddress}/api/v1/games";
        var gamesList = await WebRequestAsync<GamesDataJson>(uri, RequestType.Get);
        return gamesList.data;
    }

    public async Task<PlayerSessionIdDataJson> JoinGameAsync(string gameId)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/players/{Game.PlayerName}";
        return await WebRequestAsync<PlayerSessionIdDataJson>(uri, RequestType.Put);
    }

    public async Task<TreasurePositionJson> GetTreasurePositionAsync(string gameId)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/treasureposition_debug";
        var treasurePosition = await WebRequestAsync<TreasurePositionDataJson>(uri, RequestType.Get);
        return treasurePosition.data;
    }

    public async Task<GameJson> GetGameStateAsync(string gameId)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}";
        var gameData = await WebRequestAsync<GameDataJson>(uri, RequestType.Get);
        return gameData.data;
    }

    public async Task StartGameAsync(string gameId)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/start";
        await WebRequestAsync<GameStateDataJson>(uri, RequestType.Put);
    }

    public async Task DeleteGameAsync(string gameId)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/delete";
        await WebRequestAsync<DeleteGameDataJson>(uri, RequestType.Post);
    }

    public async Task<CurrentPlayerJson> GetCurrentPlayerAsync(string gameId)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/currentplayer";
        var currentPlayerJson = await WebRequestAsync<CurrentPlayerDataJson>(uri, RequestType.Get);
        return currentPlayerJson.data;
    }

    public async Task<PlayerInfoJson> GetPlayerInfoAsync(string gameId, string playerName)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/players/{playerName}";
        var playerInfo = await WebRequestAsync<PlayerInfoDataJson>(uri, RequestType.Get);
        return playerInfo.data;
    }

    public async Task<PlayerActionResultDataJson> PerformActionAsync(string gameId, string playerName, PlayerAction playerAction)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/performaction/player/{playerName}/action/{playerAction.Type}/{playerAction.Direction}";
        return await WebRequestAsync<PlayerActionResultDataJson>(uri, RequestType.Put);
    }

    public async Task<string> GetWinnerAsync(string gameId)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/getwinner";
        var winnerResult= await WebRequestAsync<WinnerNameDataJson>(uri, RequestType.Get);
        return winnerResult.data;
    }

    public async Task<List<PlayerMovesDetails>> GetMovesHistoryAsync(string gameId)
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

        return playerMoveStates;
    }

    private async Task<T> WebRequestAsync<T>(string uri, RequestType requestType)
    {
        HttpResponseMessage response;

        switch (requestType)
        {
            case RequestType.Get:
                response = await HttpClient.GetAsync(uri);
                break;
            case RequestType.Post:
                response = await HttpClient.PostAsync(uri, null);
                break;
            case RequestType.Put:
                response = await HttpClient.PutAsync(uri, null);
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
