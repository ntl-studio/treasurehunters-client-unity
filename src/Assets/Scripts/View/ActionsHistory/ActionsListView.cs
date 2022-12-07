using System.Collections.Generic;
using TMPro;
using TreasureHunters;
using UnityEngine;

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
    }

    public void SetPlayerName(string playerName)
    {
        PlayerNameText.text = playerName;
    }

    private Vector3 _lastActionPosition;
    private readonly List<GameObject> _actionsList = new();

    public void AddPlayerActionState(PlayerActionState playerStater)
    {
        var actionStateViewObj = Instantiate(ActionStateViewPrefab, transform, false);

        var actionView = actionStateViewObj.GetComponent<PlayerActionStateView>();
        Debug.Assert(actionView);
        actionView.SetWallsVisibility(playerStater);

        if (_actionsList.Count > 3)
        {
            Destroy(_actionsList[0]);
            _actionsList.RemoveAt(0);
        }

        foreach (var action in _actionsList)
        {
            var pos = action.transform.position;
            pos.x += 130;
            action.transform.position = pos;
        }

        _actionsList.Add(actionStateViewObj);
    }
}
