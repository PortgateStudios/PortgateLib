using System;
using UnityEngine;

namespace PortgateLib
{
	public class DestroyListener : MonoBehaviour
	{
		private Action onDestroyCallback;

		public void SetOnDestroyCallback(Action action)
		{
			this.onDestroyCallback = action;
		}

		void OnDestroy()
		{
			onDestroyCallback();
		}
	}
}