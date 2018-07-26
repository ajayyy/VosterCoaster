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

        //test velocity smoothing out method to prevent sudden stops for no reason
        if((rigidbody.velocity - lastVelocity).magnitude > 0.015) {
            //rigidbody.velocity = lastVelocity;

            print("smoothing");
        } else {
            print("not smoothing because the magnitude is only " + (rigidbody.velocity - lastVelocity).magnitude);
        }
        print(rigidbody.velocity);
        lastVelocity = rigidbody.velocity;
    }
}
