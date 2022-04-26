using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace UnityToolbag
{
	internal static class UnityConstantsGenerator
	{
		[MenuItem("Tools/Update ProjectConstants.cs")]
		private static void Generate()
		{
			// Try to find an existing file in the project called "UnityConstants.cs"
			var filePath = string.Empty;
			foreach (var file in Directory.GetFiles(Application.dataPath, "*.cs",
				SearchOption.AllDirectories))
			{
				if (Path.GetFileNameWithoutExtension(file) != "ProjectConstants") continue;
				filePath = file;
				break;
			}

			// If no such file exists already, use the save panel to get a folder in which the file will be placed.
			if (string.IsNullOrEmpty(filePath))
			{
				var directory = EditorUtility.OpenFolderPanel(
					"Choose location for file ProjectConstants.cs", Application.dataPath, "");

				// Canceled choose? Do nothing.
				if (string.IsNullOrEmpty(directory))
				{
					return;
				}

				filePath = Path.Combine(directory, "ProjectConstants.cs");
			}

			// Write out our file
			using (var writer = new StreamWriter(filePath))
			{
				writer.WriteLine("// This file is auto-generated. Modifications are not saved.");
				writer.WriteLine();
				writer.WriteLine("namespace Funzilla");
				writer.WriteLine("{");

				// Write out the Enum
				writer.WriteLine("	internal enum SceneID");
				writer.WriteLine("	{");
				foreach (var t in EditorBuildSettings.scenes)
				{
					var scene = Path.GetFileNameWithoutExtension(t.path);
					writer.WriteLine("		{0},", scene);
				}

				writer.WriteLine("		END");
				writer.WriteLine("	}");
				writer.WriteLine();

				// Write out layers
				writer.WriteLine("	internal static class Layers");
				writer.WriteLine("	{");
				for (var i = 0; i < 32; i++)
				{
					var layer = UnityEditorInternal.InternalEditorUtility.GetLayerName(i);
					if (!string.IsNullOrEmpty(layer))
					{
						writer.WriteLine("		internal const int {0} = {1};", MakeSafeForCode(layer),
							i);
					}
				}

				writer.WriteLine();
				for (var i = 0; i < 32; i++)
				{
					var layer = UnityEditorInternal.InternalEditorUtility.GetLayerName(i);
					if (!string.IsNullOrEmpty(layer))
					{
						writer.WriteLine("		internal const int {0}Mask = 1 << {1};",
							MakeSafeForCode(layer), i);
					}
				}

				writer.WriteLine("	}");
				writer.WriteLine();

				// Write out scenes' names
				writer.WriteLine("	internal static class SceneNames");
				writer.WriteLine("	{");
				writer.WriteLine("		internal const string INVALID_SCENE = \"InvalidScene\";");

				// Scenes' names in an array
				writer.WriteLine("		internal static readonly string[] ScenesNameArray = {");
				for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
				{
					var scene =
						Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
					writer.WriteLine(
						i == EditorBuildSettings.scenes.Length - 1 ? "			\"{0}\"" : "			\"{0}\",",
						scene);
				}

				writer.WriteLine("		};");

				//write method to get scene name from enum
				writer.WriteLine("		internal static string GetSceneName(SceneID scene) {");
				writer.WriteLine("			var index = (int)scene;");
				writer.WriteLine("			if(index > 0 && index < ScenesNameArray.Length) {");
				writer.WriteLine("				return ScenesNameArray[index];");
				writer.WriteLine("			} else {");
				writer.WriteLine("				return INVALID_SCENE;");
				writer.WriteLine("			}");
				writer.WriteLine("		}");

				writer.WriteLine("	}");
				writer.WriteLine();

				// Write static function to get scene name string from enum
				writer.WriteLine("	internal static class ExtentionHelpers {");
				writer.WriteLine("		internal static string GetName(this SceneID scene) {");
				writer.WriteLine("			  return SceneNames.GetSceneName(scene);");
				writer.WriteLine("		}");
				writer.WriteLine("	}");

				// End of namespace UnityConstants
				writer.WriteLine("}");
				writer.WriteLine();
			}

			// Refresh
			AssetDatabase.Refresh();

			Debug.Log("Project Constants successfully generated");
		}

		private static string MakeSafeForCode(string str)
		{
			str = Regex.Replace(str, "[^a-zA-Z0-9_]", "_", RegexOptions.Compiled);
			if (char.IsDigit(str[0]))
			{
				str = "_" + str;
			}

			return str;
		}
	}
}