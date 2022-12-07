using System;
using System.IO;
using UnityEngine;

namespace TreasureHunters
{
    public class Board : BaseBoard
    {
        public Player Player;
        public Board(string[] level)
        {
            foreach (var line in level)
            {
                var characters = line.Split(' ');
                BoardStrings.Add(characters);
            }
        }

        // For now just check the Manhattan distance between the player and the cell
        public bool IsCellVisible(int x, int y)
        {
            var pos = Player.Position;
            return (Math.Abs(pos.X - x) <= 2) && (Math.Abs(pos.Y - y) <= 2);
        }

        public override int RealWidth => GameSettings.BoardRealWidth;
        public override int RealHeight => GameSettings.BoardRealHeight;
    }
}
