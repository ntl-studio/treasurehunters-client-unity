using System.Collections;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Game.OnStartJoiningGame += () => StartCoroutine(GetPlayerDetails());
        Game.OnFinishJoiningGame += () => StartCoroutine(WaitForGameStart());
        Game.OnStartGame += () => StartCoroutine(WaitForTurn());
    }

    IEnumerator GetPlayerDetails()
    {
        ServerConnection.Instance().GetPlayerInfoAsync(Game.GameId, Game.PlayerName, UpdatePositionCallback);
        yield return null;
    }

    IEnumerator WaitForGameStart()
    {
        while (Game.State == GameClientState.WaitingForGameStart)
        {
            ServerConnection.Instance().GetGameStateAsync(Game.GameId, (state, playersCount) =>
            {
                if (state == GameState.Running.ToString())
                    Game.State = GameClientState.WaitingForTurn;
                else
                    Debug.Log($"Waiting for game to start. Current state is {state}.");
            });

            yield return new WaitForSeconds(2);
        }

        yield return null;
    }

    void UpdatePositionCallback(int x, int y, int[] visibleArea)
    {
        Game.PlayerPosition = new TreasureHunters.Position(x, y);
        Game.SetVisibleArea(visibleArea);

        Game.State = GameClientState.WaitingForGameStart;
    }

    IEnumerator WaitForTurn()
    {
        while (Game.State == GameClientState.WaitingForTurn)
        {
            ServerConnection.Instance().GetCurrentPlayerAsync(Game.GameId, (playerName) =>
            {
                if (playerName == Game.PlayerName)
                    Game.State = GameClientState.YourTurn;
                else
                    Debug.Log($"Waiting for your turn. Current player is {playerName}.");
            });

            yield return new WaitForSeconds(2);
        }
    }
}
