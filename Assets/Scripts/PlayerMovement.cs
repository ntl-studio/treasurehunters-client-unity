using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var playerPos = transform.position;
            var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (math.abs(clickPos.x - playerPos.x) < 1.5f &&
                math.abs(clickPos.y - playerPos.y) < 1.5f)
            {
                transform.position = new Vector3(math.round(clickPos.x), math.round(clickPos.y), 0);
            }
        }
    }
}