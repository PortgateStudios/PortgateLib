using System;
using UnityEngine;
using DG.Tweening;

namespace PortgateLib
{
	public static class TweenUtility
	{
		public static void Punch(this RectTransform rectTransform, float strength, float duration, Action onComplete = null)
		{
			rectTransform.Punch(strength, duration, 2, onComplete);
		}

		public static void Punch(this RectTransform rectTransform, float strength, float duration, int vibration, Action onComplete = null)
		{
			var punch = Vector3.one * strength;
			rectTransform.DOComplete();
			rectTransform.DOPunchScale(punch, duration, vibration).OnComplete(() => onComplete?.Invoke());
		}
	}
}