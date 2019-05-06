using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot : MonoBehaviour {

	public bool Grounded;

	void OnCollisionEnter () {
		Grounded = true;	
	}

	void OnCollisionExit () {
		Grounded = false;	
	}


}
