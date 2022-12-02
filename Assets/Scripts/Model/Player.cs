namespace TreasureHunters
{
    public class Player
    {
        public Position Position;
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