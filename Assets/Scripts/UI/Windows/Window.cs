using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {

    public RectTransform rectTransform;
    public RectTransform background;

    public bool moving;
    //these values will be compared against while the window is moving around
    public Vector3 movingStartPosition;
    public Vector3 movingContollerStartPosition;
    //distance away from controller
    float distance = 0;
    //the point where it touched
    RaycastHit movingHit;

    public bool resizing;
    //was resizing just finished last frame
    bool justResized;
    //these values will be compared against while the window is resizing
    public RaycastHit resizingStartHitLeft;
    public RaycastHit resizingStartHitRight;
    public float leftDistance;
    public float rightDistance;
    public RaycastHit resizingCurrentHitLeft;
    public RaycastHit resizingCurrentHitRight;
    public Vector3 resizingStartSize;

    //is this window currently animating a resize
    bool animatingResize;
    Vector3 animatingStartSize;
    Vector3 animatingTargetSize;

    //All of the buttons. To call click on them
    public WindowButton[] buttons;

    //is this window currently animating a movement to a new position
    //if so, the target position will be at animatingTargetPosition and the drawn position will be at transform.position
    bool animatingMovement;
    Vector3 animatingTargetPosition;
    Vector3 animatingStartPosition;
    float animatingStartTime;

    //for animating the rotation
    Quaternion animatingStartRotation;
    Quaternion animatingTargetRotation;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
	}
	
	void FixedUpdate () {
        GameController gameController = GameController.instance;

        //change position based on current animation
        if (animatingMovement) {
            Vector3 positionAddition = new Vector3(0, 0, 0);

            transform.position = Vector3.Lerp(animatingStartPosition, animatingTargetPosition, (Time.time - animatingStartTime) * 20f);

            transform.rotation = Quaternion.Lerp(animatingStartRotation, animatingTargetRotation, (Time.time - animatingStartTime) * 20f);

            if ((Time.time - animatingStartTime) * 20f >= 1) {
                animatingMovement = false;
            }
        }

        for (int i = 0; i < gameController.controllers.Length; i++) {
            //check if window is being moved
            if (moving && !resizing && gameController.controllers[i].GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
                Vector3 hitOffset = movingStartPosition - movingHit.point;

                Vector3 newPosition = gameController.controllerObjects[i].transform.position + gameController.controllerObjects[i].transform.forward * distance + hitOffset;

                animatingMovement = true;
                animatingStartTime = Time.time;
                animatingStartPosition = transform.position;
                animatingTargetPosition = newPosition;

                animatingStartRotation = transform.rotation;
                Vector3 relativePos = gameController.controllerObjects[i].transform.position - transform.position;
                animatingTargetRotation = Quaternion.LookRotation(relativePos);
            } else if (resizing || (moving && !gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger) && !gameController.leftController.GetPress(SteamVR_Controller.ButtonMask.Trigger))) {
                moving = false;
            }

            //check if this window is being pointed at to see if a button is being pressed or the window needs to be moved
            //only checking right controller for now. TODO: make it so that you can toggle your dominant hand
            if (!moving && !resizing && (gameController.controllers[i].GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ||
                (justResized && gameController.controllers[i].GetPress(SteamVR_Controller.ButtonMask.Trigger))) && gameController.controllersWindowPointingAt[i] != null) {

                justResized = false;
                
                //check if button on the window has been pressed
                for (int s = 0; s < buttons.Length; s++) {
                    if (gameController.controllersWindowPointingAt[i] == buttons[s].gameObject) {
                        if (buttons[s].type == 1) {
                            //self destruct
                            Destroy(gameObject);
                        } else {
                            //let the WindowButton class handle the click internally
                            buttons[s].Clicked();
                        }
                        break;
                    }
                }

                if (gameController.controllersWindowPointingAt[i] == gameObject) {
                    //start moving window
                    moving = true;
                    movingStartPosition = transform.position;
                    movingContollerStartPosition = gameController.controllerObjects[i].transform.position;
                    distance = gameController.controllersWindowDistanceAway[i];
                    movingHit = gameController.controllersWindowHit[i];
                }
            }
        }

        //change size based on current animation
        if (animatingResize) {
            transform.position = Vector3.Lerp(animatingStartPosition, animatingTargetPosition, (Time.time - animatingStartTime) * 20f);

            transform.localScale = Vector3.Lerp(animatingStartSize, animatingTargetSize, (Time.time - animatingStartTime) * 20f);

            transform.rotation = Quaternion.Lerp(animatingStartRotation, animatingTargetRotation, (Time.time - animatingStartTime) * 20f);

            if ((Time.time - animatingStartTime) * 20f >= 1) {
                animatingResize = false;
            }
        }

        //check if window is being resized
        if (resizing && gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger) && gameController.leftController.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            resizingCurrentHitLeft = gameController.leftWindowHit;
            resizingCurrentHitRight = gameController.rightWindowHit;

            Vector3 leftPosition = resizingCurrentHitLeft.point;
            Vector3 rightPosition = resizingCurrentHitRight.point;

            if (!gameController.leftWindowBHit) {
                leftPosition = gameController.leftControllerObject.transform.position + gameController.leftControllerObject.transform.forward * leftDistance;
            } else {
                leftDistance = gameController.leftWindowDistanceAway;
            }

            if (!gameController.rightWindowBHit) {
                rightPosition = gameController.rightControllerObject.transform.position + gameController.rightControllerObject.transform.forward * rightDistance;
            } else {
                rightDistance = gameController.rightWindowDistanceAway;
            }

            Vector3 newSize = (Vector3.Distance(leftPosition, rightPosition) / Vector3.Distance(resizingStartHitLeft.point, resizingStartHitRight.point)) * resizingStartSize;

            animatingStartSize = transform.localScale;
            animatingTargetSize = newSize;

            animatingStartPosition = transform.position;
            animatingTargetPosition = Vector3.Lerp(leftPosition, rightPosition, 0.5f);

            animatingResize = true;
            animatingStartTime = Time.time;
            animatingStartRotation = transform.rotation;
            Vector3 relativePos = gameController.rightControllerObject.transform.position - transform.position;
            animatingTargetRotation = Quaternion.LookRotation(relativePos);
        } else if (resizing && (!gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger) || !gameController.leftController.GetPress(SteamVR_Controller.ButtonMask.Trigger))) {
            resizing = false;

            justResized = true;
        }

        //check if the window needs to be resized
        if (!resizing && gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger) && gameController.rightControllerWindowPointingAt == gameObject
            && gameController.leftController.GetPress(SteamVR_Controller.ButtonMask.Trigger) && gameController.leftControllerWindowPointingAt == gameObject) {
            //start resizing window
            resizing = true;

            resizingStartHitLeft = gameController.leftWindowHit;
            resizingStartHitRight = gameController.rightWindowHit;
            resizingCurrentHitLeft = gameController.leftWindowHit;
            resizingCurrentHitRight = gameController.rightWindowHit;

            leftDistance = gameController.leftWindowDistanceAway;
            rightDistance = gameController.rightWindowDistanceAway;

            resizingStartSize = transform.localScale;
        }
    }
}
