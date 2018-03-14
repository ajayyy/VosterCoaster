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

        RaycastHit groundHit;

        bool groundBoxCastCollided = Physics.BoxCast(rightController.transform.position, (options[currentCoaster].GetComponent<BoxCollider>()).size * options[currentCoaster].transform.localScale.x / 2, Vector3.down, out groundHit, Quaternion.Euler(new Vector3(180, 0, 0)));

        if (groundBoxCastCollided) {
            currentThumbnail.transform.position = new Vector3(rightController.transform.position.x, groundHit.point.y, rightController.transform.position.z);
        }
        currentThumbnail.transform.eulerAngles = new Vector3(0, rightController.transform.eulerAngles.y, 0);

        if (currentMode == 0) {

            if (Input.GetAxis("RightTrigger") == 1 && !lastspawned) {

                if (groundBoxCastCollided) {

                    Vector3 spawnPosition = currentThumbnail.transform.position;

                    float spawnRotation = currentThumbnail.transform.eulerAngles.y;

                    if (groundHit.collider.gameObject.tag == "Track") {

                        Vector3 spawnPositionForward = groundHit.collider.gameObject.transform.position + (groundHit.collider.gameObject.transform.forward * ((BoxCollider)groundHit.collider).size.z) * groundHit.collider.gameObject.transform.localScale.x;
                        Vector3 spawnPositionBackward = groundHit.collider.gameObject.transform.position + ((-groundHit.collider.gameObject.transform.forward) * ((BoxCollider)groundHit.collider).size.z) * groundHit.collider.gameObject.transform.localScale.x;

                        float distForward = Vector3.Distance(currentThumbnail.transform.position, spawnPositionForward);
                        float distBackward = Vector3.Distance(currentThumbnail.transform.position, spawnPositionBackward);

                        if(distForward < distBackward) {
                            spawnPosition = spawnPositionForward;
                        } else {
                            spawnPosition = spawnPositionBackward;
                        }

                        spawnRotation = groundHit.collider.gameObject.transform.eulerAngles.y;
                    }

                    lastspawned = true;

                    GameObject newCoaster = Instantiate(options[currentCoaster]);

                    newCoaster.transform.parent = newCoaster.transform.root;

                    newCoaster.transform.position = spawnPosition;
                    newCoaster.transform.eulerAngles = new Vector3(0, spawnRotation, 0);
                }

            } else if (Input.GetAxis("RightTrigger") != 1) {
                lastspawned = false;
            }

            if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().y > 0.5) {
                //change mode
                currentMode = 1;
                modeText.GetComponent<TextMesh>().text = "Move Mode";
                modeText.GetComponent<Animator>().SetTrigger("fade");

                currentThumbnail.SetActive(false);

            } else if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().y < -0.5) {
                //change mode
                currentMode = 1;
                modeText.GetComponent<TextMesh>().text = "Move Mode";
                modeText.GetComponent<Animator>().SetTrigger("fade");

                currentThumbnail.SetActive(false);

            } else if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().x > 0) {
                currentCoaster++;

                if (currentCoaster >= options.Length) {
                    currentCoaster = 0;
                }

                Destroy(currentThumbnail);
                currentThumbnail = Instantiate(thumbnails[currentCoaster]);
                //currentThumbnail.transform.parent = thumbnailPlacement.transform;
                currentThumbnail.transform.localPosition = Vector3.zero;
                currentThumbnail.transform.localEulerAngles = Vector3.zero;
            } else if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().x < 0) {
                currentCoaster--;

                if (currentCoaster < 0) {
                    currentCoaster = options.Length - 1;
                }
                Destroy(currentThumbnail);
                currentThumbnail = Instantiate(thumbnails[currentCoaster]);
                //currentThumbnail.transform.parent = thumbnailPlacement.transform;
                currentThumbnail.transform.localPosition = Vector3.zero;
                currentThumbnail.transform.localEulerAngles = Vector3.zero;
            }

        } else if(currentMode == 1) {

            TriggerData triggerData = rightController.GetComponent<TriggerData>();

            if (triggerData.collidingObjects.Count > 0 && Input.GetAxis("RightTrigger") >= 0.96 && !attachedObjects.Contains(triggerData.collidingObjects[0])) {
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

                currentThumbnail.SetActive(true);

            } else if (Input.GetButtonDown("RightTrackpadClick") && gameController.rightController.GetAxis().y < -0.5) {

                //change mode
                currentMode = 0;

                modeText.GetComponent<TextMesh>().text = "Place Mode";
                modeText.GetComponent<Animator>().SetTrigger("fade");

                currentThumbnail.SetActive(true);

            }
        }


    }
}
