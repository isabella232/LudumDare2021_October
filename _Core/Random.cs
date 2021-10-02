using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Random number generator
/// </summary>
public static class Rand
{
    static System.Random rand = new System.Random();
    public static void SetSeed(int seed)
    {
        rand = new Random(seed);
    }

    static Swizzle swizzle;

    [StructLayout(LayoutKind.Explicit)]
    struct Swizzle
    {
        [FieldOffset(0)]
        public int rand_int1;
        [FieldOffset(4)]
        public int rand_int2;

        [FieldOffset(0)]
        public long rand_long;

        [FieldOffset(0)]
        public float rand_float;

        [FieldOffset(0)]
        public double rand_double;
    }

    static StringBuilder builder = new StringBuilder();
    public static string String(int length, string letters = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789")
    {
        builder.Clear();
        if (length <= 0) return "";
        while (builder.Length < length)
        {
            builder.Append(letters[Rand.Int(letters.Length)]);
        }
        return builder.ToString();
    }

    public static bool Bool => (rand.Next() & 1) == 0;

    /// <summary>
    /// Returns a float between -1 and 1
    /// </summary>
    public static float Float
        => Float01 * 2f - 1f;
    public static float Float01
        => (float)rand.NextDouble();
    public static float FloatMax(float max)
        => Float01 * max;
    public static float FloatRange(float min, float max)
        => Float01 * (max - min) + min;

    public static long Long
    {
        get
        {
            swizzle.rand_int1 = rand.Next();
            swizzle.rand_int2 = rand.Next();
            return swizzle.rand_long;
        }
    }
    
    public static long LongRange(long min, long max)
    {
        var diff = max - min;
        return (Long % diff) + (min > max ? max : min);
    }

    public static long LongMax(long max) => LongRange(0, max);

    /// <summary>
    /// Returns a double between -1 and 1
    /// </summary>
    public static double Double
        => Double01 * 2d - 1d;

    public static double DoubleMax(double max)
        => Double01 * max;

    public static double DoubleRange(double min, double max)
        => Double01 * (max - min) + min;

    public static double Double01
        => rand.NextDouble();

    /// <summary>
    /// returns an int between 0 and max.
    /// max value is excluded.
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Int(int max)
        => rand.Next(max);

    /// <summary>
    /// returns an int between min and max.
    /// min value is inclusive.
    /// max value is exclusive.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Int(int min, int max)
        => rand.Next(min, max);

    public static T GetRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;
        return list[Rand.Int(list.Count)];
    }

    public static T GetRandom<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
            return default;
        return array[Rand.Int(array.Length)];
    }

    public static List<T> Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            var temp = list[i];
            int index = Int(Rand.Int(list.Count));
            list[i] = list[index];
            list[index] = temp;
        }
        return list;
    }
}