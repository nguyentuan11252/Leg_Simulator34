
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	class SceneShield : MonoBehaviour
	{
		[SerializeField] Image image;
		Tweener tween = null;

		private void Awake()
		{
			image.color = new Color(0, 0, 0, 0);
		}

		void StopTween()
		{
			if (tween != null)
			{
				tween.Kill();
			}
		}

		public void Hide()
		{
			StopTween();
			tween = image.DOFade(0, 0.5f);
			tween.onComplete = () => {
				gameObject.SetActive(false);
			};
		}

		public void Show(float opacity = 0.8f, Action onComplete = null)
		{
			StopTween();
			gameObject.SetActive(true);
			tween = image.DOFade(opacity, 0.5f).OnComplete(()=> { onComplete?.Invoke(); });
		}
	}
}