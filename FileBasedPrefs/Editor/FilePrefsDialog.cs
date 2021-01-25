using UnityEngine;
using UnityEditor;

namespace FileBasedPrefs
{
	public class FilePrefsDialog : EditorWindow
	{
		private string resultMessage = "";
		private string setKeyText = "Enter key...";
		private string setKeyValueText = "Enter value...";
		private string deleteKeyText = "Enter key...";

		void OnGUI()
		{
			var centeredStyle = new GUIStyle();
			centeredStyle.alignment = TextAnchor.MiddleCenter;
			centeredStyle.fontStyle = FontStyle.Bold;

			GUILayout.Space(5);
			EditorGUILayout.LabelField("Setting keys", centeredStyle);
			GUILayout.Space(5);

			setKeyText = EditorGUILayout.TextField("Key: ", setKeyText);
			setKeyValueText = EditorGUILayout.TextField("Value: ", setKeyValueText);
			if (GUILayout.Button("Set Key"))
			{
				if (int.TryParse(setKeyValueText, out var intValue))
				{
					FilePrefs.SetInt(setKeyText, intValue);
					resultMessage = $"Key '{setKeyText}' has been set as {intValue}. (int)";
				}
				else if (float.TryParse(setKeyValueText, out var floatValue))
				{
					FilePrefs.SetFloat(setKeyText, floatValue);
					resultMessage = $"Key '{setKeyText}' has been set as {floatValue}. (float)";
				}
				else if (bool.TryParse(setKeyValueText, out var boolValue))
				{
					FilePrefs.SetBool(setKeyText, boolValue);
					resultMessage = $"Key '{setKeyText}' has been set as {boolValue}. (bool)";
				}
				else
				{
					FilePrefs.SetString(setKeyText, setKeyValueText);
					resultMessage = $"Key '{setKeyText}' has been set as '{setKeyValueText}'. (string)";
				}
			}

			GUILayout.Space(15);
			EditorGUILayout.LabelField("Deleting keys", centeredStyle);
			GUILayout.Space(5);

			deleteKeyText = EditorGUILayout.TextField("Key: ", deleteKeyText);
			if (GUILayout.Button("Delete Key"))
			{
				if (FilePrefs.HasKey(deleteKeyText))
				{
					FilePrefs.DeleteKey(deleteKeyText);
					resultMessage = $"Key '{deleteKeyText}' has been deleted.";
				}
				else
				{
					resultMessage = $"Key '{deleteKeyText}' not found!";
				}
			}

			GUILayout.Space(15);
			EditorGUILayout.LabelField("Deleting all keys", centeredStyle);
			GUILayout.Space(5);

			if (GUILayout.Button("Delete All"))
			{
				FilePrefs.DeleteAll();
				resultMessage = "All keys have been deleted.";
			}

			GUILayout.Space(15);

			EditorGUILayout.LabelField(resultMessage, centeredStyle);
		}

		[MenuItem("File Based Prefs/Open dialog")]
		private static void CreateFilePrefsDialog()
		{
			var window = ScriptableObject.CreateInstance<FilePrefsDialog>();
			window.ShowUtility();
		}
	}
}