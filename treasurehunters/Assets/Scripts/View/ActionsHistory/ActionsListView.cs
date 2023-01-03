using System.Collections.Generic;
using TMPro;
using UnityEngine;

using SM = NtlStudio.TreasureHunters.Model;

public class ActionsListView : MonoBehaviour
{
    public TextMeshProUGUI PlayerNameText;
    public GameObject ActionStateViewPrefab; 
    public Transform ActionsParent;

    void Start()
    {
        Debug.Assert(ActionStateViewPrefab);
        Debug.Assert(ActionsParent);
        Debug.Assert(PlayerNameText);

        for (int i = 0; i < SM.Game.ActionsHistorySize; ++i)
        {
            var actionStateViewObj = Instantiate(ActionStateViewPrefab, ActionsParent, false);

            actionStateViewObj.SetActive(false);

            _actionsList.Add(actionStateViewObj);
        }
    }

    public void SetPlayerName(string playerName)
    {
        PlayerNameText.text = playerName;
    }

    private Vector3 _lastActionPosition;
    private readonly List<GameObject> _actionsList = new();

    public void UpdatePlayerActionStates(List<SM.PlayerActionState> playerActions)
    {
        for (int i = 0; i < playerActions.Count; ++i)
        {
            _actionsList[i].SetActive(true);

            var stateVisibility = _actionsList[i].GetComponent<PlayerActionStateView>();
            Debug.Assert(stateVisibility);
            stateVisibility.SetWallsVisibility(playerActions[i]);
        }
    }
}
