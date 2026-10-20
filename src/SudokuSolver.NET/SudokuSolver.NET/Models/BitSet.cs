using System.Numerics;

namespace SudokuSolver.NET.Models;

internal sealed class BitSet
{
    private readonly int _arraySize;
    private readonly uint[] _values; // 4 bytes each

    public int Size { get; }

    private int _internalCount;

    public BitSet(int size, bool defaultValue)
    {
        Size = size;

        _arraySize = GetArraySize(size);
        _values = new uint[_arraySize];

        var initValue = defaultValue ? ~((uint)0) : 0;  // All bits to 1. uint.MaxValue would probably do.

        for (var i = 0; i < _arraySize; i++)
        {
            _values[i] = initValue;
        }
    }

    private BitSet(BitSet original)
    {
        Size = original.Size;
        _arraySize = original._arraySize;
        _values = new uint[_arraySize];

        for (var i = 0; i < _arraySize; i++)
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
            if (index >= Size)
            {
                throw new ArgumentException("Value of index is too large for the bitset.", nameof(index));
            }
#endif

            var arrayIndex = GetArrayIndexForIndex(index);
            var arrayPosition = GetArrayPositionForIndex(index);
            var arrayValue = _values[arrayIndex];
            return (arrayValue & (1 << arrayPosition)) != 0;
        }
        set
        {
#if DEBUG
            if (index >= Size)
            {
                throw new ArgumentException("Value of index is too large for the bitset.", nameof(index));
            }
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

            SetInternalCount();
        }
    }

    public static BitSet operator &(BitSet a, BitSet b)
    {
        if (a.Size != b.Size)
        {
            throw new NotSupportedException("Sizes of bitsets are not equal.");
        }

        var andResult = new BitSet(a);
        for (var i = 0; i < andResult._arraySize; i++)
        {
            andResult._values[i] = a._values[i] & b._values[i];
        }

        andResult.SetInternalCount();
        return andResult;
    }

    public static BitSet operator |(BitSet a, BitSet b)
    {
        if (a.Size != b.Size)
        {
            throw new NotSupportedException("Sizes of bitsets are not equal.");
        }

        var orResult = new BitSet(a);
        for (var i = 0; i < orResult._arraySize; i++)
        {
            orResult._values[i] = a._values[i] | b._values[i];
        }

        orResult.SetInternalCount();
        return orResult;
    }

    public static BitSet operator ^(BitSet a, BitSet b)
    {
        if (a.Size != b.Size)
        {
            throw new NotSupportedException("Sizes of bitsets are not equal.");
        }

        var xorResult = new BitSet(a);
        for (var i = 0; i < xorResult._arraySize; i++)
        {
            xorResult._values[i] = a._values[i] ^ b._values[i];
        }
        xorResult.SetInternalCount();

        return xorResult;
    }

    public static BitSet operator !(BitSet a)
    {
        var notResult = new BitSet(a);
        for (var i = 0; i < notResult._arraySize; i++)
        {
            notResult._values[i] = ~a._values[i];
        }

        notResult.SetInternalCount();

        return notResult;
    }

    private void SetInternalCount()
    {
        _internalCount = _values.Sum(BitOperations.PopCount);


        // Old implementation
        //var count = 0;
        //for (var i = 0; i < Size; i++)
        //{
        //    if (this[i])
        //        count++;
        //}

        //_internalCount = count;
    }

    public int Count => _internalCount;

    public bool IsEmpty => Count == 0;

    public BitSet SetWithBase(bool value, BitSet baseLayer)
    {
        if (Size != baseLayer.Size)
        {
            throw new NotSupportedException("Dimensions of bitsets are not equal.");
        }

        var resultBitSet = new BitSet(this);
        for (var i = 0; i < resultBitSet.Size; i++)
        {
            if (baseLayer[i])
            {
                resultBitSet[i] = value;
            }
        }

        resultBitSet.SetInternalCount();
        return resultBitSet;
    }
}