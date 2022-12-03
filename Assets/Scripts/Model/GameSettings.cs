namespace TreasureHunters
{
    public static class GameSettings
    {
        private const int BoardWidth = 10;
        private const int BoardHeight = 10;
        public const int BoardRealWidth = BoardWidth * 2 + 1;
        public const int BoardRealHeight = BoardHeight * 2 + 1;

        private const int WindowWidth = 3;
        private const int WindowHeight = 3;
        public const int WindowRealWidth = WindowWidth * 2 + 1;
        public const int WindowRealHeight = WindowHeight * 2 + 1;

        public const int PlayersCount = 2;
    }
}