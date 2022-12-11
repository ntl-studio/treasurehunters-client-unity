using TreasureHunters;
using UnityEngine;

public class PlayerActionsWindow : MonoBehaviour
{
    public GameObject ActionsWindow;

    private static Game _game => Game.Instance();

    void Start()
    {
        Debug.Assert(_game != null);
        Debug.Assert(ActionsWindow);

        ActionsWindow.SetActive(false);

        _game.OnPlayerClicked += () => ActionsWindow.SetActive(true);
    }
}
