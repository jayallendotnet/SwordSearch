using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class OverworldEnemySpace : MonoBehaviour{

    private OverworldSceneManager overworldSceneManager;
    GameObject playerDestination;
    BattleSetupData battleSetupData;

    void Start(){
        overworldSceneManager = FindObjectOfType<OverworldSceneManager>();
        playerDestination = transform.Find("Player Destination").gameObject;
        playerDestination.transform.GetChild(0).gameObject.SetActive(false);
        battleSetupData = GetComponent<BattleSetupData>();
    }

    public void MovePlayerToThisSpace(){
        overworldSceneManager.currentPlayerSpace = this;
        overworldSceneManager.MovePlayerToPosition(playerDestination);
    }

    public void ClickedSpace(){
        //print(overworldSceneManager.currentPlayerSpace.name);
        if (overworldSceneManager.currentPlayerSpace != this)
            MovePlayerToThisSpace();
        else if (!overworldSceneManager.isPlayerMoving){
            //print("enter battle");
            overworldSceneManager.generalSceneManager.LoadBattleWithData(battleSetupData);
            //print(battleSetupData.battleData.enemyPrefab.name);
        }
    }

}
