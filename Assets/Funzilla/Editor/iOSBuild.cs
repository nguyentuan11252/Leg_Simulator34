
#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEngine;

namespace Funzilla
{
	public class iOSBuild
	{
		static string[] localizations = new string[] { "en", "zh-Hans", "zh-Hant", "fr", "de", "ja", "ko", "es" };

		[PostProcessBuild]
		public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget != BuildTarget.iOS)
			{
				return;
			}

			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-1261794022667010~7213790780");
			rootDict.SetString("NSUserTrackingUsageDescription", "NSUserTrackingUsageDescription");
			rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
			rootDict.AddDict("NSAppTransportSecurity").SetBoolean("NSAllowsArbitraryLoads", true);
			var adNetworkItems = rootDict.CreateArray("SKAdNetworkItems");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "su67r6k2v3.skadnetwork"); // Ironsource
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "4pfyvq9l8r.skadnetwork"); // AdColony
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "cstr6suwn9.skadnetwork"); // AdMob
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "ludvb6z3bs.skadnetwork"); // AppLovin
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "v9wttpbfk9.skadnetwork"); // Facebook
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "n38lu8286q.skadnetwork"); // Facebook
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "4dzt52r2t5.skadnetwork"); // Unity Ads
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "gta9lk7p23.skadnetwork"); // Vungle
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "f38h382jlk.skadnetwork"); // Chartboost
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "9g2aggbj52.skadnetwork"); // Fyber
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "wzmmz9fp6w.skadnetwork"); // InMobi
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "238da6jt44.skadnetwork"); // Pangle (CN)
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "22mmun2rn5.skadnetwork"); // Pangle (Non CN)
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "ecpz2srf59.skadnetwork"); // Tapjoy
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "KBD757YWX3.skadnetwork"); // Mintegral

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());

			// Configure project
			string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject project = new PBXProject();
			var file = File.ReadAllText(projPath);
			project.ReadFromString(file);
			string targetGuid = project.GetUnityMainTargetGuid();
			string frameworkGuid = project.GetUnityFrameworkTargetGuid();

			foreach (var localization in localizations)
			{
				var guid = project.AddFolderReference(
					$"{Application.dataPath}/../Build/iOS/Localizations/{localization}.lproj",
					$"{localization}.lproj",
					PBXSourceTree.Source);
				project.AddFileToBuild(targetGuid, guid);
				project.AddFileToBuild(frameworkGuid, guid);
			}

			File.WriteAllText(projPath, project.WriteToString());
		}
	}
}
#endif