using System;
using UnityEngine;

namespace PortgateLib
{
	public static class VectorExtensions
	{
		public static Vector2 ToVector2XZ(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.z);
		}

		public static Vector2 ToVector2XY(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		public static Vector2Int ToVector2IntXY(this Vector3Int vector)
		{
			return new Vector2Int(vector.x, vector.y);
		}

		public static Vector3Int ToVector3IntXY(this Vector2Int vector)
		{
			return new Vector3Int(vector.x, vector.y, 0);
		}

		public static Vector3 ToVector3XZ(this Vector2Int vector)
		{
			return new Vector3(vector.x, 0, vector.y);
		}

		public static Vector3 ToVector3XZ(this Vector2 vector)
		{
			return new Vector3(vector.x, 0, vector.y);
		}

		public static Vector2Int Opposite(this Vector2Int vector)
		{
			return new Vector2Int(-vector.x, -vector.y);
		}

		public static Vector3 WithX(this Vector3 vector, float x)
		{
			return new Vector3(x, vector.y, vector.z);
		}

		public static Vector3 WithY(this Vector3 vector, float y)
		{
			return new Vector3(vector.x, y, vector.z);
		}

		public static Vector3 WithZ(this Vector3 vector, float z)
		{
			return new Vector3(vector.x, vector.y, z);
		}

		public static Vector3 GetDirectionTo(this Vector3 a, Vector3 b)
		{
			return (b - a).normalized;
		}

		public static float GetDistanceTo(this Vector3 a, Vector3 b)
		{
			return (b - a).magnitude;
		}

		public static Quaternion ToRotation(this Vector2 direction, Vector3 axis)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			float finalAngle = angle - 180;
			return Quaternion.AngleAxis(finalAngle, axis);
		}

		public static Vector2 RotateAround(this Vector2 point, float angle, Vector2 pivot = default)
		{
			var dir = point - pivot;

			var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
			var cos = Mathf.Cos(angle * Mathf.Deg2Rad);

			dir = new Vector2(dir.x * cos - dir.y * sin,
							  dir.x * sin + dir.y * cos
			);

			dir += pivot;
			return dir;
		}

		public static Vector2Int RotateAround(this Vector2Int point, float angle, Vector2Int pivot = default)
		{
			var dir = point - pivot;

			int normalizedIntegerAngle = Mathf.RoundToInt(angle) % 360;
			if (normalizedIntegerAngle < 0)
			{
				normalizedIntegerAngle += 360;
			}

			Tuple<int, int> sincos;
			switch (normalizedIntegerAngle)
			{
				case 0:
					sincos = new Tuple<int, int>(0, 1);
					break;
				case 90:
					sincos = new Tuple<int, int>(1, 0);
					break;
				case 180:
					sincos = new Tuple<int, int>(0, -1);
					break;
				case 270:
					sincos = new Tuple<int, int>(-1, 0);
					break;
				default:
					throw new Exception($"Wanted to rotate a Vector2Int into a Vector2Int with invalid angle. ({angle})");
			}

			dir = new Vector2Int(dir.x * sincos.Item2 - dir.y * sincos.Item1,
								 dir.x * sincos.Item1 + dir.y * sincos.Item2
			);

			dir += pivot;
			return dir;
		}
	}
}