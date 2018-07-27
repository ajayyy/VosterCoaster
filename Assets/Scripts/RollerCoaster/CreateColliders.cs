using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateColliders : MonoBehaviour {

    RollerCoaster rollerCoaster;

    Vector3 offset = new Vector3(0, 1.51f, -0.21f);
    Vector3 size = new Vector3(0.5f, 0.2f, 0.5f);

    void Start () {
        rollerCoaster = GetComponent<RollerCoaster>();
	}
	
	void Update () {
        Mesh mesh = new Mesh();
        
	}

    //called when the roller coaster is finished. Merges all of the box colliders that would have been created into one so that the track is smooth (having mutliple box colliders causes many issues)
    public Mesh BuildColliders() {
        //there are 10 box colliders (one for each joint), on both sides of each track piece.
        Mesh[] cubes = new Mesh[rollerCoaster.trackPieces.Count * 20];

        //create all of the cube meshes to combine
        for (int i = 0; i < rollerCoaster.trackPieces.Count; i++) {
            GameObject trackPieceObject = rollerCoaster.trackPieces[i];
            TrackPiece trackPiece = trackPieceObject.GetComponent<TrackPiece>();

            List<Transform> rightBones = new List<Transform>();
            List<Transform> leftBones = new List<Transform>();

            //find all the bones
            foreach (Transform child in GetComponentsInChildren<Transform>()) {
                if (child.parent.gameObject.name == "Right_Rail" || (rightBones.Count > 0 && child.parent == rightBones[rightBones.Count - 1])) {
                    rightBones.Add(child);
                }

                if (child.parent.gameObject.name == "Left_Rail" || (leftBones.Count > 0 && child.parent == leftBones[leftBones.Count - 1])) {
                    leftBones.Add(child);
                }
                print(child.parent.gameObject.name);
            }

            print(rightBones.Count + " " + leftBones.Count);

            //Create cubes and offset them based on each bone's position
            for (int b = 0; b < rollerCoaster.boneAmount; b++) {
                print(i + " " + (b * 2));
                cubes[i + (b * 2)] = CreateCube(offset + rightBones[b].position, size);
                cubes[i + (b * 2) + 1] = CreateCube(offset + leftBones[b].position, size);
            }
        }

        //now combine all of these cubes into one mesh
        CombineInstance[] combine = new CombineInstance[cubes.Length];
        for (int i = 0; i < combine.Length; i++) {
            combine[i].mesh = cubes[i];
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(combine);

        return finalMesh;
    }

    //modified from http://ilkinulas.github.io/development/unity/2016/04/30/cube-mesh-in-unity3d.html
    //modification adds the ability to specify an offset and size
    Mesh CreateCube(Vector3 offset, Vector3 size) {

        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        for(int i=0; i < vertices.Length; i++) {
            vertices[i].Scale(size);
            vertices[i] += offset;
        }

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        return mesh;
    }
}
