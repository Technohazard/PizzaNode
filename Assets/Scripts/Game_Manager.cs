using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// reference : http://www.indiedb.com/groups/unity-devs/tutorials/delegates-events-and-singletons-with-unity3d-c


public class Game_Manager : MonoBehaviour
{
	// Singleton
	private static Game_Manager instance;

	// Variable
	public string myGlobalVar = "Game_Manager Object String!";
	public wave_spawner refWaveSpawnerScript = null; // link to this level's wavespawner script.

		/// <summary>
		/// Number of seconds to wait before spawning the first wave.
		/// </summary>
		public float IntroSeconds = 3.0f;

	/// <summary>
	/// Reference to the message display panel.
	/// </summary>
	public MessageDisplayPanel_UI MessageDisplayPanel;

		public SimpleTimer IntroTimer;

	// Construct  
	private Game_Manager()
	{

	}

	//Instance
	public static Game_Manager Instance
	{

		get     
		{       
			if (instance ==  null)
				instance = GameObject.FindObjectOfType(typeof(Game_Manager)) as  Game_Manager;      
			return instance;    
		}   
	} // end Instance

	// Do something here, make sure this  is public so we can access it through our Instance.   
	public void DoSomething() 
	{ 
		// Debug.Log("Called DoSomething() from the Game_Manager!");
	}  

		void Start()
		{
				if (refWaveSpawnerScript == null)
				{
						FindWaveSpawner ();
				}

				MessageDisplayPanel.Hide ();

				if (IntroTimer != null)
				{
						IntroTimer.timerAction = IntroReady;
				}
				// TODO: Wait "intro" seconds
				// TODO: Wait "wave" seconds
				// TODO: Spawn first wave.

		}

		/// <summary>
		/// Intros the ready.
		/// </summary>
		public void IntroReady()
		{
				Debug.Log ("Intro Ready!");

				// Paused -> ShowText
				refWaveSpawnerScript.AdvanceState ();
		}

	/// <summary>
	/// locates the wavespawner object in this level.
	/// </summary>
	private void FindWaveSpawner()
	{
		GameObject waveSpawnerObj = GameObject.Find("wave_spawner");
		if (waveSpawnerObj != null)
		{
			refWaveSpawnerScript = waveSpawnerObj.GetComponent<wave_spawner>();

			if (refWaveSpawnerScript != null)
			{
				Debug.Log (this + ": Couldn't find wave_spawner object. ");
			}
		}		
	}
}