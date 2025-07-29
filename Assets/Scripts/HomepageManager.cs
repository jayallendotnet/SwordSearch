using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyBox;
using DG.Tweening;
using Unity.VisualScripting;

public class HomepageManager : MonoBehaviour{

    //continue adventure display
    public Text hometownContinueAdventureText;
    public GameObject continueHometown;
    public GameObject continueGrasslands;
    public GameObject continueForest;
    public GameObject continueDesert;
    public GameObject continueCity;
    public GameObject continueFrostlands;
    public GameObject continueCaverns;
    public GameObject continueDragonsDen;
    public Transform hometownEnemySpace;
    public Transform grasslandsEnemySpace;
    public Transform forestEnemySpace;
    public Transform desertEnemySpace;
    public Transform cityEnemySpace;
    public Transform frostlandsEnemySpace;
    public Transform cavernsEnemySpace;
    public Transform dragonsDenEnemySpace;

    //endless mode display
    public Transform endlessModeEnemiesParent;
    public Transform endlessModeEndPosition;
    public Transform endlessModePosition1;
    public Transform endlessModePosition2;
    public Transform endlessModePosition3;
    public Transform endlessModePosition4;
    public GameObject emptyGameObject;

    private List<GameObject> endlessModeEnemyPrefabs;
    private int endlessModeEnemyIndex = 0;
    private readonly float endlessModeMoveDuration = 9f;

    void Start(){
        Setup();
    }

    public void Setup(){
        DisplayProgress();
        ShowEndlessModeEnemies();
    }

    public void HitContinueAdventureButton(){
        //some way to designate which stage to go to when the level loads
        //todo figure out what happens after you beat the last stage in the game
        StaticVariables.lastVisitedStage = StaticVariables.highestBeatenStage.nextStage;
        StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
    }

    public void HitEndlessModeButton(){
        print("there's no endless mode yet, sorry :(");
    }

    public void HitSettingsButton(){
        StaticVariables.FadeOutThenLoadScene("Settings");
    }

    public void HitMapButton(){
        StaticVariables.lastVisitedStage = StaticVariables.highestBeatenStage.nextStage;
        StaticVariables.FadeOutThenLoadScene(StaticVariables.mapName);
    }

    private List<GameObject> CreateEndlessModeEnemyList(){
        endlessModeEnemyPrefabs = new List<GameObject>();
        if (StaticVariables.highestBeatenStage == StaticVariables.allStages[0])
            return endlessModeEnemyPrefabs;
        bool addNextEnemy = true;
        StageData stage = StaticVariables.allStages[1]; //skip the "first stage", it represents 0 progress in the game
        while (addNextEnemy){
            if (stage.enemyPrefab.name != "Overworld Book")
                endlessModeEnemyPrefabs.Add(stage.enemyPrefab);
            if ((stage == StaticVariables.highestBeatenStage) || (stage.nextStage == null))
                addNextEnemy = false;
            else
                stage = stage.nextStage;
        }
        return endlessModeEnemyPrefabs;
    }

    private void ShowEndlessModeEnemies(){
        endlessModeEnemyPrefabs = CreateEndlessModeEnemyList();
        endlessModeEnemyPrefabs.Shuffle();
        if (endlessModeEnemyPrefabs.Count == 0)
            return;

        endlessModeEnemyIndex = 0;
        
        ShowNextEndlessModeEnemy(endlessModePosition1, endlessModeMoveDuration*0.25f);
        ShowNextEndlessModeEnemy(endlessModePosition2, endlessModeMoveDuration*0.5f);
        ShowNextEndlessModeEnemy(endlessModePosition3, endlessModeMoveDuration*0.75f);
        ShowNextEndlessModeEnemy();
    }

    private void ShowNextEndlessModeEnemy(Transform startingPosition, float moveDuration){
        GameObject enemyParent = GameObject.Instantiate(emptyGameObject, endlessModeEnemiesParent);
        enemyParent.transform.localPosition = startingPosition.localPosition;
        GameObject enemy = GameObject.Instantiate(endlessModeEnemyPrefabs[endlessModeEnemyIndex], enemyParent.transform);
        enemyParent.transform.DOLocalMoveX(endlessModeEndPosition.localPosition.x, moveDuration).SetEase(Ease.Linear).OnComplete(ShowNextEndlessModeEnemy);
        StaticVariables.WaitTimeThenCallFunction(moveDuration, GameObject.Destroy, enemyParent);
        endlessModeEnemyIndex++;
        if (endlessModeEnemyIndex >= endlessModeEnemyPrefabs.Count)
            endlessModeEnemyIndex = 0;
    }

    private void ShowNextEndlessModeEnemy(){
        //spawns an enemy at the default position, the rightmost spot outside of the render window
        ShowNextEndlessModeEnemy(endlessModePosition4, endlessModeMoveDuration);
    }

    private void DisplayProgress()
    {
        int nextEnemyWorldNum = StaticVariables.highestBeatenStage.nextStage.world;
        continueHometown.SetActive(nextEnemyWorldNum == 1);
        continueGrasslands.SetActive(nextEnemyWorldNum == 2);
        continueForest.SetActive(nextEnemyWorldNum == 3);
        continueDesert.SetActive(nextEnemyWorldNum == 4);
        continueCity.SetActive(nextEnemyWorldNum == 5);
        continueFrostlands.SetActive(nextEnemyWorldNum == 6);
        continueCaverns.SetActive(nextEnemyWorldNum == 7);
        continueDragonsDen.SetActive(nextEnemyWorldNum == 8);

        GameObject enemyPrefab = StaticVariables.highestBeatenStage.nextStage.enemyPrefab;
        Transform enemySpace = nextEnemyWorldNum switch
        {
            1 => hometownEnemySpace,
            2 => grasslandsEnemySpace,
            3 => forestEnemySpace,
            4 => desertEnemySpace,
            5 => cityEnemySpace,
            6 => frostlandsEnemySpace,
            7 => cavernsEnemySpace,
            8 => dragonsDenEnemySpace,
            _ => hometownEnemySpace,
        };

        GameObject enemyParent = GameObject.Instantiate(emptyGameObject, enemySpace.parent);
        enemyParent.transform.localPosition = enemySpace.localPosition;
        GameObject enemy = GameObject.Instantiate(enemyPrefab, enemyParent.transform);

        if ((nextEnemyWorldNum == 1) && (StaticVariables.highestBeatenStage.nextStage.stage == 1))
            hometownContinueAdventureText.text = "BEGIN\nADVENTURE";
    }
}
