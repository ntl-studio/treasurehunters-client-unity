using System.Collections;
using System.Collections.Generic;
using NtlStudio.TreasureHunters.Core;

namespace NtlStudio.TreasureHunters.Model
{
    public class GameField : ValueObject<GameField>, IEnumerable<IEnumerable<FieldCell>>, IField
    {
        public const int FieldWidth = 10;
        public const int FieldHeight = 10;
        public const int MaxUsers = 5;
        private readonly FieldCell[,] _gameField;

        internal GameField(FieldCell[,] field)
        {
            _gameField = field;
        }

        public VisibleArea GetVisibleArea(Position position)
        {
            FieldCell[,] visibleCells = new FieldCell[VisibleArea.Width, VisibleArea.Height];
            for (int x = 0; x < VisibleArea.Width; x++)
            {
                for (int y = 0; y < VisibleArea.Height; y++)
                {
                    var localX = position.X - x - 1;
                    var localY = position.Y - y - 1;
                    if (localX < 0 || localY < 0 || localX >= FieldWidth || localY >= FieldHeight)
                    {
                        visibleCells[x, y] = FieldCell.Empty;
                    }
                    else
                    {
                        visibleCells[x, y] = _gameField[position.X + localX, position.Y + localY];
                    }
                }
            }

            return new VisibleArea(visibleCells);
        }

        public FieldCell this[int x, int y] => _gameField[x, y];

        protected override bool EqualsValueObject(GameField other)
        {
            for (var i = 0; i < FieldWidth; i++)
            for (var j = 0; j < FieldHeight; j++)
                if (_gameField[i, j] != other[i, j])
                    return false;

            return true;
        }

        protected override int GetValueObjectHashCode()
        {
            int hc = _gameField.Length;
            foreach (var fieldCell in _gameField)
            {
                var val = (int)fieldCell;
                hc = unchecked(hc * 397 + val);
            }

            return hc;
        }

        public IEnumerator<IEnumerable<FieldCell>> GetEnumerator()
        {
            return new FieldEnumerator(new GameField(_gameField), FieldWidth, FieldHeight);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}