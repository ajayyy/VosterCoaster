using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerData : MonoBehaviour {

    public List<GameObject> collidingObjects = new List<GameObject>();

	void Start () {

	}
	
	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        collidingObjects.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other) {
        collidingObjects.Remove(other.gameObject);
    }
}
