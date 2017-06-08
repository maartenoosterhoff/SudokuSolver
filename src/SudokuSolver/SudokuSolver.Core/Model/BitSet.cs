using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Core.Model
{
    public class BitSet : ValueCollection<bool>
    {
        public BitSet(int length, bool defaultValue) : base(length, defaultValue) { }
        public BitSet(IEnumerable<bool> setValues) : base(setValues) { }

        protected override ValueCollection<bool> AndOperator(ValueCollection<bool> other)
        {
            var result = new BitSet(Length, false);
            for (var i = 0; i < Length; i++)
            {
                result[i] = this[i] & other[i];
            }
            return result;
        }

        protected override ValueCollection<bool> OrOperator(ValueCollection<bool> other)
        {
            var result = new BitSet(Length, false);
            for (var i = 0; i < Length; i++)
            {
                result[i] = this[i] | other[i];
            }
            return result;
        }

        protected override ValueCollection<bool> XOrOperator(ValueCollection<bool> other)
        {
            var result = new BitSet(Length, false);
            for (var i = 0; i < Length; i++)
            {
                result[i] = this[i] ^ other[i];
            }
            return result;
        }

        protected override ValueCollection<bool> NotOperator()
        {
            var setC = new BitSet(Length, false);
            for (var i = 0; i < Length; i++)
            {
                setC[i] = !this[i];
            }
            return setC;
        }
    }

    
}
