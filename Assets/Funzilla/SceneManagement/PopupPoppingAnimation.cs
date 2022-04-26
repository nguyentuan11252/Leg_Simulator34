
using DG.Tweening;
using UnityEngine;

namespace Funzilla
{
	class PopupPoppingAnimation : PopupAnimation
	{
		float duration = 0.3f;

		public override void AnimateIn()
		{
			transform.localScale = Vector3.zero;
			transform.DOScale(1, duration).SetEase(Ease.OutBack).OnComplete(()=> {
				SceneManager.Instance.OnSceneAnimatedIn(popup);
			});
		}

		public override void AnimateOut()
		{
			transform.localScale = Vector3.one;
			transform.DOScale(0, duration).SetEase(Ease.InBack).OnComplete(() => {
				SceneManager.Instance.OnSceneAnimatedOut(popup);
			});
		}
	}
}