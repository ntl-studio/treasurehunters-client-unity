using System;
using System.Linq;
using UnityEngine.EventSystems;

using SM = NtlStudio.TreasureHunters.Model;

namespace TreasureHunters
{
    public class PlayerActionState : BaseBoard
    {
        public override int RealWidth => GameSettings.WindowRealWidth;
        public override int RealHeight => GameSettings.WindowRealHeight;

        public Player.EMoveDirection MoveDirection;

        public PlayerActionState(Position position, SM.VisibleArea visibleArea, Player.EMoveDirection moveDirection)
        {
            // TODO respect edge cases, like (x, y) is at the edge of the board

            for (int row = position.Y - 1; row <= position.Y + 1; ++row)
            {
                // var fullLine = board.BoardStrings[row];
                // var line = fullLine.Skip(position.X - 1).Take(3).ToArray();
                // BoardStrings.Add(line);
            }

            MoveDirection = moveDirection;
        }

        public bool IsRightWall => IsWall(2, 1);
        public bool IsDownWall => IsWall(1, 0);
        public bool IsLeftWall => IsWall(0, 1);
        public bool IsUpWall => IsWall(1, 2);
    }
}