namespace TreasureHunters
{
    public class Player
    {
        private readonly Game _game;
        private readonly int _playerIndex;
        public Player(Game game, int playerIndex)
        {
            _game = game;
            _playerIndex = playerIndex;
        }

        public Position Position => _game.PlayerPosition(_playerIndex);

        public string Name => _game.PlayerName(_playerIndex);

        public UnityEngine.Color Color = UnityEngine.Color.white;
    }
}