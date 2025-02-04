using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneFire : MonoBehaviour{

    private float originalHeight;
    private float flameGrowthTime = 0.5f;
    private RectTransform rect;

    private void Start(){
        rect = GetComponent<RectTransform>();
        originalHeight = rect.sizeDelta.y;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 0);

        //originalHeight = transform.localScale.y;
        //transform.localScale = new Vector2(transform.localScale.x, 0);
        float wait = StaticVariables.rand.Next(0, 200) / 100f;
        StaticVariables.WaitTimeThenCallFunction(wait, StartGrowth);
    }

    private void StartGrowth(){
        GetComponent<RectTransform>().DOSizeDelta(new Vector2(rect.sizeDelta.x, originalHeight), flameGrowthTime);
        //transform.DOScaleY(originalHeight, flameGrowthTime);
    }

    //public void DestroySelf(){
    //    Destroy(gameObject);
    //}

}
