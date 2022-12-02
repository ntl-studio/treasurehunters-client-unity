using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TreasureHunters
{
    public class Board
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
                _board.Add(characters);
            }
        }

        private List<string[]> _board = new();

        public bool Valid(Position pos)
        {
            return pos.X > 0 && pos.Y > 0 && 
                   pos.X < GameSettings.BoardRealWidth && 
                   pos.Y < GameSettings.BoardRealHeight;
        }

        public static bool IsWallCell(int x, int y)
        {
            return (x % 2 == 0 && y % 2 == 1) ||
                   (x % 2 == 1 && y % 2 == 0);
        }

        public static bool IsFloorCell(int x, int y)
        {
            return (x % 2 == 1 && y % 2 == 1);
        }

        public static bool IsValidCell(int x, int y)
        {
            return !(x % 2 == 0 && y % 2 == 0);
        }

        public bool IsWall(int x, int y)
        {
            return _board[y][x] == "w";
        }

        // For now just check the Manhattan distance between the player and the cell
        public bool IsCellVisible(int x, int y)
        {
            var pos = Player.Position;
            return (Math.Abs(pos.X - x) + Math.Abs(pos.Y - y)) < 5;
        }

        public bool IsPlayer(int x, int y)
        {
            return _board[y][x] == "P";
        }

        public int Width => GameSettings.BoardRealWidth;
        public int Height => GameSettings.BoardRealHeight;
    }
}
