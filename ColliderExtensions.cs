using System;
using UnityEngine;

namespace PortgateLib
{
	public static class ColliderExtensions
	{
		public static Collider[] OverlapCollider(this Collider collider, Transform transform)
		{
			var type = collider.GetType();
			if (type == typeof(BoxCollider))
			{
				var boxCollider = collider as BoxCollider;
				var center = transform.TransformPoint(boxCollider.center);
				var sizes = boxCollider.size;
				var scale = transform.localScale;
				sizes.Scale(scale);
				return Physics.OverlapBox(center, sizes / 2f, transform.rotation);
			}
			else if (type == typeof(SphereCollider))
			{
				var sphereCollider = collider as SphereCollider;
				(var center, var radius) = sphereCollider.GetSphereData(transform);
				return Physics.OverlapSphere(center, radius);
			}
			else if (type == typeof(CharacterController))
			{
				var characterController = collider as CharacterController;
				throw new Exception("NOT YET IMPLEMENTED BECAUSE I DON'T NEED IT YET AND IT TAKES TIME TO FIGURE IT OUT");
			}
			else
			{
				throw new Exception("Collider type is not handled!");
			}
		}

		public static GameObject CreateSphere(this SphereCollider collider, string name, Transform transform)
		{
			var sphereCollider = collider as SphereCollider;
			(var center, var radius) = sphereCollider.GetSphereData(transform);
			return DebugUtility.CreateSphere(name, center, radius);
		}

		public static float GetWidth(this Collider2D collider, Transform transform)
		{
			var type = collider.GetType();
			if (type == typeof(BoxCollider2D))
				return (collider as BoxCollider2D).size.x * transform.localScale.x;
			else if (type == typeof(CircleCollider2D))
				return (collider as CircleCollider2D).radius * 2 * transform.localScale.x;
			else
			{
				throw new Exception("Collider type is not handled!");
			}
		}

		public static float GetHeight(this Collider2D collider, Transform transform)
		{
			var type = collider.GetType();
			if (type == typeof(BoxCollider2D))
				return (collider as BoxCollider2D).size.y * transform.localScale.y;
			else if (type == typeof(CircleCollider2D))
				return (collider as CircleCollider2D).radius * 2 * transform.localScale.y;
			else
			{
				throw new Exception("Collider type is not handled!");
			}
		}

		public static float GetWidth(this Collider collider, Transform transform)
		{
			var type = collider.GetType();
			if (type == typeof(BoxCollider))
			{
				var boxCollider = collider as BoxCollider;
				var width = boxCollider.size.x * transform.localScale.x;
				var depth = boxCollider.size.z * transform.localScale.z;
				return Mathf.Max(width, depth);
			}
			else if (type == typeof(SphereCollider))
				return (collider as SphereCollider).radius * 2 * GetSphereScale(transform.localScale);
			else if (type == typeof(CharacterController))
				return (collider as CharacterController).radius * 2 * GetHorizontalCapsuleScale(transform.localScale);
			else
			{
				throw new Exception("Collider type is not handled!");
			}
		}

		public static float GetHeight(this Collider collider, Transform transform)
		{
			var type = collider.GetType();
			if (type == typeof(BoxCollider))
				return (collider as BoxCollider).size.y * transform.localScale.y;
			else if (type == typeof(SphereCollider))
				return (collider as SphereCollider).radius * 2 * GetSphereScale(transform.localScale);
			else if (type == typeof(CharacterController))
				return (collider as CharacterController).height * transform.localScale.y;
			else
			{
				throw new Exception("Collider type is not handled!");
			}
		}

		private static (Vector3 center, float radius) GetSphereData(this SphereCollider collider, Transform transform)
		{
			var center = transform.TransformPoint(collider.center);
			var scale = GetSphereScale(transform.localScale);
			var radius = collider.radius * scale;
			return (center, radius);
		}

		private static float GetSphereScale(Vector3 scale)
		{
			// this is how Unity handles sphere colliders' scaling.
			return Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
		}

		private static float GetHorizontalCapsuleScale(Vector3 scale)
		{
			// this is how Unity handles capsule colliders' scaling.
			return Mathf.Max(scale.x, scale.z);
		}

		public static Collider2D[] OverlapCollider(this Collider2D collider, Transform transform)
		{
			var type = collider.GetType();
			if (type == typeof(BoxCollider2D))
			{
				var boxCollider = collider as BoxCollider2D;
				var center = transform.TransformPoint(boxCollider.offset);
				var sizes = boxCollider.size;
				var scale = transform.localScale;
				sizes.Scale(scale);
				return Physics2D.OverlapBoxAll(center, sizes / 2f, transform.rotation.eulerAngles.z);
			}
			else if (type == typeof(CircleCollider2D))
			{
				var circleCollider = collider as CircleCollider2D;
				(var center, var radius) = circleCollider.GetCircleData(transform);
				return Physics2D.OverlapCircleAll(center, radius);
			}
			else
			{
				throw new Exception("Collider type is not handled!");
			}
		}

		private static (Vector2 center, float radius) GetCircleData(this CircleCollider2D collider, Transform transform)
		{
			var center = transform.TransformPoint(collider.offset);
			var scale = GetCircleScale(transform.localScale);
			var radius = collider.radius * scale;
			return (center, radius);
		}

		private static float GetCircleScale(Vector2 scale)
		{
			// this is how Unity handles circle colliders' scaling.
			return Mathf.Max(scale.x, scale.y);
		}
	}
}