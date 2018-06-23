using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerCoaster : MonoBehaviour {

    List<GameObject> trackPieces = new List<GameObject>();

    //this should be calculated on the spot
    float trackSize = -4.021012071f;

    //the length of one track's bone
    float trackBoneSize = 0.402642f;

    //the prefab for an empty piece of track
    public GameObject trackPrefab;

    void Start () {
        //just for now, since we must start with one
        trackPieces.Add(transform.Find("Track0").gameObject);

        //TODO: set tracksize dynamically based on calling the TrackPiece class
	}
	
	void Update () {
        CreatePath(Vector3.zero, gameObject);

    }

    //will create a path of tracks from a start position until the next position
    //startTrack: track that this path is starting on
    public void CreatePath(Vector3 position, GameObject startTrack) {
        Vector3 startPositon = startTrack.transform.position;

        //this won't be used in the non-testing version
        Vector3 mousePosition = Input.mousePosition;
        //times 1/n is the same as divided by n
        mousePosition.Scale(new Vector3(1f / Screen.height, 1f / Screen.height, 1f));
        mousePosition *= 10;
        mousePosition += new Vector3(- (float) Screen.width / Screen.height * 10f / 2f, -5);

        //GOAL: the smallest curve possible to reach the required distance

        //Check if it is needed to switch angles to get to it
        //slowly change angles to the target angle

        //get how far up it has to go

        Vector3 amount = mousePosition - startPositon;
        print(amount);

        //temperarily hardcoded
        Vector3 targetAngle = new Vector3(0, 100, 0);
        Vector3 currentAngle = getCurrentAngle();
        Vector3 angle = targetAngle - currentAngle;

        //create a partial circle out of that angle (circumference is known)

        //need to go from 0 to angle
        //find x of this angle
        //just using y part of angle for now
        float x = (mousePosition.y - startPositon.y) / (Mathf.Tan((180 - angle.y) * Mathf.Deg2Rad) - Mathf.Tan((90 - angle.y) * Mathf.Deg2Rad));

        //find the radius for the cicle with this point
        float radius = x / Mathf.Cos((180 - angle.y) * Mathf.Deg2Rad);

        //find the length of track required for this angle at this radius
        float trackLengthRequired = 2 * Mathf.PI * radius * ((180 - angle.y) / 360);

        //get amount of tracks needed by dividing by length of one track's bone then dividing by amount of bones per track piece
        float tracksNeeded = trackLengthRequired / trackBoneSize / 9f;
        //that many tracks can now be created with an angle of angle.y divided by each bone (tracksNeeded * 9f)




        //make for loop to theoretically count the needed track pieces


    }

    public Vector3 getCurrentAngle() {
        Vector3 currentAngle = Vector3.zero;

        foreach (GameObject track in trackPieces) {
            currentAngle += track.GetComponent<TrackPiece>().totalAngle;
        }

        return currentAngle;
    }
}
