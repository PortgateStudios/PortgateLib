using UnityEngine;

namespace PortgateLib
{
	public static class DebugUtility
	{
		public static GameObject CreateSphere(string name, Vector2 position, float radius)
		{
			return CreateSphere(name, position.ToVector3XZ(), radius);
		}

		public static GameObject CreateSphere(string name, Vector3 position, float radius)
		{
			var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			go.name = name;
			go.transform.position = position;
			var scale = radius * 2;
			go.transform.localScale = new Vector3(scale, scale, scale);
			GameObject.Destroy(go.GetComponent<Collider>());
			return go;
		}
	}
}