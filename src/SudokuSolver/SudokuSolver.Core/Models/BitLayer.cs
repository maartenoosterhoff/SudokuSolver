using System;
using System.Linq;

namespace SudokuSolver.Core.Models
{
    public class BitLayer
    {
        public int Dimension { get; }
        public bool[] Layer { get; }

        public BitLayer(int dimension, bool defaultValue)
        {
            Dimension = dimension;
            Layer = Enumerable.Range(0, dimension).Select(x => defaultValue).ToArray();
        }

        private BitLayer(BitLayer original)
        {
            Dimension = original.Dimension;
            Layer = original.Layer.ToArray();
        }

        public static BitLayer operator &(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension) {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            for (var i = 0; i < A.Dimension; i++) {
                C.Layer[i] = A.Layer[i] & B.Layer[i];
            }
            return C;
        }

        public static BitLayer operator |(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension) {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            for (var i = 0; i < A.Dimension; i++) {
                C.Layer[i] = A.Layer[i] | B.Layer[i];
            }
            return C;
        }

        public static BitLayer operator ^(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension) {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            for (var i = 0; i < A.Dimension; i++) {
                C.Layer[i] = A.Layer[i] ^ B.Layer[i];
            }
            return C;
        }

        public static BitLayer operator !(BitLayer A)
        {
            var C = new BitLayer(A.Dimension, false);
            for (var i = 0; i < A.Dimension; i++)
                C.Layer[i] = !A.Layer[i];
            return C;
        }

        public BitLayer SetWithBase(bool value, BitLayer baseLayer)
        {
            if (Dimension != baseLayer.Dimension) {
                throw new Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(this);
            for (var i = 0; i < Dimension; i++) {
                if (baseLayer.Layer[i]) {
                    C.Layer[i] = value;
                }
            }
            return C;
        }

        public int Count()
        {
            return Layer.Count(x => x);
        }

        public bool IsEmpty()
        {
            return Count() == 0;
        }
    }
}
