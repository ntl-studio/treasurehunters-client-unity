using TreasureHunters;
using UnityEngine;
using UnityEngine.UI;

public class ShowTreasure : MonoBehaviour
{
    private Toggle _toggle;
    void Start()
    {
        _toggle = gameObject.GetComponent<Toggle>();
        Debug.Assert(_toggle);
    }

    private static Game _game => Game.Instance();

    public void Show()
    {
        _game.ShowTreasure(_toggle.isOn);
    }
}
