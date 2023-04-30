using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PuzzleGenerator : MonoBehaviour {

    //representations of the game board as 2d arrays
    //the first dimension is height, second dimension is width. [0,2] represents the third element of the top row
    private LetterSpace[,] letterSpaces;
    private char[,] letters;

    private List<char> randomLetterPool = new List<char>{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'};
    public WordDisplay wordDisplay;
    System.Random rand = new System.Random();


    void Start() {
        GetPuzzleDimensions();
        GenerateLetters("SWORD");
        RenderLetters();
    }


    private void GenerateLetters(){
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                int temp = rand.Next(randomLetterPool.Count);
                letters[i,j] = randomLetterPool[temp];
            }
        }
    }

    private void GenerateLetters(string requiredWord){

        string remainingWord = requiredWord;
        Vector2 previousSpace = new Vector2(-1,-1);
        int remainingLength = requiredWord.Length - 1;
        //Vector2 previousSpace = PlaceLetter(requiredWord[0], new Vector2(-1,-1), (requiredWord.Length - 1));
        bool cont = true;
        while (cont){
            previousSpace = PlaceLetter(remainingWord[0], previousSpace, (remainingWord.Length - 1));
            remainingWord = remainingWord.Substring(1);
            print("remaining word is " + remainingWord);
            remainingLength = remainingWord.Length - 1;
            if (remainingLength == -1)
                cont = false;
        }

        FillRestOfPuzzle();
    }

    private Vector2 PlaceLetter(char letter, Vector2 previousSpace, int remainingLettersAfter){

        //get a list of all possible spaces this letter can go in
        List<Vector2> candidates1 = GetAllPossibleSpacesForLetter(previousSpace);

        //remove all candidates that are out of bounds, and all that already contain values
        List<Vector2> candidates2 = RemoveImpossibleSpaces(candidates1);

        //remove all candidates that are inside a closed loop, if the loop is too small
        List<Vector2> candidates3 = RemoveSpacesInsideSmallLoop(candidates2);

        //pick one at random
        int temp = rand.Next(candidates3.Count);
        Vector2 selectedSpot = candidates3[temp];
        letters[(int)selectedSpot[0], (int)selectedSpot[1]] = letter;
        print("placed letter (" + letter + ") in position [" + selectedSpot[0] + "," + selectedSpot[1] + "]");
        //PrintPuzzle();
        return selectedSpot;
    }


    private List<Vector2> GetAllPossibleSpacesForLetter(Vector2 previousSpace){
        List<Vector2> candidates1 = new List<Vector2>();
        if ((previousSpace[0] == -1) && (previousSpace[1] == -1)){
            //this the first letter of a new word. all tiles are possible candidates
            for (int i = 0; i < letterSpaces.GetLength(0); i++){
                for (int j = 0; j < letterSpaces.GetLength(1); j++)
                    candidates1.Add(new Vector2(i, j));
            }
            return candidates1;
        }

        //this letter is part of a pre-existing word. get all of the previous letter's neighbors        
        candidates1.Add(previousSpace + Vector2.up);
        candidates1.Add(previousSpace + Vector2.right + Vector2.up);
        candidates1.Add(previousSpace + Vector2.right);
        candidates1.Add(previousSpace + Vector2.right + Vector2.down);
        candidates1.Add(previousSpace + Vector2.down);
        candidates1.Add(previousSpace + Vector2.left + Vector2.down);
        candidates1.Add(previousSpace + Vector2.left);
        candidates1.Add(previousSpace + Vector2.left + Vector2.up);
        
        return candidates1;
    }

    private List<Vector2> RemoveImpossibleSpaces(List<Vector2> candidates1){
        List<Vector2> candidates2 = new List<Vector2>();
        foreach (Vector2 candidate in candidates1){
            if ((candidate[0] > 0) && (candidate[0] < (letterSpaces.GetLength(0) - 1)) && (candidate[1] > 0) && (candidate[1] < (letterSpaces.GetLength(1) - 1))){
                if (letters[(int)candidate[0], (int)candidate[1]].Equals('-')){
                    candidates2.Add(candidate);
                }
            }  
        }
        return candidates2;
    }

    private List<Vector2> RemoveSpacesInsideSmallLoop(List<Vector2> candidates2){
        return candidates2;
    }

    private void FillRestOfPuzzle(){
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                if (letters[i,j].Equals('-')){
                int temp = rand.Next(randomLetterPool.Count);
                letters[i,j] = randomLetterPool[temp];
                }
                
            }
        }
    }

    private void RenderLetters(){    
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                letterSpaces[i, j].UpdateLetter(letters[i,j]);
            }
        }

    }

    private void GetPuzzleDimensions(){
        int totalCount = transform.childCount;
        int width = Mathf.FloorToInt(GetComponent<GridLayoutGroup>().constraintCount);
        int height = Mathf.FloorToInt(totalCount / width);
        letterSpaces = new LetterSpace[height, width];
        letters = new char[height, width];
        for (int i = 0; i < height; i++){
            for (int j = 0; j < width; j++){
                letterSpaces[i, j] = transform.GetChild((i * width) + j).GetComponent<LetterSpace>();
                letterSpaces[i,j].wordDisplay = wordDisplay;
                letterSpaces[i,j].position = new Vector2(i,j);
                letters[i,j] = '-';
            }
        }

    }

    private void PrintPuzzle(){
        string result = "";
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
                for (int j = 0; j < letterSpaces.GetLength(1); j++)
                    result += letters[i, j] + " ";
            print(result);
            result = "";
        }
    }
}
