using System;
using System.Collections;
using System.Collections.Generic;
using JsonObjects;
using TreasureHunters;
using UnityEngine;
using UnityEngine.Networking;

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

public class ServerConnection : MonoBehaviour
{
    private static ServerConnection _instance;
    private static GameClient Game => GameClient.Instance();
    private string ServerAddress = "localhost:7209";

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

    public void CreateGameAsync(Action gameListCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/create?privateGame=false";

        StartCoroutine(WebRequest<NewGameDataJson>(uri, (_) => 
            { gameListCallback(); },
            RequestType.Post
        ));
    }

    public void UpdateGamesListAsync(Action<GamesJson> gameListCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games";

        StartCoroutine(WebRequest<GamesDataJson>(uri, (gamesDataJson) => 
            { gameListCallback(gamesDataJson.data); },
            RequestType.Get
        ));
    }

    public delegate void JoinGameCallbackAction(bool joined, string gameId, string sessionId);

    public void JoinGameAsync(string gameId, JoinGameCallbackAction joinGameCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/players/{Game.PlayerName}";

        StartCoroutine(WebRequest<PlayersDataJson>(uri, (playerDataJson) => 
            { joinGameCallback(playerDataJson.successful, gameId, playerDataJson.data.sessionid); },
            RequestType.Put
        ));
    }

    public void GetTreasurePositionAsync(string gameId, Action<int, int> getTreasurePositionCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/treasureposition_debug";

        StartCoroutine(WebRequest<TreasurePositionDataJson>(uri, (treasurePositionDataJson) =>
            { getTreasurePositionCallback(treasurePositionDataJson.data.x, treasurePositionDataJson.data.y); },
            RequestType.Get
        ));
    }

    public void GetGameStateAsync(string gameId, Action<string, int> gameStateCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}";

        StartCoroutine(WebRequest<GameDataJson>(uri, (gameDataJson) =>
            { gameStateCallback(gameDataJson.data.state, gameDataJson.data.playerscount); },
            RequestType.Get
        ));
    }

    public void StartGameAsync(string gameId, Action startGameCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/start";

        StartCoroutine(WebRequest<GameStateDataJson>(uri, (_) =>
            { startGameCallback(); },
            RequestType.Put
        ));
    }

    public void DeleteGameAsync(string gameId, Action deleteGameCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/delete";

        StartCoroutine(WebRequest<DeleteGameDataJson>(uri, (_) =>
            { deleteGameCallback(); },
            RequestType.Post
        ));
    }

    public void GetCurrentPlayerAsync(string gameId, Action<string> currentPlayerCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/currentplayer";

        StartCoroutine(WebRequest<CurrentPlayerDataJson>(uri, (currentPlayerJson) =>
            { currentPlayerCallback(currentPlayerJson.data.name); },
            RequestType.Get
        ));
    }

    public void GetPlayerInfoAsync(string gameId, string playerName, 
        Action<int, int, int[]> getPlayerInfoCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/players/{playerName}";

        StartCoroutine(WebRequest<PlayerInfoDataJson>(uri, (playerInfo) =>
            { getPlayerInfoCallback(playerInfo.data.x, playerInfo.data.y, playerInfo.data.visiblearea); },
            RequestType.Get
        ));
    }

    public void PerformActionAsync(string gameId, string playerName, string actionName, 
        Action<bool, bool, string> performActionCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/performaction/player/{playerName}/action/{actionName}";

        StartCoroutine(WebRequest<PlayerActionResultDataJson>(uri, (playerActionResult) => 
            { 
                performActionCallback(
                    playerActionResult.successful, 
                    playerActionResult.data.hastreasure,
                    playerActionResult.data.state // game state
                );
            },
            RequestType.Put
        ));
    }

    public void GetMovesHistoryAsync(string gameId, Action<List<PlayerMovesDetails>> movesHistoryCallback)
    {
        string uri = $"https://{ServerAddress}/api/v1/games/{gameId}/playersmovestates";

        StartCoroutine(WebRequest<PlayersMoveHistoryDataJson>(uri, (playersMoveStates) =>
            {
                List<PlayerMovesDetails> playerMoveStates = new();

                foreach (var player in playersMoveStates.data.players)
                {
                    PlayerMovesDetails details = new PlayerMovesDetails
                    {
                        PlayerName = player.player
                    };

                    foreach (var move in player.movestates)
                    {
                        details.Moves.Add(new PlayerMoveState()
                        {
                            Position = move.position,
                            Direction = move.direction,
                            FieldCell = move.cell
                        });
                    }

                    playerMoveStates.Add(details);
                }

                movesHistoryCallback(playerMoveStates);
            },
            RequestType.Get
        ));
    }

    enum RequestType { Get, Post, Put }

    private IEnumerator WebRequest<T>(string uri, Action<T> callback, RequestType requestType)
    {
        Debug.Log($"Sending {requestType} request: {uri}");

        UnityWebRequest request;

        switch (requestType)
        {
            case RequestType.Get:
                request = UnityWebRequest.Get(uri);
                break;
            case RequestType.Post:
                request = UnityWebRequest.Post(uri, String.Empty);
                break;
            case RequestType.Put:
                request = UnityWebRequest.Put(uri, String.Empty);
                break;
            default:
                throw new Exception($"Not supported request type {requestType}");
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var data = JsonUtility.FromJson<T>(jsonText);
            if (data == null)
            {
                Debug.Log($"Could not read data form the json: {jsonText}");
                Debug.Assert(true);
            }
            else
                Debug.Log($"Getting data: {jsonText}");

            callback(data);
        }
        else
        {
            Debug.Log($"{requestType} request {uri} failed with error: {request.error}");
        }
    }
}
