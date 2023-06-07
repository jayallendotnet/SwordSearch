using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneData: MonoBehaviour{

    public Sprite startingImage;
    public CutsceneStep[] cutsceneSteps;
}


[System.Serializable]
public class CutsceneStep{

    public enum CutsceneType{PlayerTalking, OtherTalking, ChangeImage, PlayAnimation, GoToBattle, GoToOverworld, GoToTutorial};
    public CutsceneType type;
    public EnemyData characterTalking;
    public DialogueStep.Emotion emotion;
    public Sprite newImage;
    public bool advanceAutomatically = false;
    public float advanceTime = 0.5f;
    
    [TextArea(2,5)]
    public string description;
}