using TreasureHunters;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandleClick : MonoBehaviour, IPointerClickHandler
{
    private RectTransform _rect;
    private Camera _localCamera;
    private Vector2 _localPosition;
    private static GameClient Game => GameClient.Instance();

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        Debug.Assert(_rect);

        GameObject cameraObj = GameObject.FindGameObjectWithTag("PlayerCamera");
        Debug.Assert(cameraObj);

        _localCamera = cameraObj.GetComponent<Camera>();
        Debug.Assert(_localCamera != null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var clickPosition = eventData.position;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, clickPosition, eventData.pressEventCamera, out _localPosition))
        {
            _localPosition.x += _rect.rect.width * .5f;
            _localPosition.y += _rect.rect.height * .5f;

            var worldPosition = _localCamera.ScreenToWorldPoint(_localPosition);
            Game.GameViewClick(worldPosition);
        }
    }
}
