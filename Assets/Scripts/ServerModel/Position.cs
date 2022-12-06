namespace NtlStudio.TreasureHunters.Model
{
    public struct Position
    {
        public int X;
        public int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return ((Position)obj!).X == X && ((Position)obj).Y == Y;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }

        public static bool operator ==(Position lhs, Position rhs) => lhs.X == rhs.X && lhs.Y == rhs.Y;
        public static bool operator !=(Position lhs, Position rhs) => !(lhs == rhs);
    }
}