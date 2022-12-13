using TreasureHunters;
using UnityEngine;

public class NextTurn : MonoBehaviour
{
    public GameObject NextTurnPanel;

    private static Game Game => Game.Instance();

    void Start()
    {
        Debug.Assert(NextTurnPanel);
        Debug.Assert(Game != null);

        NextTurnPanel.SetActive(false);
        Game.OnEndMove += () => NextTurnPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            NextTurnPanel.SetActive(false);
            Game.StartNextTurn();
        }
    }
}
