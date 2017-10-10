using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Message queue entry.
/// </summary>
public struct MessageQueueEntry
{
		// text of the message
		public string message;
		// time to display message
		public float DisplayTime;

		public MessageQueueEntry(string str, float flt = 1.0f)
		{
				if (string.IsNullOrEmpty (str))
				{
						str = "...";		
				}

				message = str;
				DisplayTime = flt;
		}

		// TODO: Add message style (color cycling, pulsing, etc.)
}

public class MessageDisplayPanel_UI : UI_Panel {

		// For now this is just one line.
		// TODO: add more lines.
		/// <summary>
		/// The display element for messages.
		/// </summary>
		public Text DisplayText; 

		/// <summary>
		/// The text pulser reference.
		/// </summary>
		public TextPulser TextPulserRef;

		/// <summary>
		/// The text color cycler reference.
		/// </summary>
		public TextColorCycler TextColorCyclerRef;


		/// <summary>
		/// Queue of messages to display.
		/// </summary>
		Queue<MessageQueueEntry> messageQueue;

		/// <summary>
		/// Number of seconds to display each message
		/// </summary>
		public float DefaultDisplayTime = 1.0f;
		private float timerText = 0.0f;

		public SimpleTimer displayTimer;

	// Use this for initialization
	void Start () {
				messageQueue = new Queue<MessageQueueEntry> ();

				if (displayTimer == null) {
					displayTimer = new SimpleTimer (DefaultDisplayTime);
					displayTimer.loop = true;
				}
				displayTimer.timerAction = OnTimerTick;

				Show ();
	}
	
	// Update is called once per frame
	void Update () 
		{
				
	}

	void OnTimerTick ()
	{
		// if there are messages left in the queue, display next.
		if (messageQueue.Count > 0) {
			MessageQueueEntry tmp = messageQueue.Dequeue ();

						if (string.IsNullOrEmpty (tmp.message)) {
								DisplayText.text = messageQueue.Count.ToString ();
						} else {
								DisplayText.text = tmp.message;
						}
			displayTimer.time = tmp.DisplayTime;
			displayTimer.Reset ();

			Show ();
			return;
		}
		// Text has displayed for its lifetime, turn it off now.
		Hide ();

		// Reset timer back to default so it only checks queue every so often.
		displayTimer.time = DefaultDisplayTime;
	}
							
		/// <summary>
		/// Queue a message for showing. 
		/// </summary>
		/// <param name="message">Message.</param>
		public void ShowMessage(string message, float displayTime = 0.0f)
		{
				if (displayTime == 0.0f)
				{
						displayTime = DefaultDisplayTime;
				}
								
			messageQueue.Enqueue (new MessageQueueEntry(message, displayTime));
				// Tick the timer right now.
				OnTimerTick ();
		}
}
