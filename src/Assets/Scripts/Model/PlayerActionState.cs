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

        private SM.FieldCell _cell;

        public PlayerActionState(Position position, SM.FieldCell cell, Player.EMoveDirection moveDirection)
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

        public bool IsRightWall => _cell.HasFlag(SM.FieldCell.RightWall);
        public bool IsDownWall => _cell.HasFlag(SM.FieldCell.BottomWall);
        public bool IsLeftWall => _cell.HasFlag(SM.FieldCell.LeftWall);
        public bool IsUpWall => _cell.HasFlag(SM.FieldCell.TopWall);
    }
}