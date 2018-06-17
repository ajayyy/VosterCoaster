using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerCoaster : MonoBehaviour {

    List<GameObject> trackPieces = new List<GameObject>();

    //this should be calculated on the spot
    float trackSize = -4.021012071f;


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

    }
}
