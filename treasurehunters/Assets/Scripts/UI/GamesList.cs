using System.Collections;
using JsonObjects;
using UnityEngine.Networking;
using UnityEngine;

public class GamesList : MonoBehaviour
{
    public GameObject GamesListItemPrefab;

    void Start()
    {
        Debug.Assert(GamesListItemPrefab);
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://localhost:7209/api/v1/games");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            var jsonText = request.downloadHandler.text;
            var games = JsonUtility.FromJson<DataJson>(jsonText);
            if (games != null)
            {
                Debug.Log($"Could not read games form the json: {jsonText}");
                Debug.Assert(true);
            }

            var pos = transform.position;
            foreach (var game in games.data.games)
            {
                var obj = Instantiate(GamesListItemPrefab, transform);
                obj.transform.position = pos;
                pos.y -= 120;

                var gamesListItem = obj.GetComponent<GamesListItem>();
                Debug.Assert(gamesListItem);

                gamesListItem.GameId = game.id;
                gamesListItem.GameState = game.state;
                // gamesListItem.NumberOfPlayers = game.number_of_players; // TODO: add to the API
            }
        }
    }
}
