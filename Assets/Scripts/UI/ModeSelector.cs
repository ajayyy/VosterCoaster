using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelector : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
        GameController gameController = GameController.instance;

        float vertical = Input.GetAxis("RightTrackpadVertical");
        float horizontal = Input.GetAxis("RightTrackpadHorizontal");

        //find position on a circle perimeter (angle)
        float position = Mathf.Rad2Deg * Mathf.Atan2(vertical, horizontal);


    }
}
