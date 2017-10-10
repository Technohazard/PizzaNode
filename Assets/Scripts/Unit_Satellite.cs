using UnityEngine;

using System.Collections.Generic;

// Reference: http://answers.unity3d.com/questions/463704/smooth-orbit-round-object-with-adjustable-orbit-ra.html

public class Unit_Satellite : MonoBehaviour
{

		public enum UNIT_BEHAVIORS
		{
				UNIT_IDLE = 0,
				UNIT_FOLLOW,
				UNIT_ORBIT,
				ATTACK
		}

		public UNIT_BEHAVIORS Behavior = UNIT_BEHAVIORS.UNIT_IDLE;

		/// <summary>
		/// Follow object
		/// </summary>
		public GameObject FollowTarget = null;
		private Rigidbody2D _FollowTargetRB;
		// cache of its rigidbody.

		public float mFollowDistance = 3.0f;
		// radius at which to maintain follow distance from Target.
		public float mDetectRange = 5.0f;
		// range at which this gameobject can detect Target transforms.

		public float mNeighborSpacing = 4.0f;
		// distance to attempt to maintain from neighbor.
		public float thrust = 2.0f;
		// how fast this object moves towards target.

		public float _matchVelTime = 0f;
		private float _matchVelCurrentTime = 0f;
		private bool _matchVel = false;

		// Enemy Targeting
		/// <summary>
		/// Target this drone will try to shoot at.
		/// </summary>
		public GameObject PrimaryTarget = null;

		// Movement and animation
		private Vector3 rotationAxis = Vector3.forward;
		// axis to rotate about.
		public float rotationSpeed = 80.0f;
		// how fast target rotates around central point

		public bool useStartDistanceAsRadius = true;
		// converts existing distance into radius - means no lerping at start!
		public bool initialOrbitSnap = true;
		// Snap to radius value instead of using orbit calculated from editor position
		public float orbitRadius = 1.0f;
		// distance at which this object tries to orbit target object.

		public Vector2 MoveTarget = Vector2.zero;
		// World coordinate target location this object tries to move to every frame.

		/// <summary>
		/// When target is 'in range' this is the threshold velocity difference 
		/// at which we attempt to match target velocity over _matchVelTime.
		/// </summary>
		public float TargetRangeTolerance = 1.0f;


		private Vector3 toMove = Vector3.zero;
		// the direction this unit is moving.
		private Rigidbody2D _RigidBody = null;
		// cache of this object's RB

		/// <summary>
		/// NYI: range at which this unit can see enemies to attack
		/// </summary>
		public CircleCollider2D DetectCollider = null;

		public bool showDebugLines = true;

		/// <summary>
		/// GameObject containing active game entities.
		/// </summary>
		public GameObject middleGroundGroup;

		private List<GameObject> TargetList = new List<GameObject> ();

		/// <summary>
		/// Has the distance to the PrimaryTarget been updated yet?
		/// </summary>
		private bool _distanceUpdated = false;

		/// <summary>
		/// Cache of vector3 distance check to target
		/// </summary>
		private float _distanceToPrimaryTarget = float.MaxValue;

		// Use this for initialization
		void Start ()
		{
				RegisterRigidBody ();

				if (gameObject.CompareTag ("Player_Units")) {
						Behavior = UNIT_BEHAVIORS.UNIT_FOLLOW;
						SetFollowTarget ("Player_Unit");
				} else {

				}

				if (!PrimaryTarget) {
						PrimaryTarget = FindClosestEnemy (); // find something to shoot.
				}

		}

		void SetFollowTarget (string targetName)
		{
				SetFollowTarget (GameObject.Find (targetName));
		}

		void SetFollowTarget (GameObject target)
		{
				FollowTarget = target;

				if (target == null) {
						Debug.Log ("Target set to <null>");
						return;
				}

				_FollowTargetRB = FollowTarget.GetComponent<Rigidbody2D> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				// for now, set this at the start of the frame. 
				// triggers an initial distance check.
				// TODO: for optimization, can put this on a timer and only update once every few frames / once a sec?
				_distanceUpdated = false; 

				switch (Behavior) {
				case UNIT_BEHAVIORS.UNIT_IDLE:
						{
								// nothing!
						}
						break;
				case UNIT_BEHAVIORS.UNIT_FOLLOW:
						{
								if (TargetInDetectRange (FollowTarget)) 
								{
										if (!TargetInFollowDistance (FollowTarget))
										{
												ThrustTowardsTarget (FollowTarget); 
										} 
										else 
										{
												// within follow distance.
												// do idle things.
												// for now, attempt to match target velocity.
												if ((_RigidBody != null) && (_FollowTargetRB != null))
												{
														_RigidBody.velocity = _FollowTargetRB.velocity;
												}
										}
								}
						}
						break;
				case UNIT_BEHAVIORS.UNIT_ORBIT:
						{
								OrbitFollowTarget ();
					
						}
						break;
				case UNIT_BEHAVIORS.ATTACK:
						{

						}
						break;

				}
				// Debug drawing
				if (showDebugLines) {
						DrawDebugLines ();
				}
		}

		/// <summary>
		/// Shitties the velocty matching.
		/// LOL tries to lerp first velocty to second over time
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="second">Second.</param>
		private void ShittyVelocityMatching(GameObject first, GameObject second)
		{
				Rigidbody2D FirstRB = null;
				Rigidbody2D SecondRB = null;

				if (first == this.gameObject) {
						FirstRB = _RigidBody;
				}
				else {
						FirstRB = first.GetComponent<Rigidbody2D> ();
				}

				if (second == FollowTarget) {
						SecondRB = _FollowTargetRB;
				}
				else
				{
						SecondRB = second.GetComponent<Rigidbody2D> ();
				}

				if (_matchVel) 
				{
						if (null != FirstRB) 
						{
								if (SecondRB == null)
										SecondRB = second.GetComponent<Rigidbody2D> ();

								if (SecondRB != null) 
								{
										_matchVelCurrentTime += Time.deltaTime;
										if (_matchVelCurrentTime > _matchVelTime) 
										{
												_matchVel = false;
												_matchVelCurrentTime = 0.0f;
												FirstRB.velocity = SecondRB.velocity;
										}

										Vector2 newVel = Vector2.zero;
										newVel = Vector2.Lerp (FirstRB.velocity, SecondRB.velocity, _matchVelCurrentTime / _matchVelTime);
								}
						}
				}
				else
				{
						// check to see if we're within velocity match tolerance
						// if not, start matching velocity on next update
						if ((SecondRB != null) && (FirstRB != null)) {
								Vector2 diffVels = (SecondRB.velocity - FirstRB.velocity);
								if (diffVels.magnitude > TargetRangeTolerance) {
										_matchVel = true;
										_matchVelCurrentTime = 0.0f;
								}
						}
				}
		}
		/// <summary>
		/// Registers the middle ground group that contains active gameobjects / targets
		/// </summary>
		/// <returns>The middle ground group.</returns>
		private GameObject RegisterMiddleGroundGroup ()
		{
				if (middleGroundGroup != null)
						return middleGroundGroup;

				middleGroundGroup = GameObject.Find ("Middleground");

				return middleGroundGroup;
		}

		private void ThrustTowardsTarget (GameObject target)
		{
				if (target != null) {
						ThrustTowardsTarget (target.transform.position);
				}
		}

		private void ThrustTowardsTarget (Vector3 target)
		{
				toMove = target - transform.position;
				toMove.Normalize ();
				if (_RigidBody != null) {
						_RigidBody.AddForce (toMove * thrust);
				}
		}

		private void OrbitFollowTarget ()
		{
				Vector3 orbitTarget = CalculateOrbitTarget (FollowTarget);
				SetMoveTarget (Vector3.MoveTowards (transform.position, orbitTarget, (Time.deltaTime * thrust)));
				ThrustTowardsTarget (orbitTarget);
		}

		private Vector3 CalculateOrbitTarget (GameObject target)
		{
				if (target == null)
						target = GameObject.Find ("Player_Unit");
		
				Vector3 tmpTarget = target.transform.position;

				return tmpTarget;
				// ugh what!?
				//(transform.position - Target.position).normalized * radius + Target.position;
				//moveTarget.position = (transform.position - Target.transformposition).normalized * radius + Target.transform.position;
		}

		private bool TargetInFollowDistance (GameObject target)
		{
				if (target != null) {
						// check if target is within range
						if (DistanceToTarget (target) < mFollowDistance) {
								return true;
						}
				}
				return false;
		}

		/// <summary>
		/// Is <paramref name="target"/> in detection range?
		/// </summary>
		/// <returns><c>true</c>, if in detect range, <c>false</c> otherwise.</returns>
		/// <param name="target">Target.</param>
		private bool TargetInDetectRange (GameObject target)
		{
				if (target == null) {
						return false;
				}

				if (DistanceToTarget (target) < mDetectRange) {
						return true;
				}

				return false;
		}

		/// <summary>
		/// Get the distance from this object to target
		/// Smartly caches primary target distance check.
		/// </summary>
		/// <returns>Distance to target.</returns>
		/// <param name="target">target to check</param>
		private float DistanceToTarget (GameObject target)
		{
				if (target == PrimaryTarget) {
						if (!_distanceUpdated) {
								_distanceToPrimaryTarget = Vector3.Distance (transform.position, target.transform.position);
								_distanceUpdated = true;
						}

						return _distanceToPrimaryTarget;
				} else {
						// just want the distance for a specific target. return it.
						return Vector3.Distance (transform.position, target.transform.position);
				}
		}

		/// <summary>
		/// Updates the targets list.
		/// Scans all objects in middleground group
		/// Adds only objects of opposing tag, in detection range.
		/// </summary>
		private void UpdateTargetsList ()
		{
				GameObject evalGroup = RegisterMiddleGroundGroup ();

				if (evalGroup == null) {
						return;
				}

				foreach (Transform tform in evalGroup.transform) {
						if (!tform.CompareTag (this.tag)) {
								if (TargetInDetectRange (tform.gameObject)) {
										// valid target.
										if (!TargetList.Contains (tform.gameObject)) {
												Debug.Log (string.Format ("Target {0} in range!", tform.name));
												TargetList.Add (tform.gameObject);
										}
								}
						}
				}
		}

		/// <summary>
		/// Draws useful debug lines.
		/// </summary>
		private void DrawDebugLines ()
		{
				if (FollowTarget != null) {
						// Follow Target
						Debug.DrawLine (transform.position, FollowTarget.transform.position, Color.grey);
				}

				if (PrimaryTarget != null) {
						// Target Target
						Debug.DrawLine (transform.position, PrimaryTarget.transform.position, Color.red);
				}
		}

		private GameObject FindClosestEnemy ()
		{
				GameObject closestEnemy = null;

				if (DetectCollider == null) {
						DetectCollider = gameObject.GetComponent<CircleCollider2D> ();
				}

				return closestEnemy;
		}

		public bool SetMoveTarget (Vector2 toTarget)
		{
				MoveTarget = toTarget;
				return true;
		}

		private void RegisterRigidBody ()
		{
				_RigidBody = gameObject.GetComponent<Rigidbody2D> ();
		}
}
