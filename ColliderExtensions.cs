using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortgateLib
{
	public static class ColliderExtensions
	{
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
	}
}