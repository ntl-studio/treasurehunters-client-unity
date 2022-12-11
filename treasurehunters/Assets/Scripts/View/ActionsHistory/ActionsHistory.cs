using System.Collections.Generic;
using TreasureHunters;
using UnityEngine;
using VContainer;

using SM = NtlStudio.TreasureHunters.Model;

public class ActionsHistory : MonoBehaviour
{
    public GameObject ActionsListViewPrefab; 

    private readonly Dictionary<string, ActionsListView> _actionViews= new();

    private static Game _game => Game.Instance();

    void Start()
    {
        Debug.Assert(ActionsListViewPrefab);

        _game.OnEndTurn += () =>
        {
            var player = _game.CurrentPlayer;
            UpdateStates(player.Name, _game.CurrentPlayerMoveStates);
        };

        Vector3 lastListPosition = transform.position;

        for (int i = 0; i < _game.PlayersCount; i++)
        {
            var actionsListObj = Instantiate(ActionsListViewPrefab, transform, true);
            actionsListObj.transform.SetPositionAndRotation(lastListPosition, Quaternion.identity);
            lastListPosition.y -= 120.0f;

            var playerName = _game.Players[i].Name;
            var actionsList = actionsListObj.GetComponent<ActionsListView>();
            Debug.Assert(actionsList);
            actionsList.SetPlayerName(playerName);

            _actionViews.Add(playerName, actionsList);
        }
    }

    void UpdateStates(string playerName, List<SM.PlayerMoveState> actionStates)
    {
        if (_actionViews.ContainsKey(playerName))
        {
            var actionsView = _actionViews[playerName];
            actionsView.UpdatePlayerActionStates(actionStates);
        }
        else
            Debug.Assert(false, $"Could not find a player {playerName}");
    }
}
