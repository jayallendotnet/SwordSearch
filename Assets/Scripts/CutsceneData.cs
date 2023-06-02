using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CutsceneData{

    public CutsceneStep[] cutsceneSteps;
    public enum AfterCutsceneDo {GoToOverworld, GoToNextCutscene}
    public AfterCutsceneDo afterCutsceneDo;
}


[System.Serializable]
public class CutsceneStep{

    public bool isPlayerTalking = false;
    public EnemyData characterTalking;
    public DialogueStep.Emotion emotion;
    
    [TextArea(2,5)]
    public string description;
}