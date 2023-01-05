using System.Collections;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Game.OnJoined += () => StartCoroutine(UpdatePlayerDetails(GameClientState.WaitingForStart));

        Game.OnWaitingForStart += () => StartCoroutine(WaitForGameStart());

        Game.OnWaitingForTurn += () => StartCoroutine(WaitForTurn());

        // When it is again your turn, the client needs to pull updated visibility area
        Game.OnYourTurn += () =>
        {
            StartCoroutine(UpdatePlayerDetails(GameClientState.YourTurn));
            UpdateMovesHistory();
        };

        Game.OnPerformAction += (result) => { UpdateMovesHistory(); };

        Game.OnEndMove += () =>
        {
            StartCoroutine(UpdatePlayerDetails(GameClientState.WaitingForTurn));
        };

        Game.OnGameFinished += () =>
        {
            ServerConnection.Instance().GetWinnerAsync(Game.GameId, (winnerName) => { Game.WinnerName = winnerName; });
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


    private readonly WaitForSeconds _waitFor2Seconds = new WaitForSeconds(2);

    IEnumerator WaitForGameStart()
    {
        while (Game.State == GameClientState.WaitingForStart)
        {
            ServerConnection.Instance().GetGameStateAsync(Game.GameId, (state, playersCount) =>
            {
                if (state == GameState.Running.ToString())
                {
                    Game.PlayersCount = playersCount;
                    Game.State = GameClientState.WaitingForTurn;
                }    
                else
                    Debug.Log($"Waiting for game to start. Current state is {state}.");
            });

            yield return _waitFor2Seconds;
        }

        yield return null;
    }

    IEnumerator WaitForTurn()
    {
        while (Game.State == GameClientState.WaitingForTurn)
        {
            ServerConnection.Instance().GetCurrentPlayerAsync(Game.GameId, (currentPlayerName, gameState) =>
            {
                if (currentPlayerName == Game.PlayerName)
                {
                    Game.State = gameState == "Finished" ? GameClientState.Finished : GameClientState.YourTurn;
                }
                else
                {
                    Game.CurrentPlayerName = currentPlayerName;
                    Debug.Log($"Waiting for your turn. Current player is {currentPlayerName}.");
                }
            });

            yield return _waitFor2Seconds;
        }
    }

    private void UpdateMovesHistory()
    {
        ServerConnection.Instance().GetMovesHistoryAsync(Game.GameId, (movesHistory) =>
        {
            Game.PlayersMovesHistory = movesHistory;
        });
    }
}
