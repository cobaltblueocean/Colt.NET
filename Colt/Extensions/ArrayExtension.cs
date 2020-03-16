using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ArrayExtension
    {
        public static T[] EnsureCapacity<T>(this T[] array, int minCapacity)
        {
            int oldCapacity = array.Length;
            T[] newArray;
            if (minCapacity > oldCapacity)
            {
                int newCapacity = (oldCapacity * 3) / 2 + 1;
                if (newCapacity < minCapacity)
                {
                    newCapacity = minCapacity;
                }

                newArray = new T[newCapacity];
                Array.Copy(array, 0, newArray, 0, oldCapacity);
            }
            else
            {
                newArray = array;
            }
            return newArray;
        }

        //public static byte[] EnsureCapacity(this byte[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    byte[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new byte[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static char[] EnsureCapacity(this char[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    char[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new char[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static double[] EnsureCapacity(this double[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    double[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new double[newCapacity];
        //        //for (int i = oldCapacity; --i >= 0; ) newArray[i] = array[i];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static float[] EnsureCapacity(this float[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    float[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new float[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static int[] EnsureCapacity(this int[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    int[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new int[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static long[] EnsureCapacity(this long[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    long[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new long[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static Object[] EnsureCapacity(this Object[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    Object[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new Object[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static short[] EnsureCapacity(this short[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    short[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new short[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        //public static Boolean[] EnsureCapacity(this Boolean[] array, int minCapacity)
        //{
        //    int oldCapacity = array.Length;
        //    Boolean[] newArray;
        //    if (minCapacity > oldCapacity)
        //    {
        //        int newCapacity = (oldCapacity * 3) / 2 + 1;
        //        if (newCapacity < minCapacity)
        //        {
        //            newCapacity = minCapacity;
        //        }

        //        newArray = new Boolean[newCapacity];
        //        Array.Copy(array, 0, newArray, 0, oldCapacity);
        //    }
        //    else
        //    {
        //        newArray = array;
        //    }
        //    return newArray;
        //}

        public static T[] SafeCopy<T>(this T[] array)
        {
            if (array != null)
            {
                var dist = new T[array.Length];
                Array.Copy(array, dist, array.Length);
                return dist;
            }
            else
                return null;
        }

        public static T[][] SafeCopy<T>(this T[][] array)
        {
            if (array != null)
            {
                var dist = new T[array.Length, array[0].Length];
                Array.Copy(array.ToMultidimensional(), dist, array.Length);
                return dist.ToJagged();
            }
            else
                return null;
        }

        public static T[][][] SafeCopy<T>(this T[][][] array)
        {
            if (array != null)
            {
                var dist = new T[array.Length, array[0].Length, array[0][0].Length];
                Array.Copy(array.ToMultidimensional(), dist, array.Length);
                return dist.ToJagged();
            }
            else
                return null;
        }

        /// <summary>
        /// Ensures that the specified array cannot hold more than <tt>maxCapacity</tt> elements.
        /// An application can use this operation to minimize array storage.
        /// <p>
        /// Returns the identical array if <tt>array.length &lt;= maxCapacity</tt>.
        /// Otherwise, returns a new array with a length of <tt>maxCapacity</tt>
        /// containing the first <tt>maxCapacity</tt> elements of <tt>array</tt>.
        /// 
        /// <summary>
        /// <param name=""> maxCapacity   the desired maximum capacity.</param>
        public static T[] TrimToCapacity<T>(this T[] array, int maxCapacity)
        {
            if (array.Length > maxCapacity)
            {
                T[] oldArray = array;
                array = new T[maxCapacity];
                oldArray.CopyTo(array, 0);
            }
            return array;
        }

        public static T[][] ToJagged<T>(this T[,] array)
        {
            int row = array.GetLength(0);

            T[][] jagary = new T[row][];
            for (int i = 0; i < row; i++)
            {
                int col = array.GetLength(1);
                jagary[i] = new T[col];
                for (int j = 0; j < col; j++)
                {
                    jagary[i][j] = array[i, j];
                }
            }

            return jagary;
        }

        public static T[][][] ToJagged<T>(this T[,,] array)
        {
            int slice = array.GetLength(0);

            T[][][] jagary = new T[slice][][];
            for (int i = 0; i < slice; i++)
            {
                int row = array.GetLength(1);
                jagary[i] = new T[row][];
                for (int j = 0; j < row; j++)
                {
                    int col = array.GetLength(2);
                    jagary[i][j] = new T[col];
                    for (int k = 0; k < col; k++)
                    {
                        jagary[i][j][k] = array[i, j, k];
                    }
                }
            }

            return jagary;
        }

        public static T[,] ToMultidimensional<T>(this T[][] array)
        {
            T[,] mult = new T[array.GetLength(0), array.GetLength(1)];
            int row = array.GetLength(0);
            int col = array.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    mult[i, j] = array[i][j];
                }
            }

            return mult;
        }

        public static T[,,] ToMultidimensional<T>(this T[][][] array)
        {
            T[,,] mult = new T[array.GetLength(0), array.GetLength(1), array.GetLength(2)];
            int slice = array.GetLength(0);
            int row = array.GetLength(1);
            int col = array.GetLength(2);

            for (int i = 0; i < slice; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    for (int k = 0; k < col; k++)
                    {
                        mult[i, j, k] = array[i][j][k];
                    }
                }
            }

            return mult;
        }

        public static T[][] Initialize<T>(this T[][] array, int row, int col)
        {
            array = new T[row][];
            for(int i =0; i < row; i++)
            {
                array[i] = new T[col];
            }

            return array;
        }

        public static T[][][] Initialize<T>(this T[][][] array, int slice, int row, int col)
        {
            array = new T[slice][][];
            for (int i = 0; i < slice; i++)
            {
                array[i] = new T[row][];
                for (int j = 0; j < row; j++)
                {
                    array[i][j] = new T[col];
                }
            }

            return array;
        }
    }
}
