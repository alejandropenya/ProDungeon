using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utils
{
    public static class RandomExtensions
    {
        private static Thread _unityThread;

        private static int _currentAsyncRandomSeed;

        public static int GetRandomInt(this object obj, int minInclusive, int maxInclusive)
        {
            if (Thread.CurrentThread == _unityThread || _unityThread == null)
            {
                return Random.Range(minInclusive, maxInclusive + 1);
            }

            var firstRandom = new System.Random(_currentAsyncRandomSeed).Next();
            _currentAsyncRandomSeed = firstRandom;
            var secondRandom = new System.Random(_currentAsyncRandomSeed).Next();
            _currentAsyncRandomSeed = secondRandom;

            var realRandom = firstRandom / (float) int.MaxValue;

            var random = (int) (maxInclusive * realRandom) + minInclusive;

            return random;
        }

        public static float GetRandom(this object obj, float minInclusive, float maxInclusive)
        {
            if (Thread.CurrentThread == _unityThread || !Application.isPlaying)
            {
                return Random.Range(minInclusive, maxInclusive);
            }

            var firstRandom = new System.Random(_currentAsyncRandomSeed).Next();
            _currentAsyncRandomSeed = firstRandom;
            var secondRandom = new System.Random(_currentAsyncRandomSeed).Next();
            _currentAsyncRandomSeed = secondRandom;

            var realRandom = firstRandom / (float) int.MaxValue;

            var random = (maxInclusive - 1) * realRandom + minInclusive;

            return random;
        }

        /// <summary>
        /// From 0 to 1
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static bool GetRandomChance(this object obj, float percentage)
        {
            var result = GetRandom(0, 0f, 1f);
            return result <= percentage;
        }
    }
}