using System;

namespace SudokuSolver.Core.Models
{
    public struct BitSet
    {
        private readonly int _size;
        private readonly int _arraySize;
        private readonly uint[] _values; // 4 bytes each

        public int Size { get { return _size; } }

        public BitSet(int size, bool defaultValue)
        {
            _size = size;

            _arraySize = GetArraySize(size);
            _values = new uint[_arraySize];

            uint initValue = defaultValue ? ~((uint)0) : 0;

            for (int i = 0; i < _arraySize; i++)
            {
                _values[i] = initValue;
            }
        }

        public BitSet(BitSet original)
        {
            _size = original._size;
            _arraySize = original._arraySize;
            _values = new uint[_arraySize];

            for (int i = 0; i < _arraySize; i++)
            {
                _values[i] = original._values[i];
            }
        }

        private static int GetArraySize(int size)
        {
#if DEBUG
            if (size > 128)
                throw new InvalidOperationException();
#endif

            return (size + 31) >> 5;

            //if (size <= 32)
            //    return 1;
            //if (size <= 64)
            //    return 2;
            //if (size <= 96)
            //    return 3;
            //return 4;
        }

        private static int GetArrayIndexForIndex(int index)
        {
#if DEBUG
            if (index >= 128)
                throw new InvalidOperationException();
#endif
            return index >> 5;  // index / 32
        }

        private static int GetArrayPositionForIndex(int index)
        {
#if DEBUG
            if (index >= 128)
                throw new InvalidOperationException();
#endif

            return index % 32;
        }

        public bool this[int index]
        {
            get
            {
#if DEBUG
                if (index >= _size)
                    throw new ArgumentException(nameof(index));
#endif

                var arrayIndex = GetArrayIndexForIndex(index);
                var arrayPosition = GetArrayPositionForIndex(index);
                var arrayValue = _values[arrayIndex];
                return (arrayValue & (1 << arrayPosition)) != 0;
            }
            set
            {
#if DEBUG
                if (index >= _size)
                    throw new ArgumentException(nameof(index));
#endif

                var arrayIndex = GetArrayIndexForIndex(index);
                var arrayPosition = GetArrayPositionForIndex(index);
                var arrayValue = _values[arrayIndex];

                if (value)
                {
                    _values[arrayIndex] = arrayValue | ((uint)(1 << arrayPosition));
                }
                else
                {
                    _values[arrayIndex] = arrayValue & ~((uint)(1 << arrayPosition));
                }
            }
        }

        public static BitSet operator &(BitSet A, BitSet B)
        {
            if (A.Size != B.Size)
                throw new Exception("Internal error: sizes of bitsets are not equal!");

            var andResult = new BitSet(A);
            for (int i = 0; i < andResult._arraySize; i++)
            {
                andResult._values[i] = A._values[i] & B._values[i];
            }
            return andResult;
        }

        public static BitSet operator |(BitSet A, BitSet B)
        {
            if (A.Size != B.Size)
                throw new Exception("Internal error: sizes of bitsets are not equal!");

            var orResult = new BitSet(A);
            for (int i = 0; i < orResult._arraySize; i++)
            {
                orResult._values[i] = A._values[i] | B._values[i];
            }
            return orResult;
        }

        public static BitSet operator ^(BitSet A, BitSet B)
        {
            if (A.Size != B.Size)
                throw new Exception("Internal error: sizes of bitsets are not equal!");

            var andResult = new BitSet(A);
            for (int i = 0; i < andResult._arraySize; i++)
            {
                andResult._values[i] = A._values[i] ^ B._values[i];
            }
            return andResult;
        }

        public static BitSet operator !(BitSet A)
        {
            var notResult = new BitSet(A);
            for (int i = 0; i < notResult._arraySize; i++)
            {
                notResult._values[i] = ~A._values[i];
            }
            return notResult;
        }

        public int Count()
        {
            int count = 0;
            for (int i = 0; i < _size; i++)
            {
                if (this[i])
                    count++;
            }
            return count;
        }

        public bool IsEmpty()
        {
            return Count() == 0;
        }

        public BitSet SetWithBase(bool value, BitSet baseLayer)
        {
            if (_size != baseLayer.Size)
                throw new Exception("Internal error: dimensions of bitsets are not equal!");

            var resultBitSet = new BitSet(this);
            for (var i = 0; i < resultBitSet.Size; i++)
            {
                if (baseLayer[i])
                {
                    resultBitSet[i] = value;
                }
            }
            return resultBitSet;
        }
    }
}
