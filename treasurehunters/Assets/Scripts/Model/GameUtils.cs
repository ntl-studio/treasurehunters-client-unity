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

            switch (action)
            {
                case PlayerAction.MoveRight:
                    result.x += 1;
                    break;
                case PlayerAction.MoveDown:
                    result.y -= 1;
                    break;
                case PlayerAction.MoveLeft:
                    result.x -= 1;
                    break;
                case PlayerAction.MoveUp:
                    result.y += 1;
                    break;
                case PlayerAction.SkipTurn:
                    break;
                case PlayerAction.ThrowGrenade:
                case PlayerAction.FireGun:
                    throw new Exception("Action not supported");
            }

            return result;
        }

        public static void UpdateRotation(MoveDirection moveDirection, Transform transform)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            switch (moveDirection)
            {
                case MoveDirection.Right:
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    break;
                case MoveDirection.Down:
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case MoveDirection.Left:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case MoveDirection.Up:
                    break;
            }
        }
    }
}
