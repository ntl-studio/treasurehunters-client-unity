using System;
using System.Collections.Generic;
using TreasureHunters;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class Players : MonoBehaviour
{
    public GameObject PlayerPrefab;

    [Inject] void InjectGame(Game game) { _game = game; }
    private Game _game;

    [Inject] void InjectContainer(IObjectResolver container) { _container = container; }
    private IObjectResolver _container;

    private readonly Dictionary<string, GameObject> _playerViews = new();

    void Start()
    {
        Debug.Assert(PlayerPrefab);

        for (int i = 0; i < _game.PlayersCount; ++i)
        {
            var player = _game.Players[i];

            var playerObj = _container.Instantiate(PlayerPrefab, transform);
            playerObj.SetActive(false);

            var spriteRenderer = playerObj.GetComponent<SpriteRenderer>();
            Debug.Assert(spriteRenderer);
            spriteRenderer.color = player.Color;


            _playerViews.Add(player.Name, playerObj);
        }

        _game.OnStartTurn += UpdatePlayersVisibility;
        _game.OnEndTurn += UpdatePlayersVisibility;

        UpdatePlayersVisibility();
    }

    void UpdatePlayersVisibility()
    {
        var currentPlayer = _game.CurrentPlayer;

        foreach (var player in _game.Players)
        {
            bool namesMatch = player.Name == currentPlayer.Name;

            _playerViews[player.Name].SetActive(
                namesMatch ||
                   (Math.Abs(player.Position.X - currentPlayer.Position.X) <= 2 &&
                    Math.Abs(player.Position.Y - currentPlayer.Position.Y) <= 2));

            _playerViews[player.Name].GetComponent<PlayerMovement>().AcceptInput =
                namesMatch;
        }
    }
}
