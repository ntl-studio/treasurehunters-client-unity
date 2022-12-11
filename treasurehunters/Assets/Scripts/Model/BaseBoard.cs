using System.Collections.Generic;

namespace TreasureHunters
{
    public abstract class BaseBoard
    {

        public List<string[]> BoardStrings { get; } = new();

        public virtual int RealWidth => -1;
        public virtual int RealHeight => -1;
    }
}