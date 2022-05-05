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
		private float targetAlpha;
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
			fadeTimer.Update();
			if (fadeTimer.IsRunning)
			{
				var currentAlpha = canvasGroup.alpha;
				var direction = GetDirection(currentAlpha, targetAlpha);
				var speed = 1f / duration;
				var delta = Time.deltaTime * speed;
				canvasGroup.alpha = currentAlpha + direction * delta;
			}
		}

		private int GetDirection(float currentAlpha, float targetAlpha)
		{
			return targetAlpha > currentAlpha ? 1 : -1;
		}

		private void OnFadeFinished()
		{
			canvasGroup.alpha = targetAlpha;
			onFadeFinishedCallback?.Invoke();
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
			var targetAlpha = fadeType == FadeType.In ? 1 : 0;
			StartFading(targetAlpha, onFadeFinishedCallback);
		}

		public void StartFading(float targetAlpha, Action onFadeFinishedCallback = null)
		{
			this.targetAlpha = targetAlpha;
			this.onFadeFinishedCallback = onFadeFinishedCallback;

			var interactable = targetAlpha > 0.95f ? true : false;
			canvasGroup.interactable = interactable;
			canvasGroup.blocksRaycasts = interactable;

			var timerDuration = CalculateNeededTime();
			fadeTimer = new Timer(timerDuration, OnFadeFinished);
			fadeTimer.ResetStart();
		}

		private float CalculateNeededTime()
		{
			var fadeInPercent = canvasGroup.alpha;
			var difference = Mathf.Abs(targetAlpha - fadeInPercent);
			return duration * difference;
		}
	}
}