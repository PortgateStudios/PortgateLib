using System;
using UnityEngine;

namespace PortgateLib
{
	public static class VectorExtensions
	{
		#region Converters

		// Vector3 Converters

		public static Vector2 ToVector2XZ(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.z);
		}

		public static Vector2 ToVector2XY(this Vector3 vector)
		{
			return new Vector2(vector.x, vector.y);
		}
		
		// Vector2 Converters

		public static Vector3 ToVector3XZ(this Vector2 vector)
		{
			return new Vector3(vector.x, 0, vector.y);
		}

		public static Vector3 ToVector3XY(this Vector2 vector)
		{
			return new Vector3(vector.x, vector.y, 0);
		}

		// Vector3Int Converters

		public static Vector3 ToVector3(this Vector3Int vector)
		{
			return new Vector3(vector.x, vector.y, vector.z);
		}

		public static Vector2Int ToVector2IntXY(this Vector3Int vector)
		{
			return new Vector2Int(vector.x, vector.y);
		}

		// Vector2Int Converters

		public static Vector2 ToVector2(this Vector2Int vector)
		{
			return new Vector2(vector.x, vector.y);
		}

		public static Vector3 ToVector3XZ(this Vector2Int vector)
		{
			return new Vector3(vector.x, 0, vector.y);
		}

		public static Vector3 ToVector3XY(this Vector2Int vector)
		{
			return new Vector3(vector.x, vector.y, 0);
		}

		public static Vector3Int ToVector3IntXY(this Vector2Int vector)
		{
			return new Vector3Int(vector.x, vector.y, 0);
		}

		#endregion
		#region Opposite

		public static Vector2 Opposite(this Vector2 vector)
		{
			return new Vector2(-vector.x, -vector.y);
		}

		public static Vector2Int Opposite(this Vector2Int vector)
		{
			return new Vector2Int(-vector.x, -vector.y);
		}

		#endregion
		#region Coordinate changers

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

		#endregion
		#region Direction & Distance

		public static Vector3 GetDirectionTo(this Vector2 a, Vector2 b)
		{
			return (b - a).normalized;
		}

		public static Vector3 GetDirectionTo(this Vector3 a, Vector3 b)
		{
			return (b - a).normalized;
		}

		public static float GetDistanceTo(this Vector2 a, Vector2 b)
		{
			return (b - a).magnitude;
		}

		public static float GetDistanceTo(this Vector3 a, Vector3 b)
		{
			return (b - a).magnitude;
		}

		#endregion
		#region Rotation

		// 3D Mode

		public static Quaternion ToLookRotation3D(this Vector3 direction)
		{
			return Quaternion.LookRotation(direction);
		}

		public static Quaternion ToLookRotation3D(this Vector2 direction)
		{
			return direction.ToVector3XZ().ToLookRotation3D();
		}

		public static Quaternion ToLookRotation3D(this Vector2Int direction)
		{
			return direction.ToVector3XZ().ToLookRotation3D();
		}

		// 2D Mode

		public static Quaternion ToLookRotation2D(this Vector2 direction)
		{
			var directionXZ = direction.ToVector3XZ();
			var rotationY = Quaternion.LookRotation(directionXZ);
			return Quaternion.Euler(0, 0, -rotationY.eulerAngles.y);
		}

		public static Quaternion ToLookRotation2D(this Vector3 direction)
		{
			return direction.ToVector2XY().ToLookRotation2D();
		}

		public static Quaternion ToLookRotation2D(this Vector2Int direction)
		{
			return direction.ToVector2().ToLookRotation2D();
		}

		public static Quaternion FromIsometricToUIRotation(this Vector2 direction)
		{
			// due to isometric camera, rotate it with 45 degrees.
			var rotation = direction.ToLookRotation3D() * Quaternion.Euler(0, 45, 0);
			return Quaternion.Euler(0, 0, -rotation.eulerAngles.y);
		}

		public static Quaternion FromIsometricToUIRotation(this Vector2Int direction)
		{
			return direction.ToVector2().FromIsometricToUIRotation();
		}

		#endregion
		#region Rotating

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

		#endregion
		#region Line

		public static Vector2 GetFurthestPointInLine(this Vector2 a, Vector2 b, float maxDistance)
		{
			var distanceVector = b - a;
			var clampedDistanceVector = Vector2.ClampMagnitude(distanceVector, maxDistance);
			return a + clampedDistanceVector;
		}

		#endregion
	}
}