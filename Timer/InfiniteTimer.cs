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

		public override bool IsRunning
		{
			get { return !stopped; }
		}

		[Obsolete("Do not use this for calculations. Ticks happen at irregular intervals, CycleTime is just a desired interval, not the real. Use ElapsedCycleTime.", true)] // todo: do something with this
		public float CycleTime
		{
			get { return cycleTime; }
		}

		private readonly float cycleTime = -1;
		private float elapsedTime = 0;
		private bool stopped = true;
		private Action onTickCallback;

		public InfiniteTimer(float cycleTime, Action onTickCallback) : base(cycleTime, null)
		{
			if (cycleTime <= 0)
			{
				throw new Exception("CycleTime must be positive!.");
			}
			this.cycleTime = cycleTime;
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