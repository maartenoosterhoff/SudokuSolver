using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Model
{
    public abstract class ValueCollection<T>
    {
        public int Length { get; }

        private T[] _set;

        public ValueCollection(int length, T defaultValue)
        {
            _set = Enumerable.Range(0, length).Select(x => defaultValue).ToArray();
            Length = _set.Length;
        }

        public ValueCollection(IEnumerable<T> setValues)
        {
            _set = setValues.ToArray();
            Length = _set.Length;
        }

        public T this[int index]
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
            get { return _set.Count(x => !x.Equals(default(T))); }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        protected abstract ValueCollection<T> AndOperator(ValueCollection<T> other);
        protected abstract ValueCollection<T> OrOperator(ValueCollection<T> other);
        protected abstract ValueCollection<T> XOrOperator(ValueCollection<T> other);
        protected abstract ValueCollection<T> NotOperator();

        public static ValueCollection<T> operator &(ValueCollection<T> setA, ValueCollection<T> setB)
        {
            if (setA == null)
                throw new ArgumentNullException(nameof(setA));
            if (setB == null)
                throw new ArgumentNullException(nameof(setB));
            if (setA.Length != setB.Length)
                throw new ArgumentException("Length of bitset B should be identical to the length of bitset A.", nameof(setB));

            return setA.AndOperator(setB);
        }

        public static ValueCollection<T> operator |(ValueCollection<T> setA, ValueCollection<T> setB)
        {
            if (setA == null)
                throw new ArgumentNullException(nameof(setA));
            if (setB == null)
                throw new ArgumentNullException(nameof(setB));
            if (setA.Length != setB.Length)
                throw new ArgumentException("Length of bitset B should be identical to the length of bitset A.", nameof(setB));

            return setA.OrOperator(setB);
        }

        public static ValueCollection<T> operator ^(ValueCollection<T> setA, ValueCollection<T> setB)
        {
            if (setA == null)
                throw new ArgumentNullException(nameof(setA));
            if (setB == null)
                throw new ArgumentNullException(nameof(setB));
            if (setA.Length != setB.Length)
                throw new ArgumentException("Length of bitset B should be identical to the length of bitset A.", nameof(setB));

            return setA.XOrOperator(setB);
        }

        public static ValueCollection<T> operator !(ValueCollection<T> set)
        {
            if (set == null)
                throw new ArgumentNullException(nameof(set));

            return set.NotOperator();
        }
    }
}
