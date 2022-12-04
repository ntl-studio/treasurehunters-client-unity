using TreasureHunters;
using UnityEngine;
using VContainer;

public class NextTurn : MonoBehaviour
{
    public GameObject NextTurnPanel;

    [Inject]
    void InjectGame(Game game)
    {
        _game = game;
    }
    private Game _game;

    void Start()
    {
        Debug.Assert(NextTurnPanel);

        NextTurnPanel.SetActive(false);
        _game.OnEndTurn += () => NextTurnPanel.SetActive(true);
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
