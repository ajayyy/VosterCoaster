#if UNITY_EDITOR

using UnityEngine;
using System.Collections;

[AddComponentMenu("Mesh/Vert Handler")]
[ExecuteInEditMode]
public class VertHandler : MonoBehaviour {

    public bool _destroy;

    private Mesh mesh;
    private Vector3[] verts;
    private Vector3 vertPos;
    private GameObject[] handles;

    private const string TAG_HANDLE = "VertHandle";

    void OnEnable() {
        mesh = GetComponent<MeshFilter>().mesh;
        verts = mesh.vertices;
        foreach (Vector3 vert in verts) {
            vertPos = transform.TransformPoint(vert);
            GameObject handle = new GameObject(TAG_HANDLE);
            //         handle.hideFlags = HideFlags.DontSave;
            handle.transform.position = vertPos;
            handle.transform.parent = transform;
            handle.tag = TAG_HANDLE;
            handle.AddComponent<VertHandleGizmo>()._parent = this;

        }
    }

    void OnDisable() {
        GameObject[] handles = GameObject.FindGameObjectsWithTag(TAG_HANDLE);
        foreach (GameObject handle in handles) {
            DestroyImmediate(handle);
        }
    }

    void Update() {
        if (_destroy) {
            _destroy = false;
            DestroyImmediate(this);
            return;
        }

        handles = GameObject.FindGameObjectsWithTag(TAG_HANDLE);

        //print(verts.Length);

        float angleAdjustment = 0;

        for (int i = 0; i < verts.Length; i++) {

            if(i % 30 == 0) {
                angleAdjustment += 5;
            }

            int number = i / 30 - 10;
            if (number < 0) number = 0;

            float fullAngleX = Mathf.Cos(angleAdjustment * Mathf.Deg2Rad);
            float fullAngleY = Mathf.Sin(angleAdjustment * Mathf.Deg2Rad);

            //print(fullAngleX + "   " + fullAngleY);

            verts[i] = handles[i].transform.localPosition + Vector3.up * (fullAngleY) + Vector3.right * fullAngleX + Vector3.up;
        }

        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


    }

}

[ExecuteInEditMode]
public class VertHandleGizmo : MonoBehaviour {

    private static float CURRENT_SIZE = 0.1f;

    public float _size = CURRENT_SIZE;
    public VertHandler _parent;
    public bool _destroy;

    private float _lastKnownSize = CURRENT_SIZE;

    void Update() {
        // Change the size if the user requests it
        if (_lastKnownSize != _size) {
            _lastKnownSize = _size;
            CURRENT_SIZE = _size;
        }

        // Ensure the rest of the gizmos know the size has changed...
        if (CURRENT_SIZE != _lastKnownSize) {
            _lastKnownSize = CURRENT_SIZE;
            _size = _lastKnownSize;
        }

        if (_destroy)
            DestroyImmediate(_parent);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, Vector3.one * CURRENT_SIZE);
    }

}
#endif