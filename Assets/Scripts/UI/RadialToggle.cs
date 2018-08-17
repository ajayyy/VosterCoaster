using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//This button will sit outside of the controller and be enabled/disabled when the trackpad is clicked and the item is selected
public class RadialToggle : MonoBehaviour {

    public bool optionEnabled;

    public Image image;
    public RectTransform rectTransform;

    void Start() {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update() {

    }
}
