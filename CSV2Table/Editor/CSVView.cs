using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace PortgateLib.CSV2Table
{
	public class CSVView : EditorWindow
	{
		private TextAsset csv;
		private string[][] arr;

		[MenuItem("Window/CSV View")]
		public static void ShowWindow()
		{
			//Show existing window instance. If one doesn't exist, make one.
			EditorWindow.GetWindow(typeof(CSVView));
		}

		void OnGUI()
		{
			var newCsv = EditorGUILayout.ObjectField("CSV", csv, typeof(TextAsset), false) as TextAsset;
			if (newCsv != csv)
			{
				csv = newCsv;
				arr = CsvParser2.Parse(csv.text);
			}
			if (GUILayout.Button("Refresh") && csv != null)
				arr = CsvParser2.Parse(csv.text);

			if (csv == null)
				return;

			if (arr == null)
				arr = CsvParser2.Parse(csv.text);

			for (int i = 0; i < arr.Length; i++)
			{
				EditorGUILayout.BeginHorizontal();
				for (int j = 0; j < arr[i].Length; j++)
				{
					EditorGUILayout.TextField(arr[i][j]);
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}