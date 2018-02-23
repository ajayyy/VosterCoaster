using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerData : MonoBehaviour {

    public List<Collider> collidingObjects = new List<Collider>();

	void Start () {

	}
	
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        collidingObjects.Add(other);
    }

    void OnTriggerExit(Collider other) {
        collidingObjects.Remove(other);
    }
}
