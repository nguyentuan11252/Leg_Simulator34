using UnityEngine;

namespace Funzilla
{
	internal class Preference : Singleton<Preference>
	{
		private const string OptionSfx = "Sfx";
		private const string OptionMusic = "Music";
		private const string OptionVibration = "Vibration";

		private void Awake()
		{
			_sfxOn = PlayerPrefs.GetInt(OptionSfx, 1) > 0;
			_vibrationOn = PlayerPrefs.GetInt(OptionVibration, 1) > 0;
		}

		private bool _sfxOn = true;
		public bool SfxOn
		{
			get => _sfxOn;
			set
			{
				_sfxOn = value;
				PlayerPrefs.SetInt(OptionSfx, _sfxOn ? 1 : 0);
			}
		}

		private bool _musicOn = true;
		public bool MusicOn
		{
			get => _musicOn;
			set
			{
				_musicOn = value;
				PlayerPrefs.SetInt(OptionMusic, _musicOn ? 1 : 0);
			}
		}

		private bool _vibrationOn = true;
		public bool VibrationOn
		{
			get => _vibrationOn;
			set
			{
				_vibrationOn = value;
				PlayerPrefs.SetInt(OptionVibration, _vibrationOn ? 1 : 0);
			}
		}
	}
}