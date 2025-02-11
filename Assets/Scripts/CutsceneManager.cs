using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour{

    private int cutsceneStep = 0;
    private enum Cond{Click, Wait, BackgroundChange, externalTrigger};
    private Cond advanceCondition;
    public enum Cutscene{Hometown1, Hometown2, Grasslands1, Grasslands2, Forest1, Forest2, Forest3, Desert1, Desert2};
    private Cutscene cutsceneID;
    private List<Animator> animatedObjectsInCutscene = new List<Animator>();
    private List<GameObject> searchableObjectsInCutscene = new List<GameObject>();
    private float shakeTimer = 0f;
    private Animator playerAnimator;
    private GameObject nextBackground;

    public GeneralSceneManager generalSceneManager;
    public DialogueManager dialogueManager;
    public RectTransform backgroundParent;
    public GameObject emptyGameObject;
    public RectTransform screenshakeTransform;

    [Header("Cutscene Backgrounds")]
    public GameObject hometown1;
    public GameObject hometown2;
    public GameObject grasslands1;
    public GameObject grasslands2_pt1;
    public GameObject grasslands2_pt2;
    public GameObject forest1;
    public GameObject forest2_pt1;
    public GameObject forest2_pt2;
    public GameObject forest3;
    public GameObject desert1;
    public GameObject desert2;

    private float externalTriggerParameter = 0f;

    

    public void Start() {
        SetCutsceneID();
        switch (cutsceneID){
            case (Cutscene.Hometown1):
                SetupHometown1();
                break;
            case (Cutscene.Hometown2):
                SetupHometown2();
                break;
            case (Cutscene.Grasslands1):
                SetupGrasslands1();
                break;
            case (Cutscene.Grasslands2):
                SetupGrasslands2();
                break;
            case (Cutscene.Forest1):
                SetupForest1();
                break;
            case (Cutscene.Forest2):
                SetupForest2();
                break;
            case (Cutscene.Forest3):
                SetupForest3();
                break;
            case (Cutscene.Desert1):
                SetupDesert1();
                break;
            case (Cutscene.Desert2):
                SetupDesert2();
                break;
        }
        ButtonText("CONTINUE");

        SetupDialogueManager();

        generalSceneManager.Setup();
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, AdvanceCutsceneStep);
    }

    private void SetCutsceneID(){
        cutsceneID = StaticVariables.cutsceneID;
    }

    private void SetupHometown1(){
        SetCutsceneBackground(hometown1);
        ToggleObject("Player", false);
    }

    private void SetupHometown2(){
        SetCutsceneBackground(hometown2);
        PlayAnimation("Redhead Woman", "Idle Back");
        PlayAnimation("Bartender", "Idle Front");
        PlayAnimation("Child 1", "Idle Front");
        PlayAnimation("Child 2", "Idle Front");
    }

    private void SetupGrasslands1(){
        SetCutsceneBackground(grasslands1);
    }

    private void SetupGrasslands2(){
        SetCutsceneBackground(grasslands2_pt1);
        PlayAnimation("Player", "Idle Holding Book");
    }

    private void SetupForest1(){
        SetCutsceneBackground(forest1);
    }

    private void SetupForest2(){
        SetCutsceneBackground(forest2_pt1);
        PlayAnimation("Player", "Walk");
        Transform rabbitArea = GetObjectFromName("Starting area").transform;
        rabbitArea.DOLocalMoveX(rabbitArea.localPosition.x -3000, 2.5f).SetEase(Ease.Linear);       
        //CutsceneTreeFinalSynchronizer synchronizer = GetObjectFromName("Tree Synchronizer").GetComponent<CutsceneTreeFinalSynchronizer>();
        //synchronizer.cutsceneManager = this;
    }

    private void SetupForest3(){
        SetCutsceneBackground(forest3);
    }

    private void SetupDesert1(){
        SetCutsceneBackground(desert1);
    }

    private void SetupDesert2(){
        SetCutsceneBackground(desert2);
    }

    private void AdvanceCutsceneStep(){
        cutsceneStep ++;
        switch (cutsceneID){
            case (Cutscene.Hometown1):
                DoHometown1Step();
                break;
            case (Cutscene.Hometown2):
                DoHometown2Step();
                break;
            case (Cutscene.Grasslands1):
                DoGrasslands1Step();
                break;
            case (Cutscene.Grasslands2):
                DoGrasslands2Step();
                break;
            case (Cutscene.Forest1):
                DoForest1Step();
                break;
            case (Cutscene.Forest2):
                DoForest2Step();
                break;
            case (Cutscene.Forest3):
                DoForest3Step();
                break;
            case (Cutscene.Desert1):
                DoDesert1Step();
                break;
            case (Cutscene.Desert2):
                DoDesert2Step();
                break;
        }
        CheckButtonAvailability();
    }

    private void CheckButtonAvailability(){
        switch (advanceCondition){
            case (Cond.Click):
                ToggleButton(true);
                break;
            default:
                ToggleButton(false);
                break;
        }
    }

    private void DoHometown1Step(){   
        int i = 0;
        if (++i == cutsceneStep){
            ToggleObject("Player", true);
            PlayAnimationAndMoveThenIdle("Player", "Walk", -201, 2025, 1.5f);
            AdvanceConditionWait(1.5f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionDialogue_PlayerTalking("Ah, spring! The warm sun always puts me in a reading mood!", DialogueStep.Emotion.Happy);
        }    
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Take Out Book Random Brown");
            AdvanceConditionWait(2f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh, who am I kidding... All weather puts me in a reading mood.", DialogueStep.Emotion.Normal);
        }    
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Miss " + StaticVariables.playerName + "! Miss " + StaticVariables.playerName + "!", "Child 2", DialogueStep.Emotion.Excited);
            PlayAnimationAndMoveThenIdle("Child 1", "Walk", 26, 605, 2f);
            PlayAnimationAndMoveThenIdle("Child 2", "Walk", 85, 564, 2.3f);
            AdvanceConditionWaitThenClick(2.3f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Put that dumb book down! We need you to read this newspaper!!", "Child 1", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Mikey stop! You know how Miss " + StaticVariables.playerName + " feels when you insult her books!", "Child 2", DialogueStep.Emotion.Excited);
            PlayAnimation("Player", "Put Away Book Random Brown");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("She'll never stop talking about \"the prince's manticore wears shorts\"!", "Child 2", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("That doesn't sound right, I think it's \"a pig is manlier when it snores\".", "Child 1", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I've never said anything that sounds even remotely like that nonsense before!", DialogueStep.Emotion.Angry);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Maybe if you had paid attention in class you'd know it's \"the pen is mightier than the sword\"!", DialogueStep.Emotion.Angry);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("And you'd be able to read that newspaper for yourselves!", DialogueStep.Emotion.Angry);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Oh yeah! Read it to us! Please???", "Child 2", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Alright, fine... Let me see it...", DialogueStep.Emotion.Normal);
        }     
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }        
        else if (++i == cutsceneStep){
            PlayAnimation("Child 2", "Show Newspaper");
            AdvanceConditionWait(1.3f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimation("Child 2", "Idle");
            PlayAnimation("Player", "Idle Holding Newspaper");
            AdvanceConditionWait(0.6f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Ahem!\nThe headline reads,\n\"Lich King Defeated! Duskvale in Ruins!\"", DialogueStep.Emotion.Normal);
        }       
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("What did you say?", "Blacksmith", DialogueStep.Emotion.Normal);
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", -172, 64, 1.3f);
            AdvanceConditionWaitThenClick(1.5f);
        }   
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("My daughter lives in Duskvale!", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman No Hat", "Walk", -419, 244, 1.3f);
            AdvanceConditionWaitThenClick(1.5f);
        } 
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I hate the Lich King!", "Redhead Woman", DialogueStep.Emotion.Normal);
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Redhead Woman", "Walk", 58, 334, 2f);
            AdvanceConditionWaitThenClick(2.2f);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Keep Reading!!!", "Everyone - Hometown Intro", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"Late last night, a great hole appeared in the center of Duskvale.\"", DialogueStep.Emotion.Normal);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"Out of the hole came a burst of flame, and the city was quickly beseiged by --\"", DialogueStep.Emotion.Normal);
        }     
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("ROOOOOOOAR!!!!", "Mystery Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            PlayAnimation("Red Dragon", "Fly");
            MoveObject("Red Dragon", 1200, 1377, 2f);
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Child 1");
            FlipDirection("Child 2");
            FlipDirection("Redhead Woman");
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            MoveObject("Red Dragon", 521, 1050, 2f);
            FlipDirection("Red Dragon");
            AdvanceConditionWait(2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Red Dragon", "Land");
            MoveObject("Red Dragon", 521, 1000, 1.1f);
            AdvanceConditionWait(0.6f);
        }
        else if (++i == cutsceneStep){
            StartScreenShake(1f);
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"-- a group of dragons!\"", DialogueStep.Emotion.Worried);
            PlayAnimation("Player", "Put Away Newspaper");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Ha ha ha...\nPuny humans in your simple town...", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I could eat all of you on a whim!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Instead I come with good news! Your hated Lich King has been driven from his throne!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("This town now belongs to the mighty Queen of Ash! Bow down to her, me, and all of dragonkind!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("We won't be bossed around by some overgrown lizard!", "Blacksmith", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("We're not afraid of you!", "Redhead Woman", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Don't antagonize the dragon! It could kill us all!", DialogueStep.Emotion.Worried);
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yes, the young lady makes quite the compelling argument.", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Allow me to give you a little taste of fear!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Red Dragon", "Prolonged Attack - Start");
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Red Dragon", "Prolonged Attack - End");

            ToggleObject("Fires", true);
            ToggleObject("Fire 1", true);
            ToggleObject("Fire 2", true);
            ToggleObject("Fire 3", true);
            ToggleObject("Fire 4", true);

            Image sky1 = GetObjectFromName("Sky 1").GetComponent<Image>();
            Image sky2 = GetObjectFromName("Sky 2").GetComponent<Image>();
            Image ground1 = GetObjectFromName("Ground 1").GetComponent<Image>();
            Image ground2 = GetObjectFromName("Ground 2").GetComponent<Image>();
            sky1.DOColor(sky2.color, 2f);
            sky2.color = sky1.color;
            ground1.DOColor(ground2.color, 2f);
            ground2.color = ground1.color;

            AdvanceConditionWait(3f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Ha ha ha... You humans are powerless!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("What do you want from us?", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Well of course you'll be paying taxes to your new draconic rulers!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("But it appears this commotion drew the attention of some local golbins.", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            PlayAnimationAndMoveThenIdle("Goblin 1", "Walk", 70, -64, 2.2f);
            PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", 514, 310, 2f);
            PlayAnimationAndMoveThenIdle("Goblin 3", "Walk", 446, 489, 2f);
            PlayAnimationAndMoveThenIdle("Wolf Rider", "Walk", 240, 183, 1.5f);
            AdvanceConditionWait(2.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Humans! We come for your gold!", "Wolf Rider", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I'd love to stay and help you peasants, but I have other towns to visit.", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Do your best not to die. The Queen of Ash has no use for weaklings!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Red Dragon", "Fly", -1500, 1400, 2f);
            AdvanceConditionWait(2f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Attack the invaders! Defend our homes!", "Everyone - Hometown Intro", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            HideEnemyChathead();
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", 29, 130, 0.5f);
            PlayAnimationAndMoveThenIdle("Redhead Woman", "Walk", 274, 535, 1f);
            AdvanceConditionWait(0.3f);
        }          
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", 575, 310, 0.5f);
            PlayAnimationAndMoveThenIdle("Goblin 1", "Walk", -244, 191, 1f);
            FlipDirection("Goblin 2");
            AdvanceConditionWait(1.5f);
        }   
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I don't know anything about fighting...", DialogueStep.Emotion.Worried);
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Us neither!", "Child 1", DialogueStep.Emotion.Questioning);
            FlipDirection("Child 1");
            FlipDirection("Child 2");
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("We should leave the town defense to the professionals...", DialogueStep.Emotion.Worried);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh no! The library is on fire!", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("All of my precious books are burning! I have to save them!", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            HideChatheads();
            PlayAnimation("Player", "Walk");
            MoveObject("Player", -75, 2137, 1.5f);
            AdvanceConditionWait(1.5f);
        }
        else if (++i == cutsceneStep){
            ToggleObject("Player", false);
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Tossing Books").GetComponent<CutsceneBookThrow>().StartThrow();
            AdvanceConditionWait(3f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("She really likes her books huh...", "Child 1", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yeah, what a weirdo.", "Child 2", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Do you think we should go help her?", "Child 1", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("No, I don't want to get smacked by a flying book today.", "Child 2", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Besides, how many books can she even have in there? Do you think she's almost done?", "Child 2", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Maybe we can go find a goblin to fight!", "Child 1", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Wait, do you hear that?", "Child 2", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            HideEnemyChathead();
            StartScreenShake();
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Tossing Books").GetComponent<CutsceneBookThrow>().StopThrow();
            AdvanceConditionWait(0.66f);
        }
        else if (++i == cutsceneStep){              
            AdvanceConditionDialogue_PlayerTalking("What's happening now??", DialogueStep.Emotion.Questioning);
            ToggleObject("Player", true);
            FlipDirection("Player");
            ToggleObject("Tossing Books", false);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            HideChatheads();
            PlayAnimationAndMoveThenIdle("Player", "Walk", -140, 1860, 1.5f);
            AdvanceConditionWait(2f);
        }
        else if (++i == cutsceneStep){  
            ToggleObject("Water Spray", true);
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){  
            StopShakeScreen();           
            AdvanceConditionWait(2.5f);
        }
        else if (++i == cutsceneStep){
            MagicFlash flash = GetAnimatorFromName("Water Spray").transform.GetChild(0).GetComponent<MagicFlash>();
            flash.gameObject.SetActive(true);
            flash.StartProcess(StaticVariables.waterPowerupColor);
            AdvanceConditionWait(flash.GetTotalTime() - 1f);
        }
        else if (++i == cutsceneStep){
            ToggleObject("Rain", true);
            MoveObject("Rain", -33, -1700, 7f);
            ToggleObject("Fires", false);
            ToggleObject("Fire 1", false);
            ToggleObject("Fire 2", false);
            ToggleObject("Fire 3", false);
            ToggleObject("Fire 4", false);
            ToggleObject("Puddles", true);
            ToggleObject("Sky 1", false);
            ToggleObject("Sky 2", true);
            ToggleObject("Ground 1", false);
            ToggleObject("Ground 2", true);
            StopShakeScreen();
            AdvanceConditionWait(5f);
        }
        else if (++i == cutsceneStep){
            Transform player = GetObjectFromName("Player").transform.parent;
            player.SetSiblingIndex(player.parent.childCount -3);
            AdvanceConditionDialogue_PlayerTalking("There's a book coming out of the water!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Water Spray", "End Spray");
            AdvanceConditionWait(0.7f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Walk");
            MoveObject("Player", 300, 1860, 1.1f);
            AdvanceConditionWait(0.6f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Book Catch");
            AdvanceConditionWait(0.37f);
        }
        else if (++i == cutsceneStep){
            ToggleObject("Water Spray", false);
            PlayAnimation("Player", "Idle Holding Book Flipped");
            AdvanceConditionDialogue_PlayerTalking("Got it!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("This book is completely dry!", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I always thought this was an old empty journal...", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("But now the pages are filled with random letters!", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I bet it was invisible ink... or maybe even magic?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("And the letters react to my touch! It's definitely probably magical!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
    }
    
    private void DoHometown2Step(){
        int i = 0;
        if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I saw you shooting some magic water at those goblis!", "Blacksmith", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I didn't know you had that fighting spirit in you!", "Blacksmith", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I wouldn't call myself much of a warrior...", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Nonsense! You were incredible!", "Blacksmith", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("And the magic too! How did you do that??", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("To be honest, I'm not sure.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("One of the old library books has these weird glowing letters, and...", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("It might make more sense if you give it a try yourself!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Take Out Book");
            AdvanceConditionWait(2f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Do you see all those jumbled random letters?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yeah?", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Try touching them!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }        
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman No Hat", "Walk", -200, 585, 0.4f);
            AdvanceConditionWait(0.4f);
        }   
        else if (++i == cutsceneStep){
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman No Hat", "Walk", -117, 550, 0.4f);
            AdvanceConditionWait(0.4f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Nothing interesting happens...", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That's weird, it still works for me. Watch!", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }        
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Cast Spell");
            AdvanceConditionWait(0.33f);
        }
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Player").transform.parent.GetChild(2).gameObject.SetActive(true);
            AdvanceConditionWait(1.5f);
        }
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Player").transform.parent.GetChild(2).gameObject.SetActive(false);
            AdvanceConditionDialogue_EnemyTalking("Hey! Don't start throwing magic at people!", "Short Black Man", DialogueStep.Emotion.Angry);
            FlipDirection("Short Black Man");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yeah, if you want to sling spells, go to the Academy!", "Yellowhead Woman", DialogueStep.Emotion.Angry);
            FlipDirection("Yellowhead Woman");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("It's been a long time since anyone used magic around here.", "Redhead Woman", DialogueStep.Emotion.Normal);
            PlayAnimation("Redhead Woman", "Idle");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yeah, it's been decades since Eldric and his magic were here with us...", "Short Black Man", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Hey, I'm not dead, I'm just old!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }        
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Bald Man Blue Shirt", "Walk", 427, 471, 2f);
            AdvanceConditionWait(2f);
        }   
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You whippersnappers are flappin' yer yappers about my magic?", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Well, you're right. Back in the old days, when I could still touch my toes, I had control over the wind!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Like you could make tornadoes and stuff?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yes! I could make tornadoes! And stuff!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I was an adventurer! Slaying monsters, saving princesses...", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I even fought an owlbear once!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("But one day I woke up with a shake in my hands, and that was it. I couldn't use magic after that.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I've read some of the library's medical textbooks. Maybe there's something in one of them that could heal your hand tremors?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh! My book can do some healing magic! I bet I could fix your hands with that!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Young lady, that is very kind of you. But I'm afraid we have more important matters to discuss today.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Gather 'round, everyone!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            FlipDirection("Shopkeeper");
            PlayAnimationAndMoveThenIdle("Shopkeeper", "Walk", -156, 129, 2.3f);
            FlipDirection("Bluehead Woman");
            PlayAnimationAndMoveThenIdle("Bluehead Woman", "Walk", 106, 114, 2f);
            PlayAnimationAndMoveThenIdle("Child 2", "Walk", 268, 172, 2.2f);
            PlayAnimationAndMoveThenIdle("Child 1", "Walk", 361, 370, 1.6f);
            PlayAnimationAndMoveThenIdle("Chef", "Walk", -284, 254, 1.6f);
            AdvanceConditionWait(0.8f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman No Hat", "Walk", -384, 413, 0.8f);
            AdvanceConditionWait(0.6f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Orange Shirt Black Woman No Hat");
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Shopkeeper");
            PlayAnimationAndMoveThenIdle("Redhead Woman", "Walk", 348, 573, 2.1f);
            PlayAnimationAndMoveThenIdle("Short Black Man", "Walk", 219, 724, 2.2f);
            PlayAnimationAndMoveThenIdle("Yellowhead Woman", "Walk", 73, 803, 2.3f);
            PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", -205, 763, 0.5f);
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Blacksmith");
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Allow me to be blunt here... If that dragon comes by again, we're toast!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("We need to do something!", "Yellowhead Woman", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            HideEnemyChathead();
            AdvanceConditionDialogue_PlayerTalking("I have some information that may be helpful.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I was reading this newspaper before the dragon showed up!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }        
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Take Out Newspaper");
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Here's the full article I was reading earlier: \n\"Last night a great hole appeared in the center of Duskvale.\"", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"Out of the hole came a burst of flame, and the city was quickly beseiged by a group of dragons!\"", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"The Lich King and his advisors promptly engaged the dragons in combat. Vibrant blasts of magic and jets of fire shoot the city!\"", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"After a few minutes, the dragons claimed victory, and razed the city to the ground! His Lichness has not been seen since the attack.\"", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"Local experts believe the King has been killed, or is currently searching for the famed Sword of Dragonslaying.\"", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"Other local experts believe the Sword of Dragonslaying is purely a myth, and it would be dangerous to go searching for it with dragons about.\"", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"A third group of local experts believe that despite the Lich King's low approval ratings, the dragon attack was not politically motivated.\"", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Well I'm not one for politics, but that bit about a dragon-killing sword sounds pretty useful right about now!", "Short Black Man", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I had a book about that sword in the library, but I think it got ruined in the dragon attack.", DialogueStep.Emotion.Defeated);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Oh don't worry about that, Miss " + StaticVariables.playerName + "!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I knew a swordswoman who weilded the power of that very sword!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("There weren't many dragons around back then, so we sealed the sword away in a temple in the desert.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("For all I know, it might just still be there!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("We have to go and get it!", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Let's go now!", "Blacksmith", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Which way to the desert??", "Redhead Woman", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Now don't be too hasty! It's quite a journey to get there, and they call it the \"Sunscorched Desert\" for a reason!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I can go.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("With my water magic I should be able to survive there.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I'll return with the sword, and then we can fight back against these dragon tyrants!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("If you're sure...", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I am!!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("To get to the desert, you'll have to get to the far side of the enchanted forest, beyond the grasslands to the south.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("And don't worry about us, we will make sure the town stays safe!", "Blacksmith", DialogueStep.Emotion.Excited);
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
    }

    private void DoGrasslands1Step(){   
        int i = 0;
        if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What a great day to begin an adventure!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("The sun is shining, the river is flowing, and the grass is dancing in the wind!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Going out into the grasslands, just me and my... weird magical book...", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){       
            AdvanceConditionWait(2f);
            PlayAnimation("Player", "Take Out Book");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("You're a magic book. Can you understand me?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The magic book says nothing.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Okay, well your pages fill with random letters when it's magic time. Can you say something to me with them?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The book's pages remain empty.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("It'd be nice to have a picnic in the warm grass, reading a book full of magical secrets. Doesn't that sound fun?", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The magic book continues to be silent.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Fine, be that way!" , DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Miss "+ StaticVariables.playerName + "! Wait a moment!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }      
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Bald Man Blue Shirt", "Walk", -38, 676, 2f);
            AdvanceConditionWait(2f);
        }      
        else if (++i == cutsceneStep){
            FlipDirection("Bald Man Blue Shirt");
            AdvanceConditionWait(0.5f);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Are you... arguing with your book?", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }
        else if (++i == cutsceneStep){ 
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Does it count as arguing if the thing you are talking to might not even be able to hear you?" , DialogueStep.Emotion.Angry);   
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What are you doing out here anyway?" , DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I wanted to thank you for offering to heal my hands and bring my magic back!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Actually, about that..." , DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I think I should get some more practice with magic before I try it on people!" , DialogueStep.Emotion.Surprised);   
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("That's alright, I'm an old man now. I don't know if I want to weild that power again...", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I do miss cooking though! Before I started adventuring, I was the head chef for the tavern.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("When you come back from your quest, I'll take you up on your offer. You can fix up my hands and I'll cook you a mean nine-course meal!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That sounds lovely!" , DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Oh! But that's not why I came out here! You showed me kindness, and I wanted to give you something in return.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }
        else if (++i == cutsceneStep){ 
            AdvanceConditionWait(2f);
            PlayAnimation("Bald Man Blue Shirt", "Take Out Bag");
        } 
        else if (++i == cutsceneStep){
            PlayAnimation("Bald Man Blue Shirt", "Idle");
            PlayAnimation("Player", "Idle Holding Bag");
            AdvanceConditionWait(1.6f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("This is an enchanted pouch that can carry anything and everything you put inside it!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I had the children fill it with every single one of your books that survived the flames.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("This is incredible! Thank you!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You're welcome, Miss " + StaticVariables.playerName + ".", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You have a long journey ahead of you. Best to not get bored.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("You're right! I'd better get going. Goodbye, Eldric.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }    
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Put Away Bag");
            AdvanceConditionWait(1.6f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionWait(2f);
            PlayAnimationAndMoveThenIdle("Player", "Walk", 500, 2012, 5f);
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
    }
    
    private void DoGrasslands2Step(){
        int i = 0;
        if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Alright, book.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("After that crazy fight, there's probably a lot of vague magical whatever energy in the air.", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Maybe you're feeling a little inspired to talk to me?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("For the first time, the book's pages have some clear writing on them.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("It reads,\n\"ENTER THE CAVE\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("No way!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I knew it! You're a talking book! I have so many questions!!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("How old are you? Who made you? Can you see me?", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The book's text remains unchanged.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Great. I get to talk with an actual magical book, and you just want me to go into some dark cave instead.", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Ugh, fine. But are you sure we should go in there?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("We need to get through the forest, and there really isn't any time to waste!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The ink shifts around, forming new words.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("\"GO NOW, BEFORE THE CYCLOPS AWAKENS.\"", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("You know what, you're a magic book. You can <water>summon water<>. You can <healing>heal the injured<>.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("And now you can talk!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Maybe I should just take your advice. Let's go!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }  
        else if (++i == cutsceneStep){ 
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionWait(2f);
            PlayAnimation("Player", "Walk");
            MoveObject("Player", 480, 2280, 5f);
        }
        else if (++i == cutsceneStep){
            StartCutsceneImageTransition(grasslands2_pt2);
            advanceCondition = Cond.BackgroundChange;
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I can't see a thing!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        } 
        else if (++i == cutsceneStep){ 
            AdvanceConditionWait(3f);
            PlayAnimation("Player", "Walk");
            MoveObject("Entrance", -2066, 1892, 3f);
        } 
        else if (++i == cutsceneStep){
            StartScreenShake(.2f);
            AdvanceConditionDialogue_EnemyTalking("Bump!", "Dark Object", DialogueStep.Emotion.Normal);
            PlayAnimation("Player", "Idle");
        }
        else if (++i == cutsceneStep){
            HideEnemyChathead();
            AdvanceConditionDialogue_PlayerTalking("Ouch, it looks like I ran into something.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I should take a moment and let my eyes adjust...", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            AdvanceConditionWait(3f);
            Color c = Color.black;
            c.a = 0;
            GetObjectFromName("Darkness").GetComponent<Image>().DOColor(c, 10f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Ahh!!! A skeleton!!", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh, it's dead.", DialogueStep.Emotion.Normal); //relief?
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Well of course it's dead, it's a skeleton!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Come to think of it, I have to be careful. It might come back to life!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("In every fantasy adventure novel, there's always at least one reanimated skeleton!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("And my life has certainly been looking like a fantasy adventure lately...", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What is a skeleton doing here, anyway? The cyclops talked about ecological conservation, not dead bodies.", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("You don't accidentally leave a skeleton in your house.", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Something isn't adding up. I should take a look around.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        } 
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -441, 1900, 1.5f);
            AdvanceConditionWait(1.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Let's see what you've been reading lately...", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Book name, book name...", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("These are history books.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("But they also cover burial practices and enbalming techniques.", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Has he been doing something to these skeletons?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("It's entirely possible that he could be a necromancer...", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("They say the Lich King used magic to make himself undead. Maybe the cyclops is trying to do the same?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        } 
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(0.5f);
        } 
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", 16, 1675, 2.5f);
            MoveEverythingExceptPlayer(-300, 0, 2.5f);
            AdvanceConditionWait(2.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("There's something in the dirt here...", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Hang on a second, these are Lydian Lion coins! Some of the earliest currency in human history!", DialogueStep.Emotion.Surprised);
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("This is an incredibly precious archeological digsite!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Maybe the cyclops is just a normal historian.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Is this what you wanted me to see?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Take Out Book");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The book reads,\n\"KEEP GOING, AND HURRY UP\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Are you serious? You want me to walk right past these Lydian coins?", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Herodotus, the father of history, thought Lydia was the first civilization to ever use metal coins!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("This is probably the coolest thing that I've ever seen!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Well, aside from a magical talking book anyway...", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Fine, I'll move on. But we're going to keep talking about this later!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }  
        else if (++i == cutsceneStep){ 
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionWait(3f);
            PlayAnimation("Player", "Walk");
            MoveEverythingExceptPlayer(-591, 0, 3f);
            Color c = Color.black;
            c.a = 0.45f;
            GetObjectFromName("Rock Glow Darkness").GetComponent<Image>().DOColor(c, 6f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionWait(0.5f);
            PlayAnimation("Player", "Idle");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Okay, this glowy rock must be what we're here for!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("It looks like there's a book on the table...", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("The...\nNecronomicon???", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I didn't know that was real!", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That settles it! The cyclops is definitely a necromancer!", DialogueStep.Emotion.Worried);
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Take Out Book");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Alright, book! Tell me what you want me to do, and then let's get out of here!", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Ink forms into new words,\n\"TOUCH THE ROCK\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Touch it, are you crazy?? It'd turn me into a skeleton or something!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Words reshape on the book's pages, \n\"YOU WILL BE FINE\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Alright, if you're sure...  Here goes nothing!", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionWait(1f);
            PlayAnimation("Player", "Book Catch");
        }
        else if (++i == cutsceneStep){
            StartScreenShake();
            AdvanceConditionWait(0.8f);
        }
        else if (++i == cutsceneStep){
            MagicFlash flash = GetObjectFromName("Magic Flash").GetComponent<MagicFlash>();
            flash.gameObject.SetActive(true);
            flash.StartProcess(StaticVariables.earthPowerupColor);
            AdvanceConditionWait(flash.GetTotalTime() - 1f);
        }
        else if (++i == cutsceneStep){
            StopShakeScreen();
            PlayAnimation("Player", "Idle Holding Book");
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Somehow, I'm still alive!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The book's pages are empty aside from a single word,\n\"EARTH\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("After a moment, the letters reform to say,\n\"YOU SHOULD PROBABLY LEAVE NOW\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Uhh, yep! That's a great idea.", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That cyclops could wake up any minute! Let's get out of here!", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }  
        else if (++i == cutsceneStep){ 
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(0.5f);
        }  
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -536, 1675, 5f);
            MoveEverythingExceptPlayer(1000, 0, 5f);
            AdvanceConditionWait(2f);
        }        
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
    }

    private void DoForest1Step(){   
        int i = 0;
        if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -78, 1935, 3f);
            AdvanceConditionWait(3f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Huff... Huff... Phew!", DialogueStep.Emotion.Surprised);
        } 
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I really need to get in better shape!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Especially if you keep putting me in dangerous situations!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Take Out Book");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Can you tell me what the heck happened in that cave??", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("At first, the pages are blank.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("After a moment, ink slowly forms on the paper.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("\"YOU NOW WIELD THE POWER OF THE <earth>ELEMENT OF EARTH<>\"", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Earth?? That's incredible!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Just from touching that glowing cave rock?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The book quickly responds, \"YES\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("First <water>water<> and that <healing>healing<>, and now <earth>earth<>!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I've never heard of someone commanding three elements before.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I wonder what would happen if we got Eldric to touch that rock too...", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("There's no point in worrying about that just yet, we have to save the world first!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("The next goal is making it through the forest!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
            HideEnemyChathead();
        }  
        else if (++i == cutsceneStep){ 
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }   
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("It really is beautiful here...", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Even if that cyclops was a crazy necromancer, he still cared about protecting the undisturbed state of the forest.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I should try to get through quickly and leave no trace.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I'll have to just pick a direction and move carefully and quietly.", DialogueStep.Emotion.Normal);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }  
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -178, 1435, 5f);
            AdvanceConditionWait(2f);
        }     
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
    }

    private void DoForest2Step(){   
        int i = 0;
        if (++i == cutsceneStep){
            AdvanceConditionWait(2f);
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Huff... Huff...", DialogueStep.Emotion.Surprised);
            GameObject.Destroy(GetObjectFromName("Starting area"));
            //StaticVariables.WaitTimeThenCallFunction(2f, GameObject.Destroy, GetObjectFromName("Starting area"));
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I can't outrun rabbits forever!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("We need to find that other human, and fast!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("If I were a magical human in a magical forest, where would I live?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Huff...", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Wait a second, I am a magical human in a magical forest!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That's not exactly helpful; I don't live here!", DialogueStep.Emotion.Defeated);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Okay, book...", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Take Out Book While Walking");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Do you feel anything with your magic radar?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Immediately, the book responds, \"THERE IS A STRONG MAGICAL AURA RADIATING FROM BELOW\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Okay...", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("So I'm looking for some kind of tunnel entrance, or big tree stump, or...", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }  
        else if (++i == cutsceneStep){
            advanceCondition = Cond.externalTrigger;
            List<CutsceneTreeGenerator> treeGenerators = new List<CutsceneTreeGenerator>();
            treeGenerators.Add(GetObjectFromName("Tree Spawner 1").GetComponent<CutsceneTreeGenerator>());
            treeGenerators.Add(GetObjectFromName("Tree Spawner 2").GetComponent<CutsceneTreeGenerator>());
            treeGenerators.Add(GetObjectFromName("Tree Spawner 3").GetComponent<CutsceneTreeGenerator>());
            treeGenerators.Add(GetObjectFromName("Tree Spawner 4").GetComponent<CutsceneTreeGenerator>());
            treeGenerators.Add(GetObjectFromName("Tree Spawner 5").GetComponent<CutsceneTreeGenerator>());
            treeGenerators.Add(GetObjectFromName("Tree Spawner 6").GetComponent<CutsceneTreeGenerator>());
            foreach(CutsceneTreeGenerator treeGenerator in treeGenerators)
                treeGenerator.BeginSlowdown();
        } 
        else if (++i == cutsceneStep){
            CutsceneTreeMimic mimic = GetObjectFromName("Tree Synchronizer").transform.GetChild(0).GetComponent<CutsceneTreeMimic>();
            mimic.gameObject.SetActive(true);
            mimic.originalTree = GetObjectFromName("Tree Spawner 3").transform.GetChild(1);
            StaticVariables.WaitTimeThenCallFunction(externalTriggerParameter, mimic.DestroyScript);

            print(externalTriggerParameter);
            AdvanceConditionWait(externalTriggerParameter - 0.5f);
        } 
        else if (++i == cutsceneStep){
            MoveObject("Player", -96, 2213, 0.5f);
            AdvanceConditionWait(.5f);
        } 
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Idle Holding Book");
            AdvanceConditionWait(0.5f);
        } 
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("A trapdoor!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            StartScreenShake();
            AdvanceConditionWait(0.8f);
        }
        else if (++i == cutsceneStep){
            //start rabbit running
            AdvanceConditionWait(5f);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Phew, that was close!", DialogueStep.Emotion.Surprised);
        } 
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Well, this must be it.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Nothing says \"possibly-evil lair of a possibly-evil forest wizard\" like a trapdoor.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Let's see what's inside...", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            //StartCutsceneImageTransition(forest2_pt2);
            //advanceCondition = Cond.BackgroundChange;
        }
        else if (++i == cutsceneStep){
            StopShakeScreen();
        }
    }

    private void DoForest3Step(){   
        int i = 0;
        if (++i == cutsceneStep){
        }
        else if (++i == cutsceneStep){
        }
    }
    
    private void DoDesert1Step(){   
        int i = 0;
        if (++i == cutsceneStep){
        }
        else if (++i == cutsceneStep){
        } 
    }

    private void DoDesert2Step(){   
        int i = 0;
        if (++i == cutsceneStep){
        }
        else if (++i == cutsceneStep){
        }
    }

    private void MoveEverythingExceptPlayer(float xDistance, float yDistance, float duration){
        foreach (Transform t in backgroundParent){
            if (t.name != "Player (Overworld)"){
                t.DOLocalMove(new Vector2(t.localPosition.x + xDistance, t.localPosition.y + yDistance), duration);
            }
        }
    }

    private void AdvanceConditionWaitThenClick(float duration){
        advanceCondition = Cond.Wait;
        StaticVariables.WaitTimeThenCallFunction(duration, EnableClick);
    }

    private void EnableClick(){
        advanceCondition = Cond.Click;
        CheckButtonAvailability();
    }

    private void AdvanceConditionWait(float duration){
        advanceCondition = Cond.Wait;
        StaticVariables.WaitTimeThenCallFunction(duration, AdvanceCutsceneStep);
    }

    private void ToggleButton(bool value){
        dialogueManager.realButton.SetActive(value);
    }

    private void SetupDialogueManager(){
        dialogueManager.isInBattle = false;
        dialogueManager.isInOverworld = false;
        dialogueManager.isInCutscene = true;
        dialogueManager.cutsceneManager = this;
        dialogueManager.ClearDialogue();
        dialogueManager.SetStartingValues();
        dialogueManager.TransitionToShowing();
        ToggleButton(false);
    }

    private void HideChatheads(){
        dialogueManager.HideChatheads(dialogueManager.transitionDuration);
    }

    private void HideEnemyChathead(){
        dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
    }
    
    private void AdvanceConditionDialogue_PlayerTalking(string s, DialogueStep.Emotion emotion){
        advanceCondition = Cond.Click;
        DisplayPlayerTalking(s, emotion);
    }

    private void AdvanceConditionDialogue_NobodyTalking(bool alsoHideChatheads = false){
        if (alsoHideChatheads)
            HideChatheads();
        DisplayNobodyTalking();
        AdvanceConditionWait(0.5f);
    }

    private void DisplayPlayerTalking(string s, DialogueStep.Emotion emotion){
        dialogueManager.ShowPlayerTalking(emotion);
        dialogueManager.dialogueTextBox.text = TextFormatter.FormatString(s);

    }

    private void DisplayEnemyTalking(string s, EnemyData enemyData, DialogueStep.Emotion emotion){
        dialogueManager.ShowEnemyTalking(enemyData, emotion);
        dialogueManager.dialogueTextBox.text = TextFormatter.FormatString(s);

    }

    private void AdvanceConditionDialogue_EnemyTalking(string s, string enemyName, DialogueStep.Emotion emotion){
        advanceCondition = Cond.Click;
        DisplayEnemyTalking(s, enemyName, emotion);

    }

    private void DisplayEnemyTalking(string s, string enemyName, DialogueStep.Emotion emotion){
        DisplayEnemyTalking(s, GetAnimatorFromName(enemyName).GetComponent<EnemyData>(), emotion);
    }

    private void DisplayNobodyTalking(){
        dialogueManager.ShowNobodyTalking();
    }

    private Animator GetAnimatorFromName(string name){
        if (name == "Player")
            return playerAnimator;
        foreach (Animator anim in animatedObjectsInCutscene){
            if (anim != null){
                if (anim.gameObject.name == name)
                    return anim;
            }
        }
        //print("No animator found with the name [" + name + "]");
        return null;
    }

    private GameObject GetObjectFromName(string name){
        if (name == "Player")
            return playerAnimator.gameObject;
        foreach (GameObject go in searchableObjectsInCutscene)
            if (go != null){
                if (go.name == name)
                    return go;
            }
        print("No gameObject found with the name [" + name + "]");
        return null;
    }
    

    private void ButtonText(string s){
        dialogueManager.SetButtonText(s);
    }

    public void PressedNextButton(){
        if (advanceCondition == Cond.Click)
            AdvanceCutsceneStep();
    }

    private void ToggleObject(string name, bool enabled){
        //every animated object is made the child of an empty gameobject, so we want to toggle the parent of the animator
        Animator anim = GetAnimatorFromName(name);
        //the same is not true of searchable gameobjects, we want to toggle those directly
        GameObject go = GetObjectFromName(name);
        if (anim != null)
            anim.transform.parent.gameObject.SetActive(enabled);
        else if (go != null)
            go.SetActive(enabled);
        else{
            print("no object found with name [" + name + "] to toggle");
        }
    }
    
    private void StartScreenShake(float duration = -9999){
        shakeTimer = duration;
        ShakeScreen();
    }

    private void ShakeScreen(){
        float singleShakeDuration = 0.15f;
        if (shakeTimer != -9999)
            shakeTimer -= singleShakeDuration;
        if ((shakeTimer != -9999) && (shakeTimer <= 0)){
            screenshakeTransform.DOAnchorPos(Vector2.zero, singleShakeDuration);
            return;
        }
        Vector2 newSpot = new Vector2 (StaticVariables.rand.Next(-50, 50), StaticVariables.rand.Next(-50, 50));
        screenshakeTransform.DOAnchorPos(newSpot, singleShakeDuration).OnComplete(ShakeScreen);
    }

    private void StopShakeScreen(){
        shakeTimer = 0 ;
    }

    private void PlayAnimationAndMoveThenIdle(string objectName, string animationName, float xPos, float yPos, float duration){
        PlayAnimationThenIdle(objectName, animationName, duration);
        MoveObject(objectName, xPos, yPos, duration);
    }

    private void PlayAnimationThenIdle(string objectName, string animationName, float duration){
        PlayAnimation(objectName, animationName);
        StaticVariables.WaitTimeThenCallFunction(duration, PlayIdle, objectName);
    }

    private void PlayIdle(string name){
        PlayAnimation(name, "Idle");
    }
    
    private void PlayAnimation(string objectName, string animationName){
        GetAnimatorFromName(objectName).Play(animationName);
    }

    private void MoveObject(string objectName, float xPos, float yPos, float duration){
        //every animated object is made the child of an empty gameobject, so we want to move the parent of the animator
        Animator anim = GetAnimatorFromName(objectName);
        //the same is not true of searchable gameobjects, we want to move those directly
        GameObject go = GetObjectFromName(objectName);
        if (anim != null)
            anim.transform.parent.GetComponent<RectTransform>().DOAnchorPos(new Vector2(xPos, yPos), duration);
        else if (go != null)
            go.GetComponent<RectTransform>().DOAnchorPos(new Vector2(xPos, yPos), duration);
        else{
            print("no object found with name [" + name + "] to move");
        }
    }

    private void FlipDirection(string objectName){
        Animator anim = GetAnimatorFromName(objectName);
        anim.transform.parent.localScale = new Vector2(anim.transform.parent.localScale.x * -1, anim.transform.parent.localScale.y);
    }

    private void StartCutsceneImageTransition(GameObject bg){
        nextBackground = bg;
        StaticVariables.StartFadeDarken(0.5f);
        StaticVariables.WaitTimeThenCallFunction(0.5f, MidCutsceneImageTransition);
    }

    private void MidCutsceneImageTransition(){
        SetCutsceneBackground(nextBackground);
        StaticVariables.StartFadeLighten(0.5f);
        StaticVariables.WaitTimeThenCallFunction(0.5f, EndCutsceneImageTransition);
        switch (advanceCondition){
            case (Cond.BackgroundChange):
                AdvanceCutsceneStep();
                break;
        }
    }

    private void SetCutsceneBackground(GameObject prefab){

        animatedObjectsInCutscene = new List<Animator>();
        searchableObjectsInCutscene = new List<GameObject>();
        Transform backgroundPrefab = prefab.transform.GetChild(1).transform;

        foreach (Transform t in backgroundParent)
            Destroy(t.gameObject);
        foreach(Transform t in backgroundPrefab){

            Animator a = t.gameObject.GetComponent<Animator>();
            if (a != null){
                GameObject parent = Instantiate(emptyGameObject, backgroundParent);
                parent.transform.localPosition = t.localPosition;
                parent.name = t.name;
                parent.SetActive(t.gameObject.activeSelf);
                GameObject go = Instantiate(t.gameObject, parent.transform.position, Quaternion.identity, parent.transform);
                go.name = t.name;
                parent.transform.localRotation = t.localRotation;
                go.SetActive(true);
                animatedObjectsInCutscene.Add(go.GetComponent<Animator>());
                AddToSearchableListIfAppropriate(go);
            }
            else{
                GameObject go = Instantiate(t.gameObject, backgroundParent);
                go.name = t.gameObject.name;
                if (t.name.Contains("Player"))
                    playerAnimator = go.transform.GetChild(0).GetComponent<Animator>();
                AddToSearchableListIfAppropriate(go);
            }
        }
    }

    private void AddToSearchableListIfAppropriate(GameObject go){
        if (go.tag == "ScriptSearchable")
            searchableObjectsInCutscene.Add(go);
    }

    private void EndCutsceneImageTransition(){
    }

    public void ExternalTrigger(float optionalParameter = -999){
        if (advanceCondition == Cond.externalTrigger){
            if (optionalParameter != -999)
                externalTriggerParameter = optionalParameter;
            AdvanceCutsceneStep();
        }
    }

}


