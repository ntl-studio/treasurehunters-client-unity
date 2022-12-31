using System.Collections;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Game.OnWaitingForStart += () => StartCoroutine(WaitForGameStart());

        Game.OnWaitingForTurn += () => StartCoroutine(WaitForTurn());

        Game.OnJoined += () => StartCoroutine(UpdatePlayerDetails(GameClientState.WaitingForStart));

        Game.OnMakingMove += (bool result) =>
        {
            if (result)
                StartCoroutine(UpdatePlayerDetails(GameClientState.WaitingForTurn));
            else if (Game.State != GameClientState.Finished)
                Game.State = GameClientState.YourTurn;
        };
    }

    IEnumerator UpdatePlayerDetails(GameClientState nextState)
    {
        ServerConnection.Instance().GetPlayerInfoAsync(Game.GameId, Game.PlayerName,
            (x, y, visibleArea) =>
            {
                Debug.Log($"Updating player position to ({x}, {y})");
                Game.PlayerPosition = new TreasureHunters.Position(x, y);
                Game.SetVisibleArea(visibleArea);

                if (Game.State != GameClientState.Finished)
                    Game.State = nextState;
            });

        yield return null;
    }

    IEnumerator WaitForGameStart()
    {
        while (Game.State == GameClientState.WaitingForStart)
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

    IEnumerator WaitForTurn()
    {
        while (Game.State == GameClientState.WaitingForTurn)
        {
            ServerConnection.Instance().GetCurrentPlayerAsync(Game.GameId, (currentPlayerName) =>
            {
                if (currentPlayerName == Game.PlayerName)
                {
                    Game.State = GameClientState.YourTurn;
                }
                else
                {
                    Game.CurrentPlayerName = currentPlayerName;
                    Debug.Log($"Waiting for your turn. Current player is {currentPlayerName}.");
                }
            });

            yield return new WaitForSeconds(2);
        }
    }
}
