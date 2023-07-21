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

	// The default is Complete, but make sure to check below.
	public enum InProgressFade
	{
		Kill,  // Kills the ongoing tween(s). The state will stay as it is right now.
		Rewind, // Rewinds the ongoing tween(s). The state will be as it was before the tween(s) started.
		Complete, // Instantly completes the ongoing tween(s). The state will be as if the tween(s) were completed.
		Keep // Keeps the ongoing tween(s), resulting in multiple parallel tweens.
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
			StartInteractableFadingIn(duration, InProgressFade.Complete, Ease.Unset, onFadeInFinishedCallback);
		}

		public void StartInteractableFadingIn(float duration, InProgressFade inProgressFade, Ease ease, Action onFadeInFinishedCallback = null)
		{
			SetInteractable(true);
			StartFadingIn(duration, inProgressFade, ease, onFadeInFinishedCallback);
		}

		public void StartFadingIn(float duration, Action onFadeInFinishedCallback = null)
		{
			StartFading(FadeType.In, duration, InProgressFade.Complete, Ease.Unset, onFadeInFinishedCallback);
		}

		public void StartFadingIn(float duration, InProgressFade inProgressFade, Ease ease, Action onFadeInFinishedCallback = null)
		{
			if (ease == Ease.Unset)
			{
				ease = FadeInEase;
			}
			StartFading(FadeType.In, duration, inProgressFade, ease, onFadeInFinishedCallback);
		}

		// Fading Out

		public void StartUninteractableFadingOut(float duration, Action onFadeOutFinishedCallback = null)
		{
			StartUninteractableFadingOut(duration, InProgressFade.Complete, Ease.Unset, onFadeOutFinishedCallback);
		}

		public void StartUninteractableFadingOut(float duration, InProgressFade inProgressFade, Ease ease, Action onFadeOutFinishedCallback = null)
		{
			SetInteractable(false);
			StartFadingOut(duration, inProgressFade, ease, onFadeOutFinishedCallback);
		}

		public void StartFadingOut(float duration, Action onFadeOutFinishedCallback = null)
		{
			StartFading(FadeType.Out, duration, InProgressFade.Complete, Ease.Unset, onFadeOutFinishedCallback);
		}

		public void StartFadingOut(float duration, InProgressFade inProgressFade, Ease ease, Action onFadeOutFinishedCallback = null)
		{
			if (ease == Ease.Unset)
			{
				ease = FadeOutEase;
			}
			StartFading(FadeType.Out, duration, inProgressFade, ease, onFadeOutFinishedCallback);
		}

		// Generic Fading

		public void StartInteractibilityFading(FadeType fadeType, float duration, Action onFadeFinishedCallback = null)
		{
			StartInteractibilityFading(fadeType, duration, InProgressFade.Complete, Ease.Unset, onFadeFinishedCallback);
		}

		public void StartInteractibilityFading(FadeType fadeType, float duration, InProgressFade inProgressFade, Ease ease, Action onFadeFinishedCallback = null)
		{
			var interactable = fadeType == FadeType.In;
			SetInteractable(interactable);
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, duration, inProgressFade, ease, onFadeFinishedCallback);
		}

		public void StartFading(FadeType fadeType, float duration, Action onFadeFinishedCallback = null)
		{
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, duration, InProgressFade.Complete, Ease.Unset, onFadeFinishedCallback);
		}

		public void StartFading(FadeType fadeType, float duration, InProgressFade inProgressFade, Ease ease, Action onFadeFinishedCallback = null)
		{
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, duration, inProgressFade, ease, onFadeFinishedCallback);
		}

		public void StartFading(float targetAlpha, float duration, InProgressFade inProgressFade = InProgressFade.Complete, Ease ease = Ease.Unset, Action onFadeFinishedCallback = null)
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

			if (inProgressFade == InProgressFade.Kill)
			{
				canvasGroup.DOKill();
			}
			else if (inProgressFade == InProgressFade.Rewind)
			{
				canvasGroup.DORewind();
			}
			else if (inProgressFade == InProgressFade.Complete)
			{
				canvasGroup.DOComplete();
			}

			tween = canvasGroup.DOFade(targetAlpha, duration);
			if (ease != Ease.Unset)
			{
				tween.SetEase(ease);
			}

			this.onFadeFinishedCallback = onFadeFinishedCallback;
			tween.OnComplete(OnFadeFinished);
		}

		private void OnFadeFinished()
		{
			tween = null;
			onFadeFinishedCallback?.Invoke();
		}
	}
}