using System;
using NtlStudio.TreasureHunters.Common;
using UnityEngine;

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
    }
}
