namespace TreasureHunters
{
    public class Game
    {
        private GameState GameState { get; set; } = new();

        public Board Board => GameState.Player.Board;

        public Player Player => GameState.Player;

        public Game()
        {
            Board.Init();
            GameState.Player.Position = GameUtils.FindPlayerPosition(Board);
        }

        GameState GetInitialGameState()
        {
            return new GameState();
        }

        void UpdateGameState(PlayerAction playerAction)
        {
        }
    }

    public enum EActionDirection
    {
        Left, Right, Up, Down, None
    }

    public enum EActionType
    {
        Move, Grenade, Bullet, Skip
    }

    public struct PlayerAction
    {
        public EActionDirection Direction;
        public EActionType Type;
    }
}