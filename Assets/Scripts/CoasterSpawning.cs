using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoasterSpawning : MonoBehaviour {

    //Which mode it is currently on (move or place mode)
    int currentMode = 0;

    //the coasters connected to the controller that you scroll through
    public GameObject[] options;

    //all of the possible thumbnails that are scrolled through (options is the actual one that spawns)
    public GameObject[] thumbnails;

    //the parent of the thumbnails
    public GameObject thumbnailPlacement;

    //the thumbnail currently being displayed
    GameObject currentThumbnail;

    //the current coaster scrolled to
    int currentCoaster = 0;

    public GameObject rightController;

    //for getting inputs
    public SteamVR_TrackedController rightTracking;

    //did they just hit spawn
    bool lastspawned;

	void Start () {
        currentThumbnail = Instantiate(thumbnails[currentCoaster]);
        currentThumbnail.transform.parent = thumbnailPlacement.transform;
        currentThumbnail.transform.localPosition = Vector3.zero;
        currentThumbnail.transform.localEulerAngles = Vector3.zero;
    }

    void Update () {

        GameController gameController = GameController.instance;

        //print(Input.GetAxis("RightTrigger") + " " + Input.GetAxis("LeftTrigger"));   ,

        if(Input.GetAxis("RightTrigger") == 1 && !lastspawned) {
            lastspawned = true;

            GameObject newCoaster = Instantiate(options[currentCoaster]);

            newCoaster.transform.parent = newCoaster.transform.root;

            newCoaster.transform.position = thumbnailPlacement.transform.position;
            newCoaster.transform.eulerAngles = thumbnailPlacement.transform.eulerAngles;

        } else if(Input.GetAxis("RightTrigger") != 1) {
            lastspawned = false;
        }

        if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().x > 0) {
            currentCoaster++;

            if(currentCoaster >= options.Length) {
                currentCoaster = 0;
            }

            Destroy(currentThumbnail);
            currentThumbnail = Instantiate(thumbnails[currentCoaster]);
            currentThumbnail.transform.parent = thumbnailPlacement.transform;
            currentThumbnail.transform.localPosition = Vector3.zero;
            currentThumbnail.transform.localEulerAngles = Vector3.zero;
        } else if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().x < 0) {
            currentCoaster--;

            if (currentCoaster < 0) {
                currentCoaster = options.Length - 1;
            }
            Destroy(currentThumbnail);
            currentThumbnail = Instantiate(thumbnails[currentCoaster]);
            currentThumbnail.transform.parent = thumbnailPlacement.transform;
            currentThumbnail.transform.localPosition = Vector3.zero;
            currentThumbnail.transform.localEulerAngles = Vector3.zero;
        }


    }
}
