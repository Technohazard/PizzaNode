using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

/// Wave Spawner
public class wave_spawner : MonoBehaviour {

	/// <summary>
	/// Name of the Level (collection of 'enemy_wave's)
	/// </summary>
	public string levelName;

	/// <summary>
	/// List of (child) objects with enemy_wave scripts.
	/// </summary>
	public List<enemy_wave> WaveList; 

	/// <summary>
	/// The prefab cache for enemy prefabs.
	/// </summary>
	private PrefabCache _prefabCache;

	/// <summary>
	/// Index of current wave
	/// </summary>
	public int CurrentWave = 0; 

	// Wave UI Panel
	public WavePanel_UI WavePanel;
	public MessageDisplayPanel_UI MessageDisplayPanel;
	
	public enum WaveSpawnerStates {
									Paused, 
									WaveStart, 
									ShowText, 
									Spawn, 
									AIControl, 
									WaveOver, 
									Next, 
									PlayerDied
									};
	/*
	 * States:
	 * Paused - not doing anything
	 * WaveStart - do the needful to start wave
	 * ShowText - intro / shows wave text for current wave
	 * Spawn - spawns ships into wave data structures, etc.
	 * AIControl - control active ships, checks to see if wave is over
	 * WaveOver - clean up, display over text
	 * Next - Advance to next wave, pause
	 * PlayerDied - gg get rekt
	 */

	/// <summary>
	/// Game state - progression of the wave controller's internal behavior when controlling waves.
	/// </summary>
	public WaveSpawnerStates State = WaveSpawnerStates.Paused;

	// private List<GameObject> wave_list = new List<GameObject>(); 
	
	private enemy_wave WaveScript; // reference to current wave script component.

	public KeyCode keys_NextWave = KeyCode.N; // advance 1 wave
	public KeyCode keys_NextWavePhase = KeyCode.P; // advance 1 wave phase (Debug Only!) #Debug
	
	// Sound related.
	private AudioSource _AudioSource; 
	public AudioClip snd_wave; // sound of next wave
	public AudioClip snd_state; // sound of wave state advancing

	void Awake()
	{
		// Game_Manager.Instance.DoSomething();
		// Debug.Log(Game_Manager.Instance.myGlobalVar);
	}


	// Use this for initialization
	void Start () 
	{
		_AudioSource = GetComponent<AudioSource> ();

		RegisterChildWaves ();
	}

	/// <summary>
	/// Get list of all child wave objects for future reference.
	/// </summary>
	void RegisterChildWaves ()
	{
		int temp_num = 0;

		foreach (Transform child in transform) 
		{
			enemy_wave tmpWave = child.GetComponent<enemy_wave> ();

			if (tmpWave == null)
			{
					Debug.Log ("child didn't have a Wave script.");
					continue;
			}
			
			// While this isn't the "official" index, it indicates in which order a wave was processed.
			// if there's a discrepancy between this and assigned wave index, we might have a problem.
			// in the future will probably use the explicit wave index rather than the child order.
			// as we'll want to store wave data in json or something serializable
			tmpWave.assignedIndex = temp_num;
			WaveList.Add(tmpWave);
			temp_num++;
		}
	}
	
		/// <summary>
		/// Poll input for next wave/phase key.
		/// </summary>
	private void HandleKeyInput ()
	{
		if (Input.GetKey (keys_NextWave))
		{
			// Force an advance the wave spawner state.
			Debug.Log("Forcing Wave Spawner state to advance.");
			AdvanceState();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		HandleKeyInput();

		switch (State)
		{
			case WaveSpawnerStates.Paused:
				{
					// Do nothing while paused.
				}
				break;

			case WaveSpawnerStates.ShowText:
				{
								// TODO: should wait until text goes away (poll?) then advance.
								// HOWEVER, an optimal strategy would be to trigger a callback on this spawner to advance
								// when the text in question fades away. (callback?)(queue of delegates that get dequeued and called
								// at the same time as the message display?

								// For now, just advance immediately.
								AdvanceState ();
				}
				break;

			case WaveSpawnerStates.AIControl:
				{
					// Can't do anything if we have no wavescript to work with.
					if (WaveScript == null)
							return;
	
					if (WaveScript.State != enemy_wave.WaveStates.Dead) 
					{
						// enemy wave is still being controlled
						// check to make sure there are still doods alive in the wave.
						if (CheckForDeadWave () == true) 
						{
							// no dudes left alive in the wave.
							WaveScript.State = enemy_wave.WaveStates.Dead;
						}
					}
					else 
					{
						Debug.Log ("Wave_Spawner detected dead enemy wave. Advancing wave state");
						AdvanceState ();
					}
				}
				break;
		} // end switch
	}

		/// <summary>
		/// Count the number of children tagged "enemy" remaining in this wave.
		/// TODO: This is terrible. Waves should control their children. call a method on them to get dead status.
		/// </summary>
		/// <returns><c>true</c>, if for dead wave was checked, <c>false</c> otherwise.</returns>
	private bool CheckForDeadWave()
	{
		int countEnemies = 0;

		foreach (Transform t in transform)
		{
			if (t.CompareTag("Enemy"))
			{
				countEnemies++;
			}
		}

		if (countEnemies > 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	/// <summary>
	/// Advances the state through WaveSpawnerStates.
	/// </summary>
	public void AdvanceState()
	{
		WaveSpawnerStates startState = State;

		switch (State)
		{
			case WaveSpawnerStates.Paused:
				{
					ChangeState (WaveSpawnerStates.ShowText);				
				}
				break;

			case WaveSpawnerStates.ShowText:
				{
					ChangeState (WaveSpawnerStates.Spawn);
				}
				break;

			case WaveSpawnerStates.Spawn:
				{
					ChangeState (WaveSpawnerStates.AIControl);
				}
				break;

			case WaveSpawnerStates.AIControl:
				{
					ChangeState (WaveSpawnerStates.WaveOver);
				}
				break;

			case WaveSpawnerStates.WaveOver:
				{
					ChangeState (WaveSpawnerStates.Next);
				}
				break;
			case WaveSpawnerStates.Next:
				{
					ChangeState (WaveSpawnerStates.ShowText);
				}
				break;
			case WaveSpawnerStates.PlayerDied:
				{
					// TODO: reset wave.
					ChangeState (WaveSpawnerStates.Paused);
				}
				break;

		}
		
		// Play a sound if wave successfully advanced.
		if (State != startState)
		{
						PlaySound (snd_state);
		}
	}

	/// <summary>
	/// Find child gameobject matching wave #, and spawn it
	/// </summary>
	void SpawnWave(bool advance = true)
	{
		if (CurrentWave < WaveList.Count -1)
		{
			WaveList[CurrentWave].SendMessage("triggerSpawn");
		}
		else
		{
			Debug.Log ("No more waves to spawn!");
		}

		if (advance)
		{
			NextWave();
		}
	}

	void load_all_prefabs()
	{
		GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/");
		Debug.Log("Got: " + prefabs.Length);
		foreach (var go in prefabs)
		{
			Debug.Log(go.name);
		}
	}

	/// <summary>
	/// Updates the appropriate UI panels with wave information
	/// Generally only called on wave start.
	/// </summary>
	/// <param name="wave">Wave.</param>
	void UpdateWaveText(enemy_wave wave)
	{	
		if (wave == null)
			return;
		
		if (WavePanel == null)
			return;
			
		WavePanel.SetCurrentDisplayWave (levelName, CurrentWave + 1, WaveList.Count, wave.waveName);

		if (MessageDisplayPanel == null)
			return;

		string waveMessage = string.Format ("{0}: {1}", CurrentWave, wave.waveName);
		MessageDisplayPanel.ShowMessage (waveMessage);
		
	}

	/// <summary>
	/// Advance wave counter to next wave and play sound.
	/// </summary>
	public void NextWave()
	{
		if (CurrentWave < WaveList.Count - 1)
		{
			PlaySound(snd_wave);
			CurrentWave++;
		}
		else
		{
			Debug.Log ("Last Wave Reached!");
		}
	}

	void playSound_Wave()
	{
		if (snd_wave.length > 0.0f)
		{
			_AudioSource.clip = snd_wave;
			_AudioSource.Play();
		}
	}
			
	
		void PlaySound(AudioClip clip)
		{
				if (clip == null)
						return;
				
				if (clip.length > 0.0f)
				{
						_AudioSource.clip = clip;
						_AudioSource.Play();
				}	
		}

	void HideDisplayMessage()
	{
		if (MessageDisplayPanel == null)
			return;
		
		MessageDisplayPanel.Hide();
	}
	
	/// <summary>
	/// Player died! Do appropriate things.
	/// </summary>
	void PlayerDied()
	{
		ChangeState (WaveSpawnerStates.PlayerDied);
	}

		public void ChangeState(WaveSpawnerStates toState = WaveSpawnerStates.Paused)
		{
				State = toState;	

				switch(toState)
				{
					case WaveSpawnerStates.Paused:
						{
								Debug.Log("~WaveSpawner(): Paused.");
						}
						break;

					case WaveSpawnerStates.ShowText:
						{
							Debug.Log("~WaveSpawner(): Showing Wave Text.");
							UpdateWaveText(WaveList[CurrentWave]);						
						}
						break;

				case WaveSpawnerStates.Spawn:
						{
							Debug.Log ("~WaveSpawner(): Spawning Wave");
								// immediately spawn a wave then move to control phase
								// if you don't do this, it will INFINITELY SPAWN WAVES and break the game.
								SpawnWave();
								WaveScript = WaveList[CurrentWave];
								AdvanceState();
						}
						break;

				case WaveSpawnerStates.AIControl:
						{
								Debug.Log ("~WaveSpawner(): Control Phase");
						}
						break;

				case WaveSpawnerStates.WaveOver:
						{
								Debug.Log ("~WaveSpawner(): Wave Over");
								AdvanceState ();
						}
						break;

				case WaveSpawnerStates.Next:
						{
								Debug.Log ("~WaveSpawner(): Next Wave...");
								NextWave ();
								AdvanceState ();
						}
						break;
				
				case WaveSpawnerStates.PlayerDied:
						{
								Debug.Log("~WaveSpawner(): Player Died.");
								WaveScript.WaveReset ();
						}
						break;
						
				}
		}
}
