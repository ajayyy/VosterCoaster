using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController instance;

    public SteamVR_TrackedObject leftControllerTrackedObject;
    public SteamVR_TrackedObject rightControllerTrackedObject;

    public SteamVR_Controller.Device leftController {
        get {
            return SteamVR_Controller.Input((int)leftControllerTrackedObject.index);
        }
    }
    public SteamVR_Controller.Device rightController {
        get {
            return SteamVR_Controller.Input((int)rightControllerTrackedObject.index);
        }
    }

    public RadialOptionsMenu rightMenu;
    public RadialOptionsMenu leftMenu;

    //game objects
    public GameObject leftControllerObject;
    public GameObject rightControllerObject;

    public GameObject world;

    //The window the controllers are pointing at. Calculated using a raycast. Null if nothingma
    public GameObject rightControllerWindowPointingAt;
    public GameObject leftControllerWindowPointingAt;

    //the scale the world is set at
    //the world's scale can change, but by default is 0.008
    public float scale {
        get {
            return world.transform.localScale.x;
        } set {
            world.transform.localScale = new Vector3(value, value, value);
        }
    }

    //the scale from the last frame. Used for adjusting physics based on the scale
    public float lastScale = 1;

    public LayerMask windowMask = 8;

    void Start () {
		if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}
	
	void FixedUpdate () {
        if (instance == null) {
            instance = this;
        }

        if(scale != lastScale) {
            Physics.gravity = new Vector3(0, (-9.81f) * scale, 0);
            lastScale = scale;
        }

        //calculate what is in front of each controller with a raycast
        //right controller
        RaycastHit raycast;
        if (Physics.Raycast(rightControllerObject.transform.position, rightControllerObject.transform.forward, out raycast, 25, windowMask.value)) {
            rightControllerWindowPointingAt = raycast.collider.gameObject;
        } else {
            rightControllerWindowPointingAt = null;
        }

        //left controller
        if (Physics.Raycast(leftControllerObject.transform.position, leftControllerObject.transform.forward, out raycast, 25, windowMask.value)) {
            leftControllerWindowPointingAt = raycast.collider.gameObject;
        } else {
            leftControllerWindowPointingAt = null;
        }
    }
}
