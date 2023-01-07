using System;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine;

using NtlStudio.TreasureHunters.Model;
using SM = NtlStudio.TreasureHunters.Model;

namespace TreasureHunters
{
    public static class GameUtils
    {
        public static Vector2Int ActionDirectionToVector2(ActionDirection direction)
        {
            var result = new Vector2Int(0, 0);

            switch (direction)
            {
                case ActionDirection.Right:
                    result.x += 1;
                    break;
                case ActionDirection.Down:
                    result.y -= 1;
                    break;
                case ActionDirection.Left:
                    result.x -= 1;
                    break;
                case ActionDirection.Up:
                    result.y += 1;
                    break;
                default:
                    throw new Exception($"Action direction not supported {direction}");
            }

            return result;
        }

        public static void UpdateRotation(ActionDirection direction, Transform transform)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            switch (direction)
            {
                case ActionDirection.Right:
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    break;
                case ActionDirection.Down:
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case ActionDirection.Left:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case ActionDirection.Up:
                    break;
            }
        }

        public static ActionDirection GetActionDirection(Vector3 playerPosition)
        {
            ActionDirection direction = ActionDirection.None;

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Debug.Log($"Mouse position: {Input.mousePosition}, click position {clickPos}, player position {playerPosition}");

                var shiftX = (int)math.round(clickPos.x - playerPosition.x);
                var shiftY = (int)math.round(clickPos.y - playerPosition.y);

                if ((math.abs(shiftX) == 1) ^ (math.abs(shiftY) == 1))
                {
                    if (math.abs(shiftX) > math.abs(shiftY))
                        direction = shiftX > 0 ? ActionDirection.Right : ActionDirection.Left;
                    else
                        direction = shiftY > 0 ? ActionDirection.Up : ActionDirection.Down;
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                direction = ActionDirection.Left;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                direction = ActionDirection.Up;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                direction = ActionDirection.Right;
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                direction = ActionDirection.Down;

            return direction;
        }
    }
}
