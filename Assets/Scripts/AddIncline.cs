using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddIncline : MonoBehaviour {

    //an array that contains arrays of each joint on the rails (maybe move rails to it's own class in the future)
    GameObject[][] rails = new GameObject[3][];

    //array of all of the parents of the rail bones. This will be set by the inspector
    public GameObject[] railParents = new GameObject[3];

    Vector3 adjustmentAngle = new Vector3(0, 10, 0); //the number in the x represents the total angle the whole track rotates divided by 9 (first bone does not have an angle

    void Start() {
        //create the rails array from the railParents

        //original sizes (used for scaling)
        float[] sizes = new float[3];

        for (int i = 0; i < railParents.Length; i++) {
            GameObject[] bones = new GameObject[11];

            //every iteration, parent is set to the next object in the hierchy to get the next child
            GameObject parent = railParents[i];

            for (int b = 0; b < bones.Length; b++) {
                parent = parent.transform.GetChild(0).gameObject;
                bones[b] = parent;
            }

            rails[i] = bones;

            sizes[i] = rails[i][rails[i].Length - 1].transform.position.z - rails[i][0].transform.position.z;
        }

        for (int i = 0; i < rails.Length; i++) {
            for (int r = 1; r < rails[i].Length; r++) {
                //Attempt to rotate them all
                rails[i][r].transform.localEulerAngles += adjustmentAngle;
            }
        }

        //try to stretch the newly shaped incline to the proper size
        for (int i = 0; i < rails.Length; i++) {
            //get relative total offset for the adjusted track
            float difference = rails[i][rails[i].Length - 1].transform.position.z - rails[i][0].transform.position.z;

            float multiplier = sizes[i] / difference;

            for (int r = 1; r < rails[i].Length; r++) {
                Vector3 pos = rails[i][r].transform.position;

                rails[i][r].transform.localPosition *= multiplier;

                if (adjustmentAngle.y != 0) { //making a turn, extend outside curves to accomidate
                    if (i != 1) { //in this case, 1 is the inside rail (doing just zero for now as a test, in the final version it should be based on if the angle is negative or positive)
                        //get full offset compared to rails[1]
                        float offset = Mathf.Abs(railParents[1].transform.position.x) + Mathf.Abs(railParents[i].transform.position.x);

                        //radius of the inside circle
                        float radius1 = Mathf.Abs(sizes[i]);
                        //radius of outside circle (rails[i])
                        float radius2 = radius1 + offset;

                        print(radius1 + "  " + radius2 + "    " + radius2/radius1);

                        //rails[i][r].transform.localPosition *= radius2/radius1;
                    }
                }
            }

        }
    }

    void Update () {

	}
}
