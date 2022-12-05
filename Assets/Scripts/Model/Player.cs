using System.Drawing;
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

        public Player()
        {
            PrevPosition = Position;
        }

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

                    PrevPosition = _position;
                    _position = value;
                }
            }
            get => _position;
        }

        public Position PrevPosition { private set; get; }

        public string Name;
        public UnityEngine.Color Color = UnityEngine.Color.white;
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