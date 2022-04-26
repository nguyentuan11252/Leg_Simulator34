
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	class LoadingShield : SceneShield
	{
		[SerializeField] Image icon;

		Tween rotateTween;
		Tween iconTween;

		void StopIconTween()
		{
			if (iconTween != null)
			{
				iconTween.Kill();
			}
		}

		public new void Hide()
		{
			base.Hide();
			StopIconTween();
			iconTween = icon.DOFade(0.0f, 0.5f);
			iconTween.onComplete = ()=> {
				rotateTween.Pause();
			};
		}

		public void Show(bool loadingAnimationEnabled = true, float opacity = 0.7f, Action onComplete = null)
		{
			Show(opacity, onComplete);
			icon.gameObject.SetActive(loadingAnimationEnabled);
			if (loadingAnimationEnabled)
			{
				rotateTween.Restart();
				StopIconTween();
				icon.DOFade(1.0f, 0.5f);
			}
		}

		private void Awake()
		{
			rotateTween = icon.transform.DORotate(new Vector3(0, 0, 360), 1, RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
			rotateTween.Pause();
		}
	}
}