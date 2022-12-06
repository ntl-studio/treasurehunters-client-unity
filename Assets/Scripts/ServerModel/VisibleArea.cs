using System;
using System.Collections;
using System.Collections.Generic;
using NtlStudio.TreasureHunters.Core;

namespace NtlStudio.TreasureHunters.Model
{
    public class VisibleArea : ValueObject<VisibleArea>, IEnumerable<IEnumerable<FieldCell>>, IField
    {
        public const int Width = 3;
        public const int Height = 3;

        private readonly FieldCell[,] _cells;

        public VisibleArea(FieldCell[,] cells)
        {
            if (cells.GetLength(0) != Width || cells.GetLength(1) != Height)
            {
                throw new ArgumentException("Invalid size of visible field");
            }

            _cells = cells;
        }

        protected override bool EqualsValueObject(VisibleArea other)
        {
            for (var i = 0; i < Width; i++)
            for (var j = 0; j < Height; j++)
                if (_cells[i, j] != other._cells[i, j])
                    return false;
            return true;
        }

        protected override int GetValueObjectHashCode()
        {
            int hc = _cells.Length;
            foreach (var fieldCell in _cells)
            {
                var val = (int)fieldCell;
                hc = unchecked(hc * 397 + val);
            }

            return hc;
        }

        public IEnumerator<IEnumerable<FieldCell>> GetEnumerator()
        {
            return new FieldEnumerator(new VisibleArea(_cells), Width, Height);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public FieldCell this[int x, int y] => _cells[x, y];
    }
}