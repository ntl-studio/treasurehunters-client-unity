using System.Collections;
using System.Collections.Generic;
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

    public void EnableFogIfVisible()
    {
        if (State == CeilingState.Visible)
        {
            State = CeilingState.Fog;
        }
    }

    private void SetAlpha(float alpha)
    {
        var sprite = gameObject.GetComponent<SpriteRenderer>();
        var color = sprite.color;
        color.a = alpha;
        sprite.color = color;
    }
}
