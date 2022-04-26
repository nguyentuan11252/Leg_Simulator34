
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
	[SerializeField] Text fpsText;

	float time = 0;
	float seconds = 0;
	int frames = 0;

	private void Awake()
	{
	}

	// Update is called once per frame
	void Update()
	{
		float now = Time.realtimeSinceStartup;
		float delta = now - time;
		time = now;
		if (delta > 0.2f)
		{ // jiter
			return;
		}
		seconds += delta;
		frames++;
		if (seconds >= 1.0f)
		{
			var s = (frames / seconds).ToString();
			if (fpsText != null)
			{
				fpsText.text = s;
			}
			
			seconds = 0;
			frames = 0;
		}
	}
}
