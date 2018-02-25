using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOpacityChanger : MonoBehaviour {

    public float opacity;

    public Material material;

	void Start () {

	}
	
	void Update () {
        if(material.color.a != opacity) {
            material.color = new Color(material.color.r, material.color.g, material.color.b, opacity);
        }
    }
}
