using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Panel : MonoBehaviour {

		private bool _visible;
		public bool Visible 
		{
				get{
						return _visible;
				}
				set
				{
						if (value == true)
						{
								Show ();
						}
						else
						{
								Hide ();
						}
				}
		}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		public virtual void Show()
		{
				_visible = true;
				gameObject.SetActive (true);
		}

		public virtual void Hide()
		{
				_visible = false;
				gameObject.SetActive (false);
		}

}
