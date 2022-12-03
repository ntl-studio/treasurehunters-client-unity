using System;
using System.IO;
using UnityEngine;

namespace TreasureHunters
{
    public class Board : BaseBoard
    {
        public Player Player;
        public Board(string filePath)
        {
            using var sr = new StreamReader(Path.Combine(Application.dataPath, filePath));
            var fileContents = sr.ReadToEnd();
            var lines = GameUtils.ReadLevelFromTextFile(fileContents);
            foreach (var line in lines)
            {
                var characters = line.Split(' ');
                BoardStrings.Add(characters);
            }
        }

        // For now just check the Manhattan distance between the player and the cell
        public bool IsCellVisible(int x, int y)
        {
            var pos = Player.Position;
            return (Math.Abs(pos.X - x) + Math.Abs(pos.Y - y)) < 5;
        }

        public override int RealWidth => GameSettings.BoardRealWidth;
        public override int RealHeight => GameSettings.BoardRealHeight;
    }
}
