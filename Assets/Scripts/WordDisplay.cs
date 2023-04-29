using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordDisplay : MonoBehaviour {

    private Text text;
    public List<LetterSpace> letterSpacesForWord = new List<LetterSpace>(){};
    public PuzzleGenerator puzzleGenerator;

    private LetterSpace lastLetterSpace = null;
    private LetterSpace secondToLastLetterSpace = null;

    void Start() {
        text = GetComponent<Text>();
        text.text = "";
    }

    private void SetLastTwoLetterSpaces(){
        lastLetterSpace = null;
        secondToLastLetterSpace = null;
        if (letterSpacesForWord.Count > 0)
            lastLetterSpace = letterSpacesForWord[letterSpacesForWord.Count - 1];
        if (letterSpacesForWord.Count > 1)
            secondToLastLetterSpace = letterSpacesForWord[letterSpacesForWord.Count - 2];

    }

    private void UpdateNeighborsForLastTwoLetterSpaces(){
        SetLastTwoLetterSpaces();
        if (lastLetterSpace != null)
            lastLetterSpace.ShowDirectionsToNeighbors();
        if (secondToLastLetterSpace != null)
            secondToLastLetterSpace.ShowDirectionsToNeighbors();
    }

    public void AddLetter(LetterSpace ls){
        text.text += ls.letter;
        letterSpacesForWord.Add(ls);
        if (lastLetterSpace != null){
            lastLetterSpace.nextLetterSpace = ls;
            ls.previousLetterSpace = lastLetterSpace;
        }
        UpdateNeighborsForLastTwoLetterSpaces();
    }

    public void RemoveLetter(LetterSpace ls){
        //assumes the last letter in the list is the provided letter
        if (text.text.Length < 2)
            text.text = "";
        else
            text.text = text.text.Substring(0, (text.text.Length - 1));
        letterSpacesForWord.Remove(ls);
        if (secondToLastLetterSpace != null){
            secondToLastLetterSpace.nextLetterSpace = null;
            lastLetterSpace.previousLetterSpace = null;
        }
        UpdateNeighborsForLastTwoLetterSpaces();
    }

    public bool CanAddLetter(LetterSpace letterSpace){
        if (letterSpacesForWord.Contains(letterSpace))
            return false;
        if (letterSpacesForWord.Count == 0)
            return true;
        if (letterSpace.IsAdjacentToLetterSpace(lastLetterSpace))
            return true;
        return false;
    }

    public bool CanRemoveLetter(LetterSpace letterSpace){
        if (letterSpacesForWord.Count == 0)
            return false;
        return (lastLetterSpace == letterSpace);
    }

}
