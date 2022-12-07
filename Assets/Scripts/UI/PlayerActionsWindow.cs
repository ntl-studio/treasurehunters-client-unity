using TreasureHunters;
using UnityEngine;
using VContainer;

public class PlayerActionsWindow : MonoBehaviour
{
    public GameObject ActionsWindow;

    [Inject] private void InjectGame(Game game) { _game = game; }
    private Game _game;

    void Start()
    {
        Debug.Assert(_game != null);
        Debug.Assert(ActionsWindow);

        ActionsWindow.SetActive(false);

        _game.OnPlayerClicked += () => ActionsWindow.SetActive(true);
    }
}
