using UnityEngine;

namespace PortgateLib
{
	public static class TransformExtensions
	{
		public static float GetNeededRotationTo(this Transform transform, Vector3 point)
		{
			var lookingDirection = transform.position.GetDirectionTo(point);
			var neededRotation = Vector2.SignedAngle(transform.up, lookingDirection);
			return neededRotation;
		}

		public static bool IsLookingTowardsPoint(this Transform transform, Vector3 point, float epsilon = 0f)
		{
			var neededRotation = transform.GetNeededRotationTo(point);
			var delta = Mathf.Abs(neededRotation);
			return Mathf.Approximately(Mathf.Max(0, delta - epsilon), 0);
		}
	}
}