using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MagicFlash : MonoBehaviour{

    public Image image;
    public float waitTimeUntilStart = 0.5f;
    public float initialGrowthTime = 1f;
    public float initialGrowthSize = 1.5f;
    public float initialGrowthTransparency = 0.6f;
    public float totalPulseTime = 3f;
    public int numberOfPulses = 2;
    public float pulseSize = 1.3f;
    public float pulseTransparency = 0.7f;
    public float fullExpansionTime = 1.5f;
    public float fullExpansionSize = 13f;
    public float fullExpansionTransparency = 1f;
    public float holdTime = 1f;
    public float fadeTime = 0.5f;


    private int pulseCountRemaining = 0;
    private float timePerPulse = 0f;

    public float GetTotalTime(){
        return waitTimeUntilStart + initialGrowthTime + totalPulseTime + fullExpansionTime + holdTime + fadeTime;
    }

    public void StartProcess(Color color){
        //set up the circle to be the right color but really small
        transform.localScale = new Vector3 (0,0,0);
        Color startingColor = color;
        startingColor.a = 0;
        image.color = startingColor;
        //kick off the whole expansion process
        StaticVariables.WaitTimeThenCallFunction(waitTimeUntilStart, InitialGrowth);
    }

    private void InitialGrowth(){
        Color newColor = image.color;
        newColor.a = initialGrowthTransparency;
        transform.DOScale(initialGrowthSize, initialGrowthTime);
        image.DOColor(newColor, initialGrowthTime);
        StaticVariables.WaitTimeThenCallFunction(initialGrowthTime, StartPulses);
    }

    private void StartPulses(){
        //each pulse has 2 parts, contraction then expansion
        pulseCountRemaining = numberOfPulses;
        timePerPulse = totalPulseTime / numberOfPulses;
        StartPulseContraction();
    }
    
    private void StartPulseContraction(){
        if (pulseCountRemaining == 0){
            FullExpansion();
            return;
        }
        pulseCountRemaining --;
        Color newColor = image.color;
        newColor.a = pulseTransparency;
        transform.DOScale(pulseSize, timePerPulse);
        image.DOColor(newColor, timePerPulse);
        StaticVariables.WaitTimeThenCallFunction(timePerPulse, StartPulseExpansion);
    }

    private void StartPulseExpansion(){
        if (pulseCountRemaining == 0){
            FullExpansion();
            return;
        }
        pulseCountRemaining --;
        Color newColor = image.color;
        newColor.a = initialGrowthTransparency;
        transform.DOScale(initialGrowthSize, timePerPulse);
        image.DOColor(newColor, timePerPulse);
        StaticVariables.WaitTimeThenCallFunction(timePerPulse, StartPulseContraction);
    }


    private void FullExpansion(){
        Color newColor = image.color;
        newColor.a = fullExpansionTransparency;
        transform.DOScale(fullExpansionSize, fullExpansionTime);
        image.DOColor(newColor, fullExpansionTime);
        StaticVariables.WaitTimeThenCallFunction(fullExpansionTime, HoldExpanded);
    }

    private void HoldExpanded(){
        StaticVariables.WaitTimeThenCallFunction(holdTime, Fade);
    }

    private void Fade(){
        Color c = image.color;
        c.a = 0;
        image.DOColor(c, fadeTime);
        StaticVariables.WaitTimeThenCallFunction(fadeTime, DestroySelf);
    }

    public void DestroySelf(){
        Destroy(gameObject);
    }

}
