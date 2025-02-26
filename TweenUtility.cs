using System;
using UnityEngine;
using DG.Tweening;

namespace PortgateLib
{
	public static class TweenUtility
	{
		public static Tweener CompleteAndPunch(this Transform transform, float strength, float duration, Action onComplete = null)
		{
			return transform.CompleteAndPunch(strength, duration, 2, onComplete);
		}

		public static Tweener CompleteAndPunch(this Transform transform, float strength, float duration, int vibration, Action onComplete = null)
		{
			var punch = Vector3.one * strength;
			transform.DOComplete();
			return transform.DOPunchScale(punch, duration, vibration).OnComplete(() => onComplete?.Invoke());
		}
	}
}