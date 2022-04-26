using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Funzilla
{
	internal class Profile : Singleton<Profile>
	{
		private const string Passphase = "Nama1234";
		private const string SaveFile = "/save.dat";

		[Serializable]
		private class UserData
		{
			// Soft currency
			[SerializeField] internal int nCoins;

			// Level
			[SerializeField] internal int level = 1;
			[SerializeField] internal int playCount;
			[SerializeField] internal int nTablets = 1;

			// First open time
			[SerializeField] internal string firstTime;

			[SerializeField] internal List<string> skins = new List<string>();
			[SerializeField] internal int currentSkin;
		}

		private UserData _data;
		private bool _vip;

		internal bool Vip
		{
			get => _vip;
			set
			{
				if (_vip == value)
				{
					return;
				}

				_vip = value;
				EventManager.Instance.Annouce(EventType.VipChanged);
			}
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			LoadLocal();
		}

		internal int CurrentSkinIndex
		{
			get => _data?.currentSkin ?? 0;
			set
			{
				if (_data == null || value < 0 || value >= _data.skins.Count) return;
				_data.currentSkin = value;
				RequestSave();
			}
		}

		internal string CurrentSkin
		{
			get
			{
				if (_data?.skins == null ||
					_data.currentSkin < 0 ||
					_data.currentSkin >= _data.skins.Count)
					return string.Empty;

				return _data.skins[_data.currentSkin];
			}
			
			set
			{
				if (_data?.skins == null)
					return;

				var index = _data.skins.IndexOf(value);
				if (index < 0 || index >= _data.skins.Count)
					return;

				_data.currentSkin = index;
				RequestSave();
			}
		}

		internal List<string> Skins => _data?.skins;

		internal void UnlockSkin(string skin)
		{
			if (_data == null) return;
			if (string.IsNullOrEmpty(skin)) return;
			if (_data.skins.Contains(skin)) return;
			_data.currentSkin = _data.skins.Count;
			_data.skins.Add(skin);
			RequestSave();
		}

		internal int CoinAmount
		{
			get => _data?.nCoins ?? 0;
			set
			{
				if (_data == null)
				{
					return;
				}

				_data.nCoins = value;
				EventManager.Instance.Annouce(EventType.CoinAmountChanged);
				RequestSave();
			}
		}
		
		internal int Level
		{
			get => _data?.level ?? 1;
			set
			{
				if (_data == null) return;
				_data.level = value < 1 ? 1 : value;
				RequestSave();
			}
		}

		internal int PlayCount
		{
			get => _data?.playCount ?? 0;
			set
			{
				if (_data == null) return;
				_data.playCount = value;
				RequestSave();
			}
		}

		internal int TabletAmount
		{
			get => _data?.nTablets ?? 1;
			set
			{
				if (_data == null) return;
				_data.nTablets = value;
				RequestSave();
			}
		}

		internal DateTime FirstOpenTime
		{
			get
			{
				if (_data == null || string.IsNullOrEmpty(_data.firstTime)) return DateTime.Now;
				try
				{
					return DateTime.Parse(_data.firstTime);
				}
				catch
				{
					// ignored
				}

				return DateTime.Now;
			}
		}

		private void LoadLocal()
		{
			try
			{
				TextReader tr = new StreamReader(Application.persistentDataPath + SaveFile);
				var encryptedJson = tr.ReadToEnd();
				tr.Close();

				var json = Security.Decrypt(encryptedJson, Passphase);
				_data = JsonUtility.FromJson<UserData>(json);
			}
			catch
			{
				// ignored
			}

			if (_data == null)
			{
				_data = new UserData {firstTime = DateTime.Now.ToString(CultureInfo.InvariantCulture)};
				RequestSave();
			}

			_data.skins ??= new List<string>();
			if (_data.skins.Count <= 0)
			{
				//_data.skins.Add("Humanoid");
				_data.skins.Add("Boy");
				//_data.skins.Add("Girl");
				_data.currentSkin = 0;
				RequestSave();
			}

			if (_data.level < 1)
			{
				_data.level = 1;
				RequestSave();
			}

			if (_data.nTablets < 1)
			{
				_data.nTablets = 1;
				RequestSave();
			}
		}

		private bool _modifed;

		private void RequestSave()
		{
			_modifed = true;
		}

		private void Update()
		{
			if (!_modifed) return;
			_modifed = false;
			SaveLocal();
		}

		private void SaveLocal()
		{
			try
			{
				var json = JsonUtility.ToJson(_data);
				var encryptedJson = Security.Encrypt(json, Passphase);

				TextWriter tw = new StreamWriter(Application.persistentDataPath + SaveFile);
				tw.Write(encryptedJson);
				tw.Close();
			}
			catch
			{
				// ignored
			}
		}
	}
}