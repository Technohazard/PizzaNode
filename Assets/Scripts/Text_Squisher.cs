using UnityEngine;
using System.Collections;

/// <summary>
/// Lerp transform.scale between <para>SquishMinValues</para>"> and <para>SquishMaxValues</para>
/// Lerping proceeds on update with <para>squishSpeed</para>
/// </summary>
public class Text_Squisher : MonoBehaviour {

	public Vector3 SquishMinValue = new Vector3(1.0f, 1.0f, 0.0f); // minimum level of squish to deflate to
	public Vector3 SquishMaxValue = new Vector3(4.0f, 4.0f, 0.0f); // maximum level of squish to inflate to

	public int fontSizeMin = 96;
	public int fontSizeMax = 128;

	public float TimeScale = 1.0f; // scale at which time moves for this object.

	public Vector3 StartSquishValue = new Vector3(1.0f, 1.0f, 1.0f); // lerp from this
	public Vector3 TargetSquishValue = new Vector3(1.0f, 1.0f, 1.0f); // lerp to this

	public int fontSizeStart = 96;
	public int fontSizeTarget = 128;

	private float squishTime = 0.0f; // current value of squish lerp
	public float squishUpTime_sec = 1.0f; // how long it takes to inflate
	public float squishDownTime_sec = 1.0f; // how long it takes to deflate
	private float squishTimeTarget = 0.0f; // target time to lerp over.

	private Vector3 squishOriginalValue; // initialized from editor

	public enum squishState_types{up, down, paused};
	public squishState_types squishState = squishState_types.up;
	public squishState_types lastSquishState = squishState_types.paused;

	// Use this for initialization
	void Start () 
	{
		squishOriginalValue = transform.localScale; // record whatever scale the object starts at
		StartSquishValue = squishOriginalValue; // when starting, we set start + target squish value equal
		fontSizeStart = guiText.fontSize;
		fontSizeTarget = fontSizeMax;

		TargetSquishValue = SquishMaxValue; // start inflating
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch (squishState)
		{
			case squishState_types.paused:
			{
				// do nothing
				break;
			}
			case squishState_types.up:
			{
				// Inflate! :D ( <-- + --> ) 
				if (TargetSquishValue != SquishMaxValue)
				{
					TargetSquishValue = SquishMaxValue;
				}

				// increment the current time
				squishTime += Time.deltaTime * TimeScale; 
				
				if (squishTime >= squishTimeTarget)
				{
					// we hit max squish time. reverse!
					squishTimeTarget = squishDownTime_sec; // how long it will take to deflate
					squishTime = 0.0f; // reset the timer

					StartSquishValue = transform.localScale;
					TargetSquishValue = SquishMinValue;
					fontSizeStart = guiText.fontSize;
					fontSizeTarget = fontSizeMin;

					lastSquishState = squishState_types.up;
					squishState = squishState_types.down;
				}
				break;
			}
			case squishState_types.down:
			{
				// Deflate! :( --> ( - ) <-- 

				if (TargetSquishValue != SquishMinValue)
				{
					TargetSquishValue = SquishMinValue;
				}
				
				// increment the current time
				squishTime += Time.deltaTime * TimeScale; 
				
				if (squishTime >= squishTimeTarget)
				{
					// we hit max squish time. reverse!
					squishTimeTarget = squishUpTime_sec; // how long it will take to deflate
					squishTime = 0.0f; // reset the timer

					StartSquishValue = transform.localScale;
					TargetSquishValue = SquishMaxValue;
					fontSizeStart = guiText.fontSize;
					fontSizeTarget = fontSizeMax;

					lastSquishState = squishState_types.down;
					squishState = squishState_types.up;
				}
				break;
			}
		}
		
		// Lerp the transform's actual scale over squishTime
		transform.localScale = Vector3.Lerp(StartSquishValue, TargetSquishValue, squishTime / squishTimeTarget);

		transform.localScale = Vector3.Lerp(StartSquishValue, TargetSquishValue, squishTime / squishTimeTarget);
		guiText.fontSize = (int)Mathf.Lerp((float)fontSizeStart, (float)fontSizeTarget, squishTime / squishTimeTarget);

	} // end Update();


	void AdjustScale()
	{

	}
}
