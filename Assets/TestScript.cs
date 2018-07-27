using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    Rigidbody rigidbody;

    Vector3 lastVelocity = Vector3.zero;

	void Start () {
        rigidbody = GetComponent<Rigidbody>();

        lastVelocity = rigidbody.velocity;
	}
	
	void FixedUpdate () {

    }
}
