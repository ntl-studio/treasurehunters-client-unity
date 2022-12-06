using System.Collections;
using System.Collections.Generic;

namespace NtlStudio.TreasureHunters.Model
{
    internal class FieldEnumerator : IEnumerator<IEnumerable<FieldCell>>
    {
        private readonly IField _field;
        private readonly int _width;
        private readonly int _height;

        private int _y = -1;

        public FieldEnumerator(IField field, int width, int height)
        {
            _field = field;
            _width = width;
            _height = height;
        }

        public bool MoveNext()
        {
            if (_y < _height - 1)
            {
                _y++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _y = -1;
        }

        public IEnumerable<FieldCell> Current
        {
            get
            {
                var result = new FieldCell[_width];
                for (var i = 0; i < _width; i++)
                {
                    result[i] = _field[i, _y];
                }

                return result;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}