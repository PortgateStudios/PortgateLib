using UnityEngine;

namespace PortgateLib
{
	public static class TransformExtensions
	{
		public static bool IsLookingTowardsPoint(this Transform transform, Vector2 point, float epsilon = 0f)
		{
			var neededRotation = transform.GetNeededRotationTo(point);
			var delta = Mathf.Abs(neededRotation);
			return Mathf.Approximately(Mathf.Max(0, delta - epsilon), 0);
		}

		public static float GetNeededRotationTo(this Transform transform, Vector2 point)
		{
			var lookingDirection = transform.position.ToVector2XZ().GetDirectionTo(point);
			var neededRotation = Vector3.SignedAngle(transform.forward, lookingDirection.ToVector3XZ(), Vector3.up);
			return neededRotation;
		}
	}
}