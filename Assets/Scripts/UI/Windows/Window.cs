using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {

    public RectTransform rectTransform;

    public bool moving;
    //these values will be compared against while the window is moving around
    public Vector3 movingStartPosition;
    public Vector3 movingContollerStartPosition;
    //distance away from controller
    float distance = 0;
    //the point where it touched
    RaycastHit hit;

    //All of the buttons. To call click on them
    public WindowButton[] buttons;

    //is this window currently animating a movement to a new position
    //if so, the target position will be at animatingTargetPosition and the drawn position will be at transform.position
    bool animatingMovement;
    Vector3 animatingTargetPosition;
    Vector3 animatingStartPosition;
    float animatingStartTime;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
	}
	
	void FixedUpdate () {
        GameController gameController = GameController.instance;

        //change position based on current animation
        if (animatingMovement) {
            Vector3 positionAddition = new Vector3(0, 0, 0);

            transform.position = Vector3.Lerp(animatingStartPosition, animatingTargetPosition, (Time.time - animatingStartTime) * 20f);

            if (transform.position == animatingTargetPosition) {
                animatingMovement = false;
            }
        }

        //check if window is being moved
        if (moving && gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            Vector3 hitOffset = movingStartPosition - hit.point;

            Vector3 newPosition = gameController.rightControllerObject.transform.position + gameController.rightControllerObject.transform.forward * distance + hitOffset;

            animatingMovement = true;
            animatingStartTime = Time.time;
            animatingStartPosition = transform.position;
            animatingTargetPosition = newPosition;

            //rotate it towards the controller
            transform.LookAt(gameController.rightControllerObject.transform);
        } else if (moving && !gameController.rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            moving = false;
        }

        //check if this window is being pointed at to see if a button is being pressed or the window needs to be moved
        //only checking right controller for now. TODO: make it so that you can toggle your dominant hand
        if (!moving && gameController.rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && gameController.rightControllerWindowPointingAt != null) {
            for (int i = 0; i < buttons.Length; i++) {
                if (gameController.rightControllerWindowPointingAt == buttons[i].gameObject) {
                    if (buttons[i].type == 1) {
                        //self destruct
                        Destroy(gameObject);
                    } else {
                        //let the WindowButton class handle the click internally
                        buttons[i].Clicked();
                    }
                    break;
                }
            }

            if (gameController.rightControllerWindowPointingAt == gameObject) {
                //start moving window
                moving = true;
                movingStartPosition = transform.position;
                movingContollerStartPosition = gameController.rightControllerObject.transform.position;
                distance = gameController.rightWindowDistanceAway;
                hit = gameController.rightWindowHit;
            }
        }
	}
}
