using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TreasureHunters
{
    public class PlayerStateBoard : BaseBoard
    {
        // size of the board "window" that we need to snapshot
        const int WindowHeight = 3;
        const int WindowWidth = 3;

        public override int RealWidth => GameSettings.WindowRealWidth;
        public override int RealHeight => GameSettings.WindowRealHeight;

        PlayerStateBoard(int x, int y, Board board)
        {
            // TODO respect edge cases, like (x, y) is at the edge of the board

            for (int i = y - RealHeight / 2; i < y + RealHeight / 2; ++i)
            {
                var s = board.BoardStrings[i];
                BoardStrings.Add(s);
            }
        }
    }

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

    public abstract class BaseBoard
    {

        public List<string[]> BoardStrings { get; } = new();

        public bool Valid(Position pos)
        {
            return pos.X > 0 && pos.Y > 0 && 
                   pos.X < RealWidth && 
                   pos.Y < RealHeight;
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
            return BoardStrings[y][x] == "w";
        }

        public bool IsPlayer(int x, int y)
        {
            return BoardStrings[y][x] == "P";
        }

        public virtual int RealWidth => -1;
        public virtual int RealHeight => -1;
    }
}
