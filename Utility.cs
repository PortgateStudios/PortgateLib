﻿using UnityEngine;

namespace PortgateLib
{
	public static class Utility
	{
		// Extensions

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

		public static T GetRandomElement<T>(this T[] array)
		{
			var index = Random.Range(0, array.Length);
			return array[index];
		}

		public static string ToJSON(this int[] array)
		{
			var str = "[";
			foreach (var t in array)
			{
				str += " " + t + ",";
			}
			var arrayIsNotEmpty = str.Length > 1;
			if (arrayIsNotEmpty)
			{
				str = str.Remove(str.Length - 1); // remove last comma
			}
			str += " ]";
			return str;
		}

		public static string ToJSON(this string[] array)
		{
			var str = "[";
			for (int i = 0; i < array.Length; i++)
			{
				str += $"\"{array[i]}\"";
				var isntTheLastElement = i != array.Length - 1;
				if (isntTheLastElement)
				{
					str += ", ";
				}
			}
			str += "]";
			return str;
		}

		public static int ModifyInCyclicRange(this int value, int amount, int min, int max)
		{
			var translation = min < 0 ? -min : 0;
			var newMax = max + translation;
			value += translation;

			// Now we can work in the (0 ... max + translation) range where it's easier to do stuff.

			var newValue = value + amount;
			var maxLimit = newMax + 1;

			if (newValue < 0)
			{
				var delta = Mathf.Abs(newValue);
				var remainder = delta % maxLimit;
				newValue = maxLimit - remainder;
			}
			else if (newValue >= maxLimit)
			{
				newValue = newValue % maxLimit;
			}

			newValue -= translation;
			return newValue;
		}

		public static int CountLetter(this string str, char character)
		{
			int count = 0;
			foreach (var c in str)
			{
				if (c == character) count++;
			}
			return count;
		}

		public static string ToHex(this Color color)
		{
			return "#" + ColorUtility.ToHtmlStringRGBA(color);
		}

		// Static methods

		public static Color Color255(int r, int g, int b, int a = 255)
		{
			return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
		}

		public static bool IsRightAngle(Vector2Int tile1, Vector2Int tile2, Vector2Int tile3)
		{
			var dir1 = tile2 - tile1;
			var dir2 = tile3 - tile2;
			var dot = Vector2.Dot(dir1, dir2);
			var isRightAngle = Mathf.Approximately(dot, 0);
			return isRightAngle;
		}
	}
}