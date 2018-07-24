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

    public GameObject world;

    //the scale the world is set at
    //the world's scale can change, but by default is 0.008
    public float scale {
        get {
            return world.transform.localScale.x;
        } set {
            world.transform.localScale = new Vector3(value, value, value);
        }
    }

    void Start () {
		if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}
	
	void Update () {
		
	}
}
