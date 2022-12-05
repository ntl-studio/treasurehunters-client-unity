using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreasureHunters
{
    public static class GameUtils
    {
        public static IList<string> ReadLevelFromTextFile(string levelText)
        {
            var rawLines = new List<string>();
            rawLines.AddRange(levelText.Split("\n"[0]));

            var cleanLines = new List<string>();

            foreach (var line in rawLines)
            {
                var commentPos = line.IndexOf("//", StringComparison.Ordinal);

                string newLine = line;
                if (commentPos >= 0)
                    newLine = newLine.Substring(0, commentPos);
                newLine = newLine.Trim();

                if (newLine.Length > 0)
                    cleanLines.Add(newLine);
            }

            cleanLines.Reverse();

            return cleanLines;
        }

        public static Position FindPlayerPosition(Board board)
        {
            for (var row = 0; row < board.RealWidth; ++row)
            {
                for (var col = 0; col < board.RealHeight; ++col)
                {
                    if (board.IsPlayer(col, row))
                    {
                        return new Position(col, row);
                    }
                }
            }

            Debug.Assert(false, "Did not find player position on the board");
            return new Position(-1, -1);
        }

        public static void UpdateRotation(Player.EMoveDirection moveDirection, Transform transform)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            switch (moveDirection)
            {
                case Player.EMoveDirection.Right:
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                    break;
                case Player.EMoveDirection.Down:
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case Player.EMoveDirection.Left:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Player.EMoveDirection.Up:
                    break;
            }
        }
    }
}
