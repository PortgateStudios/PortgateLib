using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace PortgateLib
{
	public static class CollectionExtensions
	{
		public static List<T> Shuffle<T>(this List<T> list)
		{
			return list.ToArray().Shuffle().ToList();
		}

		public static T[] Shuffle<T>(this T[] array)
		{
			int n = array.Length;
			var result = new T[n];
			array.CopyTo(result, 0);
			while (n > 1)
			{
				int k = Random.Range(0, n--);
				T temp = result[n];
				result[n] = result[k];
				result[k] = temp;
			}
			return result;
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
		{
			return enumerable.OrderBy(x => Random.value);
		}

		public static T GetRandomElement<T>(this List<T> list)
		{
			return list.ToArray().GetRandomElement();
		}

		public static T GetRandomElement<T>(this T[] array)
		{
			var index = Random.Range(0, array.Length);
			return array[index];
		}

		public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
		{
			var index = Random.Range(0, enumerable.Count());
			return enumerable.ElementAt(index);
		}

		public static List<T> GetRandomElements<T>(this List<T> list, int count, bool distinct)
		{
			return list.ToArray().GetRandomElements(count, distinct).ToList();
		}

		public static T[] GetRandomElements<T>(this T[] array, int count, bool distinct)
		{
			if (distinct)
			{
				var shuffledArray = array.Shuffle();
				return shuffledArray.Take(count).ToArray();
			}
			else
			{
				var result = new T[count];
				for (var i = 0; i < count; i++)
				{
					result[i] = array.GetRandomElement();
				}
				return result;
			}
		}

		public static IEnumerable<T> GetRandomElements<T>(this IEnumerable<T> enumerable, int count, bool distinct)
		{
			if (distinct)
			{
				return enumerable.Shuffle().Take(count);
			}
			else
			{
				return Enumerable.Range(0, count).Select(_ => enumerable.GetRandomElement());
			}
		}

		public static T[] Shift<T>(this T[] array, int direction, int start = 0)
		{
			var length = array.Length;
			var result = new T[length];
			array.CopyTo(result, 0);

			if (direction < 0)
			{
				for (var i = start + 1; i < length; i++)
				{
					result[i - 1] = result[i];
				}
				result[length - 1] = default(T);
			}
			else if (direction > 0)
			{
				for (var i = length - 1; i > start; i--)
				{
					result[i] = result[i - 1];
				}
				result[start] = default(T);
			}

			return result;
		}

		public static int FindIndex<T>(this T[] array, Func<T, bool> predicate)
		{
			for (var i = 0; i < array.Length; i++)
			{
				if (predicate(array[i])) return i;
			}
			return -1;
		}

		public static int FindLastIndex<T>(this T[] array, Func<T, bool> predicate)
		{
			for (var i = array.Length - 1; i >= 0; i--)
			{
				if (predicate(array[i])) return i;
			}
			return -1;
		}

		public static int GetLastIndex<T>(this List<T> list)
		{
			return list.Count - 1;
		}

		public static int GetLastIndex<T>(this T[] array)
		{
			return array.Length - 1;
		}

		public static T Pop<T>(this List<T> list)
		{
			var lastIndex = list.Count - 1;
			var lastElement = list.ElementAt(lastIndex);
			list.RemoveAt(lastIndex);
			return lastElement;
		}
	}
}