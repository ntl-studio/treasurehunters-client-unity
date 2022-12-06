using System;
using NtlStudio.TreasureHunters.Core;

namespace NtlStudio.TreasureHunters.Model
{
    public class PlayerVisibleArea : ValueObject<PlayerVisibleArea>
    {
        private readonly Position _p;
        private readonly GameField _field;

        public PlayerVisibleArea(Position p, GameField field)
        {
            _p = p;
            _field = field;
        }

        public Position Position => _p;

        public VisibleArea VisibleArea => _field.GetVisibleArea(_p);

        protected override bool EqualsValueObject(PlayerVisibleArea other)
        {
            return Position == other.Position && _field == other._field;
        }

        protected override int GetValueObjectHashCode()
        {
            return HashCode.Combine(Position.GetHashCode(), _field.GetHashCode());
        }
    }
}