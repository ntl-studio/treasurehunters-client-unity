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

        _lastActionPosition = transform.position;
    }

    public void SetPlayerName(string playerName)
    {
        PlayerNameText.text = playerName;
    }

    private Vector3 _lastActionPosition;
    private List<GameObject> _actionsList = new();

    public void AddPlayerActionState(PlayerActionState playerStater)
    {
        var actionStateViewObj = Instantiate(ActionStateViewPrefab, transform, false);
        actionStateViewObj.transform.SetPositionAndRotation(_lastActionPosition, Quaternion.identity);
        _lastActionPosition.x += 130;

        var actionView = actionStateViewObj.GetComponent<PlayerActionStateView>();
        Debug.Assert(actionView);
        actionView.SetWallsVisibility(playerStater);

        _actionsList.Add(actionStateViewObj);
    }
}
