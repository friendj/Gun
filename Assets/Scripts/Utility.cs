using System;
using System.Collections;

public static class Utility
{
    public static T[] ShuffleArray<T>(T[] array, int seed)
    {
        int length = array.Length;

        System.Random random = new System.Random(seed);

        for (int i = 0; i< length - 1; i++)
        {
            int randomIndex = random.Next(i, length);

            T item = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = item;
        }

        return array;
    }
}
