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
        if (overworldSceneManager.currentPlayerSpace != this)
            MovePlayerToThisSpace();
        else if (!overworldSceneManager.isPlayerMoving)
            overworldSceneManager.generalSceneManager.LoadBattleWithData(battleSetupData);
    }

}
