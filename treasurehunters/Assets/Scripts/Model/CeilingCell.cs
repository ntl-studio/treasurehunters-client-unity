using UnityEngine;

public enum CeilingState
{
    Hidden,
    Fog,
    Visible
}

public class CeilingCell : MonoBehaviour
{
    private CeilingState _state;
    public CeilingState State
    {
        set
        {
            switch (value)
            {
                case CeilingState.Hidden:
                    SetAlpha(1.0f);
                    gameObject.SetActive(true);
                    break;
                case CeilingState.Fog:
                    SetAlpha(0.5f);
                    gameObject.SetActive(true);
                    break;
                case CeilingState.Visible:
                    gameObject.SetActive(false);
                    break;
            }

            _state = value;
        }
        get => _state;
    }

    private void SetAlpha(float alpha)
    {
        var sprite = gameObject.GetComponent<SpriteRenderer>();
        var color = sprite.color;
        color.a = alpha;
        sprite.color = color;
    }
}
