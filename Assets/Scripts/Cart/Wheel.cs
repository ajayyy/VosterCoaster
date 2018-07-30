using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour {

    //objects below the wheel will have a multiplier of -1 to make the offset negative
    public float multiplier = 1;

    SphereCollider collider;

	void Start () {
        collider = GetComponent<SphereCollider>();
	}
	
	void Update () {
		
	}

    public void OnCollisionEnter(Collision collision) {
        GameObject collidedObject = collision.gameObject;

        transform.position = collidedObject.transform.position + collidedObject.transform.up * (collider.radius * multiplier);
    }
}
