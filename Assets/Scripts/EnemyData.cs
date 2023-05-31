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

    [Header("Chatheads")]
    public Sprite normal;
    public Sprite angry;
    public Sprite defeated;

    [Header("Dialogue Steps")]
    public DialogueStep[] overworldDialogueSteps;
    public DialogueStep[] victoryDialogueSteps;
}


[System.Serializable]
public class DialogueStep{
    public enum DialogueType{PlayerTalking, EnemyTalking, Event};
    public enum Emotion{Normal, Angry, Defeated};
    public DialogueType type;
    public Emotion emotion;
    [TextArea(2,5)]
    public string description;
}
