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

		public virtual bool IsRunning
		{
			get { return remainingTime >= 0; }
		}

		public virtual float RemainingTime
		{
			get { return remainingTime; }
		}

		public virtual float RemainingPercent
		{
			get
			{
				if (Mathf.Approximately(Duration, 0))
					return 0;
				else
					return RemainingTime / Duration;
			}
		}

		public virtual float ElapsedTime
		{
			get { return Duration - RemainingTime; }
		}

		public virtual float ElapsedPercent
		{
			get { return 1f - RemainingPercent; }
		}

		protected float remainingTime = -1;

		private readonly float duration;
		private Action onFinishedCallback;

		public Timer(float duration, Action onFinishedCallback = null)
		{
			if (duration < 0)
			{
				throw new Exception("Duration is negative!");
			}
			this.duration = duration;
			this.onFinishedCallback = onFinishedCallback;
		}

		public virtual void ResetStart()
		{
			remainingTime = duration;
		}

		public virtual void Stop()
		{
			remainingTime = -1;
		}

		public virtual void Finish()
		{
			Stop();
			OnFinished();
		}

		public virtual void Update()
		{
			if (remainingTime > 0)
			{
				remainingTime -= Time.deltaTime;
				if (remainingTime < 0)
				{
					OnFinished();
				}
			}
			else if (Mathf.Approximately(remainingTime, 0))
			{
				remainingTime = -1;
				OnFinished();
			}
		}

		private void OnFinished()
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