using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextPulser : MonoBehaviour {

		public AnimationCurve SizeCurve;

		/// <summary>
		/// Time over which to pulse curve.
		/// </summary>
		public float Duration = 5.0f; 

		public float _currentTime = 0.0f;
		public float scale = 1.0f;

		private RectTransform _rectTransform;
		private Vector3 _OriginalRectTransformScale;
		public Vector3 _TargetTransformScale;

		private RectTransform _resizeRect;

	// Use this for initialization
	void Start () 
	{
				_rectTransform = GetComponent<RectTransform>();
				_OriginalRectTransformScale = _rectTransform.localScale;
				_resizeRect = _rectTransform;
	}
	
	// Update is called once per frame
	void Update () 
	{
				_currentTime += Time.deltaTime;

				// Reset time if we've gone over Duration
				if (_currentTime > Duration)
				{
						_currentTime = 0.0f + (_currentTime % Duration);
				}
						
				scale = SizeCurve.Evaluate(_currentTime / Duration);	

				Vector3 targetScale = Vector3.Lerp(_OriginalRectTransformScale, _TargetTransformScale, scale);
				//_resizeRect.localScale = _OriginalRectTransformScale  * scale;
				_resizeRect.localScale = targetScale;
				_rectTransform = _resizeRect;
	}
}
