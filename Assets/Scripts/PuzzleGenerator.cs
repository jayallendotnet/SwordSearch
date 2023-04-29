using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PuzzleGenerator : MonoBehaviour {

    //representations of the game board as 2d arrays
    //the first dimension is height, second dimension is width. [0,2] represents the third element of the top row
    private GameObject[,] letterSpaces;
    private string[,] letters;

    private List<string> randomLetterPool = new List<string>{"A", "B", "C", "D", "E", "F", "G", "H"};


    void Start() {
        GetPuzzleDimensions();
        GenerateLetters();
        RenderLetters();
    }


    private void GenerateLetters(){
        System.Random rand = new System.Random();
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                int temp = rand.Next(randomLetterPool.Count);
                letters[i,j] = randomLetterPool[temp];
            }
        }
    }

    private void RenderLetters(){    
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                letterSpaces[i, j].transform.GetChild(0).GetComponent<Text>().text = letters[i,j];
            }
        }

    }

    private void GetPuzzleDimensions(){
        int totalCount = transform.childCount;
        int width = Mathf.FloorToInt(GetComponent<GridLayoutGroup>().constraintCount);
        int height = Mathf.FloorToInt(totalCount / width);
        if (totalCount != (width * height))
            print("your puzzle shape is not a square!!!! idiot moron");
        letterSpaces = new GameObject[height, width];
        letters = new string[height, width];
        for (int i = 0; i < height; i++){
            for (int j = 0; j < width; j++){
                letterSpaces[i, j] = transform.GetChild((i * width) + j).gameObject;
                letters[i,j] = "";
            }
        }


    }

}
