using System;
using NtlStudio.TreasureHunters.Model;
using UnityEngine;

using SM = NtlStudio.TreasureHunters.Model;

namespace TreasureHunters
{
    public static class GameUtils
    {
        public static Vector2Int ActionToVector2(PlayerAction action)
        {
            var result = new Vector2Int(0, 0);

            switch (action.Direction)
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
                    throw new Exception($"Action direction not supported {action.Direction}");
            }

            return result;
        }

        public static void UpdateRotation(PlayerAction playerAction, Transform transform)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            switch (playerAction.Direction)
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
