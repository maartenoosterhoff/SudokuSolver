using System;
using System.Linq;

namespace SudokuSolver.Core.Models
{
    public struct BitLayer
    {
        public int Dimension { get; }
        public bool[] Layer { get; }

        public BitLayer(int dimension, bool defaultValue)
        {
            Dimension = dimension;
            Layer = new bool[dimension];
            if (defaultValue)
            {
                for (int i = 0; i < dimension; i++)
                {
                    Layer[i] = defaultValue;
                }
            }

            //Layer = Enumerable.Range(0, dimension).Select(x => defaultValue).ToArray();
        }

        private BitLayer(BitLayer original)
        {
            Dimension = original.Dimension;
            Layer = original.Layer.ToArray();
        }

        public static BitLayer operator &(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension)
            {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            var cLayer = C.Layer;
            var bLayer = B.Layer;
            var aLayer = A.Layer;
            for (var i = 0; i < A.Dimension; i++)
            {
                cLayer[i] = aLayer[i] & bLayer[i];
            }
            return C;
        }

        public static BitLayer operator |(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension)
            {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            var bLayer = B.Layer;
            var aLayer = A.Layer;
            var cLayer = C.Layer;
            for (var i = 0; i < A.Dimension; i++)
            {
                cLayer[i] = aLayer[i] | bLayer[i];
            }
            return C;
        }

        public static BitLayer operator ^(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension)
            {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            var bLayer = B.Layer;
            var aLayer = A.Layer;
            var cLayer = C.Layer;
            for (var i = 0; i < A.Dimension; i++)
            {
                cLayer[i] = aLayer[i] ^ bLayer[i];
            }
            return C;
        }

        public static BitLayer operator !(BitLayer A)
        {
            var C = new BitLayer(A.Dimension, false);
            var aLayer = A.Layer;
            var cLayer = C.Layer;
            for (var i = 0; i < A.Dimension; i++)
                cLayer[i] = !aLayer[i];
            return C;
        }

        public BitLayer SetWithBase(bool value, BitLayer baseLayer)
        {
            if (Dimension != baseLayer.Dimension)
            {
                throw new Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(this);
            var layer = C.Layer;
            var baseLayerLayer = baseLayer.Layer;
            for (var i = 0; i < Dimension; i++)
            {
                if (baseLayerLayer[i])
                {
                    layer[i] = value;
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
