using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WindowButton : MonoBehaviour {

    //type 0: options toggle, type 1: exit button
    public int type = 0;
    //to toggle more than three options
    public int maxOptions = 2;

    //for each option
    public Sprite[] images;

    public int optionSelected = 0;

    public Image image;
    public RectTransform rectTransform;

	void Start () {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
	}
	
	void Update () {
		
	}

    public void Clicked() {
        if (type == 0) {
            optionSelected++;

            if (optionSelected >= maxOptions) {
                optionSelected = 0;
            }

            image.sprite = images[optionSelected];
        }
    }
}
