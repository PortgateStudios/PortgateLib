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
			get { return remainingDuration >= 0; }
		}

		public override float RemainingTime
		{
			get { return remainingDuration; }
		}

		public override float RemainingPercent
		{
			get
			{
				if (Mathf.Approximately(Duration, 0))
					return 0;
				else
					return RemainingTime / Duration;
			}
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
		private float remainingDuration = -1;
		private Action onFinishedCallback;

		public CyclicTimer(float duration, float cycleTime, Action onTickCallback, Action onFinishedCallback = null) : base(cycleTime, onTickCallback)
		{
			if (duration < 0)
			{
				throw new Exception("Duration is negative!");
			}
			this.duration = duration;
			this.onFinishedCallback = onFinishedCallback;
		}

		public override void ResetStart()
		{
			base.ResetStart();
			this.remainingDuration = duration;
		}

		public override void Stop()
		{
			this.remainingDuration = -1;
		}

		public override void Finish()
		{
			Stop();
			OnFinished();
		}

		public override void Update()
		{
			if (remainingDuration > 0)
			{
				remainingDuration -= Time.deltaTime;
				base.Update();
				if (remainingDuration < 0)
				{
					OnFinished();
				}
			}
			else if (Mathf.Approximately(remainingDuration, 0))
			{
				remainingDuration = -1;
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
	}
}