using TreasureHunters;
using UnityEngine;

public class NextTurn : MonoBehaviour
{
    public GameObject NextTurnPanel;

    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Debug.Assert(NextTurnPanel);

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
