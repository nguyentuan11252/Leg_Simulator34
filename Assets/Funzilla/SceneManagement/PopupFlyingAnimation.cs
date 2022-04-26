
using UnityEngine;

namespace Funzilla
{
	class PopupFlyingAnimation : PopupAnimation
	{
		[SerializeField] Transform target = null;
		[SerializeField] Vector2 from;
		[SerializeField] Vector2 to;
		[SerializeField] readonly float duration = 0.5f;

		float time;
		float direction;
		float targetTime;

		public override void AnimateIn()
		{
			if (target == null || duration <= 0)
			{
				SceneManager.Instance.OnSceneAnimatedIn(popup);
			}
			else
			{
				transform.localPosition = from;
				time = 0;
				direction = 1;
				targetTime = duration;
				enabled = true;
			}
		}

		public override void AnimateOut()
		{
			if (target == null || duration <= 0)
			{
				SceneManager.Instance.OnSceneAnimatedOut(popup);
			}
			else
			{
				transform.localPosition = to;
				time = duration;
				direction = -1;
				targetTime = 0;
				enabled = true;
			}
		}

		private void Update()
		{
			time += Time.smoothDeltaTime * direction;
			bool done = time * direction > targetTime;
			if (done)
			{
				time = targetTime;
			}

			float t = 1 - time / duration;
			t *= t;

			target.localPosition = Vector2.Lerp(to, from, t);

			if (done)
			{
				if (direction > 0)
				{
					SceneManager.Instance.OnSceneAnimatedIn(popup);
				}
				else
				{
					SceneManager.Instance.OnSceneAnimatedOut(popup);
				}
				enabled = false;
			}
		}
	}
}