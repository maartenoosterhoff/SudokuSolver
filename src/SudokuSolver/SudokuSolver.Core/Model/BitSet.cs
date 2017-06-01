using System;
using System.Linq;

namespace SudokuSolver.Core.Model
{
    public class BitSet
    {
        public int Length { get; }

        private bool[] _set;

        public BitSet(int length, bool defaultValue)
        {
            Length = length;
            _set = Enumerable.Range(0, length).Select(x => defaultValue).ToArray();
        }

        public bool this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return _set[index];
            }
            set
            {
                if (index < 0 || index >= Length)
                    throw new ArgumentOutOfRangeException(nameof(index));

                _set[index] = value;
            }
        }

        public int Count
        {
            get { return _set.Count(x => x); }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public static BitSet operator &(BitSet setA, BitSet setB)
        {
            if (setA == null)
                throw new ArgumentNullException(nameof(setA));
            if (setB == null)
                throw new ArgumentNullException(nameof(setB));
            if (setA.Length != setB.Length)
                throw new ArgumentException("Length of bitset B should be identical to the length of bitset A.", nameof(setB));

            var setC = new BitSet(setA.Length, false);
            for (var i = 0; i < setA.Length; i++)
            {
                setC[i] = setA[i] & setB[i];
            }
            return setC;
        }

        public static BitSet operator |(BitSet setA, BitSet setB)
        {
            if (setA == null)
                throw new ArgumentNullException(nameof(setA));
            if (setB == null)
                throw new ArgumentNullException(nameof(setB));
            if (setA.Length != setB.Length)
                throw new ArgumentException("Length of bitset B should be identical to the length of bitset A.", nameof(setB));

            var setC = new BitSet(setA.Length, false);
            for (var i = 0; i < setA.Length; i++)
            {
                setC[i] = setA[i] | setB[i];
            }
            return setC;
        }

        public static BitSet operator ^(BitSet setA, BitSet setB)
        {
            if (setA == null)
                throw new ArgumentNullException(nameof(setA));
            if (setB == null)
                throw new ArgumentNullException(nameof(setB));
            if (setA.Length != setB.Length)
                throw new ArgumentException("Length of bitset B should be identical to the length of bitset A.", nameof(setB));

            var setC = new BitSet(setA.Length, false);
            for (var i = 0; i < setA.Length; i++)
            {
                setC[i] = setA[i] ^ setB[i];
            }
            return setC;
        }

        public static BitSet operator !(BitSet set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            var setC = new BitSet(set.Length, false);
            for (var i = 0; i < set.Length; i++)
            {
                setC[i] = !set[i];
            }
            return setC;
        }
    }
}
