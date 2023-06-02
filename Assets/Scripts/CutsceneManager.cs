using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour{

    public GeneralSceneManager generalSceneManager;
    public DialogueManager dialogueManager;
    public Image cutsceneImage;
    public CutsceneData cutsceneData;

    
    void Start(){
        cutsceneImage.sprite = cutsceneData.startingImage;
        generalSceneManager.Setup();
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, StartCutscene);
    }

    private void StartCutscene(){
        dialogueManager.Setup(cutsceneData, cutsceneImage);
    }


}


