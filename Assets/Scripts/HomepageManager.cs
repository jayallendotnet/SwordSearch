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
    public Text currentProgressText;
    public GameObject hometownBackground;
    public GameObject grasslandsBackground;

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
    private readonly float endlessModeMoveDuration = 12f;

    //temp for endless mode display
    public GameObject endlessModeEnemy1;
    public GameObject endlessModeEnemy2;
    public GameObject endlessModeEnemy3;
    public GameObject endlessModeEnemy4;
    public GameObject endlessModeEnemy5;
    public GameObject endlessModeEnemy6;
    public GameObject endlessModeEnemy7;


    void Start(){
        Setup();
    }

    public void Setup(){
        if (StaticVariables.currentBattleLevel < 0)
            StaticVariables.currentBattleLevel = 0;
        if (StaticVariables.currentBattleWorld < 0)
            StaticVariables.currentBattleWorld = 0;
        DisplayProgress();
        ShowEndlessModeEnemies();
        //set up endless mode display - cycle through all enemies currently defeated in order, starting at a random spot?
        //also show which powers have been unlocked so far, maybe a symbol for each power?
        //also put an info button in the corner of the endless mode pane
    }

    public void HitContinueAdventureButton(){
        switch (StaticVariables.highestUnlockedWorld){
            case 0:
                StaticVariables.FadeOutThenLoadScene(StaticVariables.world0Name);
                break;
            case 1:
                StaticVariables.FadeOutThenLoadScene(StaticVariables.world1Name);
                break;
        }
    }

    public void HitEndlessModeButton(){
        print("there's no endless mode yet, sorry :(");
    }

    public void HitSettingsButton(){
        StaticVariables.FadeOutThenLoadScene("Settings");
    }


    private void ShowEndlessModeEnemies(){
        endlessModeEnemyPrefabs = new List<GameObject>();
        endlessModeEnemyPrefabs.Add(endlessModeEnemy1);
        endlessModeEnemyPrefabs.Add(endlessModeEnemy2);
        endlessModeEnemyPrefabs.Add(endlessModeEnemy3);
        endlessModeEnemyPrefabs.Add(endlessModeEnemy4);
        endlessModeEnemyPrefabs.Add(endlessModeEnemy5);
        endlessModeEnemyPrefabs.Add(endlessModeEnemy6);
        endlessModeEnemyPrefabs.Add(endlessModeEnemy7);
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

    private void DisplayProgress(){
        string progressString = "";
        hometownBackground.SetActive(false);
        grasslandsBackground.SetActive(false);
        switch (StaticVariables.highestUnlockedWorld){
            case 0:
                progressString = "HOMETOWN";
                hometownBackground.SetActive(true);
                break;
            case 1:
                progressString = "GRASSLANDS";
                grasslandsBackground.SetActive(true);
                break;
            default:
                progressString = "";
                break;
        }
        progressString += " - " + StaticVariables.highestUnlockedLevel;
        currentProgressText.text = progressString;

    }
}
