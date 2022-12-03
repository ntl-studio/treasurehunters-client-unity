namespace TreasureHunters
{
    public class PlayerActionState : BaseBoard
    {
        public override int RealWidth => GameSettings.WindowRealWidth;
        public override int RealHeight => GameSettings.WindowRealHeight;

        public PlayerActionState(int x, int y, Board board)
        {
            // TODO respect edge cases, like (x, y) is at the edge of the board

            for (int i = y - RealHeight / 2; i < y + RealHeight / 2; ++i)
            {
                var s = board.BoardStrings[i];
                BoardStrings.Add(s);
            }
        }

        public bool IsRightWall => IsWall(2, 1);
        public bool IsDownWall => IsWall(1, 0);
        public bool IsLeftWall => IsWall(0, 1);
        public bool IsUpWall => IsWall(1, 2);
    }
}