using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The preview cart. This will use custom physics to handle drops and turns
public class Cart : MonoBehaviour {

    //the roller coaster this cart is going to be riding
    public RollerCoaster rollerCoaster;

    //velocity in meters
    float velocity = 0;

    //position in meters on the track
    float position = 0;

    //in kg
    float mass = 600;

	void Start () {
		
	}
	
	void FixedUpdate () {
        if (!Input.GetKey(KeyCode.C)) {
            position = 7;
            return;
        }

        Transform currentBone = GetCurrentBone(true);
        Vector3 eulerAnglesOfTrack = currentBone.eulerAngles;
        //find what the incline angle is
        float inclineAngleOfTrack = Mathf.Cos(eulerAnglesOfTrack.y * Mathf.Deg2Rad) * eulerAnglesOfTrack.x + Mathf.Sin(eulerAnglesOfTrack.y * Mathf.Deg2Rad + Mathf.PI) * eulerAnglesOfTrack.z;

        //calculate the force downward (divided by 60 fps)
        float forceDown = mass * ((-9.81f) / 60f);

        //calculate the amount of that force used on an incline of the angle
        float gravityForce = Mathf.Sin(inclineAngleOfTrack * Mathf.Deg2Rad) * forceDown;

        //using that, the acceleration can be calculated
        float gravityAcceleration = gravityForce / mass;

        //calculate the new movements
        velocity += gravityAcceleration;
        position += velocity;

        print(position + " " + velocity);
    }

    Transform GetCurrentBone(bool right) {
        float boneNum = position / rollerCoaster.defaultTrackBoneSize;
        float trackNum = boneNum / rollerCoaster.boneAmount;

        List<Transform> bones = new List<Transform>();

        //find all the bones for this track piece
        foreach (Transform child in rollerCoaster.trackPieces[(int) trackNum].GetComponentsInChildren<Transform>()) {
            //if right, check under right rail, otherwise check under left rail
            if (((right && child.parent.gameObject.name == "Right_Rail") || (!right && child.parent.gameObject.name == "Left_Rail")) || (bones.Count > 0 && child.parent == bones[bones.Count - 1])) {
                //second if statement as the above one is way too long
                //if the object is disabled, then it does not matter
                //don't include the last joint as that does not matter
                if (child.gameObject.activeInHierarchy && child.gameObject.name != "Joint_10_2" && child.gameObject.name != "Joint_10") {
                    bones.Add(child);
                }
            }
        }

        return bones[(int) boneNum - ((int)trackNum * 10)];
    }
}
