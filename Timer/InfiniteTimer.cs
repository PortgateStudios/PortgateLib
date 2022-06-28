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
			get { return isRunning; }
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
		private bool isRunning;
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
			elapsedTime = 0;
		}

		// todo: Pause(), Start()

		public override void Finish()
		{
			Stop();
		}

		public override void Update()
		{
			if (IsRunning)
			{
				base.Update();
				elapsedTime += Time.deltaTime;
			}
		}

		private void OnCycleEnded()
		{
			onTickCallback();
			base.ResetStart();
		}
	}
}