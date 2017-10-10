using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Display info about the current wave.
/// </summary>
public class WavePanel_UI : MonoBehaviour {

		public Text LevelName;
		public Text CurrentWave;
		public Text MaxWave;
		public Text WaveName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

		public void SetCurrentDisplayWave(string levelname, int currentWave, int maxWaves, string waveName)
		{
				SetLevelName (levelname);
				SetCurrentWave (currentWave);
				SetMaxWave (maxWaves);
				SetWaveName (waveName);
		}

		public void SetLevelName(string levelname)
		{
				if (LevelName == null)
						return;
				
				LevelName.text = string.Format("Level:\n{0}", levelname);
		}

		public void SetCurrentWave(int currentWave)
		{
				if (CurrentWave == null)
						return;

				CurrentWave.text = string.Format ("{0}", currentWave);

		}

		public void SetMaxWave(int maxWaves)
		{
				if (MaxWave == null)
						return;

				MaxWave.text = string.Format ("{0}", maxWaves);
		}

		public void SetWaveName(string waveName)
		{
				if (WaveName == null)
						return;

				WaveName.text = string.Format("{0}", waveName);
		}

}
