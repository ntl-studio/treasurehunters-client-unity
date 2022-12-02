using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TreasureHunters
{
    public struct Cell
    {
        public bool LeftWall;
        public bool RightWall;
        public bool UpWall;
        public bool DownWall;

        // other stuff
    }

    public struct Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Position lhs, Position rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;

        public static bool operator !=(Position lhs, Position rhs) => !(lhs == rhs);
    }

    public class Player
    {
        public Position Position;
        public bool IsArmor;
        public bool IsTreasure;
        public int Grenades;
        public int Bullets;
    }

    public static class BoardSettings
    {
        public const int BoardWidth = 10;
        public const int BoardHeight = 10;
        public const int BoardRealWidth = BoardWidth * 2 + 1;
        public const int BoardRealHeight = BoardHeight * 2 + 1;
    }

    public class Board
    {
        private Player _player;
        public Board(Player player)
        {
            _player = player;
        }

        private List<string[]> _board = new();

        public void Init()
        {
            using var sr = new StreamReader(Path.Combine(Application.dataPath, "Level//level01.txt"));
            var fileContents = sr.ReadToEnd();
            var lines = GameUtils.ReadLevelFromTextFile(fileContents);
            foreach (var line in lines)
            {
                var characters = line.Split(' ');
                _board.Add(characters);
            }
        }

        public bool Valid(Position pos)
        {
            return pos.X > 0 && pos.Y > 0 && 
                   pos.X < BoardSettings.BoardRealWidth && 
                   pos.Y < BoardSettings.BoardRealHeight;
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
            var pos = _player.Position;
            return (Math.Abs(pos.X - x) + Math.Abs(pos.Y - y)) < 5;
        }

        public bool IsPlayer(int x, int y)
        {
            return _board[y][x] == "P";
        }

        public int Width => BoardSettings.BoardRealWidth;
        public int Height => BoardSettings.BoardRealHeight;
    }

    public enum EActionDirection
    {
        Left, Right, Up, Down, None
    }

    public enum EActionType
    {
        Move, Grenade, Bullet, Skip
    }

    public struct PlayerAction
    {
        public EActionDirection Direction;
        public EActionType Type;
    }

    public class Game
    {
        private GameState GameState { get; set; } = new();

        public Board Board => GameState.Board;

        public Player Player => GameState.Player;

        public Game()
        {
            Board.Init();
            GameState.Player.Position = GameUtils.FindPlayerPosition(Board);
        }

        GameState GetInitialGameState()
        {
            return new GameState();
        }

        void UpdateGameState(PlayerAction playerAction)
        {
        }
    }
}
