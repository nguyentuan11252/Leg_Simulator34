
using UnityEngine;

namespace Funzilla
{
	class PopupAnimation : MonoBehaviour
	{
		[SerializeField] protected Popup popup = null;
		public virtual void AnimateIn()
		{
			SceneManager.Instance.OnSceneAnimatedIn(popup);
		}

		public virtual void AnimateOut()
		{
			SceneManager.Instance.OnSceneAnimatedOut(popup);
		}
	}
}