using System;
using System.Collections.Generic;
using UnityEngine;

namespace PortgateLib
{
	public abstract class PersistentSystem : MonoBehaviour
	{
		protected static Dictionary<Type, PersistentSystem> systems = new Dictionary<Type, PersistentSystem>();

		protected static PersistentSystem GetInstance(Type type)
		{
			if (systems.ContainsKey(type))
			{
				return systems[type];
			}
			else
			{
				return null;
			}
		}

		void Awake()
		{
			var type = GetType();
			var ins = GetInstance(type);
			if (ins == null)
			{
				systems.Add(type, this);
				transform.SetParent(null);
				DontDestroyOnLoad(gameObject);
				OnlyOnceAwake();
			}
			else
			{
				Destroy(gameObject);
			}
		}

		protected virtual void OnlyOnceAwake()
		{
		}

		void OnDestroy()
		{
			var type = GetType();
			var ins = GetInstance(type);
			if (ins == this)
			{
				OnShutdown();
				systems.Remove(type);
			}
		}

		protected virtual void OnShutdown()
		{
		}
	}
}