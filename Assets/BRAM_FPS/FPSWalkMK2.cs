using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class FPSWalkMK2 : MonoBehaviour {

	public float height;
	public Transform PlayerCamera;
	public Transform Rotator;
	public float LookSensitivity = 1f;
	[SerializeField]private bool _grounded;
	public float WalkSpeed = 10f;
	public float Acceleration = 1.5f;
	public List<Vector3> FeelerOrigins;
	private Vector3 _groundNormal;
	private Vector3 _averageNormal;
	public float AirStrafeForce = 0.1f;
	public float MaxAirSpeed = 1f; // the maximum speed that the player can reach in the air by conventional means. should be ~10% of walk speed.
	private float _pitch;
	private bool _sprint;
	public float SprintSpeed = 20;
	public float MaxSlope = 45;
	//private bool _sliding;
	public float JumpForce = 5;
	public float CrouchSpeed = 5;
	public Collider Foot;
	private bool _foot;
	public float MaxFeelerLength = 1f;
	public float FeelerHeight = 0.5f;
	public string GroundStatus;
	private int _jumpcooldown;


	// Use this for initialization
	void Start () {
		
	}

	// FixedUpdate is called once per physics tick
	void FixedUpdate () {
		//check for a collision with ground, and stop the player if one is detected
		float feelerLength;
		if (_grounded){
			feelerLength = MaxFeelerLength;
			_grounded = false;
		} else {
			feelerLength = FeelerHeight;
		}

		RaycastHit hit;
		float shortestFeeler = MaxFeelerLength;
		Vector3 targetPosition = transform.position;
		//float bestNormal = 90f;
		_groundNormal = Vector3.zero;
		_averageNormal = Vector3.zero;
		
		foreach (Vector3 i in FeelerOrigins) {
			Debug.DrawRay(transform.position + i + transform.up * FeelerHeight, -transform.up * feelerLength);
			if (Physics.Raycast(transform.position + i + transform.up * FeelerHeight, -transform.up, out hit, feelerLength)) {
				if (hit.distance < shortestFeeler) {
					shortestFeeler = hit.distance;
					targetPosition = hit.point - i;
					_groundNormal = hit.normal;
				}
				_averageNormal += hit.normal;
				//if (Vector3.Angle(transform.up, hit.normal) < bestNormal) {
				//	bestNormal = Vector3.Angle(transform.up, hit.normal);
				//}
			}
		}

		_averageNormal = _averageNormal.normalized;
		//if (_jumpcooldown > 0) {
		//} else 
		if (shortestFeeler < FeelerHeight) {
			GroundStatus = "<";
			transform.position = targetPosition;
			GetComponent<Rigidbody>().velocity = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, _groundNormal);
			//if (bestNormal < MaxSlope){
			if (Vector3.Angle(transform.up, _groundNormal) < MaxSlope){
				_grounded = true;
			}
		} else if (_foot) {
			GroundStatus = "=";
			_foot = false;
			_grounded = true;
		} else if (shortestFeeler < MaxFeelerLength) {
			GroundStatus = ">";
			transform.position = targetPosition;
			GetComponent<Rigidbody>().velocity = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, _averageNormal);
			//if (bestNormal < MaxSlope){
			if (Vector3.Angle(transform.up, _averageNormal) < MaxSlope){
				_grounded = true;
			}
		}

		//accelerate the player according to input

		// Calculate how fast we should be moving
		Vector3 moveVector = new Vector3();
		moveVector = moveVector + PlayerCamera.forward * Input.GetAxis ("Vertical");
		moveVector = moveVector + PlayerCamera.right * Input.GetAxis("Horizontal");
		moveVector = Vector3.ProjectOnPlane(moveVector, transform.up);
		moveVector = moveVector.normalized;
		_sprint = Input.GetButton("Sprint");

		if (_grounded) 
		{
			Vector3 targetVelocity = moveVector;
			targetVelocity = Vector3.ProjectOnPlane(targetVelocity, _averageNormal).normalized; //risky
			if(_sprint) {
				targetVelocity *= SprintSpeed;
			} else {
				targetVelocity *= WalkSpeed;
			}

			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = GetComponent<Rigidbody> ().velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange = Vector3.ClampMagnitude(velocityChange, Acceleration);
			GetComponent<Rigidbody> ().AddForce (velocityChange, ForceMode.VelocityChange);
		} 
		else 
		{
			GetComponent<Rigidbody>().AddForce(Physics.gravity, ForceMode.Acceleration);
			Vector3 v = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, transform.up).normalized;
			Vector3 velocityChange = Vector3.zero;
			if (Vector3.Dot(v, moveVector) < 0 || v.magnitude < MaxAirSpeed) {
				velocityChange = moveVector;
			}
			GetComponent<Rigidbody>().AddForce(Vector3.ClampMagnitude(velocityChange * AirStrafeForce, Vector3.Project(v, velocityChange).magnitude), ForceMode.VelocityChange);
		}

		//increment the jump cooldown
		_jumpcooldown = Mathf.Max(0, _jumpcooldown - 1);
	}
	
	// Update is called once per frame
	void Update () {
		//jump
		if (_grounded && Input.GetButtonDown("Jump") && _jumpcooldown == 0){
			GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + transform.up * JumpForce;
			_grounded = false;
			_foot = false;
			_jumpcooldown = 5;
		}

		// rotate the camera
		float x = Input.GetAxis("Mouse X");
		float y = Input.GetAxis("Mouse Y");
		Rotator.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(Input.GetAxis("Mouse X") * LookSensitivity, transform.up) * Rotator.forward, transform.up);
		PlayerCamera.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * -LookSensitivity, PlayerCamera.right) * PlayerCamera.forward, transform.up);

		//placeholder cursor lock
		if (Input.GetKey ("left ctrl")) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		} else {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	IEnumerator OnCollisionStay (Collision collision)
	{
		// check if the collision was with the feet collider and not the body collider
		if (collision.contacts[0].thisCollider == Foot && _jumpcooldown == 0)
		{
			_foot = true;
		} 
		//else if (collision.contacts[0].thisCollider == Foot2)
		//{
		//	_foot2 = true;
		//}
		yield return new WaitForFixedUpdate();
	}

	//void OnCollisionExit (Collision collision) 
	//{
		// check if the collision was with the feet collider and not the body collider
		//if (collision.contacts[0].thisCollider == Foot1)
		//{
			//_foot1 = false;
		//} 
		//else if (collision.contacts[0].thisCollider == Foot2)
		//{
		//	_foot2 = false;
		//}
	//}
}
