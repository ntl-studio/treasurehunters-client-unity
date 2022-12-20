using TreasureHunters;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public GameObject TreasureImage;

    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        Debug.Assert(TreasureImage);
        TreasureImage.SetActive(false);
        Game.OnYourTurn += () => TreasureImage.SetActive(Game.CurrentPlayerHasTreasure);
        Game.OnEndMove+= () => TreasureImage.SetActive(Game.CurrentPlayerHasTreasure);
    }
}
