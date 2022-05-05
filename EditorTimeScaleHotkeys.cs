#if (UNITY_EDITOR)
using UnityEditor;
using UnityEngine;

namespace PortgateLib
{
	[InitializeOnLoad]
	public static class EditorTimeScaleHotkeys
	{
		private static float originalTimeScale;

		static EditorTimeScaleHotkeys()
		{
			originalTimeScale = Time.timeScale;
			EditorApplication.playModeStateChanged += ModeChanged;
		}

		static void ModeChanged(PlayModeStateChange stateChange)
		{
			if (stateChange == PlayModeStateChange.ExitingEditMode)
			{
				originalTimeScale = Time.timeScale;
			}
			else if (stateChange == PlayModeStateChange.EnteredEditMode)
			{
				Time.timeScale = originalTimeScale;
			}
		}

		[MenuItem("PortgateLib/TimeScale/0.05 #_0")]
		private static void Shift0()
		{
			if (EditorApplication.isPlaying)
			{
				Time.timeScale = 0.05f;
			}
		}

		[MenuItem("PortgateLib/TimeScale/1 #_1")]
		private static void Shift1()
		{
			if (EditorApplication.isPlaying)
			{
				Time.timeScale = 1;
			}
		}

		[MenuItem("PortgateLib/TimeScale/2 #_2")]
		private static void Shift2()
		{
			if (EditorApplication.isPlaying)
			{
				Time.timeScale = 2;
			}
		}
	}
}
#endif