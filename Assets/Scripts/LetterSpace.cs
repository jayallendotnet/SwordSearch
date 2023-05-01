using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterSpace : MonoBehaviour{

    [HideInInspector]
    public char letter;
    public Text text;
    public GameObject selectedSignifier;

    //directional connectors
    public GameObject topConnector;
    public GameObject topRightConnector;
    public GameObject rightConnector;
    public GameObject bottomRightConnector;
    public GameObject bottomConnector;
    public GameObject bottomLeftConnector;
    public GameObject leftConnector;
    public GameObject topLeftConnector;

    [HideInInspector]
    public WordDisplay wordDisplay;

    //when the letter space is part of the word, previousletterspace and nextletterspace link to other letters in the word
    [HideInInspector]
    public LetterSpace previousLetterSpace = null;
    [HideInInspector]
    public LetterSpace nextLetterSpace = null;


    public Vector2 position = Vector2.zero;

    [HideInInspector]
    public bool hasBeenUsedInAWordAlready = false;

    public void UpdateLetter(char letter){
        this.letter = letter;
        text.text = "" + letter;
    }

    public void ClickedLetter(){
        if (wordDisplay.CanAddLetter(this))
            AddLetter();
        else if (wordDisplay.CanRemoveLetter(this))
            RemoveLetter();
    }

    private void AddLetter(){
        selectedSignifier.SetActive(true);
        text.color = Color.white;
        wordDisplay.AddLetter(this);
    }

    private void RemoveLetter(){
        StopDisplayingLetter();
        wordDisplay.RemoveLetter(this);
    }

    public void StopDisplayingLetter(){
        selectedSignifier.SetActive(false);
        HideAllDirectionLines();
        text.color = Color.black;
    }

    private void HideAllDirectionLines(){
        foreach (Transform t in topLeftConnector.transform.parent)
            t.gameObject.SetActive(false);
    }

    public void ShowDirectionsToNeighbors(){
        HideAllDirectionLines();
        ShowDirectionToNeighbor(previousLetterSpace);
        ShowDirectionToNeighbor(nextLetterSpace);
    }

    private void ShowDirectionToNeighbor(LetterSpace otherSpace){
        //if the selected letterspace is not adjacent, do not show anything
        if (otherSpace == null)
            return;
        Vector2 distance = GetDistanceToLetterSpace(otherSpace);
        if ((distance[0] == 1) && (distance[1] == 0)){
            topConnector.SetActive(true);
        }
        if ((distance[0] == 1) && (distance[1] == -1)){
            topRightConnector.SetActive(true);
        }
        if ((distance[0] == 0) && (distance[1] == -1)){
            rightConnector.SetActive(true);
        }
        if ((distance[0] == -1) && (distance[1] == -1)){
            bottomRightConnector.SetActive(true);
        }
        if ((distance[0] == -1) && (distance[1] == 0)){
            bottomConnector.SetActive(true);
        }
        if ((distance[0] == -1) && (distance[1] == 1)){
            bottomLeftConnector.SetActive(true);
        }
        if ((distance[0] == 0) && (distance[1] == 1)){
            leftConnector.SetActive(true);
        }
        if ((distance[0] == 1) && (distance[1] == 1)){
            topLeftConnector.SetActive(true);
        }
    }
    
    public Vector2 GetDistanceToLetterSpace(LetterSpace otherSpace){
        if (otherSpace == null)
            return Vector2.zero;

        Vector2 distance = (position - otherSpace.position);
        return distance;
    }

    public bool IsAdjacentToLetterSpace(LetterSpace otherSpace){
        Vector2 distance = GetDistanceToLetterSpace(otherSpace);
        if ((Mathf.Abs(distance[0]) < 2) && (Mathf.Abs(distance[1]) < 2))
            return true;
        return false;
    }

    public void ShowHasBeenUsedForWord(){
        if (hasBeenUsedInAWordAlready)
            text.color = Color.gray;
        else
            text.color = Color.black;
    }
}
