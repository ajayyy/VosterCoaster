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

        Vector3 deltaPosition = controllerPosition - startPositon;

        Vector3 targetAngle = new Vector3(0, 1, 0) * rightController.transform.eulerAngles.y;
        Vector3 currentAngle = getCurrentAngle(startTrack);
        Vector3 angle = targetAngle - currentAngle;
        //make sure the smallest difference between the angles is found
        angle = new Vector3(Mathf.Abs(angle.x), Mathf.Abs(angle.y), Mathf.Abs(angle.z));
        //do 360 - angle if over 180 for each (see https://stackoverflow.com/questions/6722272/smallest-difference-between-two-angles)
        {
            float x1 = angle.x;
            float y1 = angle.y;
            float z1 = angle.z;

            if(x1 > 180) {
                x1 = 360 - x1;
            }

            if (y1 > 180) {
                y1 = 360 - y1;
            }

            if (z1 > 180) {
                z1 = 360 - z1;
            }

            angle = new Vector3(x1, y1, z1);
        }

        if(deltaPosition.x > 0) {
            angle = -angle;
        }

        //find slope for the line showing all the possible points on the circle (tan(180 - (0.5 * (-angle) + 45)) * x + b)
        float slope = Mathf.Tan((180 - (0.5f * (-angle.y) + 45)) * Mathf.Deg2Rad);

        //find the "b" value (y intercept of this linear equation) for the target angle line (b = y - mx)
        float b = -deltaPosition.z - (Mathf.Tan((180 - angle.y) * Mathf.Deg2Rad) * (-deltaPosition.x));

        //get the slope of the targets linear equation
        float targetSlope = Mathf.Tan((180 - angle.y) * Mathf.Deg2Rad);

        //find the intersection between the target angle and the slope line created above x = (-(m2*d) - b1 + b2) / (-m2 + m)
        float x = (-(targetSlope * (-deltaPosition.x)) + deltaPosition.z + b) / (-targetSlope + slope);
        
        //create a partial circle out of that angle (circumference is known)

        //need to go from 0 to angle

        //find the radius for the cicle with this point
        float radius = x / Mathf.Cos((180 - angle.y) * Mathf.Deg2Rad);

        //find the length of track required for this angle at this radius
        float trackLengthRequired = 2 * Mathf.PI * radius * ((180 - angle.y) / 360);

        //get amount of tracks needed by dividing by length of one track's bone then dividing by amount of bones per track piece
        //int for now just to make things easier
        int tracksNeeded = (int) Mathf.Abs(trackLengthRequired / trackBoneSize / 9f);
        print("b: " + b + " targetSlope: " + targetSlope + " slope: " + slope + " x: " + x + " radius: " + radius + " trackLengthRequired: " + trackLengthRequired + " tracksNeeded: " + tracksNeeded);

        //temperary override to fix issue with way too many tracks being needed when the angle is 0
        if(angle.y == 0) {
            tracksNeeded = 0;
        }
        //that many tracks can now be created with an angle of angle.y divided by each bone (tracksNeeded * 9f)

        int startTrackIndex = trackPieces.IndexOf(startTrack);
        for (int i = 1; i < tracksNeeded + 1; i++) {
            if(startTrackIndex + i < trackPieces.Count) {
                GameObject trackPiece = trackPieces[i + startTrackIndex];

                //reset position and angle before adjusting the track
                trackPiece.transform.position = Vector3.zero;
                trackPiece.transform.localEulerAngles = Vector3.zero;

                //adjust the track
                trackPiece.GetComponent<TrackPiece>().AdjustTrack(angle / tracksNeeded);

                //calculate adjustments
                Vector3 eulerAngles = angle / tracksNeeded * (i - 1) + getCurrentAngle(startTrack);
                //this finds the last bone plus half of the track size (because position is based off the center of the object
                Vector3 modifiedPosition = trackPieces[i + startTrackIndex - 1].transform.Find("Bottom_Rail/Joint_3_3/Joint_1_3/Joint_2_4/Joint_3_4/Joint_4_3/Joint_5_3/Joint_6_3/Joint_7_3/Joint_8_3/Joint_9_3/Joint_10_3").position;

                //need to offset it by trackBoneSize by the angle (for now just with y part of angle
                trackPiece.transform.position = modifiedPosition - (new Vector3(Mathf.Sin(eulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(eulerAngles.y * Mathf.Deg2Rad)) * (trackBoneSize * 5));

                //set track rotation (after adjustment to make sure the adjustment process goes well)
                trackPiece.transform.localEulerAngles = eulerAngles;

            } else {

                //calculate adjustments
                Vector3 eulerAngles = angle / tracksNeeded * (i - 1) + getCurrentAngle(startTrack);
                //this finds the last bone plus half of the track size (because position is based off the center of the object
                Vector3 modifiedPosition = trackPieces[i + startTrackIndex - 1].transform.Find("Bottom_Rail/Joint_3_3/Joint_1_3/Joint_2_4/Joint_3_4/Joint_4_3/Joint_5_3/Joint_6_3/Joint_7_3/Joint_8_3/Joint_9_3/Joint_10_3").position;

                GameObject trackPiece = AddTrackPiece(angle / tracksNeeded, modifiedPosition, eulerAngles);

            }
        }

        //remove all unneeded track pieces, don't add to i since trackPieces.Count will be continuing to shrink
        for (int i = startTrackIndex + tracksNeeded + 1; i < trackPieces.Count;) {
            RemoveTrackPiece(trackPieces[i]);
        }


        //make for loop to theoretically count the needed track pieces


    }

    public GameObject AddTrackPiece (Vector3 totalAngle, Vector3 modifiedPosition, Vector3 eulerAngles) {
        GameObject newTrackPiece;

        if(unusedTrackPieces.Count > 0) {
            newTrackPiece = unusedTrackPieces[0];
            unusedTrackPieces.RemoveAt(0);

            newTrackPiece.SetActive(true);
        } else {
            newTrackPiece = Instantiate(trackPrefab, transform);
            newTrackPiece.GetComponent<TrackPiece>().Start();
        }

        //reset position and angle before adjusting the track
        newTrackPiece.transform.position = Vector3.zero;
        newTrackPiece.transform.localEulerAngles = Vector3.zero;

        TrackPiece newTrackPieceClass = newTrackPiece.GetComponent<TrackPiece>();

        newTrackPieceClass.totalAngle = totalAngle;
        trackPieces.Add(newTrackPiece);

        //adjust the track
        newTrackPieceClass.AdjustTrack(totalAngle);

        //set track rotation (after adjustment to make sure the adjustment process goes well)
        newTrackPiece.transform.eulerAngles = eulerAngles;
        //need to offset it by trackBoneSize by the angle (for now just with y part of angle
        newTrackPiece.transform.position = modifiedPosition - (new Vector3(Mathf.Sin(eulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(eulerAngles.y * Mathf.Deg2Rad)) * (trackBoneSize * 5));

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
