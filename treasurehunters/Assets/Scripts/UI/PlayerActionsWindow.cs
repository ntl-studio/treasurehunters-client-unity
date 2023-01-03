using TreasureHunters;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerActionsWindow : MonoBehaviour
{
    public GameObject ActionsWindow;

    private static GameClient Game => GameClient.Instance();
    private bool _acceptInput;

    void Start()
    {
        Debug.Assert(ActionsWindow);

        ActionsWindow.SetActive(false);

        Game.OnChoosePlayerAction += () =>
        {
            _acceptInput = true;
            _frameCount = 0;
            ActionsWindow.SetActive(true);
        };

        Game.OnStartFiringGun += () => ActionsWindow.SetActive(false);
    }

    private int _frameCount = 0;
    void Update()
    {
        if (_acceptInput)
        {
            if (_frameCount > 0)
            {
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    _acceptInput = false;
                    ActionsWindow.SetActive(false);
                    Game.ChoosePlayerActionCancel();
                }
            }

            _frameCount++;
        }
    }
}
