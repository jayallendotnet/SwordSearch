using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyData : MonoBehaviour{


    [Header("Stats")]
    public int startingHealth = 10;
    public float attackSpeed = 6f;
    public int attackDamage = 2;
    
    [Header("Attributes")]
    public bool isDraconic = false;
    public bool isHorde = false;
    public bool isHoly = false;
    public bool isDark = false;

    [Header("Visuals")]
    public Sprite chathead;

    [Header("Dialogue Steps")]
    public DialogueStep[] overworldDialogueSteps;
    public DialogueStep[] victoryDialogueSteps;
}


[System.Serializable]
public class DialogueStep{
    public enum DialogueType{PlayerTalking, EnemyTalking, Event};
    public DialogueType type;
    [TextArea(2,5)]
    public string description;
}
