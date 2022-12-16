using System;
using System.Collections;
using JsonObjects;
using UnityEngine;
using UnityEngine.Networking;

public class ServerConnection : MonoBehaviour
{
    private static ServerConnection _instance;

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
        StartCoroutine(GetData("https://localhost:7209/api/v1/games", updateUICallBack));
    }

    private IEnumerator GetData(string uri, Action<GamesJson> callback)
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

            callback(games.data);
        }
        else
        {
            Debug.Log(request.error);
        }
    }

    public void JoinGameAsync(string gameId, string playerName, Action<bool> callback)
    {
        StartCoroutine(JoinGame(gameId, playerName, callback));
    }

    private IEnumerator JoinGame(string gameId, string playerName, Action<bool> callback)
    {
        string uri = $"https://localhost:7209/api/v1/games/{gameId}/players/{playerName}";
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
                callback(playersData.successful);
            }
        }
        else
        {
            Debug.Log(request.error);
        }
    }

}
