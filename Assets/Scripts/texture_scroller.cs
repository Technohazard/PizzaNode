using UnityEngine;
using System.Collections;

public class texture_scroller : MonoBehaviour {

	public Vector2 offset_scroll_amt = new Vector2(1,1);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Vector2 offset = Time.time * offset_scroll_amt;
		Vector2 offset = offset_scroll_amt;
		renderer.material.mainTextureOffset = offset;
	}

}
