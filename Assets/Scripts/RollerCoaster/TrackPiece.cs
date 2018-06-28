using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour {

    //array of all of the parents of the rail bones. This will be set by the inspector
    GameObject[] railParents = new GameObject[3];

    //variable that stores the default distance between bone points, used to reset the meshes
    Vector3 defaultBonePosition = new Vector3(0, 0, -0.402642f);

    public Vector3 totalAngle = new Vector3(0, 0, 0);

    //used for testing, if enabled the adjust track function will be called at the start. This normally would be called by code, but if the track is added manually while debugging, this variable will need to be enabled
    public bool DEBUG_TEST = false;

    //has this trackpiece been initialised yet
    bool initialised = false;

    public void Start() {
        if (!initialised) {
            GetParents();

            if (DEBUG_TEST) {
                AdjustTrack(totalAngle);
            }

            initialised = true;
        }
    }

    void Update () {
        
    }

    //adjustment angle: the number represents the total angle the whole track rotates divided by 9 (first bone does not have an angle)
    public void AdjustTrack(Vector3 totalAngle) {
        //set variable for total angle for other classes to view
        this.totalAngle = totalAngle;
        Vector3 adjustmentAngle = totalAngle / 9;

        //an array that contains arrays of each joint on the rails (maybe move rails to it's own class in the future)
        GameObject[][] rails = new GameObject[3][];

        //create the rails array from the railParents

        for (int i = 0; i < railParents.Length; i++) {
            GameObject[] bones = new GameObject[11];

            //every iteration, parent is set to the next object in the hierchy to get the next child
            GameObject parent = railParents[i];

            for (int b = 0; b < bones.Length; b++) {
                parent = parent.transform.GetChild(0).gameObject;
                bones[b] = parent;
            }

            rails[i] = bones;
        }

        for (int i = 0; i < rails.Length; i++) {
            for (int r = 1; r < rails[i].Length - 1; r++) {
                //Attempt to rotate them all
                rails[i][r].transform.localEulerAngles = adjustmentAngle;

                //reset their position
                rails[i][r].transform.localPosition = defaultBonePosition;
            }
        }

        //try to stretch the newly shaped incline to the proper size
        for (int i = 0; i < rails.Length; i++) {
            //get relative total offset for the adjusted track
            float difference = rails[i][rails[i].Length - 1].transform.position.z - rails[i][0].transform.position.z;

            for (int r = 1; r < rails[i].Length - 1; r++) {

                float height = difference; //calculate height of this track piece

                rails[i][r].transform.localPosition = defaultBonePosition;
                if (r == rails[i].Length - 2) {
                    rails[i][r].transform.localPosition *= 2;
                }

                if (adjustmentAngle.y != 0) { //making a turn, extend inside curves to accommodate
                    int outsideRail = 1;
                    if (adjustmentAngle.y > 0) {
                        outsideRail = 0;
                    }

                    if (i != outsideRail) {
                        //get full offset compared to rails[outsideRail]
                        float offset = Mathf.Abs(railParents[outsideRail].transform.localPosition.x - railParents[i].transform.localPosition.x) * RollerCoaster.scale;

                        //calculate the full angle this track piece gets to
                        float totalAngleOfCurve = 90 - adjustmentAngle.y * 9f;

                        //radius of the outside circle (SOH CAH TOA, cosA = a/h, h = a/cosA)
                        float radius1 = Mathf.Abs(height) / Mathf.Cos(totalAngleOfCurve * Mathf.Deg2Rad);
                        //radius of inside circle (rails[i])
                        float radius2 = radius1 - offset;

                        rails[i][r].transform.localPosition *= radius2 / radius1;
                    }
                }
            }

        }
    }

    public void ResetTrack() {
        //an array that contains arrays of each joint on the rails (maybe move rails to it's own class in the future)
        GameObject[][] rails = new GameObject[3][];

        //go through all the bones in each rail parent and reset their position and rotation
        for (int i = 0; i < railParents.Length; i++) {

            //every iteration, parent is set to the next object in the hierchy to get the next child
            GameObject parent = railParents[i];

            for (int b = 0; b < 11; b++) {
                GameObject bone = parent.transform.GetChild(0).gameObject;

                parent = bone;

                //reset this bone
                bone.transform.localEulerAngles = Vector3.zero;
                bone.transform.localPosition = defaultBonePosition;

                //bone 0 has no offset since it is the start of the object
                if (b == 0) {
                    bone.transform.localPosition = Vector3.zero;
                }
            }

        }

    }

    //gets rail parents
    public void GetParents() {

        railParents[0] = transform.Find("Left_Rail").gameObject;
        railParents[1] = transform.Find("Right_Rail").gameObject;
        railParents[2] = transform.Find("Bottom_Rail").gameObject;

    }

}
