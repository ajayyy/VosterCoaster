using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSelector : MonoBehaviour {

    public RadialToggle[] buttons;

    float defaultSize = 0.02f;
    float maxSize = 0.03f;

    //float position;

	void Start () {
		
	}
	
	void Update () {
        GameController gameController = GameController.instance;

        float vertical = gameController.rightController.GetAxis().y;
        float horizontal = gameController.rightController.GetAxis().x;

        //find position on a circle perimeter(angle)
        float position = Mathf.Rad2Deg * Mathf.Atan2(vertical, horizontal) + 180;

        //position += 1f;
        //if (position > 360) {
        //    position = 0;
        //}

        SetButtonSizes(position);

        if (Input.GetButtonDown("RightTrackpadClick") || Input.anyKeyDown) {
            GetSelectedToggle().Toggle();
        }
    }

    //the largest toggle would be the one selected
    public RadialToggle GetSelectedToggle() {
        RadialToggle largestToggle = null;

        foreach (RadialToggle radialToggle in buttons) {
            if (largestToggle == null || radialToggle.rectTransform.sizeDelta.x > largestToggle.rectTransform.sizeDelta.x) {
                largestToggle = radialToggle;
            }
        }

        return largestToggle;
    }

    public void SetButtonSizes(float position) {
        float[] positions = new float[buttons.Length];

        for (int i = 0; i < positions.Length; i++) {
            positions[i] = (360 / buttons.Length) * i;
        }

        for (int i = 0; i < buttons.Length; i++) {

            float difference = Mathf.Abs(position - positions[i]);

            //make sure it is the actual difference (difference between 359 and 0 is 1 not 359)
            if (difference > 180) {
                difference = 360 - difference;
            }
            if (difference < 90) {
                float size = defaultSize + (maxSize - defaultSize) * (1 - difference / 90);

                buttons[i].rectTransform.sizeDelta = new Vector2(1, 1) * size;
            } else {
                buttons[i].rectTransform.sizeDelta = new Vector2(1, 1) * defaultSize;
            }
        }
    }
}
