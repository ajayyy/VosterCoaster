using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//This button will sit outside of the controller and be enabled/disabled when the trackpad is clicked and the item is selected
public class RadialToggle : MonoBehaviour {

    public bool optionEnabled;

    public Image image;
    public RectTransform rectTransform;

    public Sprite selected;
    public Sprite deselected;

    void Start() {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        UpdateImage();
    }

    void Update() {

    }

    public void Toggle() {
        optionEnabled = !optionEnabled;

        UpdateImage();
    }

    public void UpdateImage() {
        if (optionEnabled) {
            image.sprite = selected;
        } else {
            image.sprite = deselected;
        }
    }
}
