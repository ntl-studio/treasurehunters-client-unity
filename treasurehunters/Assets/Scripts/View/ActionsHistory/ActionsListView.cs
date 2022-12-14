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

        var pos = transform.position;
        for (int i = 0; i < SM.Game.ActionsHistorySize; ++i)
        {
            var actionStateViewObj = Instantiate(ActionStateViewPrefab, transform, false);
            actionStateViewObj.SetActive(false);

            pos.x += 130;
            actionStateViewObj.transform.position = pos;

            _actionsList.Add(actionStateViewObj);
        }
    }

    public void SetPlayerName(string playerName)
    {
        PlayerNameText.text = playerName;
    }

    private Vector3 _lastActionPosition;
    private readonly List<GameObject> _actionsList = new();

    public void UpdatePlayerActionStates(List<SM.PlayerMoveState> playerStates)
    {
        for (int i = 0; i < playerStates.Count; ++i)
        {
            _actionsList[i].SetActive(true);

            var stateVisibility = _actionsList[i].GetComponent<PlayerActionStateView>();
            Debug.Assert(stateVisibility);
            stateVisibility.SetWallsVisibility(playerStates[i]);
        }
    }
}
