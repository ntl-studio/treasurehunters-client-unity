namespace TreasureHunters
{
    public class Player
    {
        public Position Position;
        public bool IsArmor;
        public bool IsTreasure;
        public int Grenades;
        public int Bullets;
        public Board Board;

        public Player()
        {
            Board = new Board(this);
        }
    }
}