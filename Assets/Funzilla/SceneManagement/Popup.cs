
using UnityEngine;

namespace Funzilla
{
	internal class Popup : SceneBase
	{
		[SerializeField] private new PopupAnimation animation = null;

		internal override void AnimateIn()
		{
			if (animation)
			{
				animation.AnimateIn();
			}
			else
			{
				SceneManager.Instance.OnSceneAnimatedIn(this);
			}
		}

		internal override void AnimateOut()
		{
			if (animation)
			{
				animation.AnimateOut();
			}
			else
			{
				SceneManager.Instance.OnSceneAnimatedOut(this);
			}
		}
	}
}