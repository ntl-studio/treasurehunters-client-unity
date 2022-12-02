namespace TreasureHunters
{
    public class GameState
    {
        public Player Player = new();
        public Board Board;
        public int CurrentPlayer = 0;

        public GameState()
        {
            Board = new Board(Player);
        }
    }
}