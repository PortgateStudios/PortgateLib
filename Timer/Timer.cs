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
			get { return isRunning; }
		}

		public virtual float RemainingTime
		{
			get { return remainingTime; }
		}

		public virtual float RemainingPercent
		{
			get { return GetRemainingPercent(); }
		}

		public virtual float ElapsedTime
		{
			get { return duration - remainingTime; }
		}

		public virtual float ElapsedPercent
		{
			get { return 1f - RemainingPercent; }
		}

		private readonly float duration;
		private float remainingTime;
		private Action onFinishedCallback;
		private bool isRunning;

		public Timer(float duration, Action onFinishedCallback = null)
		{
			if (duration < 0)
			{
				throw new Exception("Duration is negative!");
			}
			this.duration = duration;
			this.remainingTime = duration;
			this.onFinishedCallback = onFinishedCallback;
		}

		public virtual void ResetStart()
		{
			Reset();
			isRunning = true;
		}
	
		public virtual void Stop()
		{
			Reset();
			isRunning = false;
		}

		// not public, otherwise I would have to implement it at all the different Timers.
		private void Reset()
		{
			remainingTime = duration;
		}

		// todo: Pause(), Start()

		public virtual void Finish()
		{
			OnFinished();
		}

		public virtual void Update()
		{
			if (isRunning)
			{
				remainingTime -= Time.deltaTime;
				if (remainingTime < 0)
				{
					OnFinished();
				}
			}
		}

		private void OnFinished()
		{
			remainingTime = 0;
			isRunning = false;
			onFinishedCallback?.Invoke();
		}

		protected void SetOnFinishedCallback(Action onFinishedCallback)
		{
			this.onFinishedCallback = onFinishedCallback;
		}

		private float GetRemainingPercent()
		{
			if (Mathf.Approximately(duration, 0))
				return 0;
			else
				return remainingTime / duration;
		}
	}
}