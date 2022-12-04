using System.Diagnostics;
using UnityEditor.SceneManagement;
using Debug = UnityEngine.Debug;

namespace TreasureHunters
{
    public class Player
    {
        public enum EMoveDirection
        {
            Right,
            Down,
            Left,
            Up
        }

        public EMoveDirection MoveDirection;

        private Position _position = new Position(-1, -1);
        public Position Position
        {
            set
            {
                if (value != _position)
                {
                    if (value.X > _position.X) 
                        MoveDirection = EMoveDirection.Right;
                    else if (value.X < _position.X) 
                        MoveDirection = EMoveDirection.Left;
                    else if (value.Y > _position.Y) 
                        MoveDirection = EMoveDirection.Up;
                    else if (value.Y < _position.Y) 
                        MoveDirection = EMoveDirection.Down;
                    else 
                        Debug.Assert(false, "Invalid position value");

                    _position = value;
                }
            }
            get => _position;
        }

        public string Name;
        public bool IsArmor;
        public bool IsTreasure;
        public int Grenades;
        public int Bullets;

        private Board _board;
        public Board Board
        {
            set
            {
                _board = value;
                _board.Player = this;
            }
            get => _board;
        }
    }
}