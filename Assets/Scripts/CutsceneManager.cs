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

    private void SetupHometown1()
    {
        SetCutsceneBackground(hometown1);
        ToggleObject("Player", false);
        //cutsceneStep = 79; //for testing the water spray
    }

    private void SetupHometown2(){
        SetCutsceneBackground(hometown2);
        PlayAnimation("Redhead Lady", "Idle Back");
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
        //cutsceneStep = 28;  
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
            DisplayEnemyTalking("Miss " + StaticVariables.playerName + "! Miss " + StaticVariables.playerName + "!", "Child 2", DialogueStep.Emotion.Happy);
            PlayAnimationAndMoveThenIdle("Child 1", "Walk", 26, 605, 2f);
            PlayAnimationAndMoveThenIdle("Child 2", "Walk Holding Newspaper", 85, 564, 2.3f);
            //PlayAnimation("Child 2", "Idle Holding Newspaper");
            //MoveObject("Child 2", 85, 564, 2.3f);
            AdvanceConditionWaitThenClick(2.3f);
        }
        else if (++i == cutsceneStep){
            //PlayAnimation("Child 2", "Idle Holding Newspaper");
            AdvanceConditionDialogue_EnemyTalking("Put that dumb book down! We need you to read this newspaper!!", "Child 1", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Mikey stop! You know how Miss " + StaticVariables.playerName + " feels when you insult her books!", "Child 2", DialogueStep.Emotion.Angry);
            PlayAnimation("Player", "Put Away Book Random Brown");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("She'll never stop talking about \"the prince's manticore wears shorts\"!", "Child 2", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("That doesn't sound right, I think it's \"a pig is manlier when it snores\".", "Child 1", DialogueStep.Emotion.Questioning);
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
            AdvanceConditionDialogue_EnemyTalking("Oh yeah! Read it to us! Please???", "Child 2", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Okay, okay...\nLet's see here...", DialogueStep.Emotion.Normal);
        }     
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_NobodyTalking();
        //}        
        //else if (++i == cutsceneStep){
        //    PlayAnimation("Child 2", "Show Newspaper");
        //    AdvanceConditionWait(1.3f);
        //}        
        //else if (++i == cutsceneStep){
        //    PlayAnimation("Child 2", "Idle Not Holding Newspaper");
        //    PlayAnimation("Player", "Idle Holding Newspaper");
        //    AdvanceConditionWait(0.6f);
        //}
        else if (++i == cutsceneStep){
            PlayAnimation("Child 2", "Idle Not Holding Newspaper");
            PlayAnimation("Player", "Idle Holding Newspaper");
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
            DisplayEnemyTalking("My daughter lives in Duskvale!", "Brown Hair Lady No Hat", DialogueStep.Emotion.Angry);
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Brown Hair Lady No Hat", "Walk", -419, 244, 1.3f);
            AdvanceConditionWaitThenClick(1.5f);
        } 
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I hate the Lich King!", "Redhead Lady", DialogueStep.Emotion.Angry);
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Redhead Lady", "Walk", 58, 334, 2f);
            AdvanceConditionWaitThenClick(2.2f);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Keep Reading!!!", "Everyone - Hometown Intro", DialogueStep.Emotion.Custom1); //trio of adults, angry
        }
        else if (++i == cutsceneStep){
            HideEnemyChathead();
            AdvanceConditionDialogue_PlayerTalking("\"Late last night, a great hole appeared in the center of Duskvale.\"", DialogueStep.Emotion.Normal);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"Out of the hole came a burst of flame, and the city was quickly beseiged by --\"", DialogueStep.Emotion.Normal);
        }     
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("ROOOOOOOAR!!!!", "Red Dragon", DialogueStep.Emotion.Mystery, "???");
        }
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_NobodyTalking(true);
        }
        else if (++i == cutsceneStep){
            //DisplayNobodyTalking();
            PlayAnimation("Red Dragon", "Fly");
            MoveObject("Red Dragon", 1200, 1377, 2f);
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Child 1");
            FlipDirection("Child 2");
            FlipDirection("Redhead Lady");
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
            AdvanceConditionDialogue_EnemyTalking("Ha ha ha...\nPuny humans in your simple town...", "Red Dragon", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I could eat all of you on a whim!", "Red Dragon", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Instead I come with good news! Your hated Lich King has been driven from his throne!", "Red Dragon", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("This town now belongs to the mighty Queen of Ash! Bow down to her, me, and all of dragonkind!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("We won't be bossed around by some overgrown lizard!", "Blacksmith", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("We're not afraid of you!", "Redhead Lady", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Don't antagonize the dragon! It could kill us all!", DialogueStep.Emotion.Worried);
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yes, the young lady makes quite the compelling argument.", "Red Dragon", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Allow me to give you a little taste of fear!", "Red Dragon", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
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
            AdvanceConditionDialogue_EnemyTalking("Ha ha ha... You humans are powerless!", "Red Dragon", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("What do you want from us?", "Brown Hair Lady No Hat", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Well of course you'll be paying taxes to your new draconic rulers!", "Red Dragon", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("But it appears this commotion drew the attention of some local golbins.", "Red Dragon", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }
        else if (++i == cutsceneStep){
            //DisplayNobodyTalking();
            PlayAnimationAndMoveThenIdle("Goblin 1", "Walk", 70, -64, 2.2f);
            PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", 514, 310, 2f);
            PlayAnimationAndMoveThenIdle("Goblin 3", "Walk", 446, 489, 2f);
            PlayAnimationAndMoveThenIdle("Goblin General", "Walk", 240, 183, 1.5f);
            AdvanceConditionWait(2.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Humans! We come for your gold!", "Goblin General", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I'd love to stay and help you peasants, but I have other towns to visit.", "Red Dragon", DialogueStep.Emotion.Normal);
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
            AdvanceConditionDialogue_EnemyTalking("Attack the invaders! Defend our homes!", "Everyone - Hometown Intro", DialogueStep.Emotion.Custom1);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            HideEnemyChathead();
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", 29, 130, 0.5f);
            PlayAnimationAndMoveThenIdle("Redhead Lady", "Walk", 274, 535, 1f);
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
            AdvanceConditionDialogue_EnemyTalking("Us neither!", "Child 1", DialogueStep.Emotion.Worried);
            FlipDirection("Child 1");
            FlipDirection("Child 2");
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("We should leave the town defense to the professionals...", DialogueStep.Emotion.Normal);
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
            AdvanceConditionDialogue_EnemyTalking("She really likes her books huh...", "Child 1", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yeah, what a weirdo.", "Child 2", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Do you think we should go help her?", "Child 1", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("No, I don't want to get smacked by a flying book today.", "Child 2", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Besides, how many books can she even have in there? Do you think she's almost done?", "Child 2", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Maybe we can go find a goblin to fight!", "Child 1", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Wait, do you hear that?", "Child 2", DialogueStep.Emotion.Questioning);
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
            AdvanceConditionWait(0.1f);        
            //AdvanceConditionWait(2.5f);
        }
        else if (++i == cutsceneStep){
            MagicFlash flash = GetAnimatorFromName("Water Spray").transform.GetChild(0).GetComponent<MagicFlash>();
            flash.gameObject.SetActive(true);
            flash.StartProcess(StaticVariables.waterPowerupColor);
            AdvanceConditionWait(flash.GetTotalTime() - flash.fadeTime);
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
            AdvanceConditionWait(4f);
        }
        else if (++i == cutsceneStep){
            Transform player = GetObjectFromName("Player").transform.parent;
            player.SetSiblingIndex(player.parent.childCount -3);
            AdvanceConditionDialogue_PlayerTalking("There's a book coming out of the water!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            HideChatheads();
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
            AdvanceConditionWait(0.367f);
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
            AdvanceConditionDialogue_PlayerTalking("It's called \n\"The Spell-Book\".", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Isn't this just an introduction to spelling and the alphabet?", DialogueStep.Emotion.Questioning);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("I always thought this was an old empty journal...", DialogueStep.Emotion.Questioning);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("But... now the pages are filled with random letters!", DialogueStep.Emotion.Questioning);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("I bet it was invisible ink... or maybe even magic?", DialogueStep.Emotion.Questioning);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Could it be an actual spellbook?", DialogueStep.Emotion.Questioning);
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
            AdvanceConditionDialogue_EnemyTalking("I didn't know you had that fighting spirit in you!", "Blacksmith", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I wouldn't call myself much of a warrior...", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Nonsense! You were incredible!", "Blacksmith", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("And the magic too! How did you do that??", "Brown Hair Lady No Hat", DialogueStep.Emotion.Questioning);
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
            AdvanceConditionDialogue_EnemyTalking("The paper has a bunch of strange symbols on it...", "Brown Hair Lady No Hat", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Those are letters. You know, the kind people use for reading and writing?", DialogueStep.Emotion.Questioning);
        } 
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Wow! Magic is crazy!", "Brown Hair Lady No Hat", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Um, we haven't quite arrived at the magic yet...", DialogueStep.Emotion.Worried);
        } 
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("There's more?", "Brown Hair Lady No Hat", DialogueStep.Emotion.Happy);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Try touching the letters!", DialogueStep.Emotion.Happy);
        } 
        //the paper has a bunch of strange symbols on it...
        //those are letters. you know, the kind people use for reading and writing?
        //wow! magic is crazy!
        //well we haven't exactly gotten to the interesting part yet...
        //try touching the letters!

        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("T", "Brown Hair Lady No Hat", DialogueStep.Emotion.Questioning);
        //}
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("Do you see all those jumbled random letters?", DialogueStep.Emotion.Questioning);
        //}
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("Yeah?", "Brown Hair Lady No Hat", DialogueStep.Emotion.Questioning);
        //}
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("Try touching them!", DialogueStep.Emotion.Happy);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }        
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Brown Hair Lady No Hat", "Walk", -200, 585, 0.4f);
            AdvanceConditionWait(0.4f);
        }   
        else if (++i == cutsceneStep){
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Brown Hair Lady No Hat", "Walk", -117, 550, 0.4f);
            AdvanceConditionWait(0.4f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Nothing interesting happens...", "Brown Hair Lady No Hat", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That's weird, it works for me. Watch!", DialogueStep.Emotion.Questioning);
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
            AdvanceConditionDialogue_EnemyTalking("Hey! Don't start throwing magic at people!", "Redhead Guy", DialogueStep.Emotion.Angry);
            FlipDirection("Redhead Guy");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yeah, if you want to sling spells, go to the Academy!", "Blond Lady", DialogueStep.Emotion.Angry);
            FlipDirection("Blond Lady");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("It's been a long time since anyone used magic around here.", "Redhead Lady", DialogueStep.Emotion.Normal);
            PlayAnimation("Redhead Lady", "Idle");
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("The last person was who, old man Eldric? ", "Redhead Guy", DialogueStep.Emotion.Normal);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yeah, it's been decades since Eldric was stomping around here, slinging his spells...", "Redhead Guy", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Hey, I'm not dead, I'm just old!", "Elder", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }        
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Elder", "Walk", 427, 471, 2f);
            AdvanceConditionWait(2f);
        }   
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You whippersnappers are flappin' yer yappers about my magic?", "Elder", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Well, you're right. Back in the old days, when I could still touch my toes, I had control over the wind!", "Elder", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Like you could make tornadoes and stuff?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yes! I could make tornadoes! And stuff!", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I was an adventurer! Slaying monsters, saving princesses...", "Elder", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I even fought an owlbear once!", "Elder", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("But one day I woke up with a shake in my hands, and that was it. I couldn't use magic after that.", "Elder", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I've read some of the library's medical textbooks. Maybe there's something in one of them that could heal your hand tremors?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh! My book can do some healing magic! I bet I could fix your hands with that!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Young lady, that is very kind of you. But I'm afraid we have more important matters to discuss today.", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Gather 'round, everyone!", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            FlipDirection("Shopkeeper");
            PlayAnimationAndMoveThenIdle("Shopkeeper", "Walk", -156, 129, 2.3f);
            FlipDirection("Blue-haired Lady");
            PlayAnimationAndMoveThenIdle("Blue-haired Lady", "Walk", 106, 114, 2f);
            PlayAnimationAndMoveThenIdle("Child 2", "Walk", 268, 172, 2.2f);
            PlayAnimationAndMoveThenIdle("Child 1", "Walk", 361, 370, 1.6f);
            PlayAnimationAndMoveThenIdle("Chef", "Walk", -284, 254, 1.6f);
            AdvanceConditionWait(0.8f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Brown Hair Lady No Hat", "Walk", -384, 413, 0.8f);
            AdvanceConditionWait(0.6f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Brown Hair Lady No Hat");
            AdvanceConditionWait(0.2f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Shopkeeper");
            PlayAnimationAndMoveThenIdle("Redhead Lady", "Walk", 348, 573, 2.1f);
            PlayAnimationAndMoveThenIdle("Redhead Guy", "Walk", 219, 724, 2.2f);
            PlayAnimationAndMoveThenIdle("Blond Lady", "Walk", 73, 803, 2.3f);
            PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", -205, 763, 0.5f);
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Blacksmith");
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Allow me to be blunt here... If that dragon comes by again, we're toast!", "Elder", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("We need to do something!", "Blond Lady", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            HideEnemyChathead();
            AdvanceConditionDialogue_PlayerTalking("I have some information that may be helpful.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I was reading this newspaper before the dragon showed up!", DialogueStep.Emotion.Happy);
            PlayAnimation("Player", "Take Out Newspaper");
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_NobodyTalking();
        //}        
        //else if (++i == cutsceneStep){
        //    PlayAnimation("Player", "Take Out Newspaper");
        //    AdvanceConditionWait(1f);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Let me read you all the full article...\n\"Last night a great hole appeared in the center of Duskvale.\"", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"Out of the hole came a burst of flame, and the city was quickly beseiged by a group of dragons!\"", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"The Lich King and his advisors promptly engaged the dragons in combat. Vibrant blasts of magic and jets of fire shook the city!\"", DialogueStep.Emotion.Worried);
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
            AdvanceConditionDialogue_EnemyTalking("Well I'm not one for politics, but that bit about a dragon-killing sword sounds pretty useful right about now!", "Redhead Guy", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I had a book about that sword in the library, but I think it got ruined in the dragon attack.", DialogueStep.Emotion.Defeated);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Oh don't worry about that, Miss " + StaticVariables.playerName + "!", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I knew a swordswoman who weilded the power of that very sword!", "Elder", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("There weren't many dragons around back then, so we sealed the sword away in a temple in the desert.", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("For all I know, it might just still be there!", "Elder", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("We have to go and get it!", "Brown Hair Lady No Hat", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Let's go now!", "Blacksmith", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Which way to the desert??", "Redhead Lady", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Now don't be too hasty! It's quite a journey to get there, and they call it the \"Sunscorched Desert\" for a reason!", "Elder", DialogueStep.Emotion.Worried);
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
            AdvanceConditionDialogue_EnemyTalking("If you're sure...", "Elder", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I am!!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("To get to the desert, you'll have to get to the far side of the enchanted forest, beyond the grasslands to the south.", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("And don't worry about us, we will make sure the town stays safe!", "Blacksmith", DialogueStep.Emotion.Happy);
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
            AdvanceConditionDialogue_PlayerTalking("Or should I call you \"The Spell-Book\"?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("How about just \"Spellbook\", for short?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Can you even understand me?", DialogueStep.Emotion.Questioning);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("You're a magic book. Can you understand me?", DialogueStep.Emotion.Questioning);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The magic book says nothing.", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Okay, well your pages fill with random letters when it's magic time. Can you say something to me with them?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The book's pages are empty.", "Magic Book", DialogueStep.Emotion.Normal);
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
            AdvanceConditionDialogue_EnemyTalking("Miss "+ StaticVariables.playerName + "! Wait a moment!", "Elder", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }      
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Elder", "Walk", -38, 676, 2f);
            AdvanceConditionWait(2f);
        }      
        else if (++i == cutsceneStep){
            FlipDirection("Elder");
            AdvanceConditionWait(0.5f);
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Are you... arguing with your book?", "Elder", DialogueStep.Emotion.Questioning);
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
            AdvanceConditionDialogue_EnemyTalking("I wanted to thank you for offering to heal my hands and bring my magic back!", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Actually, about that..." , DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I think I should get some more practice with magic before I try it on people!" , DialogueStep.Emotion.Surprised);   
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("That's alright, I'm an old man now. I don't even know if I want to weild that power again...", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I do miss cooking though! Before I started adventuring, I was the head chef for the tavern.", "Elder", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("When you come back from your quest, I'll take you up on your offer. You can fix up my hands and I'll cook you a mean nine-course meal!", "Elder", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That sounds lovely!" , DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Oh! But that's not why I came out here! You showed me kindness, and I wanted to give you something in return.", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }
        else if (++i == cutsceneStep){ 
            AdvanceConditionWait(2f);
            PlayAnimation("Elder", "Take Out Bag");
        } 
        else if (++i == cutsceneStep){
            PlayAnimation("Elder", "Idle");
            PlayAnimation("Player", "Idle Holding Bag");
            AdvanceConditionWait(1.6f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("This is an enchanted pouch that can carry anything and everything you put inside it!", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I had the children fill it with every single one of your books that survived the flames.", "Elder", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("This is incredible! Thank you!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You're welcome, Miss " + StaticVariables.playerName + ".", "Elder", DialogueStep.Emotion.Normal);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("You have a long journey ahead of you. Best to not get bored.", "Elder", DialogueStep.Emotion.Normal);
        //}
        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("It'd be best not to get bored on your journey.", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("And it will be quite the long journey indeed! I shouldn't keep you any longer.", "Elder", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Make haste! The future of the whole continent may depend on it!", "Elder", DialogueStep.Emotion.Angry);
        }
        //it'd be best to not get bored on your journey
        //and it will be quite the long journey indeed! i shouldn't keep you any longer
        //make haste! the future of the whole continent may depend on it!
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("You're right! I'd better get going!", DialogueStep.Emotion.Surprised);
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
            AdvanceConditionDialogue_PlayerTalking("Alright, Spellbook.", DialogueStep.Emotion.Normal);
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
            AdvanceConditionDialogue_PlayerTalking("Great. You're an actual real-life talking book! You want me to just ignore that and go into some dark cave?", DialogueStep.Emotion.Questioning);
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
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("And my life has certainly been looking like a fantasy adventure lately...", DialogueStep.Emotion.Happy);
        //}
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
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_PlayerTalking("\"From Here to Eternity\", \"Stiff\"...\nThese are books about burial practices!", DialogueStep.Emotion.Surprised);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("Book name, book name...", DialogueStep.Emotion.Normal);
        //}
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("These are history books.", DialogueStep.Emotion.Normal);
        //}
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("But they also cover burial practices and enbalming techniques.", DialogueStep.Emotion.Questioning);
        //}
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
            AdvanceConditionDialogue_PlayerTalking("Well, aside from a literal spellbook anyway...", DialogueStep.Emotion.Normal);
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
            AdvanceConditionDialogue_PlayerTalking("I don't understand, this shouldn't be possible.", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("The Necronomicon was just a narrative device that H. P. Lovecraft made up for his horror stories.", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Its legend became so notorious that many libraries received check-out requests for it!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("But no copies ever actually existed, and certainly not with any forbidden eldritch secrets.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("But here one is, right in front of me. As real as I am.", DialogueStep.Emotion.Normal);
        }
        //i don't understand, this shouldn't be possible.
        //the necronomicon was just a storytelling device H. P. Lovecraft used in his horror stories
        //its legend became so notorious that many libraries received actual requests for it
        //but no actual copies exist, and certainly not with eldritch secrets within!
        //but here it is, right in front of me. as real as i am.
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("I didn't know that was real!", DialogueStep.Emotion.Worried);
        //}
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
            AdvanceConditionDialogue_PlayerTalking("Alright, Spellbook! Tell me what you want me to do, and then let's get out of here!", DialogueStep.Emotion.Worried);
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
        else if (++i == cutsceneStep) {
            FlipDirection("Player");
            PlayAnimationAndMoveThenIdle("Player", "Walk While Holding Book Flipped", -536, 1675, 5f);
            MoveEverythingExceptPlayer(1000, 0, 5f);
            AdvanceConditionWait(2f);
        }
        else if (++i == cutsceneStep) {
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
        /*
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
        */
    }

    private void DoForest1Step(){   
        int i = 0;
        if (++i == cutsceneStep) {
            PlayAnimation("Player", "Walk While Holding Book Flipped");
            MoveObject("Player", -78, 1935, 3f);
            //PlayAnimationAndMoveThenIdle("Player", "Walk While Holding Book", -78, 1935, 3f);
            AdvanceConditionWait(3f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Idle Holding Book Flipped");
            AdvanceConditionDialogue_PlayerTalking("Huff... Huff... Phew!", DialogueStep.Emotion.Surprised);
        } 
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I really need to get in better shape!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Especially if you keep putting me in dangerous situations!", DialogueStep.Emotion.Angry);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_NobodyTalking();
        //}  
        //else if (++i == cutsceneStep){
        //    AdvanceConditionWait(1.5f);
        //    PlayAnimation("Player", "Take Out Book");
        //}
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
            AdvanceConditionDialogue_PlayerTalking("Our next goal is to get through this forest!", DialogueStep.Emotion.Normal);
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
            AdvanceConditionDialogue_PlayerTalking("I can't outrun these rabbits forever!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I need to find that other human, and fast!", DialogueStep.Emotion.Surprised);
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
            AdvanceConditionDialogue_PlayerTalking("Okay, Spellbook...", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionWait(1.5f);
            PlayAnimation("Player", "Take Out Book While Walking");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Please help me out here!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Do you feel anything with your magic radar thing?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The book immediately responds,\n\"THERE IS A STRONG MAGICAL AURA RADIATING FROM BELOW\".", "Magic Book", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Great!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I should look for some kind of tunnel entrance, or big tree stump, or...", DialogueStep.Emotion.Surprised);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_NobodyTalking(true);
        //}  
        else if (++i == cutsceneStep){
            HideChatheads();
            DisplayNobodyTalking();
            advanceCondition = Cond.externalTrigger;
            List<CutsceneTreeGenerator> treeGenerators = new() {
                GetObjectFromName("Tree Spawner 1").GetComponent<CutsceneTreeGenerator>(),
                GetObjectFromName("Tree Spawner 2").GetComponent<CutsceneTreeGenerator>(),
                GetObjectFromName("Tree Spawner 3").GetComponent<CutsceneTreeGenerator>(),
                GetObjectFromName("Tree Spawner 4").GetComponent<CutsceneTreeGenerator>(),
                GetObjectFromName("Tree Spawner 5").GetComponent<CutsceneTreeGenerator>(),
                GetObjectFromName("Tree Spawner 6").GetComponent<CutsceneTreeGenerator>()
            };
            foreach(CutsceneTreeGenerator treeGenerator in treeGenerators)
                treeGenerator.BeginSlowdown();
        } 
        else if (++i == cutsceneStep){
            CutsceneTreeMimic mimic = GetObjectFromName("Tree Synchronizer").transform.GetChild(0).GetComponent<CutsceneTreeMimic>();
            mimic.gameObject.SetActive(true);
            mimic.originalTree = GetObjectFromName("Tree Spawner 3").transform.GetChild(1);
            StaticVariables.WaitTimeThenCallFunction(externalTriggerParameter, mimic.DestroyScript);

            //print(externalTriggerParameter);
            AdvanceConditionWait(externalTriggerParameter - 0.5f);
        } 
        else if (++i == cutsceneStep){
            MoveObject("Player", -96, 2213, 0.5f);
            AdvanceConditionWait(0.5f);
        } 
        else if (++i == cutsceneStep){
            DOTween.KillAll();
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
            GetObjectFromName("Rabbit Cycles").SetActive(true);
            //start rabbit running
            AdvanceConditionWait(3.5f);
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
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            StartCutsceneImageTransition(forest2_pt2);
            advanceCondition = Cond.BackgroundChange;
        }
        else if (++i == cutsceneStep){
            StopShakeScreen();
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Wow!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I wasn't expecting a high-tech laboratory!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("There's so much bubbling and buzzing.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That magical object must be around here somewhere...", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("If I can find it, I might be able to figure out what's really going on in this forest!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        }  
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -198, 2004, 1f);
            AdvanceConditionWait(1.3f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(0.4f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -127, 1640, 1.5f);
            MoveEverythingExceptPlayer(-205, 0, 1.5f);
            AdvanceConditionWait(1.7f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionWait(0.3f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -213, 1914, 1.5f);
            MoveEverythingExceptPlayer(-360, 0, 1.5f);
            AdvanceConditionWait(1.7f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", 100, 1914, 1f);
            AdvanceConditionWait(1.2f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(0.3f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -213, 1914, 1f);
            AdvanceConditionWait(1.2f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            AdvanceConditionWait(0.3f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -173, 1756, 2f);
            MoveEverythingExceptPlayer(-635, 0, 2f);
            AdvanceConditionWait(2.5f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Idle");
            AdvanceConditionDialogue_PlayerTalking("This looks pretty important!\nI wonder if -", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("A surprise visitor! How exciting!", "Wizard", DialogueStep.Emotion.Mystery, "???");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh!!", DialogueStep.Emotion.Worried);
            FlipDirection("Player");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        } 
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Wizard").transform.parent.GetComponent<RectTransform>().anchoredPosition = (new Vector2(-800, 343));
            PlayAnimationAndMoveThenIdle("Wizard", "Walk", -474, 343, 1.5f);
            AdvanceConditionWait(1.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Welcome to my laboratory!", "Wizard", DialogueStep.Emotion.Happy, "Wizard");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I'd offer you something to drink, but I'm afraid everything here is quite toxic to humans.", "Wizard", DialogueStep.Emotion.Questioning, "Wizard");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Is that a threat??", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("No, but please do keep your voice down.", "Wizard", DialogueStep.Emotion.Normal, "Wizard");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I am delighted to finally meet you!", "Wizard", DialogueStep.Emotion.Happy, "Wizard");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I've heard whispers of your movements around the forest in recent days.", "Wizard", DialogueStep.Emotion.Normal, "Wizard");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I've spent a considerable amount of my time here looking for you, actually.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I'm hoping you could tell me what's really going on here...", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I'd be happy to answer any questions!", "Wizard", DialogueStep.Emotion.Happy, "Wizard");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("But first, allow me to introduce myself.\nMy name is Mustrum, I'm 138 years old, and I'm...", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I'm...", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I'm sorry, Miss... " + StaticVariables.playerName + "? Are you wearing a name tag?", "Wizard", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh yeah, it's a funny story!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Here in the forest, everyone sees me as just another human.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("They're all suspicious of you, so they're suspicious of me too.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I thought a name tag might make me a little more personable.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Come to think of it, that's not a funny story. It's been a little dehumanizing.", DialogueStep.Emotion.Defeated);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Oh, that is unfortunate! You have my apologies.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Acutally, that brings me to my first question!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Why is everyone in the forest suspicious of you, anyway?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Since I've come here, my magical prowess has been steadily growing.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Meanwhile, the strength of the forest wanes.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Some creatures have even lost their magical intelligence altogether!", "Wizard", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh, like that big fluffy monster, by the huge tree!", DialogueStep.Emotion.Surprised); //player didnt previously know it as an owlbear?
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Precisely! That tree, the Quercus giganteum, lies at the heart of the forest.", "Wizard", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("It is what I call a \"catalyst\", a sentient object that contains the powers of a school of magic.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I've come to discover that it's tied to the very essence of wild magic in these lands.", "Wizard", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Its roots spread throughout the forest, intertwining with nearly every living thing.", "Wizard", DialogueStep.Emotion.Normal);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("One such root is right above you, steeping in that cauldron.", "Wizard", DialogueStep.Emotion.Normal);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Interesting...", DialogueStep.Emotion.Questioning);
            FlipDirection("Player");
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("So this tree is the reason all the forest critters can think and talk and have an organized police force?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yes, it would seem so.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That's amazing! Are these overhead roots part of this \"catalyst\" too?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Yes! Although I may have to find a new root to work with soon. I doubt this sample will survive much longer.", "Wizard", DialogueStep.Emotion.Normal);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("Although I doubt it will survive much longer. I may have to find a new root to work with soon...", "Wizard", DialogueStep.Emotion.Normal);
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What??", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What are you doing to the roots??", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Please, don't raise your voice!", "Wizard", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What are you doing to the tree??", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking(StaticVariables.playerName + "!", "Wizard", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What are you doing to the whole forest??", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
            StartScreenShake();        
        }  
        else if (++i == cutsceneStep){
            GetObjectFromName("tiny roots").GetComponent<CutsceneBranchOrganizer>().StartDrops();
            AdvanceConditionWait(0.5f);
        }       
        else if (++i == cutsceneStep){
            Transform cluster = GetObjectFromName("Root Cluster").transform;
            cluster.DORotate(new Vector3(0,0,-6), 1.3f);
            cluster.DOLocalMoveY(-609, 1.3f);
            AdvanceConditionWait(1.3f);
        }         
        else if (++i == cutsceneStep){      
            Transform cluster = GetObjectFromName("Root Cluster").transform;
            foreach (Transform t in cluster){
                t.GetChild(0).gameObject.SetActive(false);
                t.GetChild(1).gameObject.SetActive(true);
            }
            AdvanceConditionWait(0.3f);
        }      
        else if (++i == cutsceneStep){
            Transform cluster = GetObjectFromName("Root Cluster").transform;
            cluster.GetComponent<CutsceneBranchOrganizer>().StartDrops();
            AdvanceConditionWait(4.7f);
        }   
        else if (++i == cutsceneStep){
            StopShakeScreen();
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Oh, dear...", "Wizard", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That's it, no more questions!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I'm putting a stop to your mad science here and now!", DialogueStep.Emotion.Angry);
        }    
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking(true);
        } 
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Cast Spell");
            AdvanceConditionWait(0.33f);
        }
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Player").transform.parent.GetChild(2).gameObject.SetActive(true);
            AdvanceConditionWait(0.5f);
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
            AdvanceConditionWait(0.75f);
        }
        //something breaks on the rock impact
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Player").transform.parent.GetChild(2).gameObject.SetActive(false);
            AdvanceConditionWait(0.1f);
        }        
        //else if (++i == cutsceneStep){
        //    StaticVariables.hasCompletedStage = true;
        //    StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        //}

        /*


        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Plus, that old cyclops necromancer said you were tampering with the forest!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Cyclops? Are you referring to the so-called \"Guardian of the Forest\"?", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I don't know where you got it in your head that he's a necromancer.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("He was studying the Necronomicon!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You must be mistaken. The Necronomicon isn't real.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I thought so too, but then I saw it with my own eyes!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I assure you, that cyclops is no necromancer. Merely a humble historian.", "Wizard", DialogueStep.Emotion.Normal);
        }
        */
    }

    private void DoForest3Step(){   
        int i = 0;
        //player should be to the left, wizard to the right in front of cauldron
        if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("You want to tell me the truth? Fine, out with it!!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("And if you try anything funny, I'll knock you out again!", DialogueStep.Emotion.Angry);
        }
        else if (++i == cutsceneStep){
            //AdvanceConditionDialogue_NobodyTalking(true);
            AdvanceConditionDialogue_EnemyTalking("Sigh...\nAs you wish.", "Wizard", DialogueStep.Emotion.Defeated);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Ninety-nine years ago, I graduated from The Academy in Duskvale with a degree in biochemistry.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Shortly thereafter, I came to the Enchanted Forest to research the nature of magic.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Year after year, hypothesis after hypothesis...", "Wizard", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I became a better practitioner of the sciences, and a much more powerful pyromancer!", "Wizard", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("But I also came to discover that the forest was slowly dying.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("And you weren't just stealing the magic of the tree?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking(StaticVariables.playerName +", it's impossible to \"steal\" magic.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Where do you think magic comes from?", "Wizard", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I can't say I've ever given it much thought...", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("My magic comes from a talking book!", DialogueStep.Emotion.Normal);
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_NobodyTalking();
        //}  
        //else if (++i == cutsceneStep){
        //    AdvanceConditionWait(1.5f);
        //    PlayAnimation("Player", "Take Out Book");
        //}
        //player takes out book
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Could my spellbook be one of those catalysts you mentioned earlier?", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Precisely! I'd guess it's the catalyst for the <water>element of water<>.", "Wizard", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("When someone comes into contact with such a catalyst, they may gain control over an elemental power.", "Wizard", DialogueStep.Emotion.Normal);
            //when someone comes into contact with such a catalyst, they gain control over the associated elemental power
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("But... I'm the only one that can use the powers of the book!", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I let some friends give it a test, with no results.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I've come across a few other magical people, and none of them had a spellbook either.", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Would you mind telling me a bit about them?", "Wizard", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Well most recently, there's you. You live in a forest and control <fire>fire magic<>. You studied at the Academy, and you experiment on a sentient tree.", DialogueStep.Emotion.Normal);        
            //well most recently, there's you. you live in a forest and control fire magic. you studied at the academy, and you perform scientific experiments on sentient trees.
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Then before you, there was the cyclops in the grasslands. He threw <earth>magical rocks<>, spoke very slowly, and is probably a necromancer.", DialogueStep.Emotion.Normal);        
            //then before you, there's the cyclops in the grasslands. he threw magical rocks, spoke very slowly, and is probably a necromancer
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I have reason to believe he isn't a necromancer, but please continue.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("The cyclops had an apprentice who had his own face on his torso. He was good at throwing rocks, but not much else.", DialogueStep.Emotion.Normal);        
            //well the cyclops had an apprentice, a blemmyae, who had his face attached to his torso. he was good at throwing rocks, but not much else.
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Before him, there was this retired adventurer in my hometown who used to have control over the wind.", DialogueStep.Emotion.Normal);        
            //then there was a retired adventurer in my hometown who used to have control over the wind.
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Plus there's me! I have a magical book that lets me <water>make waves<>, <healing>heal the injured<>, and <earth>toss rocks<> at people.", DialogueStep.Emotion.Happy);        
            //plus there's me! i have a magical book that lets me make waves, heal the injured, and toss rocks about.
        }
        //i've also heard the lich king uses magic, although i'm not sure what kind.
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Why does this matter anyway?", DialogueStep.Emotion.Questioning);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Magic is magic, the people just use it.", DialogueStep.Emotion.Normal);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("After a century of research in the field, I can assure you that is not the case.", "Wizard", DialogueStep.Emotion.Normal);
           //from a century of research in the field, i can assure you that is not the case.          
        }        
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Is there something that makes these people special or worthy somehow?", DialogueStep.Emotion.Questioning);        
            //there's something about all of these people that makes them special or worthy somehow?
        }   
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Eldric went on epic adventures! You and the cyclops have lived a long time; I bet you've done some pretty cool stuff.", DialogueStep.Emotion.Questioning);        
            //old man eldric went on epic adventures. you and the cyclops have lived a long time; i bet you've done some pretty cool stuff.
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("But that doesn't make sense! I'm just a librarian, and I can use magic too!", DialogueStep.Emotion.Surprised);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("And I've never heard of anyone else who can control three different elements!", DialogueStep.Emotion.Surprised);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What's special about me? I just like to read books...", DialogueStep.Emotion.Defeated);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("...", DialogueStep.Emotion.Questioning);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("That's it! I like to read books!", DialogueStep.Emotion.Happy);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Does magic come from reading?", DialogueStep.Emotion.Questioning);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Ah! Not just reading. We have arrived at the crux of the matter.", "Wizard", DialogueStep.Emotion.Happy);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Each branch of magic is tied to a unique field of study.", "Wizard", DialogueStep.Emotion.Normal);    
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("For example, <fire>fire<> is the element of science and discovery.", "Wizard", DialogueStep.Emotion.Happy);    
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Great pyromancers are biologists, chemists, and physicists, seeking to understand the fundamentals of our universe.", "Wizard", DialogueStep.Emotion.Normal);    
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("<earth>Earth<> is the element of history. Historians and archeologists uncover, record, and preserve the past from the sands of time.", "Wizard", DialogueStep.Emotion.Normal);    
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Language is dynamic. It flows and changes as the generations pass, much like the <water>element of water<> you carry.", "Wizard", DialogueStep.Emotion.Normal);    
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I'm sure you can guess what your <healing>power of healing<> is tied to.", "Wizard", DialogueStep.Emotion.Questioning);    
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("I did serve as our town's doctor, but that was mainly because I was the only one who could read medical textbooks.", DialogueStep.Emotion.Normal);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Or maybe I was the only one who cared to try.", DialogueStep.Emotion.Defeated);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking(StaticVariables.playerName + ", I think that is exactly why you are capable of weilding so many elements.", "Wizard", DialogueStep.Emotion.Normal);    
            //rebecca, i think that is exactly why you are capable of weilding the power of so many elements.
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Magic doesn't come from having knowledge, but from the pursuit of it.", "Wizard", DialogueStep.Emotion.Happy);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("And you have a passion for every discipline.", "Wizard", DialogueStep.Emotion.Normal);    
        }  
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("So that explains why there are so few magical people in the world!", DialogueStep.Emotion.Surprised);        
        //}
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("You need to find a catalyst and have the right interests for its magic too!", DialogueStep.Emotion.Surprised);        
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I'm willing to bet you are also capable of harnessing <fire>fire<>!", "Wizard", DialogueStep.Emotion.Questioning);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("You think so??", DialogueStep.Emotion.Questioning);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Would you like to find out?", "Wizard", DialogueStep.Emotion.Questioning);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Well, duh! Who wouldn't want to shoot fire from my fingertips??", DialogueStep.Emotion.Surprised);        
        }
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("I have the <fire>catalyst of fire<> here with me!", "Wizard", DialogueStep.Emotion.Happy);    
        //}  
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("What do you think, spellbook? Is it worth a try?", DialogueStep.Emotion.Normal);        
        //}
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("The book quickly spells out \"YES\". After a moment, it adds an exclamation point.", "Magic Book", DialogueStep.Emotion.Normal);    
        //}  
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("Okay, okay! I get it, you're excited.", DialogueStep.Emotion.Happy);        
        //}
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("I'll give it a go, Mustrum", DialogueStep.Emotion.Happy);        
        //}
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Spectacular!", "Wizard", DialogueStep.Emotion.Happy);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Oh, um...", "Wizard", DialogueStep.Emotion.Worried);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Would you mind if I attach some sensors to you first?", "Wizard", DialogueStep.Emotion.Questioning);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I've never witnessed someone acquiring the power of a new element before.", "Wizard", DialogueStep.Emotion.Normal);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("The data would certainly help with my studies.", "Wizard", DialogueStep.Emotion.Happy);    
        }  
        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_EnemyTalking("Several more exclamation points appear in the book's pages.", "Magic Book", DialogueStep.Emotion.Normal);    
        //}  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Oh, why not. You're not going to electrocute me or anything, I hope?", DialogueStep.Emotion.Happy);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I wouldn't dream of it, " + StaticVariables.playerName + ".", "Wizard", DialogueStep.Emotion.Normal);    
        }  
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        else if (++i == cutsceneStep){
            PlayAnimation("Wizard", "Walk");
            MoveObject("Wizard", -93, 399, 1f);
            AdvanceConditionWait(1f);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Just this one here...", "Wizard", DialogueStep.Emotion.Normal);    
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Just this one here...\nAnd this one over here...", "Wizard", DialogueStep.Emotion.Normal);    
        } 
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("How many of these things are you putting on me??", DialogueStep.Emotion.Angry);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Only a few dozen more!", "Wizard", DialogueStep.Emotion.Happy);    
        } 
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Hurry it up, these things are itchy!", DialogueStep.Emotion.Worried);        
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("And...", "Wizard", DialogueStep.Emotion.Normal);    
        } 
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("And...\nDone!", "Wizard", DialogueStep.Emotion.Happy);    
        } 
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_EnemyTalking("Now, " + StaticVariables.playerName + ". Are you ready to become a pyromancer?", "Wizard", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_PlayerTalking("Let me touch that catalyst so I can get out of these things!", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_EnemyTalking("Yes, of course!", "Wizard", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_NobodyTalking();
        }  
        //wizard holds up catalyst, animation ends on a still frame of him holding it up
        //player does reaching animation like in earth cutscene
        //screen flash happens, maybe shake, then fade
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_PlayerTalking("That was incredible! Back up, I want to test this out!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_EnemyTalking("Please, hold your fire until you're well clear of the forest! It is already struggling for survival as-is.", "Wizard", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Fair enough.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_PlayerTalking("I'm sorry for attacking you and destroying your lab.", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("That's quite alright. You just provided me with some truly invaluable data!", "Wizard", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_EnemyTalking("It might help me finally figure out how to communicate with the Quercus giganteum!", "Wizard", DialogueStep.Emotion.Happy);
        }
        //make a better ending
        else if (++i == cutsceneStep) {
            AdvanceConditionDialogue_EnemyTalking("Safe travels, " + StaticVariables.playerName, "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }




        //stuff i thought of including:
        //wizard says he has been trying to figure out how to communicate with the tree
        //wizard says he isnt 100p sure what type of magic the tree is a catalyst for, but its probably some kind of knowledge/wisdom magic
        //wizard says he isnt 100p sure what field of study the tree is a catalyst for



        //do other magic users have something they interact with to use magic?
        //you're on the right track!
        //in my century of research, i have come to believe that magical powers are contained within objects spread throughout the world.
        //i call these objects \"catalysts\"
        //your spellbook is likely the catalyst for the element of water!

        //a necromancer?
        //yes, i saw he had the necronomicon in his study
        //you must be mistaken, the necronomicon isnt real
        //that's what i thought too, but then i saw it with my own eyes!
        //i can assure you, that cyclops in no necromancer.
        //please continue telling me about these other magical people!


        /*
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Tell me, what do you know of the scientific method?", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("What? Um...", DialogueStep.Emotion.Worried);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("In Posterior Analytics, Aristotle describes it as \"something\".", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("One time I tested how different factors affected the growth of tomato plants.", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("The children liked that lesson!", DialogueStep.Emotion.Happy);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Much more than the time we cataloged the library books, anyway...", DialogueStep.Emotion.Defeated);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("I see.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You and I are not so different, Rebecca.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Ugh, you really shouldn't say things like that if you're trying not to sound like a villain.", DialogueStep.Emotion.Defeated);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You doubt my intentions still?", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("If you'd care to take a look in that broken cabinet behind you...", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("You should find all the proof you need.", "Wizard", DialogueStep.Emotion.Normal);
        }
        //player goes over to cabinet
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"Year 96. Day 118.\nExperiments with a 7kg bark sample submerged in compound yielded no results.\"", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("\"It is statistically unlikely that the corruption inhabits the organelle. The compound is five parts water to one part...\"", DialogueStep.Emotion.Questioning);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("Are these your research notes?", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_PlayerTalking("There are dozens of pages in here!", DialogueStep.Emotion.Surprised);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Thousands, actually. I've been here for a lifetime.", "Wizard", DialogueStep.Emotion.Normal);
        }
        */

        //do you have any proof you weren't just stealing the magic of the tree?

        //else if (++i == cutsceneStep){
        //    AdvanceConditionDialogue_PlayerTalking("But that doesn't exactly prove you aren't trying to steal the magic of the forest.", DialogueStep.Emotion.Normal);
        //}
        /*
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Rebecca, it's impossible to \"steal\" magic.", "Wizard", DialogueStep.Emotion.Normal);
        }
        else if (++i == cutsceneStep){
            AdvanceConditionDialogue_EnemyTalking("Where do you think magic comes from?", "Wizard", DialogueStep.Emotion.Normal);
        }
        */
        //i cant say ive ever given it much thought...
        //my magic comes from this talking book here...
        //player takes out book
        //do other magic users have something they interact with to use magic?
        //you're on the right track!
        //in my century of research, i have come to believe that magical powers are contained within objects spread throughout the world.
        //i call these objects \"catalysts\"
        //your spellbook is likely the catalyst for the element of water!
        //when someone comes into contact with such a catalyst, they gain control over the associated elemental power
        //but... i'm the only one that is able to use the powers of the book!
        //i've let some friends give it a test, with no results!
        //and i've come across a few other magical people, and none of them had a spellbook either.
        //would you mind telling me a bit about them?
        //well most recently, there's you. you live in a forest and control fire magic. you studied at the academy, and you perform scientific experiments on trees.
        //then before you, there's the cyclops in the grasslands. he threw magical rocks, spoke very slowly, and is probably a necromancer

        //a necromancer?
        //yes, i saw he had the necronomicon in his study
        //you must be mistaken, the necronomicon isnt real
        //that's what i thought too, but then i saw it with my own eyes!
        //i can assure you, that cyclops in no necromancer.

        //please continue telling me about these other magical people!
        //the cyclops had an apprentice, a blemmyae, who had his face attached to his torso. he was good at throwing rocks, but not much else.
        //then there was a retired adventurer in my hometown, by the name of Eldric. he used to have control over the wind.
        //plus there's me! i have a magical book that lets me make waves, heal the injured, and toss rocks about.
        //i've also heard the lich king uses magic, although i'm not sure what kind.
        //why does this matter anyway?
        //magic is magic, the people just use it.
        //from a century of research in the field, i can assure you that is not the case.     
        //there's something about all of these people that makes them special or worthy somehow?
        //old man eldric went on epic adventures. the cyclops and the wizard have lived a long time; i bet they've done some pretty cool stuff.
        //but that doesnt make sense! im just a librarian, and i can use magic too!
        //plus i can control three different elements, which is pretty unheard of!
        //what's special about me? i just like to read books.
        //...
        //that's it! i like to read books!
        //does magic come from reading? (player) 
        //ah! not just reading. each branch of magic is tied to a unique field of study.
        //for example, fire is the element of science and discovery. (hide player chathead)
        //great pyromancers are biologists, chemists, and physicists, who seek to understand the fundamentals of our universe.
        //earth is the element of history. Historians and archeologists uncover, record, and preserve the past from the sands of time.
        //language is dynamic. it flows and changes as the generations pass, much like the element of water you carry.
        //i'm sure you can guess what your power of healing is tied to.
        //i did serve as our town's doctor, but that was mainly because i was the only one who could read medical textbooks.
        //or maybe i was the only one who cared to try.
        //rebecca, i think that is exactly why you are capable of weilding the power of so many elements.
        //magic doesn't come from having knowledge, but from the pursuit of it.
        //and you have a passion for every discipline.
        //i'm willing to bet you are already capable of harnessing the power of fire.
        //would you like to try?
        //i have the catalyst of fire here with me!



        //if you'll allow me to go fetch it...
        //sure, sure. just remember i'll blast you if y

        //take a minute to think on it. if you'll allow me to go fetch something for a moment?
        //sure, sure. just remember i'll blast you if you're up to something
        //wizard exits stage right
        //people don't just use magic?


        //wizard returns, holding a box

        


        //wizard offers player to touch object
        //explains the objects can convey the elemental power to a person if they are sufficiently ("worthy")
        //magical objects have a kind of sentience, too. like this (fire thing).
        //and my book!
        //and also the big forest tree?
        //precisely. i haven't been able to communicate with the quercus giganteum yet, but i suspect its magic is related to (botany, agriculture?)

        //thats why magical abilities are so rare. you have to be (worthy) of the magic, and then also come into contact with the (related object)

        //not just reading. each branch of magic is tied to an academnic pursuit

        //you have a thirst for knowledge. a passion for every discipline.


        //as i pursued my scientific education, i began to develop control over fire.





        //cooperative or semi cooperative mechanics
        //luck based gameplay
        //party games
        //replayability


        //take a minute to think on it. if you'll allow me to go fetch something for a moment?
        //sure, sure. just remember i'll blast you if you're up to something
        //wizard exits stage right
        //let me think about the magical people i know...
        //well there's me, this wizard, eldric, the cyclops...
        //that torso guy is still in training, plus i've heard the lich king uses magic
        

        //magical objects have a kind of sentience, too. like this (fire thing).
        //and my book!
        //and also the big forest tree?
        //precisely. i haven't been able to communicate with the quercus giganteum yet, but i suspect its magic is related to (botany?)
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

    private void DisplayEnemyTalking(string s, EnemyData enemyData, DialogueStep.Emotion emotion, string nameOverride = ""){
        dialogueManager.ShowEnemyTalking(enemyData, emotion, nameOverride);
        dialogueManager.dialogueTextBox.text = TextFormatter.FormatString(s);

    }

    private void AdvanceConditionDialogue_EnemyTalking(string s, string enemyName, DialogueStep.Emotion emotion, string nameOverride = ""){
        advanceCondition = Cond.Click;
        DisplayEnemyTalking(s, enemyName, emotion, nameOverride);

    }

    private void DisplayEnemyTalking(string s, string enemyName, DialogueStep.Emotion emotion, string nameOverride = ""){
        DisplayEnemyTalking(s, GetAnimatorFromName(enemyName).GetComponent<EnemyData>(), emotion, nameOverride);
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
        //print("No gameObject found with the name [" + name + "]");
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


