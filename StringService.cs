using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

internal partial class Extensions
{
    public static string Repr<T>(this T[,] matrix)
    {
        string result = "";
        for (int x = 0; x < matrix.GetLength(1); x++)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
                result += $"{matrix[y, x]}, ";
            result += "\n";
        }
        return result;
    }
}

internal static class StringService
{
    private static int Min(params int[] values)
    {
        int min = values[0];

        foreach (int value in values)
            if (value < min)
                min = value;

        return min;
    }

    /// <summary>
    /// Computes the number of edits required to transform one string to another. The edits count the following as an operation:
    /// <list> Insertion of a character </list>
    /// <list> Deletion of a character </list>
    /// <list> Substitution of a character </list>
    /// <para> More the number of operations, less is the similarity between the two strings. </para>
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns></returns>
    public static int LevenshteinDistance(string str1, string str2)
    {
        int str1Len = str1.Length;
        int str2Len = str2.Length;

        // TODO
        // TODO some comments would be nice
        int[,] matrix = new int[str1Len + 1, str2Len + 1];

        // Fill the first row and first colomn
        for (int x = 0; x < str2Len + 1; x++)
            matrix[0, x] = x;
        for (int y = 0; y < str1Len + 1; y++)
            matrix[y, 0] = y;


        for (int y = 1; y < str1Len + 1; y++)
        {
            for (int x = 1; x < str2Len + 1; x++)
            {
                // Add one if str1[:x - 1] is not equal to str2[:y - i]
                matrix[y, x] = Min(matrix[y - 1, x], matrix[y, x - 1], matrix[y - 1, x - 1]) + (str2.Substring(0, x) != str1.Substring(0, y) ? 1 : 0);
            }
        }

        // TODO comments
        return matrix[str1Len, str2Len];
    }
}
