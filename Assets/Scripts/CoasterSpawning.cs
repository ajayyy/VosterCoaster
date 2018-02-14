using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoasterSpawning : MonoBehaviour {

    //the coasters connected to the controller that you scroll through
    public GameObject[] options;

    //the current coaster scrolled to
    int currentCoaster = 0;

    public GameObject rightController;

    //for getting inputs
    public SteamVR_TrackedController rightTracking;

    //did they just hit spawn
    bool lastspawned;

	void Start () {
		
	}
	
	void Update () {

        GameController gameController = GameController.instance;

        //print(Input.GetAxis("RightTrigger") + " " + Input.GetAxis("LeftTrigger"));   ,

        if(Input.GetAxis("RightTrigger") == 1 && !lastspawned) {
            lastspawned = true;

            GameObject newCoaster = Instantiate(options[currentCoaster]);

            newCoaster.transform.parent = newCoaster.transform.root;

            newCoaster.transform.position = rightController.transform.position + Vector3.up * 0;
            newCoaster.transform.eulerAngles = rightController.transform.eulerAngles + Vector3.up * 90; //the thumbnail is rotated 90 degrees from the controller

        } else if(Input.GetAxis("RightTrigger") != 1) {
            lastspawned = false;
        }

        if (Input.GetButton("RightTrackpadClick")) {
            print("sdasdsad");
        } else {
            print(gameController.rightController.GetAxis());
        }


    }
}
