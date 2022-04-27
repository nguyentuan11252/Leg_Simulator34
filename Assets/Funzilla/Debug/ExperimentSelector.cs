using System.Collections.Generic;
using UnityEngine;

namespace Funzilla
{
	internal class ExperimentSelector : MonoBehaviour
	{
		[SerializeField] private ExperimentButton buttonPrefab;
		private List<ExperimentButton> _buttons;
		
		private void Start()
		{
			var experiments = ExperimentManager.Experiments;
			_buttons = new List<ExperimentButton>(experiments.Count);
			for (var i = transform.childCount; i < experiments.Count; i++)
			{
				Instantiate(buttonPrefab, transform);
			}
			
			for (var i = 0; i < experiments.Count; i++)
			{
				var button = transform.GetChild(i).GetComponent<ExperimentButton>();
				button.Init(experiments[i], this);
				_buttons.Add(button);
			}
			Refresh();
		}

		private void OnEnable()
		{
			Refresh();
		}

		private void Refresh()
		{
			if (_buttons == null) return;
			foreach (var button in _buttons)
			{
				button.SetActive(button.Experiment == ExperimentManager.ActiveExperiment);
			}
		}
	}
}

