using NtlStudio.TreasureHunters.Model;
using System.Collections.Generic;
using TreasureHunters;
using UnityEditor;
using UnityEngine;

using SM = NtlStudio.TreasureHunters.Model;

public class PlayerMovesDetails
{
    public string PlayerName;
    public List<PlayerMoveState> Moves = new();
}

public class ActionsHistory : MonoBehaviour
{
    public GameObject ActionsListViewPrefab; 

    private readonly List<ActionsListView> _actionViews = new();

    private static GameClient Game => GameClient.Instance();

    private bool _isInitialized;

    void Start()
    {
        Debug.Assert(ActionsListViewPrefab);

        Vector3 lastListPosition = transform.position;

        Game.OnUpdatePlayersMoveHistory += UpdateStates;

        Game.OnWaitingForTurn += () =>
        {
            if (_isInitialized) 
                return;

            for (int i = 0; i < Game.PlayersCount; i++)
            {
                var actionsListObj = Instantiate(ActionsListViewPrefab, transform, true);
                actionsListObj.transform.SetPositionAndRotation(lastListPosition, Quaternion.identity);
                lastListPosition.y -= 120.0f;

                var actionsList = actionsListObj.GetComponent<ActionsListView>();
                Debug.Assert(actionsList);

                _actionViews.Add(actionsList);
            }

            _isInitialized = true;
        };
    }

    void UpdateStates()
    {
        Debug.Assert(_actionViews.Count == Game.PlayersMovesHistory.Count);

        for (int i = 0; i < Game.PlayersMovesHistory.Count; ++i)
        {
            var states = Game.PlayersMovesHistory[i];
            _actionViews[i].SetPlayerName(states.PlayerName);
            _actionViews[i].UpdatePlayerActionStates(states.Moves);
        }
    }
}
