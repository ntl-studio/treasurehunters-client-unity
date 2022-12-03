using System.Collections.Generic;
using TreasureHunters;
using UnityEngine;
using VContainer;

public class ActionsHistory : MonoBehaviour
{
    public GameObject ActionsListViewPrefab; 

    private readonly Dictionary<string, ActionsListView> _actionViews= new();

    [Inject]
    void InjectGame(Game game)
    {
        _game = game;
    }
    private Game _game;

    void Start()
    {
        Debug.Assert(ActionsListViewPrefab);

        _game.OnBeforeEndTurn += () =>
        {
            var player = _game.CurrentPlayer;
            var pos = player.Position;
            var playerState = new PlayerActionState(pos.X, pos.Y, _game.CurrentBoard);
            AddActionState(player.Name, playerState);
        };

        Vector3 lastListPosition = transform.position;

        for (int i = 0; i < _game.PlayersCount; i++)
        {
            var actionsListObj = Instantiate(ActionsListViewPrefab, transform, true);
            actionsListObj.transform.SetPositionAndRotation(lastListPosition, Quaternion.identity);
            lastListPosition.y -= 100.0f;

            var playerName = _game.Players[i].Name;
            var actionsList = actionsListObj.GetComponent<ActionsListView>();
            Debug.Assert(actionsList);
            actionsList.SetPlayerName(playerName);

            _actionViews.Add(playerName, actionsList);
        }
    }

    void AddActionState(string playerName, PlayerActionState actionState)
    {
        if (_actionViews.ContainsKey(playerName))
        {
            var actionsView = _actionViews[playerName];
            actionsView.AddPlayerActionState(actionState);
        }
        else
            Debug.Assert(false, $"Could not find a player {playerName}");
    }
}
