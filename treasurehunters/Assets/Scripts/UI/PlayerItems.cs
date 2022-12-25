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
        Game.OnUpdatePlayerHasTreasure += hasTreasure => TreasureImage.SetActive(hasTreasure);
    }
}
