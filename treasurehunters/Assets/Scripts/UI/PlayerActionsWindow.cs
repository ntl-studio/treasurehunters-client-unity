using TreasureHunters;
using UnityEngine;

public class PlayerActionsWindow : MonoBehaviour
{
    public GameObject ActionsWindow;

    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Debug.Assert(ActionsWindow);

        ActionsWindow.SetActive(false);

        Game.OnPlayerClicked += () => ActionsWindow.SetActive(true);
    }
}
