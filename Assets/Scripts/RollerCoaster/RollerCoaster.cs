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

        //get amount of tracks needed by dividing by length of one track's bone then dividing by amount of bones per track piece
        //int for now just to make things easier
        //for now just set to a static number

        //TODO: delete this
        //print("b: " + b + " targetSlope: " + targetSlope + " slope: " + slope + " x: " + x + " radius: " + radius + " trackLengthRequired: " + trackLengthRequired + " tracksNeeded: " + tracksNeeded);

        //that many tracks can now be created with an angle of angle.y divided by each bone (tracksNeeded * 9f)

        //find the collision between the start line and the target line (x = (b2 - b1) / (m1 - m2))

        //calculate the slope for the target angle
        float targetSlope = Mathf.Tan((90 - targetAngle.y) * Mathf.Deg2Rad);
        //calculate slope for the start
        float startSlope = Mathf.Tan((90 - getCurrentAngle(startTrack).y) * Mathf.Deg2Rad);

        //the b value for the target angle (b = y - mx)
        float targetB = rightController.transform.position.z - targetSlope * rightController.transform.position.x;
        //the b value for the start angle (b = y - mx)
        float startB = startTrack.transform.position.z - startSlope * startTrack.transform.position.x;

        //calculate the collision point
        float collisionX = (startB - targetB) / (targetSlope - startSlope);
        float collisionY = targetSlope * collisionX + targetB;

        //get distance from the start
        float distanceFromStart = Mathf.Sqrt(Mathf.Pow(collisionX - startTrack.transform.position.x, 2) 
            + Mathf.Pow(collisionY - startTrack.transform.position.z, 2));

        //get distance from target
        float distanceFromTarget = Mathf.Sqrt(Mathf.Pow(collisionX - rightController.transform.position.x, 2) 
            + Mathf.Pow(collisionY - rightController.transform.position.z, 2));

        //float trackLengthRequired = 2 * Mathf.PI * radius * ((180 - angle.y) / 360);

        //get amount of tracks needed by dividing by length of one track's bone then dividing by amount of bones per track piece
        //int for now just to make things easier

        //the amount of tracks need coming straight off the start track
        int startTracksNeeded = (int) Mathf.Abs(distanceFromStart / trackBoneSize / 9f);
        int targetTracksNeeded = (int) Mathf.Abs(distanceFromTarget / trackBoneSize / 9f);
        int curveTracksNeeded = Mathf.Min(startTracksNeeded, targetTracksNeeded);

        startTracksNeeded -= curveTracksNeeded;
        targetTracksNeeded -= curveTracksNeeded;

        int totalTracksNeeded = startTracksNeeded + curveTracksNeeded + targetTracksNeeded;

        //Amount of tracks already placed down
        int startTrackAmount = trackPieces.IndexOf(startTrack) + 1;
        for (int i = 0; i < totalTracksNeeded; i++) {
            Vector3 eulerAngles = getCurrentAngle(startTrack);
            //the total angle going through one whole track piece
            Vector3 totalTrackAngle = Vector3.zero;

            if(i + 1 > startTracksNeeded) {
                //then it is time to create a curve instead of just a straight line coming off the start track
                //calculate the adjustment needed for the curve
                eulerAngles = angle / curveTracksNeeded * (i - startTracksNeeded) + getCurrentAngle(startTrack);

                totalTrackAngle = angle / curveTracksNeeded;
            }

            if (i + 1 > startTracksNeeded + curveTracksNeeded) {
                //back to straight path, but in the angle of the target
                eulerAngles = targetAngle;
                totalTrackAngle = Vector3.zero;
            }

            if (startTrackAmount + i < trackPieces.Count) {
                GameObject trackPiece = trackPieces[i + startTrackAmount];

                //reset position and angle before adjusting the track
                trackPiece.transform.position = Vector3.zero;
                trackPiece.transform.localEulerAngles = Vector3.zero;

                //adjust the track
                trackPiece.GetComponent<TrackPiece>().AdjustTrack(totalTrackAngle);

                //calculate adjustments
                //this finds the last bone plus half of the track size (because position is based off the center of the object
                Vector3 modifiedPosition = trackPieces[i + startTrackAmount - 1].transform.Find("Bottom_Rail/Joint_3_3/Joint_1_3/Joint_2_4/Joint_3_4/Joint_4_3/Joint_5_3/Joint_6_3/Joint_7_3/Joint_8_3/Joint_9_3/Joint_10_3").position;

                //need to offset it by trackBoneSize by the angle (for now just with y part of angle
                trackPiece.transform.position = modifiedPosition - (new Vector3(Mathf.Sin(eulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(eulerAngles.y * Mathf.Deg2Rad)) * (trackBoneSize * 5));

                //set track rotation (after adjustment to make sure the adjustment process goes well)
                trackPiece.transform.localEulerAngles = eulerAngles;

            } else {
                //calculate adjustments
                //this finds the last bone plus half of the track size (because position is based off the center of the object
                Vector3 modifiedPosition = trackPieces[i + startTrackAmount - 1].transform.Find("Bottom_Rail/Joint_3_3/Joint_1_3/Joint_2_4/Joint_3_4/Joint_4_3/Joint_5_3/Joint_6_3/Joint_7_3/Joint_8_3/Joint_9_3/Joint_10_3").position;

                GameObject trackPiece = AddTrackPiece(totalTrackAngle, modifiedPosition, eulerAngles);

            }
        }

        //remove all unneeded track pieces, don't add to i since trackPieces.Count will be continuing to shrink
        for (int i = startTrackAmount + totalTracksNeeded; i < trackPieces.Count;) {
            RemoveTrackPiece(trackPieces[i]);
        }

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
