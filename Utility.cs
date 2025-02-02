using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PortgateLib
{
	public static class Utility
	{
		// Extensions

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

		public static string ComputeMD5Hash(this string input)
		{
			// Use input string to calculate MD5 hash
			using var md5 = MD5.Create();
			var inputBytes = Encoding.ASCII.GetBytes(input);
			var hashBytes = md5.ComputeHash(inputBytes);

			// Convert the byte array to hexadecimal string
			var sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++)
			{
				sb.Append(hashBytes[i].ToString("X2"));
			}
			return sb.ToString();
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
				newValue %= maxLimit;
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

		public static Sprite ConvertTextureToSprite(Texture2D texture)
		{
			var rectangle = new Rect(0, 0, texture.width, texture.height);
			return Sprite.Create(texture, rectangle, new Vector2(0.5f, 0.5f), 100);
		}

		public static Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
		{
			var original = originalTexture.GetPixels32();
			var rotated = new Color32[original.Length];
			var w = originalTexture.width;
			var h = originalTexture.height;

			for (var j = 0; j < h; ++j)
			{
				for (var i = 0; i < w; ++i)
				{
					var iRotated = (i + 1) * h - j - 1;
					var iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
					rotated[iRotated] = original[iOriginal];
				}
			}

			var rotatedTexture = new Texture2D(h, w);
			rotatedTexture.SetPixels32(rotated);
			rotatedTexture.Apply();
			return rotatedTexture;
		}

		public static IEnumerable<T> GetEnumValues<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static void DestroyChildren(this Transform transform)
		{
			var childCount = transform.childCount;
			for (var i = 0; i < childCount; i++)
			{
				var child = transform.GetChild(0);
				GameObject.Destroy(child.gameObject);
			}
		}

		public static void DestroyChildrenImmediate(this Transform transform)
		{
			var childCount = transform.childCount;
			for (var i = 0; i < childCount; i++)
			{
				var child = transform.GetChild(0);
				GameObject.DestroyImmediate(child.gameObject);
			}
		}

		public static Vector2 GetSize(this RectTransform rectTransform)
		{
			var size = rectTransform.sizeDelta;
			var anchorX = (min: rectTransform.anchorMin.x, max: rectTransform.anchorMax.x);
			var anchorY = (min: rectTransform.anchorMin.y, max: rectTransform.anchorMax.y);
			if (Mathf.Approximately(anchorX.min, 0) && Mathf.Approximately(anchorX.max, 1))
			{
				size.x = GetParentSize(rectTransform).x + size.x;
			}
			if (Mathf.Approximately(anchorY.min, 0) && Mathf.Approximately(anchorY.max, 1))
			{
				size.y = GetParentSize(rectTransform).y + size.y;
			}
			return size;
		}

		private static Vector2 GetParentSize(this RectTransform rectTransform)
		{
			var parent = rectTransform.parent.GetComponent<RectTransform>();
			return parent.GetSize();
		}

		// Index starts at 0.
		public static bool IsDigitZero(this float value, int indexOfDigit)
		{
			var multiplier = Mathf.Pow(10, indexOfDigit + 1);
			var integer = (int)(value * multiplier);
			return integer % multiplier == 0;
		}

		public static string GetPath(this Transform transform, int depth = 999, bool pretty = true)
		{
			var path = transform.name;
			for (var i = 0; i < depth; i++)
			{
				var parent = transform.parent;
				var parentName = parent != null ? parent.name : "root";
				var separator = pretty ? " / " : "/";
				path = $"{parentName}{separator}{path}";
				transform = parent;
				if (transform == null)
				{
					break;
				}
			}
			return path;
		}

		public static string SecondsToMinuteSecondString(float seconds)
		{
			var t = TimeSpan.FromSeconds(seconds);
			var text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
			return text;
		}

		public static string TryGettingName(this UnityEngine.Object unityObj)
		{
			return unityObj == null ? GetNullInfo(unityObj) : unityObj.name;
		}

		// We need a more strictly parametrized version for this method, because Unity overrides "== null".
		// Using the generic version for Unity objects would cast it into an object and the null check would evaulate to false. (Because the reference itself is not null yet, it's just "destroyed".)
		// Thus the ToString() would be called, which is a "null" string for destroyed Unity objects. It would cause confusion, loss of information, harder debugging, etc.
		public static string TryToString(this UnityEngine.Object unityObj)
		{
			return unityObj == null ? GetNullInfo(unityObj) : unityObj.ToString();
		}

		private static string GetNullInfo(UnityEngine.Object unityObj)
		{
			var literallyNull = unityObj is null;
			var equalsNull = unityObj == null;
			return $"<literally null: {literallyNull}, equals null: {equalsNull}>";
		}

		public static string TryToString(this object obj)
		{
			return obj == null ? "<null>" : obj.ToString();
		}

		public static string BytesToString(ulong bytes)
		{
			var gb = bytes / 1024f / 1024f / 1024f;
			if (gb > 1)
			{
				return $"{gb:0.00} GB";
			}
			else
			{
				var mb = bytes / 1024f / 1024f;
				if (mb > 1)
				{
					return $"{mb:0.00} MB";
				}
				else
				{
					var kb = bytes / 1024f;
					if (kb > 1)
					{
						return $"{kb:0.00} KB";
					}
					else
					{
						return $"{bytes:0.00} B";
					}
				}
			}
		}

		public static string IntToRomanNumerals(int number)
		{
			if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
			if (number < 1) return string.Empty;
			if (number >= 1000) return "M" + IntToRomanNumerals(number - 1000);
			if (number >= 900) return "CM" + IntToRomanNumerals(number - 900);
			if (number >= 500) return "D" + IntToRomanNumerals(number - 500);
			if (number >= 400) return "CD" + IntToRomanNumerals(number - 400);
			if (number >= 100) return "C" + IntToRomanNumerals(number - 100);
			if (number >= 90) return "XC" + IntToRomanNumerals(number - 90);
			if (number >= 50) return "L" + IntToRomanNumerals(number - 50);
			if (number >= 40) return "XL" + IntToRomanNumerals(number - 40);
			if (number >= 10) return "X" + IntToRomanNumerals(number - 10);
			if (number >= 9) return "IX" + IntToRomanNumerals(number - 9);
			if (number >= 5) return "V" + IntToRomanNumerals(number - 5);
			if (number >= 4) return "IV" + IntToRomanNumerals(number - 4);
			if (number >= 1) return "I" + IntToRomanNumerals(number - 1);
			throw new ArgumentOutOfRangeException("something bad happened");
		}

		public static Vector2 RandomPointInAnnulus(Vector2 origin, float minRadius, float maxRadius)
		{
			var randomDirection = (Random.insideUnitCircle * origin).normalized;
			var randomDistance = Random.Range(minRadius, maxRadius);
			var point = origin + randomDirection * randomDistance;
			return point;
		}

		public static bool IsObjectTypeOrSubclassOf(this object obj, Type type)
		{
			var objType = obj.GetType();
			return objType == type || objType.IsSubclassOf(type);
		}
	}
}