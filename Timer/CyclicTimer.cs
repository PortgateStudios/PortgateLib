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

		public override float ElapsedTime
		{
			get { return duration - remainingDuration; }
		}

		public override float ElapsedPercent
		{
			get { return Mathf.Max(0, ElapsedTime / duration); }
		}

		public override bool IsRunning
		{
			get { return remainingDuration > 0; }
		}

		private readonly float duration;
		private float cycleTime = -1;
		private float remainingDuration = -1;
		private Action onFinishedCallback;

		public CyclicTimer(float duration, float cycleTime, OnTick onTickCallback, Action onFinishedCallback = null) : base(cycleTime, onTickCallback)
		{
			if (duration <= 0)
			{
				throw new Exception("Duration must be positive!.");
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
			if (IsRunning)
			{
				remainingDuration -= Time.deltaTime;
				base.Update();
				if (remainingDuration <= 0)
				{
					OnFinished();
				}
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