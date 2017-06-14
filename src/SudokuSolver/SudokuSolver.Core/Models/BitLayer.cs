using System.Linq;

namespace SudokuSolver.Core.Models
{
    /// <summary>
    /// Class used to represent a part of a sudoku. It has overloaded operators for easy bitwise functions.
    /// </summary>
    public class BitLayer
    {
        #region Properties
        //private bool[] Layer;
        //private int Dimension;

        /// <summary>
        /// The dimension of the array of booleans of the BitLayer
        /// </summary>
        public int Dimension { get; }
        //{
        //    get { return Dimension; }
        //}

        /// <summary>
        /// The array of booleans representing the BitLayer.
        /// </summary>
        public bool[] Layer { get; }
        //{
        //    get { return Layer; }
        //    //set { Layer = value; }
        //}

        /// <summary>
        /// Boolean-value indicating if the dimension of this BitLayer is equal to the dimension of the Solution
        /// </summary>
        public bool EqualToSolutionDimension
        {
            get { return Dimension == Solution.Dimension; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with the dimension and the defaultvalue of the array representing the BitLayer.
        /// </summary>
        /// <param name="dimension">The dimension of the BitLayer</param>
        /// <param name="defaultValue">The defaultvalue of the array of the BitLayer</param>
        public BitLayer(int dimension, bool defaultValue)
        {
            Dimension = dimension;
            Layer = Enumerable.Range(0, dimension).Select(x => defaultValue).ToArray();
            //Layer = new bool[Dimension];
            //for (int i = 0; i < Dimension; i++) {
            //    Layer[i] = defaultValue;
            //}
        }

        private BitLayer(BitLayer original)
        {
            Dimension = original.Dimension;
            Layer = original.Layer.ToArray();
        }

        #endregion

        #region Overloaded Operators

        /// <summary>
        /// Overloaded bitwise-and operator. Uses two BitLayers resulting in a new BitLayers. Internally
        /// all items in the arrays are bitwise-and'ed.
        /// </summary>
        /// <param name="A">The leftside BitLayer</param>
        /// <param name="B">The rightside BitLayer</param>
        /// <returns>The resulting BitLayer</returns>
        public static BitLayer operator &(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension) {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            for (var i = 0; i < A.Dimension; i++) {
                C.Layer[i] = A.Layer[i] & B.Layer[i];
                //if (A.Layer[i] == true && B.Layer[i] == true) {
                //    C.Layer[i] = true;
                //}
            }
            return C;
        }

        /// <summary>
        /// Overloaded bitwise-or operator. Uses two BitLayers resulting in a new BitLayers. Internally
        /// all items in the arrays are bitwise-or'ed.
        /// </summary>
        /// <param name="A">The leftside BitLayer</param>
        /// <param name="B">The rightside BitLayer</param>
        /// <returns>The resulting BitLayer</returns>
        public static BitLayer operator |(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension) {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            for (var i = 0; i < A.Dimension; i++) {
                C.Layer[i] = A.Layer[i] | B.Layer[i];
                //if (A.Layer[i] == true || B.Layer[i] == true) {
                //    C.Layer[i] = true;
                //}
            }
            return C;
        }

        /// <summary>
        /// Overloaded bitwise-xor operator. Uses two BitLayers resulting in a new BitLayers. Internally
        /// all items in the arrays are bitwise-xor'ed.
        /// </summary>
        /// <param name="A">The leftside BitLayer</param>
        /// <param name="B">The rightside BitLayer</param>
        /// <returns>The resulting BitLayer</returns>
        public static BitLayer operator ^(BitLayer A, BitLayer B)
        {
            if (A.Dimension != B.Dimension) {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = new BitLayer(A.Dimension, false);
            for (var i = 0; i < A.Dimension; i++) {
                C.Layer[i] = A.Layer[i] ^ B.Layer[i];
                //if ((A.Layer[i] == true || B.Layer[i] == true) && (A.Layer[i] == false || B.Layer[i] == false)) {
                //    C.Layer[i] = true;
                //}
            }
            return C;
        }

        /// <summary>
        /// Overloaded bitwise-not operator. Uses one BitLayer resulting in a new BitLayers. Internally
        /// all items in the arrays are bitwise-not'ed.
        /// </summary>
        /// <param name="A">The BitLayer</param>
        /// <returns>The resulting BitLayer</returns>
        public static BitLayer operator !(BitLayer A)
        {
            var C = new BitLayer(A.Dimension, false);
            for (var i = 0; i < A.Dimension; i++)
                C.Layer[i] = !A.Layer[i];
            return C;
        }
        #endregion

        /// <summary>
        /// Sets items of the internal array of booleans to a specified value. Items processed are indicated using
        /// a BitLayer with the same dimensions.
        /// </summary>
        /// <param name="value">The new value</param>
        /// <param name="baseLayer">The BitLayer used as a Base</param>
        /// <returns>The new BitLayer</returns>
        public BitLayer SetWithBase(bool value, BitLayer baseLayer)
        {
            if (Dimension != baseLayer.Dimension) {
                throw new System.Exception("Internal error: dimensions of BitLayers are not equal!");
            }
            var C = Copy();
            for (var i = 0; i < Dimension; i++) {
                if (baseLayer.Layer[i]) {
                    C.Layer[i] = value;
                }
            }
            return C;
        }

        /// <summary>
        /// Makes an identical copy of this BitLayer.
        /// </summary>
        /// <returns>The new copy of this BitLayer</returns>
        public BitLayer Copy()
        {
            return new BitLayer(this);
            //BitLayer C = new BitLayer(Dimension, false);
            //for (var i = 0; i < Dimension; i++) {
            //    C.Layer[i] = Layer[i];
            //}
            //return C;
        }

        /// <summary>
        /// Counts the amount of items in this BitLayer with the value set to True.
        /// </summary>
        /// <returns>The amount of items in this BitLayer with the value set to True</returns>
        public int Count()
        {
            return Layer.Count(x => x);
            //int result = 0;
            //foreach (bool i in Layer)
            //    if (i) result++;
            //return result;
        }
        /// <summary>
        /// Indicates if all items in this BitLayer are set to False.
        /// Identical to Count() == 0.
        /// </summary>
        /// <returns>True if all items in this BitLayer are set to False, False if not</returns>
        public bool IsEmpty()
        {
            return Count() == 0;
        }

        ///// <summary>
        ///// Indicates if all items in this BitLayer are set to True.
        ///// </summary>
        ///// <returns>True if all items in this BitLayer are set to True, False if not</returns>
        //public bool IsFull()
        //{
        //    return (Count() == Dimension);
        //}
    }
}
