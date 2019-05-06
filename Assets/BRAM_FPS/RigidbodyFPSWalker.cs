/*
	~~~ TODO ~~~
 1. floor 'grabbing' when grounded
 2. step-up
 3. apply reverse force to floor
 4. 
*/

using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RigidbodyFPSWalker : MonoBehaviour
{
	public Transform Camera;
	public Collider FootCollider;
	public Image Crosshair;
	public float LookSensitivity = 1f;
	public float Speed = 10.0f;
	public float RunMultiplier = 2.0f;
	public float Gravity = 9.8f;
	public float MaxVelocityChange = 10.0f;
	public bool CanJump = true;
	public float JumpHeight = 1.5f;
	[SerializeField]private bool _grounded;
	private Vector3 _lookDirection;


	// Use this for initialization
	void Start ()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update ()
	{
		// rotate the camera
		transform.rotation = Quaternion.Euler (0, transform.rotation.eulerAngles.y + Input.GetAxis ("Mouse X") * LookSensitivity, 0);
		Camera.rotation = Quaternion.Euler (Camera.rotation.eulerAngles.x + Input.GetAxis ("Mouse Y") * -1 * LookSensitivity, transform.rotation.eulerAngles.y, 0);
	}

	void FixedUpdate ()
	{
		// move the player
		if (_grounded) {
			// Calculate how fast we should be moving
			Vector3 targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			targetVelocity = transform.TransformDirection (targetVelocity);
			targetVelocity *= Speed;

			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = GetComponent<Rigidbody> ().velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp (velocityChange.x, -MaxVelocityChange, MaxVelocityChange);
			velocityChange.z = Mathf.Clamp (velocityChange.z, -MaxVelocityChange, MaxVelocityChange);
			velocityChange.y = 0;
			GetComponent<Rigidbody> ().AddForce (velocityChange, ForceMode.VelocityChange);

			// Jump
			if (CanJump && Input.GetButton ("Jump")) {
				GetComponent<Rigidbody> ().velocity = new Vector3 (velocity.x, CalculateJumpVerticalSpeed (), velocity.z);
			}
		}
		if (Input.GetKey ("left ctrl")) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		} else {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}

		// We apply gravity manually for more tuning control
		GetComponent<Rigidbody> ().AddForce (new Vector3 (0, -Gravity * GetComponent<Rigidbody> ().mass, 0));

		_grounded = false;
	}

	void OnCollisionStay (Collision collision)
	{
		// check if the collision was with the feet collider and not the body collider
		if (collision.contacts[0].thisCollider == FootCollider)
		{
			_grounded = true;
		}
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards Speed
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * JumpHeight * Gravity);
	}
}