using NtlStudio.TreasureHunters.Common;
using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    public Toggle MoveToggle;
    public Toggle GrenadeToggle;
    public Toggle GunToggle;
    public Button SkipButton;

    private ClientActionType _clientActionTypeUI;

    void Start()
    {
        Debug.Assert(MoveToggle);
        MoveToggle.onValueChanged.AddListener(MoveToggleChanged);

        Debug.Assert(GunToggle);
        GunToggle.onValueChanged.AddListener(GunToggleChanged);

        Debug.Assert(GrenadeToggle);
        GrenadeToggle.onValueChanged.AddListener(GrenadeToggleChanged);

        Debug.Assert(SkipButton);
        SkipButton.onClick.AddListener(SkipAction);

        Game.OnClientActionTypeChanged += (clientActionType) =>
        {
            if (clientActionType != _clientActionTypeUI)
            {
                switch (clientActionType)
                {
                    case ClientActionType.Move:
                        MoveToggle.isOn = true;
                        break;
                    case ClientActionType.Gun:
                        GunToggle.isOn = true;
                        break;
                    case ClientActionType.Grenade:
                        GrenadeToggle.isOn = true;
                        break;
                }
            }

            _clientActionTypeUI = clientActionType;
        };
    }

    public void MoveToggleChanged(bool value)
    {
        if (value)
            Game.ClientActionType = ClientActionType.Move;
    }

    public void GunToggleChanged(bool value)
    {
        if (value)
            Game.ClientActionType = ClientActionType.Gun;
    }

    public void GrenadeToggleChanged(bool value)
    {
        if (value)
            Game.ClientActionType = ClientActionType.Grenade;
    }
    
    public void SkipAction()
    {
        Debug.Log("Skip turn");
        Game.PerformAction(new PlayerAction { Type = ActionType.Skip });
    }
}
