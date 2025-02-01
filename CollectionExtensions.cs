using System;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace PortgateLib
{
	public static class CollectionExtensions
	{
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
		{
			var array = collection.ToArray();
			int n = array.Length;
			var result = new T[n];
			array.CopyTo(result, 0);
			while (n > 1)
			{
				var k = Random.Range(0, n--);
				(result[k], result[n]) = (result[n], result[k]);
			}
			return result;
		}

		public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
		{
			var index = Random.Range(0, enumerable.Count());
			return enumerable.ElementAt(index);
		}

		public static IEnumerable<T> GetRandomElements<T>(this IEnumerable<T> collection, int count, bool distinct)
		{
			if (distinct)
			{
				var shuffledCollection = collection.Shuffle();
				return shuffledCollection.Take(count);
			}
			else
			{
				var result = new T[count];
				for (var i = 0; i < count; i++)
				{
					result[i] = collection.GetRandomElement();
				}
				return result;
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

		public static int GetLastIndex<T>(this IEnumerable<T> enumerable)
		{
			if (enumerable is ICollection<T> collection)
			{
				return collection.Count - 1;
			}
			else if (enumerable is IReadOnlyCollection<T> readOnlyCollection)
			{
				return readOnlyCollection.Count - 1;
			}
			else
			{
				// Fallback for other IEnumerable<T> types (e.g., LINQ queries)
				// Note: This will enumerate the sequence, which can be inefficient for large or infinite sequences
				return enumerable.Count() - 1;
			}
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