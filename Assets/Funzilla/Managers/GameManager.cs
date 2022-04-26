
using Facebook.Unity;
using UnityEngine;
using Firebase;
using Firebase.Analytics;

namespace Funzilla
{
	internal class GameManager : Singleton<GameManager>
	{
		internal static bool FirebaseOk { get; private set; }
		private void Awake()
		{

			Application.targetFrameRate = 60;
			FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
			{
				if (task.Result != DependencyStatus.Available) return;
				FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
				FirebaseOk = true;
			});
			FB.Init();
		}

		private void Update()
		{
			if (!FirebaseOk) return;
			enabled = false;
			Config.Instance.Init();
			Adjust.Instance.Init();
			SceneManager.Instance.OpenScene(SceneID.Gameplay);
		}
	}
}