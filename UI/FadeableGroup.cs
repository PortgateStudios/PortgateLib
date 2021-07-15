using System;
using UnityEngine;

namespace PortgateLib.UI
{
	using PortgateLib.Timer;

	public enum FadeType
	{
		In,
		Out
	}

	[RequireComponent(typeof(CanvasGroup))]
	public class FadeableGroup : MonoBehaviour
	{
		private static readonly float FADE_DURATION = 0.33f;

		public bool IsInteractable
		{
			get { return canvasGroup.interactable; }
		}

		public bool IsFading
		{
			get { return fadeTimer.IsRunning; }
		}

		public float FadePercent
		{
			get { return fadeTimer.ElapsedPercent; }
		}

		public float Alpha
		{
			get { return canvasGroup.alpha; }
		}

		private CanvasGroup canvasGroup;
		private Timer fadeTimer;
		private FadeType fadeType;
		private Action onFadeFinishedCallback;
		private float duration;

		protected virtual void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			duration = FADE_DURATION;
			fadeTimer = new Timer(duration, OnFadeFinished);
		}

		protected virtual void Update()
		{
			if (fadeTimer.IsRunning)
			{
				fadeTimer.Update();
				var currentAlpha = canvasGroup.alpha;
				var direction = fadeType == FadeType.In ? 1 : -1;
				var speed = 1f / duration;
				var delta = Time.deltaTime * speed;
				canvasGroup.alpha = currentAlpha + direction * delta;
			}
		}

		private void OnFadeFinished()
		{
			canvasGroup.alpha = fadeType == FadeType.In ? 1f : 0f;
			if (onFadeFinishedCallback != null)
			{
				onFadeFinishedCallback();
			}
			onFadeFinishedCallback = null;
		}

		public void SetVisible(bool visible)
		{
			canvasGroup.alpha = visible ? 1 : 0;
			canvasGroup.interactable = visible;
			canvasGroup.blocksRaycasts = visible;
		}

		public void OverrideDuration(float duration)
		{
			this.duration = duration;
		}

		public void StartFading(FadeType fadeType, Action onFadeFinishedCallback = null)
		{
			this.onFadeFinishedCallback = onFadeFinishedCallback;
			this.fadeType = fadeType;
			canvasGroup.interactable = fadeType == FadeType.In ? true : false;
			canvasGroup.blocksRaycasts = fadeType == FadeType.In ? true : false;
			var timerDuration = CalculateNeededTime();
			fadeTimer = new Timer(timerDuration, OnFadeFinished);
			fadeTimer.ResetStart();
		}

		private float CalculateNeededTime()
		{
			var fadeInPercent = canvasGroup.alpha;
			if (fadeType == FadeType.In)
			{
				return duration * (1f - fadeInPercent);
			}
			else
			{
				return duration * fadeInPercent;
			}
		}
	}
}