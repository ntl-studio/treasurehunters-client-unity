using TreasureHunters;
using UnityEngine;

public class PlayerActionsWindow : MonoBehaviour
{
    public GameObject ActionsWindow;
    public PlayerMovement PlayerMovement;

    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Debug.Assert(ActionsWindow);
        Debug.Assert(PlayerMovement);

        ActionsWindow.SetActive(false);

        Game.OnPlayerClicked += () => ActionsWindow.SetActive(true);
    }
}
