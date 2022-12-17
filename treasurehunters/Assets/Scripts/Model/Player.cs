namespace TreasureHunters
{
    public class Player
    {
        private readonly GameClient _game;
        private readonly int _playerIndex;

        public Player(GameClient game, int playerIndex)
        {
            _game = game;
            _playerIndex = playerIndex;
        }

        public Position PreviousPosition = new Position(-1, -1);

        public Position Position => _game.PlayerPosition(_playerIndex);

        public string Name => _game.PlayerName;

        public UnityEngine.Color Color = UnityEngine.Color.white;
    }
}