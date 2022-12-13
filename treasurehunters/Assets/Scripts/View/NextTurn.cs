using TreasureHunters;
using UnityEngine;
using VContainer;

public class NextTurn : MonoBehaviour
{
    public GameObject NextTurnPanel;

    private static Game _game => Game.Instance();

    void Start()
    {
        Debug.Assert(NextTurnPanel);
        Debug.Assert(_game != null);

        NextTurnPanel.SetActive(false);
        _game.OnEndMove += () => NextTurnPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextTurnPanel.SetActive(false);
            _game.StartNextTurn();
        }
    }
}
