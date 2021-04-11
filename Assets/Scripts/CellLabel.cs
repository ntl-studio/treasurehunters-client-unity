using UnityEngine;

public class CellLabel : MonoBehaviour
{
    public GameObject _labelText;

    void Start()
    {
        Debug.Assert(_labelText);
        _labelText.GetComponent<MeshRenderer>().sortingLayerName = "DebugLabels";
    }

    public void UpdateCellLabel(Vector2Int worldPosition)
    {
        _labelText.GetComponent<TextMesh>().text = worldPosition.x.ToString() + worldPosition.y.ToString();
    }
}