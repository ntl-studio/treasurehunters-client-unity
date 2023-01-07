using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

public class ShowTreasure : MonoBehaviour
{
    private Toggle _toggle;
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        _toggle = gameObject.GetComponent<Toggle>();
        Debug.Assert(_toggle);
    }

    public void Show()
    {
        Game.ShowTreasure(_toggle.isOn);
    }
}
