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

    //for getting velocities
    SteamVR_Controller.Device rightDevice;

    //did they just hit spawn
    bool lastspawned;

    //objects that the controller is currently holding on to (only right controller for now
    List<GameObject> attachedObjects = new List<GameObject>();

    //the text that says what mode the rightcontroller is currenly on
    public GameObject modeText;

	void Start () {
        currentThumbnail = Instantiate(thumbnails[currentCoaster]);
        currentThumbnail.transform.parent = thumbnailPlacement.transform;
        currentThumbnail.transform.localPosition = Vector3.zero;
        currentThumbnail.transform.localEulerAngles = Vector3.zero;

        rightDevice = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));

    }

    void Update () {

        //print(rightController.GetComponent<TriggerData>().colliding);

        GameController gameController = GameController.instance;

        print(gameController.rightController.GetAxis().y);

        if (currentMode == 0) {

            if (Input.GetAxis("RightTrigger") == 1 && !lastspawned) {
                lastspawned = true;

                GameObject newCoaster = Instantiate(options[currentCoaster]);

                newCoaster.transform.parent = newCoaster.transform.root;

                newCoaster.transform.position = thumbnailPlacement.transform.position;
                newCoaster.transform.eulerAngles = thumbnailPlacement.transform.eulerAngles;

            } else if (Input.GetAxis("RightTrigger") != 1) {
                lastspawned = false;
            }

            if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().y > 0.5) {
                //change mode
                currentMode = 1;
                modeText.GetComponent<TextMesh>().text = "Move Mode";
                modeText.GetComponent<Animator>().SetTrigger("fade");

            } else if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().y < -0.5) {
                //change mode
                currentMode = 1;
                modeText.GetComponent<TextMesh>().text = "Move Mode";
                modeText.GetComponent<Animator>().SetTrigger("fade");

            } else if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().x > 0) {
                currentCoaster++;

                if (currentCoaster >= options.Length) {
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

        }else if(currentMode == 1) {

            TriggerData triggerData = rightController.GetComponent<TriggerData>();

            if (triggerData.collidingObjects.Count > 0 && Input.GetAxis("RightTrigger") == 1 && !attachedObjects.Contains(triggerData.collidingObjects[0])) {
                triggerData.collidingObjects[0].transform.parent = thumbnailPlacement.transform;
                triggerData.collidingObjects[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

                attachedObjects.Add(triggerData.collidingObjects[0]);
            }

            if (attachedObjects.Count > 0 && Input.GetAxis("RightTrigger") == 0) {
                attachedObjects[0].transform.parent = transform.root;

                print(rightDevice.velocity);

                attachedObjects[0].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                attachedObjects[0].GetComponent<Rigidbody>().AddForce(rightDevice.velocity * 100);

                attachedObjects.RemoveAt(0);
            }

            if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().y > 0.5) {

                //change mode
                currentMode = 0;

                modeText.GetComponent<TextMesh>().text = "Place Mode";
                modeText.GetComponent<Animator>().SetTrigger("fade");

            } else if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().y < -0.5) {

                //change mode
                currentMode = 0;

                modeText.GetComponent<TextMesh>().text = "Place Mode";
                modeText.GetComponent<Animator>().SetTrigger("fade");

            }
        }


    }
}
