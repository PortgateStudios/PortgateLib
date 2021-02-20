using UnityEngine;

namespace PortgateLib.Timer
{
	public class Timer
	{
		public delegate void OnFinished();

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
		private OnFinished onFinishedCallback;
		private float elapsedTime;

		public Timer(float duration, OnFinished onFinishedCallback)
		{
			if (duration < 0)
			{
				Debug.Log("Time is negative.");
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
			onFinishedCallback();
		}

		public virtual void Update()
		{
			if (!IsRunning) return;
			if (currentTime > 0)
			{
				currentTime -= Time.deltaTime;
				elapsedTime += Time.deltaTime;
				if (currentTime < 0 && onFinishedCallback != null)
				{
					onFinishedCallback();
				}
			}
		}

		protected void SetOnFinishedCallback(OnFinished onFinishedCallback)
		{
			this.onFinishedCallback = onFinishedCallback;
		}
	}
}