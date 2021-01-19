using UnityEngine;

namespace PortgateLib.Timer
{
	public class CyclicTimer : InfiniteTimer
	{
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
		private OnFinished onFinishedCallback;

		public CyclicTimer(float duration, float cycleTime, OnTick onTickCallback, OnFinished onFinishedCallback) : base(cycleTime, onTickCallback)
		{
			if (duration < 0)
			{
				Debug.Log("Duration is negative.");
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
			onFinishedCallback();
		}

		public override void Update()
		{
			if (!IsRunning) return;
			remainingDuration -= Time.deltaTime;
			base.Update();
			if (remainingDuration < 0)
			{
				onFinishedCallback();
			}
		}
	}
}