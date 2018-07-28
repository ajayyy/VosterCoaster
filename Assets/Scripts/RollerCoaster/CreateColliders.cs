using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateColliders : MonoBehaviour {

    RollerCoaster rollerCoaster;

    Vector3 offset = new Vector3(0, 1.51f, -0.21f);
    Vector3 size = new Vector3(1f, 1f, 0.5f);

    void Start () {
        rollerCoaster = GetComponent<RollerCoaster>();
	}
	
	void Update () {
        Mesh mesh = new Mesh();
        
	}

    //called when the roller coaster is finished. Merges all of the box colliders that would have been created into one so that the track is smooth (having mutliple box colliders causes many issues)
    //right: whether it should build colliders for the right side or the left (true: right, false: left)
    public Mesh BuildColliders(bool right) {
        //there are 10 box colliders (one for each joint)
        Mesh[] cubes = new Mesh[rollerCoaster.trackPieces.Count * 10];

        //create all of the cube meshes to combine
        List<Transform> bones = new List<Transform>();

        //find all the bones
        foreach (Transform child in GetComponentsInChildren<Transform>()) {
            //if right, check under right rail, otherwise check under left rail
            if (((right && child.parent.gameObject.name == "Right_Rail") || (!right && child.parent.gameObject.name == "Left_Rail")) || (bones.Count > 0 && child.parent == bones[bones.Count - 1])) {
                //second if statement as the above one is way too long
                //if the object is disabled, then it should not have colliders built for it
                //don't include the last joint as that does not have colliders
                if (child.gameObject.activeInHierarchy && child.gameObject.name != "Joint_10_2" && child.gameObject.name != "Joint_10") {
                    bones.Add(child);
                }
            }
        }

        //Create cubes and offset them based on each bone's position
        for (int b = 0; b < bones.Count; b++) {
            cubes[b] = CreatePlane(offset + bones[b].position / GameController.instance.scale, size, bones[b].rotation);
        }

        //now combine all of these cubes into one mesh
        CombineInstance[] combine = new CombineInstance[cubes.Length];
        for (int i = 0; i < combine.Length; i++) {
            combine[i] = new CombineInstance();
            combine[i].mesh = cubes[i];
            //print(cubes[i].subMeshCount);
            //combine[i].subMeshIndex = 100;
        }

        Mesh finalMesh = new Mesh();
        //combine the meshes without merging the submeshes
        finalMesh.CombineMeshes(combine, true, false);

        MeshUtility.Optimize(finalMesh);

        //simplify the mesh
        //UnityMeshSimplifier.MeshSimplifier meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();

        //meshSimplifier.Initialize(finalMesh);
        //meshSimplifier.SimplifyMesh(0.9f);

        //finalMesh = meshSimplifier.ToMesh();

        return finalMesh;
    }

    //modified from http://ilkinulas.github.io/development/unity/2016/04/30/cube-mesh-in-unity3d.html
    //modification adds the ability to specify an offset and size
    Mesh CreatePlane(Vector3 offset, Vector3 size, Quaternion angle) {
        //print(angle);

        Vector3[] vertices = {
            new Vector3 (-0.5f, 0.5f, -0.5f),
            new Vector3 (0.5f, 0.5f, -0.5f),
            new Vector3 (0.5f, -0.5f, -0.5f),
            new Vector3 (-0.5f, -0.5f, -0.5f),
            new Vector3 (-0.5f, -0.5f, 0.5f),
            new Vector3 (0.5f, -0.5f, 0.5f),
            new Vector3 (0.5f, 0.5f, 0.5f),
            new Vector3 (-0.5f, 0.5f, 0.5f),
        };

        for (int i = 0; i < vertices.Length; i++) {
            vertices[i].Scale(size);

            //rotate around center
            vertices[i] = MathHelper.RotatePointAroundPivot(vertices[i], Vector3.zero, angle);

            vertices[i] += offset;
        }

        int[] triangles = {
            2, 3, 4, //face top
			2, 4, 5,
        };

        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        return mesh;
    }

    //from http://wiki.unity3d.com/index.php/ReverseNormals
    void ReverseNormals(Mesh mesh) {
        Vector3[] normals = mesh.normals;

        for (int i = 0; i < normals.Length; i++) {
            normals[i] = -normals[i];
        }

        mesh.normals = normals;

        for (int m = 0; m < mesh.subMeshCount; m++) {
            int[] triangles = mesh.GetTriangles(m);

            for (int i = 0; i < triangles.Length; i += 3) {
                int temp = triangles[i + 0];
                triangles[i + 0] = triangles[i + 1];
                triangles[i + 1] = temp;
            }

            mesh.SetTriangles(triangles, m);
        }
    }
}
