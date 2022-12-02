using System;

namespace TreasureHunters
{
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
}