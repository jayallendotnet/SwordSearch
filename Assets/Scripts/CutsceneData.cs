using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class CutsceneData: MonoBehaviour{

    public GameObject startingBackground;
    public CutsceneStep[] cutsceneSteps;
}


[System.Serializable]
public class CutsceneStep{

    public enum CutsceneType{PlayerTalking, OtherTalking, ChangeBackground, PlayAnimation, GoToBattle, GoToOverworld, GoToTutorial};
    public CutsceneType type;
    
    [ConditionalField(nameof(type), false, CutsceneType.OtherTalking)]
    public EnemyData characterTalking;


    [ConditionalField(nameof(type), false, CutsceneType.PlayerTalking, CutsceneType.OtherTalking)]
    public DialogueStep.Emotion emotion;


    [ConditionalField(nameof(type), false, CutsceneType.ChangeBackground)]
    public GameObject newBackground;


    [ConditionalField(nameof(type), false, CutsceneType.ChangeBackground, CutsceneType.PlayAnimation)]
    public bool advanceAutomatically = false;
    [ConditionalField(nameof(type), false, CutsceneType.ChangeBackground, CutsceneType.PlayAnimation)]
    public float advanceTime = 0.5f;

    [ConditionalField(nameof(type), false, CutsceneType.PlayerTalking, CutsceneType.OtherTalking)]
    [TextArea(2,5)]
    public string dialogue;


    [ConditionalField(nameof(type), false, CutsceneType.PlayAnimation)]
    public string characterToAnimate;

    [ConditionalField(nameof(type), false, CutsceneType.PlayAnimation)]
    public string animationName;
}