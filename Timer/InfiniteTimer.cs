using System;
using UnityEngine;

namespace PortgateLib.Timer
{
	public class InfiniteTimer : Timer
	{
		public override float Duration
		{
			get { throw new Exception("Duration is invalid for InfiniteTimer."); }
		}

		public override bool IsRunning
		{
			get { return !stopped; }
		}

		public override float RemainingTime
		{
			get { throw new Exception("RemainingTime is invalid for InfiniteTimer."); }
		}

		public override float RemainingPercent
		{
			get { throw new Exception("RemainingPercent is invalid for InfiniteTimer."); }
		}

		public override float ElapsedTime
		{
			get { return elapsedTime; }
		}

		public override float ElapsedPercent
		{
			get { throw new Exception("ElapsedPercent is invalid for InfiniteTimer."); }
		}

		public float ElapsedCycleTime
		{
			get { return base.ElapsedTime; }
		}

		public float ElapsedCyclePercent
		{
			get { return base.ElapsedPercent; }
		}

		private float elapsedTime = 0;
		private bool stopped = true;
		private Action onTickCallback;

		public InfiniteTimer(float cycleTime, Action onTickCallback) : base(cycleTime, null)
		{
			if (cycleTime < 0)
			{
				throw new Exception("CycleTime is negative!");
			}
			this.onTickCallback = onTickCallback;
			SetOnFinishedCallback(OnCycleEnded);
		}

		public override void ResetStart()
		{
			base.ResetStart();
			elapsedTime = 0;
			stopped = false;
		}

		public override void Stop()
		{
			stopped = true;
		}

		public override void Finish()
		{
			Stop();
		}

		public override void Update()
		{
			if (!IsRunning) return;
			base.Update();
			elapsedTime += Time.deltaTime;
		}

		private void OnCycleEnded()
		{
			onTickCallback();
			base.ResetStart();
		}
	}
}