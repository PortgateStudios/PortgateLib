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
		public const Ease DefaultEase = Ease.Unset;
		public const InProgressFade DefaultInProgressFade = InProgressFade.Complete;

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

		public void Toggle()
		{
			SetVisibleInteractable(!IsVisible);
		}

		public void SetVisibleInteractable(bool visible)
		{
			SetVisible(visible);
			SetInteractable(visible);
		}

		public void SetVisible(bool visible)
		{
			canvasGroup.DOKill();
			canvasGroup.alpha = visible ? 1 : 0;
		}

		public void SetInteractable(bool interactable)
		{
			canvasGroup.DOKill();
			canvasGroup.interactable = interactable;
			canvasGroup.blocksRaycasts = interactable;
		}

		// Interactibility Fading In

		public void StartInteractableFadingIn(float duration, Action onFadeInFinishedCallback = null)
		{
			StartInteractableFadingIn(duration, DefaultInProgressFade, DefaultEase, onFadeInFinishedCallback);
		}

		public void StartInteractableFadingIn(float duration, InProgressFade inProgressFade, Action onFadeInFinishedCallback = null)
		{
			StartInteractableFadingIn(duration, inProgressFade, DefaultEase, onFadeInFinishedCallback);
		}

		public void StartInteractableFadingIn(float duration, Ease ease, Action onFadeInFinishedCallback = null)
		{
			StartInteractableFadingIn(duration, DefaultInProgressFade, ease, onFadeInFinishedCallback);
		}

		public void StartInteractableFadingIn(float duration, InProgressFade inProgressFade, Ease ease, Action onFadeInFinishedCallback = null)
		{
			SetInteractable(true);
			StartFadingIn(duration, inProgressFade, ease, onFadeInFinishedCallback);
		}

		// Fading In

		public void StartFadingIn(float duration, Action onFadeInFinishedCallback = null)
		{
			StartFading(FadeType.In, duration, DefaultInProgressFade, DefaultEase, onFadeInFinishedCallback);
		}

		public void StartFadingIn(float duration, InProgressFade inProgressFade, Action onFadeInFinishedCallback = null)
		{
			StartFadingIn(duration, inProgressFade, DefaultEase, onFadeInFinishedCallback);
		}

		public void StartFadingIn(float duration, Ease ease, Action onFadeInFinishedCallback = null)
		{
			StartFadingIn(duration, DefaultInProgressFade, ease, onFadeInFinishedCallback);
		}

		public void StartFadingIn(float duration, InProgressFade inProgressFade, Ease ease, Action onFadeInFinishedCallback = null)
		{
			if (ease == Ease.Unset)
			{
				ease = FadeInEase;
			}
			StartFading(FadeType.In, duration, inProgressFade, ease, onFadeInFinishedCallback);
		}

		// Interactibility Fading Out

		public void StartUninteractableFadingOut(float duration, Action onFadeOutFinishedCallback = null)
		{
			StartUninteractableFadingOut(duration, DefaultInProgressFade, DefaultEase, onFadeOutFinishedCallback);
		}

		public void StartUninteractableFadingOut(float duration, InProgressFade inProgressFade, Action onFadeInFinishedCallback = null)
		{
			StartUninteractableFadingOut(duration, inProgressFade, DefaultEase, onFadeInFinishedCallback);
		}

		public void StartUninteractableFadingOut(float duration, Ease ease, Action onFadeInFinishedCallback = null)
		{
			StartUninteractableFadingOut(duration, DefaultInProgressFade, ease, onFadeInFinishedCallback);
		}

		public void StartUninteractableFadingOut(float duration, InProgressFade inProgressFade, Ease ease, Action onFadeOutFinishedCallback = null)
		{
			SetInteractable(false);
			StartFadingOut(duration, inProgressFade, ease, onFadeOutFinishedCallback);
		}

		// Fading Out

		public void StartFadingOut(float duration, Action onFadeOutFinishedCallback = null)
		{
			StartFading(FadeType.Out, duration, DefaultInProgressFade, DefaultEase, onFadeOutFinishedCallback);
		}

		public void StartFadingOut(float duration, InProgressFade inProgressFade, Action onFadeInFinishedCallback = null)
		{
			StartFadingOut(duration, inProgressFade, DefaultEase, onFadeInFinishedCallback);
		}

		public void StartFadingOut(float duration, Ease ease, Action onFadeInFinishedCallback = null)
		{
			StartFadingOut(duration, DefaultInProgressFade, ease, onFadeInFinishedCallback);
		}

		public void StartFadingOut(float duration, InProgressFade inProgressFade, Ease ease, Action onFadeOutFinishedCallback = null)
		{
			if (ease == Ease.Unset)
			{
				ease = FadeOutEase;
			}
			StartFading(FadeType.Out, duration, inProgressFade, ease, onFadeOutFinishedCallback);
		}

		// Generic Interactibility Fading

		public void InteractibilityToggle(float duration)
		{
			var fadeType = IsVisible ? FadeType.Out : FadeType.In;
			StartInteractibilityFading(fadeType, duration);
		}

		public void StartInteractibilityFading(FadeType fadeType, float duration, Action onFadeFinishedCallback = null)
		{
			StartInteractibilityFading(fadeType, duration, DefaultInProgressFade, DefaultEase, onFadeFinishedCallback);
		}

		public void StartInteractibilityFading(FadeType fadeType, float duration, InProgressFade inProgressFade, Action onFadeInFinishedCallback = null)
		{
			StartInteractibilityFading(fadeType, duration, inProgressFade, DefaultEase, onFadeInFinishedCallback);
		}

		public void StartInteractibilityFading(FadeType fadeType, float duration, Ease ease, Action onFadeInFinishedCallback = null)
		{
			StartInteractibilityFading(fadeType, duration, DefaultInProgressFade, ease, onFadeInFinishedCallback);
		}

		public void StartInteractibilityFading(FadeType fadeType, float duration, InProgressFade inProgressFade, Ease ease, Action onFadeFinishedCallback = null)
		{
			var interactable = fadeType == FadeType.In;
			SetInteractable(interactable);
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, duration, inProgressFade, ease, onFadeFinishedCallback);
		}

		// Generic  Fading

		public void Toggle(float duration)
		{
			var fadeType = IsVisible ? FadeType.Out : FadeType.In;
			StartFading(fadeType, duration);
		}

		public void StartFading(FadeType fadeType, float duration, Action onFadeFinishedCallback = null)
		{
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, duration, DefaultInProgressFade, DefaultEase, onFadeFinishedCallback);
		}

		public void StartFading(FadeType fadeType, float duration, InProgressFade inProgressFade, Action onFadeInFinishedCallback = null)
		{
			StartFading(fadeType, duration, inProgressFade, DefaultEase, onFadeInFinishedCallback);
		}

		public void StartFading(FadeType fadeType, float duration, Ease ease, Action onFadeInFinishedCallback = null)
		{
			StartFading(fadeType, duration, DefaultInProgressFade, ease, onFadeInFinishedCallback);
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

			OnFadeStarted();
			if (targetAlpha > Alpha)
			{
				OnFadeInStarted();
			}
			else
			{
				OnFadeOutStarted();
			}
		}

		protected virtual void OnFadeStarted()
		{
		}

		protected virtual void OnFadeInStarted()
		{
		}

		protected virtual void OnFadeOutStarted()
		{
		}

		private void OnFadeFinished()
		{
			tween = null;
			onFadeFinishedCallback?.Invoke();
		}

		protected virtual void OnDestroy()
		{
			tween?.Kill();
		}
	}
}