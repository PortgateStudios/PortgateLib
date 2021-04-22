using System;
using UnityEngine;

namespace PortgateLib.Timer
{
	public class Timer
	{
		public virtual float Duration
		{
			get { return duration; }
		}

		public virtual float ElapsedTime
		{
			get { return elapsedTime; }
		}

		public virtual float ElapsedPercent
		{
			get
			{
				if (Mathf.Approximately(duration, 0))
					return 1;
				else
					return Mathf.Max(0, Mathf.Min(1, elapsedTime / duration));
			}
		}

		public virtual bool IsRunning
		{
			get { return currentTime > 0; }
		}

		protected float currentTime = -1;
		private readonly float duration;
		private Action onFinishedCallback;
		private float elapsedTime;

		public Timer(float duration, Action onFinishedCallback = null)
		{
			if (duration <= 0)
			{
				throw new Exception("Duration must be positive!");
			}
			this.duration = duration;
			this.onFinishedCallback = onFinishedCallback;
		}

		public virtual void ResetStart()
		{
			currentTime = duration;
			elapsedTime = 0;
		}

		public virtual void Stop()
		{
			currentTime = -1;
		}

		public virtual void Finish()
		{
			Stop();
			CallOnFinishedCallback();
		}

		public virtual void Update()
		{
			if (!IsRunning) return;
			if (currentTime > 0)
			{
				currentTime -= Time.deltaTime;
				elapsedTime += Time.deltaTime;
				if (currentTime < 0)
				{
					CallOnFinishedCallback();
				}
			}
		}

		private void CallOnFinishedCallback()
		{
			if (onFinishedCallback != null)
			{
				onFinishedCallback();
			}
		}

		protected void SetOnFinishedCallback(Action onFinishedCallback)
		{
			this.onFinishedCallback = onFinishedCallback;
		}
	}
}