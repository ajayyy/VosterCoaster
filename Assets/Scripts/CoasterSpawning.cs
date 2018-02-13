using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoasterSpawning : MonoBehaviour {

    public GameObject coaster;

    //the coaster connected to the controller
    public GameObject thumbnailCoaster;

    public GameObject rightController;

    //did they just hit spawn
    bool lastspawned;

	void Start () {
		
	}
	
	void Update () {
        //print(Input.GetAxis("RightTrigger") + " " + Input.GetAxis("LeftTrigger"));   ,

        if(Input.GetAxis("RightTrigger") == 1 && !lastspawned) {
            lastspawned = true;

            GameObject newCoaster = Instantiate(coaster);

            newCoaster.transform.parent = newCoaster.transform.root;

            newCoaster.transform.position = thumbnailCoaster.transform.position + Vector3.up * 0;
            newCoaster.transform.rotation = thumbnailCoaster.transform.rotation;

        } else if(Input.GetAxis("RightTrigger") != 1) {
            lastspawned = false;
        }
	}
}
