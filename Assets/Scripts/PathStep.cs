using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MyBox;

public class PathStep : MonoBehaviour{

    public void HideStep(float duration){
        Image im1 = transform.GetChild(0).GetComponent<Image>();
        Image im2 = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        Color im1Color = im1.color;
        Color im2Color = im2.color;

        im1Color.a = 0;
        im2Color.a = 0;

        if (duration == 0){
            im1.color = im1Color;
            im2.color = im2Color;
        }
        else{
            im1.DOColor(im1Color, duration);
            im2.DOColor(im2Color, duration);
        }
    }

    public void ShowStep(float duration){
        Image im1 = transform.GetChild(0).GetComponent<Image>();
        Image im2 = transform.GetChild(0).GetChild(0).GetComponent<Image>();

        Color im1Color = im1.color;
        Color im2Color = im2.color;

        im1Color.a = 1;
        im2Color.a = 1;

        if (duration == 0){
            im1.color = im1Color;
            im2.color = im2Color;
        }
        else{
            im1.DOColor(im1Color, duration);
            im2.DOColor(im2Color, duration);
        }
    }

}

