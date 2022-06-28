using System;
using UnityEngine;

namespace PortgateLib.Timer
{
	public class CyclicTimer : InfiniteTimer
	{
		public override float Duration
		{
			get { return duration; }
		}

		public override bool IsRunning
		{
			get { return isRunning; }
		}

		public override float RemainingTime
		{
			get { return remainingDuration; }
		}

		public override float RemainingPercent
		{
			get { return GetRemainingPercent(); }
		}

		public override float ElapsedTime
		{
			get { return Duration - RemainingTime; }
		}

		public override float ElapsedPercent
		{
			get { return 1f - RemainingPercent; }
		}

		private readonly float duration;
		private float remainingDuration;
		private Action onFinishedCallback;
		private bool isRunning;

		public CyclicTimer(float duration, float cycleTime, Action onTickCallback, Action onFinishedCallback = null) : base(cycleTime, onTickCallback)
		{
			if (duration < 0)
			{
				throw new Exception("Duration is negative!");
			}
			this.duration = duration;
			this.remainingDuration = duration;
			this.onFinishedCallback = onFinishedCallback;
		}

		public override void ResetStart()
		{
			base.ResetStart();
			Reset();
			isRunning = true;
		}

		public override void Stop()
		{
			Reset();
			isRunning = false;
		}

		// not public, otherwise I would have to implement it at all the different Timers.
		private void Reset()
		{
			remainingDuration = duration;
		}

		// todo: Pause(), Start()

		public override void Finish()
		{
			OnFinished();
		}

		public override void Update()
		{
			if (isRunning)
			{
				remainingDuration -= Time.deltaTime;
				base.Update();
				if (remainingDuration < 0)
				{
					OnFinished();
				}
			}
		}

		private void OnFinished()
		{
			remainingDuration = 0;
			isRunning = false;
			onFinishedCallback?.Invoke();
		}

		private float GetRemainingPercent()
		{
			if (Mathf.Approximately(Duration, 0))
				return 0;
			else
				return RemainingTime / Duration;
		}
	}
}