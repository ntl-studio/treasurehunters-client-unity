using System;
using System.Collections;
using JsonObjects;
using TreasureHunters;
using UnityEngine;
using UnityEngine.Networking;

using CurrentPlayerDataJson = JsonObjects.DataJson<JsonObjects.CurrentPlayerJson>;
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

    public void UpdateGamesListAsync(Action<GamesJson> gameListCallback)
    {
        string uri = "https://localhost:7209/api/v1/games";

        StartCoroutine(WebRequest<GamesDataJson>(uri, (gamesDataJson) => 
            { gameListCallback(gamesDataJson.data); },
            RequestType.Get
        ));
    }

    public delegate void JoinGameCallbackAction(bool joined, string gameId, string sessionId);

    public void JoinGameAsync(string gameId, JoinGameCallbackAction joinGameCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/players/{Game.PlayerName}";

        StartCoroutine(WebRequest<PlayersDataJson>(uri, (playerDataJson) => 
            { joinGameCallback(playerDataJson.successful, gameId, playerDataJson.data.sessionid); },
            RequestType.Put
        ));
    }

    public void GetTreasurePositionAsync(string gameId, Action<int, int> getTreasurePositionCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/treasureposition_debug";

        StartCoroutine(WebRequest<TreasurePositionDataJson>(uri, (treasurePositionDataJson) =>
            { getTreasurePositionCallback(treasurePositionDataJson.data.x, treasurePositionDataJson.data.y); },
            RequestType.Get
        ));
    }

    public void GetGameStateAsync(string gameId, Action<string, int> gameStateCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}";

        StartCoroutine(WebRequest<GameDataJson>(uri, (gameDataJson) =>
            { gameStateCallback(gameDataJson.data.state, gameDataJson.data.playerscount); },
            RequestType.Get
        ));
    }

    public void StartGameAsync(string gameId, Action startGameCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/start";

        StartCoroutine(WebRequest<GameStateDataJson>(uri, (gameStateJson) =>
            { startGameCallback(); },
            RequestType.Put
        ));
    }

    public void GetCurrentPlayerAsync(string gameId, Action<string> currentPlayerCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/currentplayer";

        StartCoroutine(WebRequest<CurrentPlayerDataJson>(uri, (currentPlayerJson) =>
            { currentPlayerCallback(currentPlayerJson.data.name); },
            RequestType.Get
        ));
    }

    public void GetPlayerInfoAsync(string gameId, string playerName, 
        Action<int, int, int[]> getPlayerInfoCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/players/{playerName}";

        StartCoroutine(WebRequest<PlayerInfoDataJson>(uri, (playerInfo) =>
            { getPlayerInfoCallback(playerInfo.data.x, playerInfo.data.y, playerInfo.data.visiblearea); },
            RequestType.Get
        ));
    }

    public void PerformActionAsync(string gameId, string playerName, string actionName, 
        Action<bool, bool, string> performActionCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/performaction/player/{playerName}/action/{actionName}";

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

    enum RequestType { Put, Get }

    private IEnumerator WebRequest<T>(string uri, Action<T> callback, RequestType requestType)
    {
        Debug.Log($"Sending {requestType} request: {uri}");
        UnityWebRequest request =
            (requestType == RequestType.Get)
            ? UnityWebRequest.Get(uri)
            : UnityWebRequest.Put(uri, String.Empty);

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
