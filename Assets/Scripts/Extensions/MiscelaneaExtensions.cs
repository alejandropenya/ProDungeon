using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utils
{
    public static class MiscelaneaExtensions
    {
        public static bool IsNullOrEmpty(this string word)
        {
            return string.IsNullOrEmpty(word);
        }

        public static int Clamp(this int numberToClamp, int min, int max)
        {
            var result = numberToClamp;
            if (numberToClamp < min)
            {
                result = min;
            }

            if (numberToClamp > max)
            {
                result = max;
            }

            return result;
        }

        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        public static float Distance(this Vector3 vector, Vector3 anotherVector)
        {
//            var differenceVector = new Vector3(
//                vector.x - anotherVector.x,
//                vector.y - anotherVector.y,
//                vector.z - anotherVector.z);
//
//            var distance = Mathf.Sqrt(
//                Mathf.Pow(differenceVector.x, 2f) +
//                Mathf.Pow(differenceVector.y, 2f) +
//                Mathf.Pow(differenceVector.z, 2f));
//
//            return distance;
            return Vector3.Distance(vector, anotherVector);
        }

        public static float Pow(this float value, float pow)
        {
            return Mathf.Pow(value, pow);
        }

        public static bool HasDecimals(this float value)
        {
            var integer = Mathf.FloorToInt(value * 10);
            var floatValue = Mathf.FloorToInt(value) * 10;
            return integer - floatValue != 0;
        }

        public static void ResizeWithChildren(this RectTransform transform, Vector2 newSize)
        {
            transform.sizeDelta = newSize;
            transform.GetComponentsInChildren<RectTransform>().ForEach(x => x.sizeDelta = newSize);
        }


        public static int ToInt(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return int.Parse(value);
        }

        public static float ToFloat(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return float.Parse(value);
        }

        public static string[] Split(this string text, string separator)
        {
            return text.Split(new[] {separator}, StringSplitOptions.None);
        }

        public static bool IsNumeric(this string text)
        {
            try
            {
                var number = text.ToFloat();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ToCamel(this string text)
        {
            var result = "";
            result += text.Substring(0, 1).ToLower();
            result += text.Substring(1, text.Length - 1);
            return result;
        }

        public static string FirstLetterToUpperCase(this string text)
        {
            var result = "";
            result += text.Substring(0, 1).ToUpper();
            result += text.Substring(1, text.Length - 1);
            return result;
        }

        public static bool ToBool(this string text)
        {
            return text.ToLower() == "true";
        }

        public static string ToString(this string text)
        {
            return text;
        }

        public static string RemoveSpaces(this string text)
        {
            return text.Replace(" ", "");
        }

        public static List<GameObject> GetAllChildren(this Transform t)
        {
            var result = new List<GameObject>();
            for (var i = 0; i < t.childCount; i++)
            {
                result.Add(t.GetChild(i).gameObject);
            }

            return result;
        }

        public static string RemovePrefix(this string text, string prefix)
        {
            var split = text.Split(prefix);
            if (split.TryGetValue(0) == "")
            {
                var result = "";
                split.Range(1, split.Length).ForEachIndexed((x, i) =>
                {
                    result += x;
                    if (i != split.Length - 2)
                    {
                        result += "\n";
                    }
                });

                return split.TryGetValue(1);
            }

            return text;
        }

        public static void SetFieldValue<T>(this object o, string fieldName, T value)
        {
            var currentType = o.GetType();

            while (currentType.Name != "MonoBehaviour" && currentType.Name != "ScriptableObject")
            {
                var field = currentType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == null)
                {
                    currentType = currentType.BaseType;
                    continue;
                }

                field.SetValue(o, value);
                return;
            }

            Debug.LogError("Errorrrrrr");
        }
    }
}