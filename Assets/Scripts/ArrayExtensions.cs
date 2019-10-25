using System.Collections;
using System.Collections.Generic;

public static class ArrayExtensions
{
    public static T[] GetRow<T>(T[,] matrix, int row, int firstColumn)
    {
        var columns = matrix.GetLength(1);
        var array = new T[columns];
        for (int i = firstColumn; i < firstColumn + columns; ++i)
            array[i - firstColumn] = matrix[row, i];
        return array;
    }
}
