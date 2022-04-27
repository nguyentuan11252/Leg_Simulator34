using System;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class ExperimentButton : MonoBehaviour
	{
		[SerializeField] private Text text;
		[SerializeField] private Button button;

		internal ExperimentType Experiment { get; private set; }

		internal void Init(ExperimentType experiment, ExperimentSelector selector)
		{
			Experiment = experiment;
			text.text = experiment.ToString();
			button.onClick.AddListener(()=>
			{
				ExperimentManager.ChangeTest(experiment);
				selector.gameObject.SetActive(false);
			});
		}

		internal void SetActive(bool active)
		{
			button.interactable = !active;
		}
	}
}