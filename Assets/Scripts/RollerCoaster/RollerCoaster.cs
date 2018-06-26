using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerCoaster : MonoBehaviour {

    List<GameObject> trackPieces = new List<GameObject>();

    //List containing disabled track pieces. This is used because creating and destroying gameobjects constantly causes massive amounts of lag.
    List<GameObject> unusedTrackPieces = new List<GameObject>();

    //this should be calculated on the spot
    float trackSize = -4.021012071f;

    //the scale the world is set at
    public static float scale = 0.008f;

    //the length of one track's bone
    float trackBoneSize = 0.402642f;

    //the prefab for an empty piece of track
    public GameObject trackPrefab;

    //just for now, the right controller is going to be loaded in here
    public GameObject rightController;

    void Start () {
        //just for now, since we must start with one
        trackPieces.Add(transform.Find("TrackPiece0").gameObject);

        //TODO: set tracksize dynamically based on calling the TrackPiece class

        //set track bone size based on scale
        trackBoneSize *= scale;

    }
	
	void Update () {
        CreatePath(Vector3.zero, trackPieces[0]);

    }

    //will create a path of tracks from a start position until the next position
    //startTrack: track that this path is starting on
    public void CreatePath(Vector3 position, GameObject startTrack) {
        Vector3 startPositon = startTrack.transform.position;

        Vector3 controllerPosition = rightController.transform.position;

        //GOAL: the smallest curve possible to reach the required distance

        //Check if it is needed to switch angles to get to it
        //slowly change angles to the target angle

        //get how far up it has to go

        //temperarily hardcoded
        Vector3 targetAngle = new Vector3(0, 1, 0) * rightController.transform.eulerAngles.y;
        Vector3 currentAngle = getCurrentAngle(startTrack);
        Vector3 angle = targetAngle - currentAngle;

        //create a partial circle out of that angle (circumference is known)

        //need to go from 0 to angle
        //find x of this angle
        //just using y part of angle for now

        float x = (controllerPosition.z - startPositon.z) / (Mathf.Tan((180 - angle.y) * Mathf.Deg2Rad) - Mathf.Tan((90 - angle.y) * Mathf.Deg2Rad));

        //find the radius for the cicle with this point
        float radius = x / Mathf.Cos((180 - angle.y) * Mathf.Deg2Rad);

        //find the length of track required for this angle at this radius
        float trackLengthRequired = 2 * Mathf.PI * radius * ((180 - angle.y) / 360);

        //get amount of tracks needed by dividing by length of one track's bone then dividing by amount of bones per track piece
        //int for now just to make things easier
        int tracksNeeded = (int) Mathf.Abs(trackLengthRequired / trackBoneSize / 9f);
        print(tracksNeeded);
        //that many tracks can now be created with an angle of angle.y divided by each bone (tracksNeeded * 9f)

        int startTrackIndex = trackPieces.IndexOf(startTrack);
        for (int i = 1; i < tracksNeeded + 1; i++) {
            if(startTrackIndex + i < trackPieces.Count) {
                GameObject trackPiece = trackPieces[i + startTrackIndex];

                Vector3 eulerAngles = angle / (tracksNeeded * (i - 1));
                trackPiece.transform.eulerAngles = eulerAngles;
                //this finds the last bone plus half of the track size (because position is based off the center of the object
                Vector3 modifiedPosition = trackPieces[i + startTrackIndex - 1].transform.Find("Bottom_Rail/Joint_3_3/Joint_1_3/Joint_2_4/Joint_3_4/Joint_4_3/Joint_5_3/Joint_6_3/Joint_7_3/Joint_8_3/Joint_9_3/Joint_10_3").position + new Vector3(0, 0, trackBoneSize * 5);

                //need to offset it by trackBoneSize by the angle (for now just with y part of angle
                trackPiece.transform.position = modifiedPosition + new Vector3(0, Mathf.Sin(eulerAngles.y * Mathf.Deg2Rad), Mathf.Cos(eulerAngles.y * Mathf.Deg2Rad)) * (trackBoneSize * 5);

                //adjust the track
                trackPiece.GetComponent<TrackPiece>().AdjustTrack(angle / tracksNeeded);

            } else {
                //position is the last bone
                GameObject trackPiece = AddTrackPiece(angle / tracksNeeded, angle / (tracksNeeded * (i - 1)), trackPieces[trackPieces.Count - 1].transform.Find("Bottom_Rail/Joint_3_3/Joint_1_3/Joint_2_4/Joint_3_4/Joint_4_3/Joint_5_3/Joint_6_3/Joint_7_3/Joint_8_3/Joint_9_3/Joint_10_3").position);

            }
        }

        //remove all unneeded track pieces, don't add to i since trackPieces.Count will be continuing to shrink
        for (int i = startTrackIndex + tracksNeeded + 1; i < trackPieces.Count;) {
            RemoveTrackPiece(trackPieces[i]);
        }


        //make for loop to theoretically count the needed track pieces


    }

    public GameObject AddTrackPiece (Vector3 angle, Vector3 position, Vector3 eulerAngles) {
        GameObject newTrackPiece;

        if(unusedTrackPieces.Count > 0) {
            newTrackPiece = unusedTrackPieces[0];
            unusedTrackPieces.RemoveAt(0);

            newTrackPiece.SetActive(true);
        } else {
            newTrackPiece = Instantiate(trackPrefab, transform);
        }

        newTrackPiece.transform.eulerAngles = eulerAngles;
        //need to offset it by trackBoneSize by the angle (for now just with y part of angle
        newTrackPiece.transform.position = position + new Vector3(0, Mathf.Sin(eulerAngles.y * Mathf.Deg2Rad), Mathf.Cos(eulerAngles.y * Mathf.Deg2Rad)) * (trackBoneSize * 5);

        TrackPiece newTrackPieceClass = newTrackPiece.GetComponent<TrackPiece>();

        newTrackPieceClass.totalAngle = angle;
        trackPieces.Add(newTrackPiece);


        return newTrackPiece;
    }

    public void RemoveTrackPiece(GameObject trackPiece) {
        unusedTrackPieces.Add(trackPiece);
        trackPieces.Remove(trackPiece);

        trackPiece.SetActive(false);
    }

    public Vector3 getCurrentAngle(GameObject startTrack) {
        Vector3 currentAngle = Vector3.zero;

        currentAngle = startTrack.GetComponent<TrackPiece>().totalAngle;
        currentAngle += startTrack.transform.eulerAngles;

        return currentAngle;
    }
}
