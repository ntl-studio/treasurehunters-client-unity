using System.Collections.Generic;

namespace TreasureHunters
{
    public abstract class BaseBoard
    {

        public List<string[]> BoardStrings { get; } = new();

        public bool Valid(Position pos)
        {
            return pos.X > 0 && pos.Y > 0 && 
                   pos.X < RealWidth && 
                   pos.Y < RealHeight;
        }


        public bool IsWall(int x, int y)
        {
            return BoardStrings[y][x] == "w";
        }

        public bool IsPlayer(int x, int y)
        {
            return BoardStrings[y][x] == "P";
        }

        public virtual int RealWidth => -1;
        public virtual int RealHeight => -1;
    }
}