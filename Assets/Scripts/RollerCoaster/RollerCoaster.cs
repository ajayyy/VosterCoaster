using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerCoaster : MonoBehaviour {

    List<GameObject> trackPieces = new List<GameObject>();

    //List containing disabled track pieces. This is used because creating and destroying gameobjects constantly causes massive amounts of lag.
    List<GameObject> unusedTrackPieces = new List<GameObject>();

    //the scale the world is set at
    public static float scale = 0.008f;

    //the length of one track's bone
    public float trackBoneSize = 0.402642f;

    //the prefab for an empty piece of track
    public GameObject trackPrefab;

    //just for now, the right controller is going to be loaded in here
    public GameObject rightController;

    void Start () {
        //just for now, since we must start with one
        transform.Find("TrackPiece0").gameObject.GetComponent<TrackPiece>().rollerCoaster = this;
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

        //the position of the first track piece that will be a part of this new edition (previous track pieces are not edited)
        //Vector3 startPosition = startTrack.transform.position + new Vector3(Mathf.Cos(getCurrentAngle(startTrack).y) * trackBoneSize * 10f, 0, Mathf.Sin(getCurrentAngle(startTrack).y) * trackBoneSize * 10f);
        Vector3 startPosition = startTrack.transform.position;

        Vector3 targetAngle = new Vector3(0, 1, 0) * rightController.transform.eulerAngles.y;
        Vector3 currentAngle = getCurrentAngle(startTrack);
        Vector3 angleDifference = targetAngle - currentAngle;
        //make sure the smallest difference between the angles is found
        Vector3 smallestAngleDifference = new Vector3(Mathf.Abs(angleDifference.x), Mathf.Abs(angleDifference.y), Mathf.Abs(angleDifference.z));
        //do 360 - angle if over 180 for each (see https://stackoverflow.com/questions/6722272/smallest-difference-between-two-angles)
        {
            float x1 = smallestAngleDifference.x;
            float y1 = smallestAngleDifference.y;
            float z1 = smallestAngleDifference.z;

            if (x1 > 180) {
                x1 = 360 - x1;
            }

            if (y1 > 180) {
                y1 = 360 - y1;
            }

            if (z1 > 180) {
                z1 = 360 - z1;
            }

            smallestAngleDifference = new Vector3(x1, y1, z1);
        }

        //get amount of tracks needed by dividing by length of one track's bone then dividing by amount of bones per track piece
        //int for now just to make things easier
        //for now just set to a static number

        //that many tracks can now be created with an angle of angle.y divided by each bone (tracksNeeded * 10f)

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
        float startTracksNeeded = Mathf.Abs(distanceFromStart / (trackBoneSize * 10f));
        float targetTracksNeeded = (int) Mathf.Abs(distanceFromTarget / (trackBoneSize * 10f));
        float curveTracksNeeded = 0;

        //if the controller is on the right side
        bool rightSide = rightController.transform.position.x > 0;

        //amount to check for for the first if statement in the curve
        float checkAmount = startTracksNeeded;
    
        if (Mathf.Min(startTracksNeeded, targetTracksNeeded) == checkAmount) {
            //find intersection between line to the end of curve from the start of curve
            float startToEndCurveSlope = Mathf.Tan((((180 - targetAngle.y) / 2) - getCurrentAngle(startTrack).y) * Mathf.Deg2Rad);
            //the b value (b = y - mx)
            float startToEndCurveB = startTrack.transform.position.z - startToEndCurveSlope * startTrack.transform.position.x;

            //find intersection between this line and the target line (x = (b2 - b1) / (m1 - m2))
            //this position will be the second point on the circle of the curve (end point), the first is the start track
            float circleTargetX = (startToEndCurveB - targetB) / (targetSlope - startToEndCurveSlope);
            float circleTargetY = startToEndCurveSlope * circleTargetX + startToEndCurveB;
            //startTrack.transform.position = new Vector3(circleTargetX, 0, circleTargetY);

            //y = rsinA, x = rcosA
            //these are the positions of these angles on a circle with a radius of 1
            float targetNormalX = Mathf.Cos((-targetAngle.y + 360) * Mathf.Deg2Rad);
            float targetNormalY = Mathf.Sin((-targetAngle.y + 360) * Mathf.Deg2Rad);
            float startNormalX = Mathf.Cos(getCurrentAngle(startTrack).y * Mathf.Deg2Rad);
            float startNormalY = Mathf.Sin(getCurrentAngle(startTrack).y * Mathf.Deg2Rad);

            //the radius would be equal to 1 for a circle like this. Find how much the distances between the points account for the radius of the circle
            float percentageOfRadius = Mathf.Sqrt(Mathf.Pow(startNormalX - targetNormalX, 2) + Mathf.Pow(startNormalY - targetNormalY, 2));

            //radius of the curve using the percentage calculations from above
            float radius = Mathf.Sqrt(Mathf.Pow(circleTargetX - startPosition.x, 2) + Mathf.Pow(circleTargetY - startPosition.z, 2)) / percentageOfRadius;

            //calculate the cirumference of this circle multiplied by the amount this curve takes up of the whole circle
            float curveLength = 2 * Mathf.PI * radius * (smallestAngleDifference.y / 360f);

            curveTracksNeeded = (curveLength / (trackBoneSize * 10f));

            startTracksNeeded = 0;

            //Find difference between circleTarget and the target position
            targetTracksNeeded = (Mathf.Sqrt(Mathf.Pow(circleTargetX - rightController.transform.position.x, 2) + Mathf.Pow(circleTargetY - rightController.transform.position.z, 2)) / (trackBoneSize * 10f));

        } else {
            //find intersection between line to the start of curve from the end of curve
            float endToStartCurveSlope = Mathf.Tan((((180 - targetAngle.y) / 2) - getCurrentAngle(startTrack).y) * Mathf.Deg2Rad);
            //the b value (b = y - mx)
            float endToStartCurveB = rightController.transform.position.z - endToStartCurveSlope * rightController.transform.position.x;

            //find intersection between this line and the start line (x = (b2 - b1) / (m1 - m2))
            //this position will be the second point on the circle of the curve (end point), the first is the target track
            float circleStartX = (endToStartCurveB - startB) / (startSlope - endToStartCurveSlope);
            float circleStartY = endToStartCurveSlope * circleStartX + endToStartCurveB;

            //y = rsinA, x = rcosA
            //these are the positions of these angles on a circle with a radius of 1
            float targetNormalX = Mathf.Cos((-targetAngle.y + 360) * Mathf.Deg2Rad);
            float targetNormalY = Mathf.Sin((-targetAngle.y + 360) * Mathf.Deg2Rad);
            float startNormalX = Mathf.Cos(getCurrentAngle(startTrack).y * Mathf.Deg2Rad);
            float startNormalY = Mathf.Sin(getCurrentAngle(startTrack).y * Mathf.Deg2Rad);

            //the radius would be equal to 1 for a circle like this. Find how much the distances between the points account for the radius of the circle
            float percentageOfRadius = Mathf.Sqrt(Mathf.Pow(startNormalX - targetNormalX, 2) + Mathf.Pow(startNormalY - targetNormalY, 2));

            //radius of the curve using the percentage calculations from above
            float radius = Mathf.Sqrt(Mathf.Pow(circleStartX - rightController.transform.position.x, 2) + Mathf.Pow(circleStartY - rightController.transform.position.z, 2)) / percentageOfRadius;

            //calculate the cirumference of this circle multiplied by the amount this curve takes up of the whole circle
            float curveLength = 2 * Mathf.PI * radius * (smallestAngleDifference.y / 360f);

            curveTracksNeeded = (curveLength / (trackBoneSize * 10f));

            //Find difference between circleTarget and the target position
            startTracksNeeded = (Mathf.Sqrt(Mathf.Pow(circleStartX - startTrack.transform.position.x, 2) + Mathf.Pow(circleStartY - startTrack.transform.position.z, 2)) / (trackBoneSize * 10f));

            targetTracksNeeded = 0;
        }

        Func<int> totalTracksNeeded = () => Mathf.CeilToInt(startTracksNeeded) + Mathf.CeilToInt(curveTracksNeeded) + Mathf.CeilToInt(targetTracksNeeded);

        if (rightSide) {
            smallestAngleDifference = new Vector3(smallestAngleDifference.x, -(smallestAngleDifference.y), smallestAngleDifference.z);
        }

        //check if this is actually a proper angle to create a track
        if(((angleDifference.y < 180 || angleDifference.y > 270) && rightSide) || ((angleDifference.y < 90 || angleDifference.y > 180) && !rightSide)) {
            totalTracksNeeded = () => 0;
        }

        //angle to start from when curves start if part of the curve is drawn during the start tracks
        Vector3 startTrackAngle = Vector3.zero;

        //Amount of tracks already placed down
        int startTrackAmount = trackPieces.IndexOf(startTrack) + 1;
        for (int i = 0; i < totalTracksNeeded(); i++) {
            Vector3 eulerAngles = getCurrentAngle(startTrack);
            //the total angle going through one whole track piece
            Vector3 totalTrackAngle = Vector3.zero;

            float percentageOfTrack = 1;

            if (i < Mathf.CeilToInt(startTracksNeeded)) {
                //set it to the part of the track nessesary to finish drawing the startTracksNeeded
                percentageOfTrack = 1;
                if (startTracksNeeded - i < 1) {
                    percentageOfTrack = startTracksNeeded - i;

                    int curveStartNum = (int)((1 - percentageOfTrack) * 10f);

                    totalTrackAngle = (smallestAngleDifference / (curveTracksNeeded * 10f)) * curveStartNum;

                    startTrackAngle = totalTrackAngle;
                    smallestAngleDifference -= startTrackAngle;

                    //the remaining part of the track can be used to start the curve
                    curveTracksNeeded -= curveStartNum / 10f;
                }
            }

            if (i >= Mathf.CeilToInt(startTracksNeeded)) {
                //then it is time to create a curve instead of just a straight line coming off the start track
                //calculate the adjustment needed for the curve
                eulerAngles = smallestAngleDifference / curveTracksNeeded * (i - Mathf.CeilToInt(startTracksNeeded)) + getCurrentAngle(startTrack);
                eulerAngles += startTrackAngle;

                totalTrackAngle = smallestAngleDifference / curveTracksNeeded;

                //set it to the part of the track nessesary to finish drawing the curveTracksNeeded
                percentageOfTrack = 1;
                if (curveTracksNeeded - (i - Mathf.CeilToInt(startTracksNeeded)) < 1) {
                    percentageOfTrack = curveTracksNeeded - (i - Mathf.CeilToInt(startTracksNeeded));
                }
            }

            if (i >= Mathf.CeilToInt(startTracksNeeded) + Mathf.CeilToInt(curveTracksNeeded)) {
                //back to straight path, but in the angle of the target
                eulerAngles = targetAngle;
                totalTrackAngle = Vector3.zero;

                //set it to the part of the track nessesary to finish drawing the targetTracksNeeded
                percentageOfTrack = 1;
                if (targetTracksNeeded - (i - Mathf.CeilToInt(startTracksNeeded) - Mathf.CeilToInt(curveTracksNeeded)) < 1) {
                    percentageOfTrack = targetTracksNeeded - (i - Mathf.CeilToInt(startTracksNeeded) - Mathf.CeilToInt(curveTracksNeeded));
                }
            }

            int curveStart = -1;
            if (percentageOfTrack < 1 && i < Mathf.CeilToInt(startTracksNeeded)) {
                //the remaining track will be used for the curve
                curveStart = (int)((percentageOfTrack) * 10f);
            }

            if (startTrackAmount + i < trackPieces.Count) {
                GameObject trackPiece = trackPieces[i + startTrackAmount];

                //reset position and angle before adjusting the track
                trackPiece.transform.position = Vector3.zero;
                trackPiece.transform.localEulerAngles = Vector3.zero;

                //adjust the track
                trackPiece.GetComponent<TrackPiece>().AdjustTrack(totalTrackAngle, percentageOfTrack, curveStart);

                //calculate adjustments
                //this finds the last bone plus half of the track size (because position is based off the center of the object
                Vector3 modifiedPosition = trackPieces[i + startTrackAmount - 1].transform.Find("Bottom_Rail/Joint_3_3/Joint_1_3/Joint_2_4/Joint_3_4/Joint_4_3/Joint_5_3/Joint_6_3/Joint_7_3/Joint_8_3/Joint_9_3/Joint_10_3").position;

                //need to offset it by trackBoneSize by the angle (for now just with y part of angle
                trackPiece.transform.position = modifiedPosition - (new Vector3(Mathf.Sin(eulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(eulerAngles.y * Mathf.Deg2Rad)) * (trackBoneSize * 5f));

                //set track rotation (after adjustment to make sure the adjustment process goes well)
                trackPiece.transform.localEulerAngles = eulerAngles;

            } else {
                //calculate adjustments
                //this finds the last bone plus half of the track size (because position is based off the center of the object
                Vector3 modifiedPosition = trackPieces[i + startTrackAmount - 1].transform.Find("Bottom_Rail/Joint_3_3/Joint_1_3/Joint_2_4/Joint_3_4/Joint_4_3/Joint_5_3/Joint_6_3/Joint_7_3/Joint_8_3/Joint_9_3/Joint_10_3").position;

                GameObject trackPiece = AddTrackPiece(totalTrackAngle, modifiedPosition, eulerAngles, percentageOfTrack, curveStart);

            }
        }

        //remove all unneeded track pieces, don't add to i since trackPieces.Count will be continuing to shrink
        for (int i = Mathf.CeilToInt(startTrackAmount + totalTracksNeeded()); i < trackPieces.Count;) {
            RemoveTrackPiece(trackPieces[i]);
        }

    }

    public GameObject AddTrackPiece (Vector3 totalAngle, Vector3 modifiedPosition, Vector3 eulerAngles, float percentageOfTrack, int curveStart) {
        GameObject newTrackPiece;

        if(unusedTrackPieces.Count > 0) {
            newTrackPiece = unusedTrackPieces[0];
            unusedTrackPieces.RemoveAt(0);

            newTrackPiece.SetActive(true);
        } else {
            newTrackPiece = Instantiate(trackPrefab, transform);

            TrackPiece trackPieceClass = newTrackPiece.GetComponent<TrackPiece>();

            trackPieceClass.Start();
            trackPieceClass.rollerCoaster = this;
        }

        //reset position and angle before adjusting the track
        newTrackPiece.transform.position = Vector3.zero;
        newTrackPiece.transform.localEulerAngles = Vector3.zero;

        TrackPiece newTrackPieceClass = newTrackPiece.GetComponent<TrackPiece>();

        newTrackPieceClass.totalAngle = totalAngle;
        trackPieces.Add(newTrackPiece);

        //adjust the track
        newTrackPieceClass.AdjustTrack(totalAngle, percentageOfTrack, curveStart);

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
