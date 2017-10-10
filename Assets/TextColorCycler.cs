using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColorCycler : MonoBehaviour {

	Color colorTarget;
	Color text_color = Color.white;

	private float timerTextColor = 0.0f;
	private float timerTextColor_rate = 0.1f;
	
	private Text _text;
	
	// Use this for initialization
	void Start () 
	{
		_text = this.GetComponent<Text> ();	
		colorTarget = new Color (Random.value, Random.value, Random.value);
	}
	
	// Update is called once per frame
	void Update () 
	{
		text_color = color_cycle ();
		_text.color = text_color;
	}

	/// <summary>
	/// Cycles text color rapidly.
	/// </summary>
	/// <returns> Appropriately lerped color between current color and target</returns>
	private Color color_cycle()
	{
		Color lerpedColor = Color.white;

		timerTextColor += Time.deltaTime;

		if (timerTextColor >= timerTextColor_rate)
		{
			// Change target color
			colorTarget = new Color(Random.value, Random.value, Random.value);
			
			// reset timer. 
			timerTextColor = 0.0f; 
		}

		// Haven't yet reached target color
		if (text_color != colorTarget)
		{
				lerpedColor = Color.Lerp(text_color, colorTarget, timerTextColor / timerTextColor_rate);
		}
		else
		{
				lerpedColor = colorTarget;
		}

		return lerpedColor;
	}
}
