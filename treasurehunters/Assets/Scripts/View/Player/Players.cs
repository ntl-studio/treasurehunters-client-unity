using System;
using System.Collections.Generic;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class Players : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private static Game Game => Game.Instance();

    private readonly Dictionary<string, GameObject> _playerViews = new();

    void Start()
    {
        Debug.Assert(PlayerPrefab);

        for (int i = 0; i < Game.PlayersCount; ++i)
        {
            var player = Game.Players[i];

            var playerObj = Instantiate(PlayerPrefab, transform);
            playerObj.SetActive(false);

            var spriteRenderer = playerObj.GetComponent<SpriteRenderer>();
            Debug.Assert(spriteRenderer);
            spriteRenderer.color = player.Color;

            var playerMovement = playerObj.GetComponent<PlayerMovement>();
            Debug.Assert(playerMovement);
            playerMovement.Player = player;

            _playerViews.Add(player.Name, playerObj);
        }

        Game.OnStartTurn += UpdatePlayersVisibility;
        Game.OnEndMove += UpdatePlayersVisibility;

        UpdatePlayersVisibility();
    }

    void UpdatePlayersVisibility()
    {
        var currentPlayer = Game.CurrentPlayer;

        var visibilityArea = Game.CurrentVisibleArea();

        foreach (var player in Game.Players)
        {
            _playerViews[player.Name].SetActive(false);

            bool namesMatch = player.Name == currentPlayer.Name;

            if (namesMatch)
            {
                _playerViews[player.Name].SetActive(true);
            }
            else
            {
                var currPos = currentPlayer.Position;
                var pos = player.Position;

                if (Math.Abs(pos.X - currPos.X) <= 1 &&
                    Math.Abs(pos.Y - currPos.Y) <= 1)
                {
                    if (!visibilityArea[Math.Abs(currPos.X - pos.X - 1), Math.Abs(currPos.Y - pos.Y - 1)]
                            .HasFlag(FieldCell.Invisible))
                    {
                        _playerViews[player.Name].SetActive(true);
                    }
                }
            }

            _playerViews[player.Name].GetComponent<PlayerMovement>().AcceptInput =
                namesMatch;
        }
    }
}
