using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple timer component with basic functions and callbacks.
/// </summary>
public class SimpleTimer : MonoBehaviour {

		/// <summary>
		/// Convenient reference string for this timer.
		/// </summary>
		public string TimerName;

		/// <summary>
		/// Target time for this timer
		/// </summary>
		public float time = 0.0f;

		/// <summary>
		/// The current time.
		/// </summary>
		public float currentTime = 0.0f;

		/// <summary>
		/// Restart timer on reset.
		/// </summary>
		public bool loop = false;

		/// <summary>
		/// Reset the timer to 0 when enabling.
		/// </summary>
		public bool ResetOnEnable = true;

		/// <summary>
		/// is timer currently updating?
		/// </summary>
		public bool active = true;

		public System.Action timerAction;

		public SimpleTimer(float val)
		{
				time = val;
				Reset ();
		}

	// Update is called once per frame
	void Update () 
	{
		if (active) 
			{
				currentTime += Time.deltaTime;
		
				if (currentTime >= time)
				{
						Trigger ();
						if (loop) {
								Reset ();
						} 
						else
						{
								Stop ();
						}
				}
			}
				else
				{
						if (loop)
						{
								if (currentTime >= time)
								{
										Reset ();
										active = true;
								}
						}
				}

	}

		void OnEnable()
		{
				if (ResetOnEnable)
				{
						Reset ();
				}
		}

		public void Reset()
		{
				currentTime = 0;
		}

		public void Stop()
		{
				active = false;
		}

		public void Start()
		{
				active = true;
		}

		public void Trigger()
		{
				Debug.Log ("Tick!");
				if (timerAction != null)
				{
						timerAction();
				}
		}
}
