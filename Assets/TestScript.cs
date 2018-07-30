using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    Rigidbody rigidbody;

	void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate () {
        GetComponent<Rigidbody>().maxAngularVelocity = Mathf.Infinity;
    }
}
