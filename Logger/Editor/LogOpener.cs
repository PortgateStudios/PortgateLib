using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PortgateLib.Logger
{
	public static class LogOpener
	{
		[MenuItem("PortgateLib/Open Last Log")]
		private static void OpenLastLog()
		{
			var files = Directory.GetFiles(Logger.LogDirectory).OrderBy(fileName => fileName);

			if (files.Count() <= 0)
			{
				Debug.LogError("There is no logs available");
				return;
			}

			Application.OpenURL(files.Last());
		}

		[MenuItem("PortgateLib/Open Log Directory")]
		private static void OpenLogDirectory()
		{
			Application.OpenURL(Logger.LogDirectory);
		}
	}
}