using UnityEngine;
using System.Collections.Generic;

namespace Infra.Utils {
public static class RandomUtils {
    public static T ChooseRandom<T>(T[] array, int fromIndex = 0) {
        return array[Random.Range(fromIndex, array.Length)];
    }

    public static T ChooseRandom<T>(T[] array, int fromIndex, int toIndex) {
        return array[Random.Range(fromIndex, toIndex)];
    }

    public static T ChooseRandom<T>(List<T> array, int fromIndex = 0) {
        return array[Random.Range(fromIndex, array.Count)];
    }

    public static T ChooseRandom<T>(List<T> array, int fromIndex, int toIndex) {
        return array[Random.Range(fromIndex, toIndex)];
    }

    public static bool RandBool(float trueChance = 0.5f) {
        return Random.value <= trueChance;
    }

    public static uint Range(uint minInclusive, uint maxExclusive) {
        return (uint)Random.Range((int)minInclusive, (int)maxExclusive);
    }

    public static void Shuffle<T>(T[] arr) {
        // Implementation from: http://answers.unity3d.com/questions/16531/randomizing-arrays.html
        for (int i = arr.Length - 1; i > 0; i--) {
            int r = Random.Range(0, i + 1);
            T tmp = arr[i];
            arr[i] = arr[r];
            arr[r] = tmp;
        }
    }
}
}
