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
}
