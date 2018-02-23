using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerData : MonoBehaviour {

    BoxCollider collider;

    public bool colliding;

	void Start () {
        collider = GetComponent<BoxCollider>();
	}
	
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        colliding = true;
    }

    void OnTriggerStay(Collider other) {
        colliding = true;
    }

    void OnTriggerExit(Collider other) {
        colliding = false;
    }
}
