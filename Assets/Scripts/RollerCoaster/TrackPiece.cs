using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPiece : MonoBehaviour {

    //array of all of the parents of the rail bones. This will be set by the inspector
    GameObject[] railParents = new GameObject[3];

    //variable that stores the default distance between bone points, used to reset the meshes
    Vector3 defaultBonePosition = new Vector3(0, 0, -0.402642f);

    public Vector3 totalAngle = new Vector3(0, 0, 0);
    //used when one track piece has mutliple angles on it
    public Vector3 startAngle = new Vector3(0, 0, 0);
    public float percentageOfTrack = 1;
    public float secondCurveStart = -1;

    //has this trackpiece been initialised yet
    bool initialised = false;

    //has this track piece been modified by the current incomplete track
    public bool modified = false;
    public Vector3 oldTotalAngle = Vector3.zero;

    //the roller coaster this is a part of
    public RollerCoaster rollerCoaster;

    //amount of bones per track piece
    float boneAmount = 10f;

    public void Start() {
        if (!initialised) {
            GetParents();

            initialised = true;
        }
    }

    void Update () {
        
    }

    //secondCurveStart: bone where the second curve starts
    //startAngle: angle for time before curveStart
    public void AdjustTrack(Vector3 totalAngle, Vector3 startAngle, float percentageOfTrack, int secondCurveStart) {
        //set variable for total angle for other classes to view
        if (secondCurveStart == -1) {
            this.totalAngle = totalAngle * percentageOfTrack;
        } else {
            this.totalAngle = totalAngle;
        }
        int startAmount = secondCurveStart;
        this.startAngle = startAngle;
        this.percentageOfTrack = percentageOfTrack;
        this.secondCurveStart = secondCurveStart;
        Vector3 adjustmentAngle = totalAngle / boneAmount;
        //if it were a negative number, it would not divide properly (-1 means N/A)
        if (secondCurveStart > 0) {
            adjustmentAngle = totalAngle / (boneAmount - startAmount);
            startAngle = startAngle / (startAmount);
        }


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

        Vector3 currentAngle = startAngle;

        for (int i = 0; i < rails.Length; i++) {
            for (int r = 1; r < rails[i].Length; r++) {

                //check if current angle should switch from the start angle to the full angle
                if (r - 1 >= secondCurveStart) {
                    currentAngle = adjustmentAngle;
                } else {
                    currentAngle = startAngle;
                }

                //Attempt to rotate them all
                rails[i][r].transform.localEulerAngles = currentAngle;

                //reset their position
                rails[i][r].transform.localPosition = defaultBonePosition;

                //set active
                rails[i][r].SetActive(true);
            }
        }

        //try to stretch the newly shaped incline to the proper size
        for (int i = 0; i < rails.Length; i++) {
            //get relative total offset for the adjusted track
            float difference = rails[i][rails[i].Length - 1].transform.position.z - rails[i][0].transform.position.z;

            for (int r = 1; r < rails[i].Length; r++) {

                //check if current angle should switch from the start angle to the full angle
                if(r - 1 >= secondCurveStart) {
                    currentAngle = adjustmentAngle;
                } else {
                    currentAngle = startAngle;
                }

                float height = difference; //calculate height of this track piece

                rails[i][r].transform.localPosition = defaultBonePosition;

                if (currentAngle.y != 0) { //making a turn, extend inside curves to accommodate
                    int middleRail = 2;

                    if (i != middleRail) {
                        //get full offset compared to rails[middleRail]
                        float offset = railParents[middleRail].transform.localPosition.x - railParents[i].transform.localPosition.x * RollerCoaster.scale;

                        //if the angle is negative, the outside rail is the inside rail and the inside rail is the outside rail
                        if(currentAngle.y < 0) {
                            offset = -offset;
                        }

                        //calculate the full angle this track piece gets to
                        float totalAngleOfCurve = 90 - Mathf.Abs(currentAngle.y) * boneAmount;

                        //radius of the middle circle (SOH CAH TOA, cosA = a/h, h = a/cosA)
                        float radius1 = Mathf.Abs(height) / Mathf.Cos(totalAngleOfCurve * Mathf.Deg2Rad);
                        //radius of this circle (rails[i])
                        float radius2 = radius1 - offset;

                        rails[i][r].transform.localPosition *= radius2 / radius1;
                    }
                }
            }
        }

        //cut this off to make sure it is only the percentageOfTrack
        for (int i = 0; i < rails.Length; i++) {
            for (int r = 1; r < rails[i].Length; r++) {
                //if the curve start is normal, treat this normally, otherwise just use the start angle
                if (secondCurveStart == -1) {
                    currentAngle = adjustmentAngle;
                } else {
                    currentAngle = startAngle;
                }

                if ((r - 1) / boneAmount > percentageOfTrack && secondCurveStart != -1) {
                    //if the curve start is not zero, treat the rest of the track as the upcomming angle instead of the start angle
                    rails[i][r].transform.localPosition = defaultBonePosition;
                    rails[i][r].transform.localEulerAngles = adjustmentAngle;
                    rails[i][r].SetActive(true);
                } else if ((r - 1) / boneAmount > percentageOfTrack && secondCurveStart == -1) {
                    //if the curve start is zero, then treat the rest of the track as if it does not exist
                    rails[i][r].transform.localPosition = Vector3.zero;
                    rails[i][r].transform.localEulerAngles = Vector3.zero;
                    rails[i][r].SetActive(false);
                } else if ((r + 1 - 1) / boneAmount > percentageOfTrack && percentageOfTrack != 1) {
                    rails[i][r].transform.localPosition = ((percentageOfTrack - ((r - 1) / boneAmount)) * boneAmount) * defaultBonePosition;
                    rails[i][r].transform.localEulerAngles = ((percentageOfTrack - ((r - 1) / boneAmount)) * boneAmount) * currentAngle;
                    rails[i][r].SetActive(true);
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
