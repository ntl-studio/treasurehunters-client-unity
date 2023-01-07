using NtlStudio.TreasureHunters.Model;
using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    private static GameClient Game => GameClient.Instance();

    public Button GrenadeButton;
    public Button GunButton;
    public Button SkipButton;

    void Start()
    {
        Debug.Assert(GrenadeButton);
        GrenadeButton.onClick.AddListener(GrenadeAction);

        Debug.Assert(GunButton);
        GunButton.onClick.AddListener(GunAction);

        Debug.Assert(SkipButton);
        SkipButton.onClick.AddListener(SkipAction);
    }

    public void GrenadeAction()
    {
        Debug.Log("Throw grenade");
    }

    public void GunAction()
    {
        Debug.Log("Shoot gun");
        Game.StartFiringGun();
    }
    
    public void SkipAction()
    {
        Debug.Log("Skip turn");
        Game.PerformAction(new PlayerAction { Type = ActionType.Skip });
    }
}
