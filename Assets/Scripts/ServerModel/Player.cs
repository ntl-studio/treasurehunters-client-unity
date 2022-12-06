using System;
using NtlStudio.TreasureHunters.Core;

namespace NtlStudio.TreasureHunters.Model
{
    public class Player : ValueObject<Player>
    {
        public Player(string name, Position position)
        {
            Name = name;
            Position = position;
        }

        public string Name { get; }

        public Position Position { get; }

        protected override bool EqualsValueObject(Player other)
        {
            return other.Name == Name && other.Position == Position;
        }

        protected override int GetValueObjectHashCode()
        {
            return HashCode.Combine(Name.GetHashCode(), Position.GetHashCode());
        }
    }
}