using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class EnemyData : MonoBehaviour{

    public bool isBattleable = true;
    public string nameOverride = "";

    
    [Header("Battle Stats")]
    [ConditionalField(nameof(isBattleable))]    public int startingHealth = 10;
    [ConditionalField(nameof(isBattleable))]    public float attackSpeed = 6f;
    [ConditionalField(nameof(isBattleable))]    public int attackDamage = 2;
    [ConditionalField(nameof(isBattleable))]    public bool isDraconic = false;
    [ConditionalField(nameof(isBattleable))]    public bool isHorde = false;
    [ConditionalField(nameof(isBattleable))]    public bool isHoly = false;
    [ConditionalField(nameof(isBattleable))]    public bool isDark = false;

    [Header("Chatheads")]
    public Sprite normal;
    public Sprite angry;
    public Sprite defeated;
    public Sprite excited;
    public Sprite questioning;

    [Header("Dialogue Steps")]
    public DialogueStep[] overworldDialogueSteps;
    public DialogueStep[] victoryDialogueSteps;
}


[System.Serializable]
public class DialogueStep{
    public enum DialogueType{PlayerTalking, EnemyTalking, Event};
    public enum Emotion{Normal, Angry, Defeated, Excited, Happy, Questioning, Worried};
    public DialogueType type;
    public Emotion emotion;
    [TextArea(2,5)]
    public string description;
}
