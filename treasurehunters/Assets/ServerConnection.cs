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

    public void UpdateGamesListAsync(Action<GamesJson> updateUICallBack)
    {
        StartCoroutine(GetGamesList("https://localhost:7209/api/v1/games", updateUICallBack));
    }

    private IEnumerator GetGamesList(string uri, Action<GamesJson> gameListCallback)
    {
        UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var games = JsonUtility.FromJson<GamesDataJson>(jsonText);
            if (games == null)
            {
                Debug.Log($"Could not read games form the json: {jsonText}");
                Debug.Assert(true);
            }

            gameListCallback(games.data);
        }
        else
        {
            Debug.Log(request.error);
        }
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
        StartCoroutine(GetGameState(gameId, gameStateCallback));
    }

    private IEnumerator GetGameState(string gameId, Action<string, int> gameStateCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}";
        UnityWebRequest request = UnityWebRequest.Get(uri);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var gameData = JsonUtility.FromJson<GameDataJson>(jsonText);
            if (gameData == null)
            {
                Debug.Log($"Could not read game from json: {jsonText}");
                Debug.Assert(true);
            }
            else
            {
                gameStateCallback(gameData.data.state, gameData.data.playerscount);
            }
        }
        else
        {
            Debug.Log(request.error);
        }
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

    public void GetVisibleAreaAsync(string gameId, string playerName, Action<int[]> getVisibleAreaCallback)
    {
        StartCoroutine(GetVisibleArea(gameId, playerName, getVisibleAreaCallback));
    }
    private IEnumerator GetVisibleArea(string gameId, string playerName, Action<int[]> getVisibleAreaCallback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/visiblearea/{playerName}";
        UnityWebRequest request = UnityWebRequest.Get(uri);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonText = request.downloadHandler.text;

            var visibleArea = JsonUtility.FromJson<VisibleAreaDataJson>(jsonText);
            if (visibleArea == null)
            {
                Debug.Log($"Could not read current player from json: {jsonText}");
                Debug.Assert(true);
            }
            else
            {
                var cells = visibleArea.data.visiblearea;
                getVisibleAreaCallback(cells);
            }
        }
        else
        {
            Debug.Log(request.error);
        }
    }
}
