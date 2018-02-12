using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoasterSpawning : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
        print(Input.GetAxis("RightTrigger") + " " + Input.GetAxis("LeftTrigger"));
	}
}
