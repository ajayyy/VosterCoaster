using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//modified from https://unity3d.college/2017/06/17/steamvr-laser-pointer-menus/
public class VRUIInput : MonoBehaviour {
    private SteamVR_LaserPointer laserPointer;
    private SteamVR_TrackedController trackedController;

    private void OnEnable() {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;

        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null) {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e) {
        GameController gameController = GameController.instance;

        if (gameController.rightControllerObject == gameObject) {
            gameController.rightControllerWindowPointingAt = e.target.gameObject;
            gameController.rightWindowDistanceAway = e.distance;
        } else if (gameController.leftControllerObject == gameObject) {
            gameController.leftControllerWindowPointingAt = e.target.gameObject;
            gameController.leftWindowDistanceAway = e.distance;
        }
    }

    private void HandlePointerOut(object sender, PointerEventArgs e) {
        GameController gameController = GameController.instance;

        if (gameController.rightControllerObject == gameObject) {
            gameController.rightControllerWindowPointingAt = null;
            gameController.rightWindowDistanceAway = 0;
        } else if (gameController.leftControllerObject == gameObject) {
            gameController.leftControllerWindowPointingAt = null;
        }
    }
}