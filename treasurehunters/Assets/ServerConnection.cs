using System;
using System.Collections;
using JsonObjects;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;
using UnityEngine.Networking;

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

        StartCoroutine(WebRequest<GamesDataJson>(uri, 
                (gamesDataJson) => { gameListCallback(gamesDataJson.data); },
                RequestType.Get
            ));
    }

    public delegate void JoinGameCallbackAction(bool joined, string gameId, int playersCount, string sessionId);

    public void JoinGameAsync(string gameId, JoinGameCallbackAction joinGameCallback)
    {
        StartCoroutine(JoinGame(gameId, joinGameCallback));
    }

    private IEnumerator JoinGame(string gameId, JoinGameCallbackAction joinGameCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/players/{Game.PlayerName}";
        UnityWebRequest request = UnityWebRequest.Put(uri, string.Empty);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var playersData = JsonUtility.FromJson<PlayersDataJson>(jsonText);
            if (playersData == null)
            {
                Debug.Log($"Could not read players list form json: {jsonText}");
                Debug.Assert(true);
            }
            else
            {
                joinGameCallback(playersData.successful, gameId, 1, playersData.data.sessionid);
            }
        }
        else
        {
            Debug.Log(request.error);
        }
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
        StartCoroutine(StartGame(gameId, startGameCallback));
    }
    private IEnumerator StartGame(string gameId, Action startGameCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/start";
        UnityWebRequest request = UnityWebRequest.Put(uri, string.Empty);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var gameStateData = JsonUtility.FromJson<GameStateDataJson>(jsonText);
            if (gameStateData == null)
            {
                Debug.Log($"Could not read game state from json: {jsonText}");
                Debug.Assert(true);
            }
            else
            {
                startGameCallback();
            }
        }
        else
        {
            Debug.Log(request.error);
        }
    }

    public void GetCurrentPlayerAsync(string gameId, Action<string> currentPlayerCallback)
    {
        StartCoroutine(GetCurrentPlayer(gameId, currentPlayerCallback));
    }
    private IEnumerator GetCurrentPlayer(string gameId, Action<string> currentPlayerCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/currentplayer";
        UnityWebRequest request = UnityWebRequest.Get(uri);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var currentPlayer = JsonUtility.FromJson<CurrentPlayerDataJson>(jsonText);
            if (currentPlayer == null)
            {
                Debug.Log($"Could not read current player from json: {jsonText}");
                Debug.Assert(true);
            }
            else
            {
                currentPlayerCallback(currentPlayer.data.name);
            }
        }
        else
        {
            Debug.Log(request.error);
        }
    }

    public void GetPlayerInfoAsync(string gameId, string playerName, Action<int, int, int[]> getPlayerInfoCallback)
    {
        StartCoroutine(GetPlayerInfo(gameId, playerName, getPlayerInfoCallback));
    }
    private IEnumerator GetPlayerInfo(string gameId, string playerName, Action<int, int, int[]> getPlayerInfoCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/players/{playerName}";
        UnityWebRequest request = UnityWebRequest.Get(uri);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var playerInfo = JsonUtility.FromJson<PlayerInfoDataJson>(jsonText);
            if (playerInfo == null)
            {
                Debug.Log($"Could not read player position from json: {jsonText}");
                Debug.Assert(true);
            }
            else
            {
                var data = playerInfo.data;
                getPlayerInfoCallback(data.x, data.y, data.visiblearea);
            }
        }
        else
        {
            Debug.Log(request.error);
        }
    }

    public void PerformActionAsync(string gameId, string playerName, string actionName, Action<bool> performActionCallback)
    {
        StartCoroutine(PerformAction(gameId, playerName, actionName, performActionCallback));
    }

    private IEnumerator PerformAction(string gameId, string playerName, string actionName, Action<bool> performActionCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/performaction/player/{playerName}/action/{actionName}";
        UnityWebRequest request = UnityWebRequest.Put(uri, String.Empty);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var playerActionResult = JsonUtility.FromJson<PlayerActionResultDataJson>(jsonText);
            if (playerActionResult == null)
            {
                Debug.Log($"Could not read action result from json: {jsonText}");
                Debug.Assert(true);
            }
            else
            {
                performActionCallback(playerActionResult.successful);
            }
        }
        else
        {
            Debug.Log(request.error);
        }
    }

    enum RequestType { Put, Get }

    private IEnumerator WebRequest<T>(string uri, Action<T> callback, RequestType requestType)
    {
        Debug.Log($"Sending request: {uri}");
        UnityWebRequest request =
            (requestType == RequestType.Put)
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

            callback(data);
        }
        else
        {
            Debug.Log(request.error);
        }
    }

}
