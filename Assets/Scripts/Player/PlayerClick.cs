using UnityEngine;

public class PlayerClick : MonoBehaviour
{
    [SerializeField]
    GameObject _playerActionsWindow; // TODO move window to DI constainer

    private void OnMouseDown()
    {
        Debug.Log("Mouse down");
        _playerActionsWindow.SetActive(true);
    }
}
