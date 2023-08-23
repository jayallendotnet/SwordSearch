using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;
using DG.Tweening;

public class BoulderGroup : MonoBehaviour{

    public List<LetterSpace> coveredLetters;
    //public enum EnterDirection{Left, Right};
    //public EnterDirection enterDirection;

    public void MoveBouldersIntoPosition(){
        foreach (Transform t in transform){
            Vector2 originalPos = t.transform.position;
            StartBoulderInRandomPosition(t);
            float duration = StaticVariables.rand.Next(50, 100) / 100f;
            t.DOMove(originalPos, duration);
        }
    }

    public void ResetBouldersColor(){
        foreach(Transform t in transform)
            t.GetComponent<Image>().color = Color.white;
    }

    private void StartBoulderInRandomPosition(Transform boulder){
        int quad = StaticVariables.rand.Next(0, 3);
        int minX;
        int maxX;
        int minY;
        int maxY;
        if (quad == 0){ //left side
            minX = -900;
            maxX = -600;
            minY = 200;
            maxY = 1600;
        }
        else if (quad == 1){ //bottom
            minX = -600;
            maxX = 2600;
            minY = -800;
            maxY = 0;
        }
        else{ //right side
            minX = 2600;
            maxX = 2900;
            minY = 200;
            maxY = 1600;
        }
        
        int randomX = StaticVariables.rand.Next(minX, maxX);
        int randomY = StaticVariables.rand.Next(minY, maxY);
        Vector2 startingPos = new Vector2(randomX, randomY);
        boulder.position = startingPos;

    }
}
