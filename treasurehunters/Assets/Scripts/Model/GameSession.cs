using System;
using System.Collections;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Game.OnJoinGame += () => StartCoroutine(CheckGameState());
        Game.OnGameStarted += () => StartCoroutine(CheckGameState());
    }

    IEnumerator CheckGameState()
    {
        bool isRunning = false;

        while (!isRunning)
        {
            ServerConnection.Instance().GetGameStateAsync(Game.GameId, (state) =>
            {
                if (state == GameState.Running.ToString())
                {
                    isRunning = true;
                    Debug.Log("Starting the game");
                    Game.StartGame();
                }
                else
                    Debug.Log($"Waiting for game to start. Current state is {state}.");
            });

            if (!isRunning)
                yield return new WaitForSeconds(2);
        }
    }

    IEnumerator WaitForTurn()
    {
        while (Game.WaitingForTurn)
        {
            ServerConnection.Instance().GetCurrentPlayerAsync(Game.GameId, (playerName) =>
            {
                if (playerName == Game._playerName)
                {
                    Game.WaitingForTurn = false;
                    Debug.Log("Your turn");
                    Game.StartNextTurn();
                }
                else
                    Debug.Log($"Waiting for your turn. Current player is {playerName}.");
            });

            if (Game.WaitingForTurn)
                yield return new WaitForSeconds(2);
        }
    }
}
