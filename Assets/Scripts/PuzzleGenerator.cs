using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PuzzleGenerator : MonoBehaviour {

    //representations of the game board as 2d arrays
    //the first dimension is height, second dimension is width. [0,2] represents the third element of the top row
    [HideInInspector]
    public LetterSpace[,] letterSpaces;
    private char[,] letters;
    private BattleManager.PowerupTypes[,] powerupTypes;
    [HideInInspector]
    public bool useSmallerLayout = false;
    private char[,] smallerLayout = {{'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'=', '=', '=', '=', '='}, {'=', '=', '=', '=', '='}, {'=', '=', '=', '=', '='}};
    List<BattleManager.PowerupTypes> powerupTypeSelection;
    private bool hasPickedBuffedTypeYet = false;

    [Header("Puzle Generation Rules")]
    public int wordCount = 3;
    public int maxAttemptsPerPuzzle = 3;
    public int minGenerationWordLength = 3;
    public int maxGenerationWordLength = 6;

    [Header("Letters To Update As The Page Turns")]
    public LetterSpace[] letterSection1;
    public LetterSpace[] letterSection2;
    public LetterSpace[] letterSection3;
    public LetterSpace[] letterSection4;

    [Header("Misc")]
    public BattleManager battleManager;

    public void Setup(){
        SetPowerupTypeList();
        GetPuzzleDimensions();
        GenerateNewPuzzle();
        UpdateAllLetterVisuals();

    }

    public void GenerateNewPuzzle(){
        foreach (LetterSpace ls in letterSpaces)
            ls.hasBeenUsedInAWordAlready = false;
        ClearPuzzle();
        bool succeeded = false;
        while (!succeeded)
            succeeded = PickWordsAndAttemptToGenerateSolution();
        FillRestOfPuzzle();
        ClearAllPowerups();
        PickAllSpacesForPowerups(); 
        SetNextPuzzleData();
    }

    public void SetCustomPowerups(char[,] powerupsLayout){
        ClearAllPowerups();
        for (int i = 0; i < powerupsLayout.GetLength(0); i++){
            for (int j = 0; j < powerupsLayout.GetLength(1); j++){
                switch (powerupsLayout[i,j]){
                    case ('H'):
                        letterSpaces[i,j].powerupType = BattleManager.PowerupTypes.Heal;
                        break;
                    case ('W'):
                        letterSpaces[i,j].powerupType = BattleManager.PowerupTypes.Water;
                        break;
                    case ('F'):
                        letterSpaces[i,j].powerupType = BattleManager.PowerupTypes.Fire;
                        break;
                    case ('L'):
                        letterSpaces[i,j].powerupType = BattleManager.PowerupTypes.Lightning;
                        break;
                    case ('D'):
                        letterSpaces[i,j].powerupType = BattleManager.PowerupTypes.Dark;
                        break;
                    case ('E'):
                        letterSpaces[i,j].powerupType = BattleManager.PowerupTypes.Earth;
                        break;
                    case ('S'):
                        letterSpaces[i,j].powerupType = BattleManager.PowerupTypes.Sword;
                        break;

                }
                letterSpaces[i,j].ShowPowerup();
            }
        }
    }

    private bool PickWordsAndAttemptToGenerateSolution(){
        string[] words = GetRandomWordsFromLibrary();
        //print("new puzzle about to be generated using words:");
        //foreach (string w in words)
        //    print(w);
        return AttemptToGenerateSolution(words);
    }

    private string[] GetRandomWordsFromLibrary(){
        string[] library = StaticVariables.wordLibraryForGeneration;
        if (useSmallerLayout){
            print("using smaller layout for generation");
            library = StaticVariables.wordLibraryForGeneratingSmallerPuzzles;
        }
        string[] result = new string[wordCount];
        for (int i = 0; i < wordCount; i++)
            result[i] = library[StaticVariables.rand.Next(library.Length)];
        return result;
    }


    private bool AttemptToGenerateSolution(string[] words){
        int attemptCount = 0;
        bool succeeded = true;
        while (attemptCount < maxAttemptsPerPuzzle){
            foreach (string word in words){
                bool couldGenerateWord = GenerateLetters(word);
                if (!couldGenerateWord)
                    succeeded = false;
            }
            if (succeeded)
                attemptCount += maxAttemptsPerPuzzle;
            else
                ClearPuzzle();
            attemptCount++;
        }
        return succeeded;
    }

    private bool GenerateLetters(string requiredWord){
        string remainingWord = requiredWord.ToUpper();;
        Vector2 previousSpace = new Vector2(-1,-1);
        int remainingLength = requiredWord.Length;
        bool cont = true;
        while (cont){
            previousSpace = PlaceLetter(remainingWord[0], previousSpace, remainingWord.Length);
            remainingWord = remainingWord.Substring(1);
            remainingLength = remainingWord.Length - 1;
            if (remainingLength == -1)
                cont = false;
            if ((previousSpace[0] == -1) && (previousSpace[1] == -1))
                return false;
        }
        return true;

    }

    private Vector2 PlaceLetter(char letter, Vector2 previousSpace, int numberOfSpacesRequiredForWordToFit){

        //get a list of all possible spaces this letter can go in
        List<Vector2> candidates1 = GetAllPossibleSpacesForLetter(previousSpace);

        //remove all candidates that are out of bounds, and all that already contain values
        List<Vector2> candidates2 = RemoveImpossibleSpaces(candidates1);

        //remove all candidates that are inside a closed loop, if the loop is too small
        List<Vector2> candidates3 = RemoveSpacesInsideSmallRegion(candidates2, numberOfSpacesRequiredForWordToFit);

        //print("picking the final spot from " + candidates3.Count + " possible candidates");

        //pick one at random
        if (candidates3.Count == 0){
            //print("there is no space to finish the word.");
            return new Vector2(-1,-1);
        }
        int temp = StaticVariables.rand.Next(candidates3.Count);
        Vector2 selectedSpot = candidates3[temp];
        letters[(int)selectedSpot[0], (int)selectedSpot[1]] = letter;
        //print("placed letter (" + letter + ") in position [" + selectedSpot[0] + "," + selectedSpot[1] + "]");
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
        candidates1 = GetNeighbors(previousSpace);
        
        return candidates1;
    }

    private List<Vector2> RemoveImpossibleSpaces(List<Vector2> candidates1){
        List<Vector2> candidates2 = new List<Vector2>();
        foreach (Vector2 candidate in candidates1){
            if ((candidate[0] >= 0) && (candidate[0] < letterSpaces.GetLength(0)) && (candidate[1] >= 0) && (candidate[1] < letterSpaces.GetLength(1))){
                if (letters[(int)candidate[0], (int)candidate[1]].Equals('-')){
                    candidates2.Add(candidate);
                }
            }  
        }
        return candidates2;
    }

    private List<Vector2> GetNeighbors(Vector2 position){
        //returns a list of coordinates adjacent to the provided position. Includes out-of-bounds options
        List<Vector2> neighbors = new List<Vector2>();
        neighbors.Add(position + Vector2.up);
        neighbors.Add(position + Vector2.right + Vector2.up);
        neighbors.Add(position + Vector2.right);
        neighbors.Add(position + Vector2.right + Vector2.down);
        neighbors.Add(position + Vector2.down);
        neighbors.Add(position + Vector2.left + Vector2.down);
        neighbors.Add(position + Vector2.left);
        neighbors.Add(position + Vector2.left + Vector2.up);
        return neighbors;
    }

    private List<Vector2> RemoveSpacesInsideSmallRegion(List<Vector2> candidates2, int requiredSizeOfRegion){
        List<Vector2> candidates3 = new List<Vector2>();

        List<List<Vector2>> regions = GetSeparateRegions(GetListOfAllSpaces());
        foreach (List<Vector2> region in regions){
            if (region.Count >= requiredSizeOfRegion){
                foreach(Vector2 space in region){
                    foreach (Vector2 candidate in candidates2){
                        if (candidate == space)
                            candidates3.Add(space);
                    }
                }
            }
        }

        if (requiredSizeOfRegion == 1)
            return candidates3;

        //check if placing the letter in a spot would create separate regions
        List<Vector2> candidates4 = new List<Vector2>();
        foreach (Vector2 candidate in candidates3){
            letters[(int)candidate[0], (int)candidate[1]] = '#';
            List<Vector2> regionContainingCandidate = regions[0];
            foreach (List<Vector2> region in regions){
                if (region.Contains(candidate))
                    regionContainingCandidate = region;
            }
            List<List<Vector2>> subRegions = GetSeparateRegions(regionContainingCandidate);
            bool isValid = false;
            foreach (List<Vector2> subRegion in subRegions){
                if (subRegion.Count >= requiredSizeOfRegion - 1)
                    isValid = true;
            }
            if (isValid)
                candidates4.Add(candidate);
            letters[(int)candidate[0], (int)candidate[1]] = '-';
        }
        return candidates4;
    }

    private List<Vector2> GetListOfAllSpaces(){
        List<Vector2> allSpaces = new List<Vector2>();
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                allSpaces.Add(new Vector2(i, j));
            }
        }
        return allSpaces;
    }


    private List<List<Vector2>> GetSeparateRegions(List<Vector2> spacesToCheck){
        //creates a list of separate regions in the puzzle area
        //each region is bounded by the walls of the puzzle or filled spaces

        //start with a list of all unfilled spaces
        List<Vector2> allSpaces = new List<Vector2>();
        foreach (Vector2 s in spacesToCheck){
            if (letters[(int)s[0],(int)s[1]].Equals('-'))
                allSpaces.Add(s);
        }


        List<List<Vector2>> regions = new List<List<Vector2>>();
        if (allSpaces.Count == 0)
            return regions;
        List<Vector2> firstRegion = new List<Vector2>();
        firstRegion.Add(allSpaces[0]);
        regions.Add(firstRegion);
        allSpaces.RemoveAt(0);

        foreach(Vector2 space in allSpaces){ //35 iterations

            //check if any of its neighbors are in a region already
            List<int> regionsThisBelongsTo = new List<int>();
            int regionIndex = 0;
            List<Vector2> neighbors = GetNeighbors(space);
            foreach (List<Vector2> region in regions){ //low int number of regions
                //check if a neighbor is in the region
                bool isNeighborInRegion = false;
                foreach (Vector2 neighbor in neighbors){ //8 iterations
                    if (region.Contains(neighbor)) //35 iterations
                        isNeighborInRegion = true;
                }
                if (isNeighborInRegion){
                    regionsThisBelongsTo.Add(regionIndex);
                    if (regionsThisBelongsTo.Count == 1)
                        region.Add(space); //only add the space to the first region. the regions will be combined later
                }
                regionIndex++;
            }


            if (regionsThisBelongsTo.Count > 1){
                //add the contents of each other region to the first region that this space belongs to
                foreach (int i in regionsThisBelongsTo){
                    if (i != regionsThisBelongsTo[0]){
                        foreach(Vector2 newSpaceToAdd in regions[i]){
                            regions[regionsThisBelongsTo[0]].Add(newSpaceToAdd);
                        }
                    }
                }
                //remove the other regions. they have been combined and are no longer required
                for (int z = regionsThisBelongsTo.Count - 1; z > 0; z--){
                    regions.RemoveAt(regionsThisBelongsTo[z]);
                }
            }
            else if (regionsThisBelongsTo.Count == 0){
                List<Vector2> newRegion = new List<Vector2>();
                newRegion.Add(space);
                regions.Add(newRegion);
            }
        }
        return regions;
    }

    private void FillRestOfPuzzle(){
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                if (letters[i,j].Equals('-')){
                    int index = StaticVariables.rand.Next(StaticVariables.randomLetterPool.Length);
                    char l = StaticVariables.randomLetterPool[index];
                    letters[i,j] = l;
                    if (l.Equals('Q')){
                        Vector2 temp = PlaceLetter('U', new Vector2(i,j), 1);
                        if ((temp[0] == -1) && (temp[1] == -1)){ //could not place a U nearby
                            letters[i,j] = 'E';
                        }
                    }
                }
            }
        }
    }

    public void FillPuzzleFromList(char[,] layout){
        letters = layout;        
        ClearAllPowerups();
        PickAllSpacesForPowerups(); 
        SetNextPuzzleData();
        UpdateAllLetterVisuals();
    }

    
    public void GenerateNextPuzzleForTutorial(char[,] layout){
        foreach (LetterSpace ls in letterSpaces)
            ls.hasBeenUsedInAWordAlready = false;
        letters = layout;        
        ClearAllPowerups();
        PickAllSpacesForPowerups(); 
        SetNextPuzzleData();
    }
    

    private void SetNextPuzzleData(){
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                LetterSpace ls = letterSpaces[i,j];
                ls.SetNextPuzzleData(letters[i,j], powerupTypes[i,j]);
            }
        }
    }

    private void UpdateAllLetterVisuals(){
        for (int i = 0; i < letterSpaces.GetLength(0); i++){
            for (int j = 0; j < letterSpaces.GetLength(1); j++){
                LetterSpace ls = letterSpaces[i,j];
                ls.ApplyNextPuzzleData();
                ls.ShowAsNotPartOfWord();
            }
        }
    }

    public void UpdateLetterVisualsForSection(int sectionNum){
        LetterSpace[] spaces;
        switch (sectionNum){
            case 1:
                spaces = letterSection1;
                break;
            case 2:
                spaces = letterSection2;
                break;
            case 3:
                spaces = letterSection3;
                break;
            default:
                spaces = letterSection4;
                break;
        }
        foreach (LetterSpace ls in spaces){
            ls.ApplyNextPuzzleData();
            ls.ShowAsNotPartOfWord();
        }
    }

    public void UpdateLetterVisual(LetterSpace ls){
        ls.ApplyNextPuzzleData();
        ls.ShowAsNotPartOfWord();
    }

    private void GetPuzzleDimensions(){
        int totalCount = transform.childCount;
        int width = Mathf.FloorToInt(GetComponent<GridLayoutGroup>().constraintCount);
        int height = Mathf.FloorToInt(totalCount / width);
        letterSpaces = new LetterSpace[height, width];
        letters = new char[height, width];
        powerupTypes = new BattleManager.PowerupTypes[height, width];
        for (int i = 0; i < height; i++){
            for (int j = 0; j < width; j++){
                letterSpaces[i, j] = transform.GetChild((i * width) + j).GetComponent<LetterSpace>();
                letterSpaces[i,j].battleManager = battleManager;
                letterSpaces[i,j].position = new Vector2(i,j);
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

    private void ClearPuzzle(){
        for (int i = 0; i < letters.GetLength(0); i++){
            for (int j = 0; j < letters.GetLength(1); j++){
                letters[i,j] = '-';
                if (useSmallerLayout)
                    letters[i,j] = smallerLayout[i,j];
            }
        }
    }

    private void ClearAllPowerups(){
        for (int i = 0; i < letters.GetLength(0); i++){
            for (int j = 0; j < letters.GetLength(1); j++){
                powerupTypes[i,j] = BattleManager.PowerupTypes.None;
            }
        }
    }

    private void PickAllSpacesForPowerups(){
        hasPickedBuffedTypeYet = false;
        for (int i = 0; i < StaticVariables.powerupsPerPuzzle; i++)
            PickRandomSpaceForPowerup(i == StaticVariables.powerupsPerPuzzle - 1);
    }

    private void PickRandomSpaceForPowerup(bool finalSpace){
        int t1 = StaticVariables.rand.Next(letters.GetLength(0));
        int t2 = StaticVariables.rand.Next(letters.GetLength(1));
        if (useSmallerLayout)
            t1 = StaticVariables.rand.Next(4);
        if (powerupTypes[t1, t2] != BattleManager.PowerupTypes.None)
            PickRandomSpaceForPowerup(finalSpace);
        else{
            powerupTypes[t1, t2] = PickRandomPowerupType(finalSpace);
        }  
    }
    
    private BattleManager.PowerupTypes PickRandomPowerupType(bool finalSpace){
        //randomly picks a powerup type. also takes buffed type into account!
        if (StaticVariables.buffedType != BattleManager.PowerupTypes.None){
            if (finalSpace && !hasPickedBuffedTypeYet) //if last powerup, and the buffed type has not been picked yet, guarantee it is the buffed type
                return StaticVariables.buffedType;
            int r = StaticVariables.rand.Next(0,100); //otherwise, pick a random number, with a sizable chance to pick the buffed type
            if (r <= 40){
                hasPickedBuffedTypeYet = true;
                return StaticVariables.buffedType;
            }
        }
        //if the buffed type was not selected, pick a random type from the remaining (non-buffed) types
        int range = powerupTypeSelection.Count;
        int i = StaticVariables.rand.Next(0,range);
        return powerupTypeSelection[i];
    }

    private void SetPowerupTypeList(){
        powerupTypeSelection = new List<BattleManager.PowerupTypes> {BattleManager.PowerupTypes.Water, BattleManager.PowerupTypes.Heal}; //these are always included
        if (StaticVariables.fireActive)
            powerupTypeSelection.Add(BattleManager.PowerupTypes.Fire);
        if (StaticVariables.earthActive)
            powerupTypeSelection.Add(BattleManager.PowerupTypes.Earth);
        if (StaticVariables.lightningActive)
            powerupTypeSelection.Add(BattleManager.PowerupTypes.Lightning);
        if (StaticVariables.darkActive)
            powerupTypeSelection.Add(BattleManager.PowerupTypes.Dark);
        if (StaticVariables.swordActive)
            powerupTypeSelection.Add(BattleManager.PowerupTypes.Sword);
        
        if (StaticVariables.buffedType != BattleManager.PowerupTypes.None)//remove the buffed type from the list, its prescence is determined separately
            powerupTypeSelection.Remove(StaticVariables.buffedType);
    }
}
