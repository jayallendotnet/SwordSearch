using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MyBox;
using DG.Tweening;
using Unity.VisualScripting;

public class LaunchGameSetup : MonoBehaviour{

    public List<GameObject> hometownEnemies;
    public List<GameObject> grasslandsEnemies;
    public List<GameObject> enchantedForestEnemies;
    public List<GameObject> desertEnemies;
    public List<GameObject> cityEnemies;
    public List<GameObject> frostlandsEnemies;
    public List<GameObject> cavernsEnemies;
    public List<GameObject> dragonsDenEnemies;

    private List<StageData> allStages;

    //just used for generating the list of all stages
    private StageData previousStage;
    //private List<StageData> hometownStages;
    //private List<StageData> grasslandsStages;

    void Start(){
        SetupStageList();
        //this is where we load the player's progress data, in the future from the game save data
        StaticVariables.highestBeatenStage = StaticVariables.GetStage(4, 12);
        SceneManager.LoadScene(StaticVariables.mainMenuName);
    }



    private void SetupStageList(){
        allStages = new List<StageData>();

        //create a dummy stage to represent "has not beaten any stages yet"
        StageData dummyStage = new (-1, "has not started", -1, null);
        dummyStage.previousStage = null;
        previousStage = dummyStage;
        allStages.Add(dummyStage);

        CreateStagesForEnemiesInWorld(1, hometownEnemies);
        CreateStagesForEnemiesInWorld(2, grasslandsEnemies);
        CreateStagesForEnemiesInWorld(3, enchantedForestEnemies);
        CreateStagesForEnemiesInWorld(4, desertEnemies);
        CreateStagesForEnemiesInWorld(5, cityEnemies);
        CreateStagesForEnemiesInWorld(6, frostlandsEnemies);
        CreateStagesForEnemiesInWorld(7, cavernsEnemies);
        CreateStagesForEnemiesInWorld(8, dragonsDenEnemies);

        //create another dummy stage to represent "has beaten the game"
        //StageData dummyStage2 = new (99, "beat all stages", 99, null);
        //dummyStage2.nextStage = null;
        //dummyStage2.previousStage = previousStage;
        //allStages.Add(dummyStage2);

        StaticVariables.allStages = allStages;
        //StaticVariables.hometownStages = hometownStages;
        //StaticVariables.grasslandsStages = grasslandsStages;
    }

    private void CreateStagesForEnemiesInWorld(int worldNum, List<GameObject> enemiesInWorld){
        int stageNum = 0;
        string worldName = worldNum switch {
            1 => StaticVariables.world1Name,
            2 => StaticVariables.world2Name,
            3 => StaticVariables.world3Name,
            4 => StaticVariables.world4Name,
            5 => StaticVariables.world5Name,
            6 => StaticVariables.world6Name,
            7 => StaticVariables.world7Name,
            8 => StaticVariables.world8Name,
            _ => StaticVariables.world1Name,
        };
        foreach (GameObject enemyPrefab in enemiesInWorld){
            stageNum ++;
            StageData newStage = new(worldNum, worldName, stageNum, enemyPrefab);
            newStage.previousStage = previousStage;
            if (previousStage !=  null)
                previousStage.nextStage = newStage;
            previousStage = newStage;
            allStages.Add(newStage);
        }
    }

}
