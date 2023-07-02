using System;
using UnityEngine;

namespace PortgateLib.UI
{
	using DG.Tweening;

	public enum FadeType
	{
		In,
		Out
	}

	[RequireComponent(typeof(CanvasGroup))]
	public class FadeableGroup : MonoBehaviour
	{
		public Ease FadeInEase
		{
			get;
			set;
		} = DOTween.defaultEaseType;

		public Ease FadeOutEase
		{
			get;
			set;
		} = DOTween.defaultEaseType;

		public float? InPunchStrength
		{
			get;
			set;
		}

		public float? OutPunchStrength
		{
			get;
			set;
		}

		public bool IsVisible
		{
			get { return !Mathf.Approximately(canvasGroup.alpha, 0); }
		}

		public bool IsInteractable
		{
			get { return canvasGroup.interactable; }
		}

		public bool IsFading
		{
			get { return tween != null; }
		}

		public float Alpha
		{
			get { return canvasGroup.alpha; }
		}

		protected RectTransform RectTransform
		{
			get;
			private set;
		}

		private CanvasGroup canvasGroup;
		private Tween tween;
		private Action onFadeFinishedCallback;

		protected virtual void Awake()
		{
			RectTransform = GetComponent<RectTransform>();
			canvasGroup = GetComponent<CanvasGroup>();
		}

		public void SetVisibleInteractable(bool visible)
		{
			SetVisible(visible);
			SetInteractable(visible);
		}

		public void SetVisible(bool visible)
		{
			canvasGroup.alpha = visible ? 1 : 0;
		}

		public void SetInteractable(bool interactable)
		{
			canvasGroup.interactable = interactable;
			canvasGroup.blocksRaycasts = interactable;
		}

		// Fading in

		public void StartInteractableFadingIn(float duration, Action onFadeInFinishedCallback = null)
		{
			StartInteractableFadingIn(duration, Ease.Unset, onFadeInFinishedCallback);
		}

		public void StartInteractableFadingIn(float duration, Ease ease, Action onFadeInFinishedCallback = null)
		{
			SetInteractable(true);
			StartFadingIn(duration, ease, onFadeInFinishedCallback);
		}

		public void StartFadingIn(float duration, Action onFadeInFinishedCallback = null)
		{
			StartFading(FadeType.In, duration, Ease.Unset, onFadeInFinishedCallback);
		}

		public void StartFadingIn(float duration, Ease ease, Action onFadeInFinishedCallback = null)
		{
			if (ease == Ease.Unset)
			{
				ease = FadeInEase;
			}
			StartFading(FadeType.In, duration, ease, onFadeInFinishedCallback);
		}

		// Fading Out

		public void StartUninteractableFadingOut(float duration, Action onFadeOutFinishedCallback = null)
		{
			StartUninteractableFadingOut(duration, Ease.Unset, onFadeOutFinishedCallback);
		}

		public void StartUninteractableFadingOut(float duration, Ease ease, Action onFadeOutFinishedCallback = null)
		{
			SetInteractable(false);
			StartFadingOut(duration, ease, onFadeOutFinishedCallback);
		}

		public void StartFadingOut(float duration, Action onFadeOutFinishedCallback = null)
		{
			StartFading(FadeType.Out, duration, Ease.Unset, onFadeOutFinishedCallback);
		}

		public void StartFadingOut(float duration, Ease ease, Action onFadeOutFinishedCallback = null)
		{
			if (ease == Ease.Unset)
			{
				ease = FadeOutEase;
			}
			StartFading(FadeType.Out, duration, ease, onFadeOutFinishedCallback);
		}

		// Generic Fading

		public void StartInteractibilityFading(FadeType fadeType, float duration, Action onFadeFinishedCallback = null)
		{
			StartInteractibilityFading(fadeType, duration, Ease.Unset, onFadeFinishedCallback);
		}

		public void StartInteractibilityFading(FadeType fadeType, float duration, Ease ease, Action onFadeFinishedCallback = null)
		{
			var interactable = fadeType == FadeType.In;
			SetInteractable(interactable);
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, duration, ease, onFadeFinishedCallback);
		}

		public void StartFading(FadeType fadeType, float duration, Action onFadeFinishedCallback = null)
		{
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, duration, Ease.Unset, onFadeFinishedCallback);
		}

		public void StartFading(FadeType fadeType, float duration, Ease ease, Action onFadeFinishedCallback = null)
		{
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, duration, ease, onFadeFinishedCallback);
		}

		public void StartFading(float targetAlpha, float duration, Ease ease = Ease.Unset, Action onFadeFinishedCallback = null)
		{
			var fadingIn = Alpha < targetAlpha;
			var noFadeNeeded = Mathf.Approximately(Alpha, targetAlpha);
			if (fadingIn || noFadeNeeded)
			{
				if (InPunchStrength.HasValue)
				{
					RectTransform.Punch(InPunchStrength.Value, duration);
				}
			}
			else
			{
				if (OutPunchStrength.HasValue)
				{
					RectTransform.Punch(OutPunchStrength.Value, duration);
				}
			}

			this.onFadeFinishedCallback = onFadeFinishedCallback;
			canvasGroup.DORewind();
			tween = canvasGroup.DOFade(targetAlpha, duration);
			if (ease != Ease.Unset)
			{
				tween.SetEase(ease);
			}
			tween.OnComplete(OnFadeFinished);
		}

		private void OnFadeFinished()
		{
			tween = null;
			onFadeFinishedCallback?.Invoke();
		}
	}
}