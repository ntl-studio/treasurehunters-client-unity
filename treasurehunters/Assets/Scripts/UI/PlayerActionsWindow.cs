using TreasureHunters;
using UnityEngine;

public class PlayerActionsWindow : MonoBehaviour
{
    public GameObject ActionsWindow;

    private static Game Game => Game.Instance();

    void Start()
    {
        Debug.Assert(Game != null);
        Debug.Assert(ActionsWindow);

        ActionsWindow.SetActive(false);

        Game.OnPlayerClicked += () => ActionsWindow.SetActive(true);
    }
}
