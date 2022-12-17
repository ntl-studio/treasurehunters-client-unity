using System;
using System.Collections.Generic;
using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;

public class Players : MonoBehaviour
{
    public GameObject PlayerPrefab;

    private static GameClient Game => GameClient.Instance();

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

        Game.OnUpdateVisibleArea += UpdatePlayersVisibility;
        Game.OnEndMove += UpdatePlayersVisibility;
    }

    void UpdatePlayersVisibility()
    {
        var visibilityArea = Game.CurrentVisibleArea();

        foreach (var player in Game.Players)
        {
            _playerViews[player.Name].SetActive(false);

            bool namesMatch = player.Name == Game.PlayerName;

            if (namesMatch)
            {
                _playerViews[player.Name].SetActive(true);
            }
            else
            {
                var currPos = Game.Position;
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
