using System;
using System.Collections.Generic;
using Utils;

namespace Extensions
{
    public static class EnumerableExtensions
    {
        public static T GetOrDefault<T>(this IEnumerable<T> list, int index)
        {
            if (index < 0) return default(T);

            if (list is List<T> listAsList)
            {
                return listAsList.Count > index ? listAsList[index] : default(T);
            }
            
            var listClone = list.CloneList();

            return listClone.Count > index ? listClone[index] : default(T);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> list, Func<T, bool> condition = null)
        {
            return condition != null ? list.Where(condition).GetOrDefault(0) : list.GetOrDefault(0);
        }

        public static T LastOrDefault<T>(this IEnumerable<T> list, Func<T, bool> condition = null)
        {
            if (condition != null)
            {
                var filteredList = list.Where(condition);
                return filteredList.GetOrDefault(list.Count() - 1);
            }
            return list.GetOrDefault(list.Count() - 1);
        }

        public static int GetLastIndex<T>(this IEnumerable<T> list)
        {
            return list.Count() - 1;
        }

        public static bool Contains<T>(this IEnumerable<T> list, T value)
        {
            foreach (var v in list)
            {
                if (Equals(v, value))
                {
                    return true;
                }
            }

            return false;
        }

        public static T Max<T>(this IEnumerable<T> list, Func<T, float> selectable)
        {
            var currentMax = float.MinValue;
            T reference = default;
            foreach (var value in list)
            {
                var selected = selectable(value);
                if (selected > currentMax)
                {
                    currentMax = selected;
                    reference = value;
                }
            }

            return reference;
        }
        
        public static T Min<T>(this IEnumerable<T> list, Func<T, float> selectable)
        {
            var currentMin = float.MaxValue;
            T reference = default;
            foreach (var value in list)
            {
                var selected = selectable(value);
                if (selected < currentMin)
                {
                    currentMin = selected;
                    reference = value;
                }
            }

            return reference;
        }

        public static bool NotContains<T>(this IEnumerable<T> list, T value)
        {
            return !list.Contains(value);
        }

        public static T Pop<T>(this List<T> list)
        {
            var obj = list.LastOrDefault();
            list.RemoveAt(list.Count - 1);
            return obj;
        }

        public static T GetRandom<T>(this IEnumerable<T> list)
        {
            return list.GetOrDefault(0.GetRandomInt(0, list.Count() - 1));
        }

        public static List<T> CloneList<T>(this IEnumerable<T> list)
        {
            var ret = new List<T>();
            foreach (var v in list)
            {
                ret.Add(v);
            }

            return ret;
        }

        public static List<T> ToList<T>(this IEnumerable<T> list)
        {
            return list as List<T>;
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> iteration)
        {
            foreach (var v in list.CloneList())
            {
                iteration(v);
            }
        }

        public static int Count<T>(this IEnumerable<T> list)
        {
            if (list is List<T> listOfList) return listOfList.Count;
            return list.CloneList().Count;
        }

        public static IEnumerable<TS> Select<T, TS>(this IEnumerable<T> list, Func<T, TS> selector)
        {
            var ret = new List<TS>();
            list.ForEach(x => ret.Add(selector(x)));
            return ret;
        }

        public static List<T> ListOf<T>(this object o, params T[] items)
        {
            return items.CloneList();
        }

        public static T[] ArrayOf<T>(this object o, params T[] items)
        {
            return items;
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> list, Func<T, bool> condition)
        {
            var ret = new List<T>();
            list.ForEach(x =>
            {
                if (condition(x))
                {
                    ret.Add(x);
                }
            });
            return ret;
        }

        public static bool All<T>(this IEnumerable<T> list, Func<T, bool> condition)
        {
            var listClone = list.CloneList();

            var conditions = listClone.Select(condition).CloneList();

            return !conditions.Contains(false);
        }

        public static bool Any<T>(this IEnumerable<T> list, Func<T, bool> condition = null)
        {
            var listClone = list.CloneList();

            if (listClone.Count == 0) return false;

            if (condition == null)
            {
                return listClone.Count > 0;
            }

            var conditions = listClone.Select(condition).CloneList();

            return conditions.Contains(true);
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> list)
        {
            var ret = new List<T>();
            list.ForEach(y =>
            {
                if (!ret.Any(x => Equals(x, y)))
                    ret.Add(y);
            });

            return ret;
        }

        public static IEnumerable<T> Distinct<T, TS>(this IEnumerable<T> list, Func<T, TS> equalizer)
        {
            var ret = new List<T>();

            list.ForEach(y =>
            {
                if (!ret.Any(x => equalizer(x).Equals(equalizer(y))))
                    ret.Add(y);
            });

            return ret;
        }

        public static IEnumerable<int> IntRange(this object o, int min, int max, int slop = 1)
        {
            var ret = new List<int>();
            for (var i = min; i <= max; i += slop)
            {
                ret.Add(i);
            }

            return ret;
        }
        
        public static IEnumerable<float> FloatRange(this object o, float min, float max, float slop = 1)
        {
            var ret = new List<float>();
            for (var i = min; i <= max; i += slop)
            {
                ret.Add(i);
            }

            return ret;
        }

        public static void ForEachIndexed<T>(this IEnumerable<T> list, Action<T, int> action)
        {
            var i = 0;
            list.ForEach(x => { action(x, i++); });
        }

        public static IEnumerable<T> Range<T>(this IEnumerable<T> list, int min, int max)
        {
            var result = new List<T>();
            var clampedMin = min.Clamp(0, list.Count() - 1);
            var clampedMax = max.Clamp(0, list.Count() - 1);
            
            for (var i = clampedMin; i <= clampedMax; i++)
            {
                result.Add(list.GetOrDefault(i));
            }

            return result;
        }

        public static float Sum<T>(this IEnumerable<T> list, Func<T, float> selector)
        {
            var sum = 0.0f;
            foreach (var item in list)
            {
                sum += selector(item);
            }

            return sum;
        }

        public static T TryGetValue<T>(this IList<T> list, int i)
        {
            return list.Count > i ? list[i] : default;
        }

        public static T[] ToArray<T>(this IEnumerable<T> list)
        {
            var ret = new T[list.Count()];
            list.ForEachIndexed((item, i) => { ret[i] = item; });
            return ret;
        }

        public static void AddRangeUnique<T>(this IList<T> list, IEnumerable<T> listToAdd)
        {
            foreach (var item in listToAdd)
            {
                if(!list.Contains(item)) list.Add(item);
            }
        }

        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                list.Remove(item);
            }
        }

        public static Dictionary<TK, TV> ToDictionary<T, TK, TV>(this IEnumerable<T> list, Func<T, TK> keySelector,
            Func<T, TV> valueSelector)
        {
            var dictionary = new Dictionary<TK, TV>();
            list.ForEach(x => dictionary.Add(keySelector(x), valueSelector(x)));
            return dictionary;
        }

        public static string Stringify<T>(this IEnumerable<T> list, Func<T, string> selector, string startingString = "", string separator = ",", string lastSeparator = ",", string endingString = "")
        {
            if (!list.Any()) return "";
            
            var result = startingString.Length > 0 ? startingString + " " : "";
            var listCount = list.Count();

            if (listCount == 1)
            {
                var startingStringWithSpace = result;
                var endingStringWithSpace = endingString.Length > 0 ? " " + endingString  : "";
                
                return $"{startingStringWithSpace}{selector(list.GetOrDefault(0))}{endingStringWithSpace}";
            }
            
            list.ForEachIndexed((item, i) =>
            {
                if (i == listCount - 1 )
                {
                    result = result.Substring(0, result.Length - (1 + separator.Length));
                    result += $"{lastSeparator} {selector(item)}";
                }
                else
                {
                    result += selector(item) + $"{separator} ";
                }
            });

            if (endingString.Length != 0)
            {
                result += $"{endingString}";
            }
            
            return result;
        }

        public static IEnumerable<T> Map<T>(this IEnumerable<T> list, Action<T> mapAction)
        {
            return list.Select(x =>
            {
                mapAction(x);
                return x;
            });
        }

        public static IEnumerable<T> ReverseList<T>(this IEnumerable<T> list)
        {
            var result = new List<T>();
            for (var i = list.Count() - 1; i >= 0; i--)
            {
                result.Add(list.GetOrDefault(i));
            }

            return result;
        }

        public static IEnumerable<S> SelectMany<T, S>(this IEnumerable<IEnumerable<T>> list, Func<T, S> selector)
        {
            var result = new List<S>();
            for (var i = 0; i < list.Count(); i++)
            {
                var currentList = list.GetOrDefault(i);
                for (var j = 0; j < currentList.Count(); j++)
                {
                    result.Add(selector(currentList.GetOrDefault(j)));
                }
            }

            return result;
        }

        public static List<TSource> OrderByAscending<TSource> (
            this IEnumerable<TSource> source,
            Converter<TSource, float> projection)
        {
            var listClone = source.ToList ();
            listClone.Sort ((source1, source2) => (10000 * projection (source1)).CompareTo (10000 * projection (source2)));
            return listClone;
        }
        public static List<TSource> OrderByDescending<TSource> (
            this IEnumerable<TSource> source,
            Converter<TSource, float> projection)
        {
            var listClone = source.ToList ();
            listClone.Sort ((source1, source2) => (10000 * projection (source2)).CompareTo (10000 * projection (source1)));
            return listClone;
        }

        public static List<TSource> Shuffle<TSource>(this IEnumerable<TSource> list)
        {
            return list.OrderByAscending(input => 0.GetRandomInt(1, 100000));
        }

        public static int Count<T>(this IEnumerable<T> list, Func<T, bool> condition)
        {
            return list.Where(condition).Count();
        }

        public static IEnumerable<S> SelectMany<T, S>(this IEnumerable<T> list, Func<T, IEnumerable<S>> selector)
        {
            var result = new List<S>();
            list.ForEach(lt =>
            {
                result.AddRange(selector(lt));
            });
            return result;
        }

        public static IEnumerable<(TS, List<T>)> GroupBy<T, TS>(this IEnumerable<T> list, Func<T, TS> groupSelector)
        {
            var result = new List<(TS, List<T>)>();
            list.ForEach(x =>
            {
                var currentGroup = groupSelector(x);
                if (result.Select(y => y.Item1).Contains(currentGroup))
                {
                    result.FirstOrDefault(z => z.Item1.Equals(currentGroup)).Item2.Add(x);
                }
                else
                {
                    result.Add((currentGroup, new List<T>()));
                }
            });
            return result;
        }
        
        public static IEnumerable<T> Remove<T>(this IEnumerable<T> list, T removeItem)
        {
            return list.Where(x => !x.Equals(removeItem));
        }
        
    }
}