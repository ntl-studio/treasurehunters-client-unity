using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public GameObject PlayerPrefab;

    void Start()
    {
        Debug.Assert(PlayerPrefab);
        Instantiate(PlayerPrefab, transform);
    }
}
