
using UnityEngine;

namespace Funzilla
{
	internal class SceneBase : MonoBehaviour
	{
		// Should this scene be destroyed after it's closed?
		[SerializeField] private bool aliveAfterClose;
		internal bool AliveAfterClose => aliveAfterClose;

		internal SceneID ID { get; set; }

		internal virtual void Init(object data)
		{

		}

		internal virtual void OnBackButtonPressed()
		{

		}

		internal virtual void AnimateIn()
		{
			SceneManager.Instance.OnSceneAnimatedIn(this);
		}

		internal virtual void AnimateOut()
		{
			SceneManager.Instance.OnSceneAnimatedOut(this);
		}
	}
}