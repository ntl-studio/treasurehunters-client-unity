using TreasureHunters;
using UnityEngine;

public class PlayerItems : MonoBehaviour
{
    public GameObject TreasureImage;

    private static Game _game => Game.Instance();

    void Start()
    {
        Debug.Assert(TreasureImage);
        TreasureImage.SetActive(false);
        _game.OnStartTurn += () => TreasureImage.SetActive(_game.CurrentPlayerHasTreasure);
        _game.OnEndMove+= () => TreasureImage.SetActive(_game.CurrentPlayerHasTreasure);
    }
}
