using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour{

    public GeneralSceneManager generalSceneManager;
    public DialogueManager dialogueManager;
    public CutsceneStep[] cutsceneSteps;
    public enum AfterCutsceneDo {GoToOverworld, GoToNextCutscene}
    public AfterCutsceneDo afterCutsceneDo;

    
    void Start(){
        generalSceneManager.Setup();
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, StartCutscene);
    }

    private void StartCutscene(){
        dialogueManager.Setup(cutsceneSteps, afterCutsceneDo);
    }


}

[System.Serializable]
public class CutsceneStep{

    public bool isPlayerTalking = false;
    public EnemyData characterTalking;
    public DialogueStep.Emotion emotion;
    
    [TextArea(2,5)]
    public string description;
}

