using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour{

    private int cutsceneStep = 0;
    private enum Cond{Click, Wait, BackgroundChange};
    private Cond advanceCondition;
    public enum Cutscene{HometownIntro, HometownOutro, GrasslandsIntro, GrasslandsOutro};
    private Cutscene cutsceneID;
    private List<Animator> animatedObjectsInCutscene = new List<Animator>();
    private float shakeTimer = 0f;
    private Animator playerAnimator;
    private GameObject nextBackground;

    public GeneralSceneManager generalSceneManager;
    public DialogueManager dialogueManager;
    public RectTransform backgroundParent;
    public GameObject emptyGameObject;
    public RectTransform screenshakeTransform;

    public GameObject hometownIntroBackground1;
    public GameObject hometownIntroBackground2;
    public GameObject hometownIntroBackground3;
    public GameObject hometownOutroBackground;
    public GameObject grasslandsIntroBackground;
    public GameObject grasslandsOutroBackground1;
    public GameObject grasslandsOutroBackground2;
    

    public void Start() {
        //manually start at a later cutscene step (put 1 less than the step you actually want to start on)
        //cutsceneStep = 50;
        SetCutsceneID();
        switch (cutsceneID){
            case (Cutscene.HometownIntro):
                SetupHometownIntro();
                break;
            case (Cutscene.HometownOutro):
                SetupHometownOutro();
                break;
            case (Cutscene.GrasslandsIntro):
                SetupGrasslandsIntro();
                break;
            case (Cutscene.GrasslandsOutro):
                SetupGrasslandsOutro();
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

    private void SetupHometownIntro(){
        SetCutsceneBackground(hometownIntroBackground1);
        ToggleObject("Player", false);
        //PlayAnimation("Player", "Idle Holding Book Random Brown");
    }

    private void SetupHometownOutro(){
        SetCutsceneBackground(hometownOutroBackground);
        PlayAnimation("Redhead Woman", "Idle Back");
        PlayAnimation("Bartender", "Idle Front");
        PlayAnimation("Child 1", "Idle Front");
        PlayAnimation("Child 2", "Idle Front");
    }

    private void SetupGrasslandsIntro(){
        SetCutsceneBackground(grasslandsIntroBackground);
    }

    private void SetupGrasslandsOutro(){
        SetCutsceneBackground(grasslandsOutroBackground1);
        PlayAnimation("Player", "Idle Holding Book");
    }

    private void AdvanceCutsceneStep(){
        cutsceneStep ++;
        switch (cutsceneID){
            case (Cutscene.HometownIntro):
                DoHometownIntroStep();
                break;
            case (Cutscene.HometownOutro):
                DoHometownOutroStep();
                break;
            case (Cutscene.GrasslandsIntro):
                DoGrasslandsIntroStep();
                break;
            case (Cutscene.GrasslandsOutro):
                DoGrasslandsOutroStep();
                break;
        }
        switch (advanceCondition){
            case (Cond.Click):
                ToggleButton(true);
                break;
            default:
                ToggleButton(false);
                break;
        }
    }

    private void DoHometownIntroStep(){   
        int i = 0;
        if (++i == cutsceneStep){
            ToggleObject("Player", true);
            PlayAnimationAndMoveThenIdle("Player", "Walk", -201, 2025, 1.5f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            DisplayPlayerTalking("Ah, spring! The warm sun always puts me in a reading mood!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }    
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Take Out Book Random Brown");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Oh, who am I kidding... All weather puts me in a reading mood.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }    
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Miss " + StaticVariables.playerName + "! Miss " + StaticVariables.playerName + "!", "Child 2", DialogueStep.Emotion.Excited);
            PlayAnimationAndMoveThenIdle("Child 1", "Walk", 26, 605, 2f);
            PlayAnimationAndMoveThenIdle("Child 2", "Walk", 85, 564, 2.3f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2.3f);
        }
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Put that dumb book down! We need you to read this newspaper!!", "Child 1", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Mikey stop! You know how Miss " + StaticVariables.playerName + " feels when you insult her books!", "Child 2", DialogueStep.Emotion.Excited);
            PlayAnimation("Player", "Put Away Book Random Brown");
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("She'll never stop talking about \"the prince's manticore wears shorts\"!", "Child 2", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("That doesn't sound right, I think it's \"a pig is manlier when it snores\".", "Child 1", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I've never said anything that sounds even remotely like that nonsense before!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }  
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Maybe if you had paid attention in class you'd know it's \"the pen is mightier than the sword\"!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }  
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("And you'd be able to read that newspaper for yourselves!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }  
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Oh yeah! Read it to us! Please???", "Child 2", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Alright, fine... Let me see it...", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }     
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimation("Child 2", "Show Newspaper");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2.3f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimation("Child 2", "Idle");
            PlayAnimation("Player", "Idle Holding Newspaper");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.6f);
        }
        //else if (++i == cutsceneStep){
        //    advanceCondition = Cond.Click;
        //}    
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Ahem!\nThe headline reads,\n\"Lich King Defeated! Duskvale in Ruins!\"", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }       
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("What did you say?", "Blacksmith", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", -172, 64, 1.3f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
        }
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Click;
        }      
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("My daughter lives in Duskvale!", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman No Hat", "Walk", -419, 244, 1.3f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
        }
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Click;
        }    
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I hate the Lich King!", "Redhead Woman", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Redhead Woman", "Walk", 58, 334, 2f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2.2f);
        }
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Click;
        }    
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Keep Reading!!!", "Everyone - Hometown Intro", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"Late last night, a great hole appeared in the center of Duskvale.\"", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }  
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"Out of the hole came a burst of flame, and the city was quickly beseiged by --\"", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }     
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("ROOOOOOOAR!!!!", "Mystery Dragon", DialogueStep.Emotion.Angry);
            //PlayAnimation("Child 2", "Put Away Newspaper");
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            PlayAnimation("Red Dragon", "Fly");
            MoveObject("Red Dragon", 1200, 1377, 2f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Child 1");
            FlipDirection("Child 2");
            FlipDirection("Redhead Woman");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            MoveObject("Red Dragon", 521, 1050, 2f);
            FlipDirection("Red Dragon");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Red Dragon", "Land");
            MoveObject("Red Dragon", 521, 1000, 1.1f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.6f);
        }
        else if (++i == cutsceneStep){
            StartScreenShake(1f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"-- a group of dragons!\"", DialogueStep.Emotion.Worried);
            PlayAnimation("Player", "Put Away Newspaper");
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Ha ha ha...\nPuny humans in your simple town...", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I could eat all of you on a whim!", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Instead I come with good news! Your hated Lich King has been driven from his throne!", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("This town now belongs to the mighty Queen of Ash! Bow down to her, me, and all of dragonkind!", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("We won't be bossed around by some overgrown lizard!", "Blacksmith", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("We're not afraid of you!", "Redhead Woman", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Don't antagonize the dragon! It could kill us all!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }        
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Yes, the young lady makes quite the compelling argument.", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Allow me to give you a little taste of fear!", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Red Dragon", "Prolonged Attack - Start");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
        }
        else if (++i == cutsceneStep){
            StartCutsceneImageTransition(hometownIntroBackground2);
            advanceCondition = Cond.BackgroundChange;
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Red Dragon", "Prolonged Attack - End");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Ha ha ha... You humans are powerless!", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("What do you want from us?", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Well of course you'll be paying taxes to your new draconic rulers!", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("But it appears this commotion drew the attention of some local golbins.", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        //else if (++i == cutsceneStep){
        //    DisplayEnemyTalking("Just try to protect your homes now... Goblins!", "Red Dragon", DialogueStep.Emotion.Angry);
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            //PlayAnimationAndMoveThenIdle("Wolf Rider", "Walk", 188, 451, 2f);
            //PlayAnimationAndMoveThenIdle("Goblin 1", "Walk", -47, 348, 2f);
            //PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", 575, 348, 2f);
            //PlayAnimationAndMoveThenIdle("Goblin 3", "Walk", 391, 348, 2f);
            PlayAnimationAndMoveThenIdle("Goblin 1", "Walk", 70, -64, 2.2f);
            PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", 514, 310, 2f);
            //PlayAnimationAndMoveThenIdle("Goblin 3", "Walk", 375, 400, 2f);
            PlayAnimationAndMoveThenIdle("Goblin 3", "Walk", 446, 489, 2f);
            PlayAnimationAndMoveThenIdle("Wolf Rider", "Walk", 240, 183, 1.5f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2.5f);
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Humans! We come for your gold!", "Wolf Rider", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Well, I have other towns to visit. Try not to die!", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("The Queen of Ash has no use for weaklings!", "Red Dragon", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            //dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Red Dragon", "Fly", -1500, 1400, 2f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
        }
        //else if (++i == cutsceneStep){
        //    advanceCondition = Cond.Click;
        //}  
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Attack the invaders! Defend our homes!", "Everyone - Hometown Intro", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.2f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", 29, 130, 0.5f);
            PlayAnimationAndMoveThenIdle("Redhead Woman", "Walk", 274, 535, 1f);
            //PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", 29, 130, 1.5f);
            //PlayAnimationAndMoveThenIdle("Wolf Rider", "Walk", 57, 117, 2f);
            //PlayAnimationAndMoveThenIdle("Goblin 3", "Walk", 446, 489, 2f);
            //PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", 575, 348, 1f);
            //FlipDirection("Goblin 2");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.3f);
        }          
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", 575, 310, 0.5f);
            PlayAnimationAndMoveThenIdle("Goblin 1", "Walk", -244, 191, 1f);
            FlipDirection("Goblin 2");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
        }   
        //else if (++i == cutsceneStep){
        //    advanceCondition = Cond.Wait;
        //    WaitThenAdvance(2f);
        //}
        //else if (++i == cutsceneStep){
        //    advanceCondition = Cond.Click;
        //}  
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I don't know anything about fighting...", DialogueStep.Emotion.Worried);
            //dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            advanceCondition = Cond.Click;
        }        
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Us neither!", "Child 1", DialogueStep.Emotion.Questioning);
            //DisplayEnemyTalking("Us neither!", "Child 1", DialogueStep.Emotion.Excited);
            FlipDirection("Child 1");
            FlipDirection("Child 2");
            advanceCondition = Cond.Click;
        }        
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("We should leave the town defense to the professionals.", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }  
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("AH! The library is on fire!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("All of my precious books are burning! I have to save them!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        //else if (++i == cutsceneStep){
        //    DisplayPlayerTalking("I'll have to toss some of the books into the sacred town well... I hope the goddess doesn't mind.", DialogueStep.Emotion.Questioning);
        //    FlipDirection("Player");
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            PlayAnimation("Player", "Walk");
            MoveObject("Player", -75, 2137, 1.5f);
            //FlipDirection("Player");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
        }
        else if (++i == cutsceneStep){
            ToggleObject("Player", false);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Tossing Books").GetComponent<CutsceneBookThrow>().StartThrow();
            //ToggleObject("Tossing Books", true);
            //advanceCondition = Cond.Click;
            advanceCondition = Cond.Wait;
            WaitThenAdvance(3f);
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("She really likes her books huh...", "Child 1", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Yeah, what a weirdo.", "Child 2", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Do you think we should go help her?", "Child 1", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("No, I don't want to get smacked by a flying book today.", "Child 2", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Besides, how many books can she even have in there? She's probably maybe almost done by now.", "Child 2", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Maybe we can go find a goblin to fight!", "Child 1", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Wait, do you hear that?", "Child 2", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            StartScreenShake(3000);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Tossing Books").GetComponent<CutsceneBookThrow>().StopThrow();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.66f);
        }
        else if (++i == cutsceneStep){              
            DisplayPlayerTalking("What's happening now??", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
            ToggleObject("Player", true);
            FlipDirection("Player");
            ToggleObject("Tossing Books", false);
            //dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            PlayAnimationAndMoveThenIdle("Player", "Walk", -140, 1860, 1.5f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
        }
        else if (++i == cutsceneStep){             
            ToggleObject("Water Spray", true);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(3f);
        }
        else if (++i == cutsceneStep){             
            ToggleObject("Rain", true);
            MoveObject("Rain", -33, 630, 1.5f);
            advanceCondition = Cond.Wait;
            StopShakeScreen();
            WaitThenAdvance(5f);
        }
        else if (++i == cutsceneStep){
            StartCutsceneImageTransition(hometownIntroBackground3);
            advanceCondition = Cond.BackgroundChange;
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Water Spray", "Sustained Spray");
            DisplayPlayerTalking("There's a book coming out of the water!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        //else if (++i == cutsceneStep){
        //    DisplayEnemyTalking("Wow!!!!", "Child 1", DialogueStep.Emotion.Questioning);
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == cutsceneStep){
            //DisplayNobodyTalking();
            //dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            PlayAnimation("Water Spray", "End Spray");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.7f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Walk");
            MoveObject("Player", 300, 1860, 1.1f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.6f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Book Catch");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.37f);
        }
        else if (++i == cutsceneStep){
            ToggleObject("Water Spray", false);
            PlayAnimation("Player", "Idle Holding Book Flipped");
            DisplayPlayerTalking("Got it!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("This book is completely dry!", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I always thought this was an old empty journal...", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("But now the pages are filled with random letters!", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I bet it was invisible ink... or maybe even magic?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("And the letters react to my touch! It's definitely probably magical!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
    }
    
    private void DoHometownOutroStep(){
        int i = 0;
        if (++i == cutsceneStep){
            DisplayEnemyTalking("I saw you shooting some magic water at those goblis!", "Blacksmith", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
            //DisplayNarrator("After the invaders were driven off, the town held a celebration in the tavern.");
            //advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I didn't know you had that fighting spirit in you!", "Blacksmith", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I wouldn't call myself much of a warrior...", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Nonsense! You were incredible!", "Blacksmith", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("And the magic too! How did you do that??", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("To be honest, I'm not sure.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("One of the old library books has these weird glowing letters, and...", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("It might make more sense if you give it a try yourself!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Take Out Book");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Do you see all those jumbled random letters?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Yeah?", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Try touching them!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman No Hat", "Walk", -200, 585, 0.4f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.4f);
        }   
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman No Hat", "Walk", -117, 550, 0.4f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.4f);
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Nothing interesting happens...", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("That's weird, it still works for me. Watch!", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Cast Spell");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.33f);
        }
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Player").transform.parent.GetChild(2).gameObject.SetActive(true);
            advanceCondition = Cond.Wait;
            //WaitThenAdvance(1.25f);
            WaitThenAdvance(1.5f);
        }
        //else if (++i == cutsceneStep){
        //    advanceCondition = Cond.Wait;
        //    WaitThenAdvance(0.25f);
        //}
        else if (++i == cutsceneStep){
            GetAnimatorFromName("Player").transform.parent.GetChild(2).gameObject.SetActive(false);
            DisplayEnemyTalking("Hey! Don't start throwing magic at people!", "Short Black Man", DialogueStep.Emotion.Angry);
            FlipDirection("Short Black Man");
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Yeah, if you want to sling spells, go to the Academy!", "Yellowhead Woman", DialogueStep.Emotion.Angry);
            FlipDirection("Yellowhead Woman");
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("It's been a long time since anyone used magic around here.", "Redhead Woman", DialogueStep.Emotion.Normal);
            PlayAnimation("Redhead Woman", "Idle");
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Yeah, it's been decades since Eldric and his magic were here with us...", "Short Black Man", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Hey, I'm not dead, I'm just old!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Bald Man Blue Shirt", "Walk", 427, 471, 2f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
        }   
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("You whippersnappers are flappin' yer yappers about my magic?", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Well, you're right. Back in the old days, when I could still touch my toes, I had control over the wind!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Like you could make tornadoes and stuff?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Yes! I could make tornadoes! And stuff!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I was an adventurer! Slaying monsters, saving princesses...", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I even fought an owlbear once!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("But one day I woke up with a shake in my hands, and that was it. I couldn't use magic after that.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        //else if (++i == cutsceneStep){
        //    DisplayEnemyTalking("I even tried to go back to my old life, just being a simple chef. But I couldn't do that either.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I've read some of the library's medical textbooks. Maybe there's something in one of them that could heal your hand tremors?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Oh! My book can do some healing magic! I bet I could fix your hands with that!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Young lady, that is very kind of you. But I'm afraid we have more important matters to discuss today.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Gather 'round, everyone!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){
            //DisplayEnemyTalking("How did you do it??", "Child 1", DialogueStep.Emotion.Excited);
            FlipDirection("Shopkeeper");
            PlayAnimationAndMoveThenIdle("Shopkeeper", "Walk", -156, 129, 2.3f);
            FlipDirection("Bluehead Woman");
            PlayAnimationAndMoveThenIdle("Bluehead Woman", "Walk", 106, 114, 2f);
            PlayAnimationAndMoveThenIdle("Child 2", "Walk", 268, 172, 2.2f);
            PlayAnimationAndMoveThenIdle("Child 1", "Walk", 361, 370, 1.6f);
            PlayAnimationAndMoveThenIdle("Chef", "Walk", -284, 254, 1.6f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.8f);
        }        
        else if (++i == cutsceneStep){
            //FlipDirection("Orange Shirt Black Woman No Hat");
            PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman No Hat", "Walk", -384, 413, 0.8f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.6f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Orange Shirt Black Woman No Hat");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.2f);
        }
        //else if (++i == cutsceneStep){
            //advanceCondition = Cond.Click;
        //}
        else if (++i == cutsceneStep){
            //DisplayEnemyTalking("Don't keep any secrets to yourself!", "Short Black Man", DialogueStep.Emotion.Excited);
            FlipDirection("Shopkeeper");
            //FlipDirection("Short Black Man");
            //FlipDirection("Yellowhead Woman");
            PlayAnimationAndMoveThenIdle("Redhead Woman", "Walk", 348, 573, 2.1f);
            PlayAnimationAndMoveThenIdle("Short Black Man", "Walk", 219, 724, 2.2f);
            PlayAnimationAndMoveThenIdle("Yellowhead Woman", "Walk", 73, 803, 2.3f);
            PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", -205, 763, 0.5f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Blacksmith");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Allow me to be blunt here... If that dragon comes by again, we're toast!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        //else if (++i == cutsceneStep){
        //    DisplayEnemyTalking("He's right! And I know a thing or two about toast!", "Chef", DialogueStep.Emotion.Excited);
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("We need to do something!", "Yellowhead Woman", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            DisplayPlayerTalking("I have some information that may be helpful.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I was reading this newspaper before the dragon showed up!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }        
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Take Out Newspaper");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Here's the full article I was reading earlier: \n\"Last night a great hole appeared in the center of Duskvale.\"", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"Out of the hole came a burst of flame, and the city was quickly beseiged by a group of dragons!\"", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"The Lich King and his advisors promptly engaged the dragons in combat. Vibrant blasts of magic and jets of fire shoot the city!\"", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"After a few minutes, the dragons claimed victory, and razed the city to the ground! His Lichness has not been seen since the attack.\"", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"Local experts believe the King has been killed, or is currently searching for the famed Sword of Dragonslaying.\"", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"Other local experts believe the Sword of Dragonslaying is purely a myth, and it would be dangerous to go searching for it with dragons about.\"", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("\"A third group of local experts believe that despite the Lich King's low approval ratings, the dragon attack was not politically motivated.\"", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Well I'm not one for politics, but that bit about a dragon-killing sword sounds pretty useful right about now!", "Short Black Man", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I had a book about that sword in the library, but I think it got ruined in the dragon attack.", DialogueStep.Emotion.Defeated);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Oh don't worry about that, Miss " + StaticVariables.playerName + "!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        //else if (++i == cutsceneStep){
        //    DisplayEnemyTalking("There used to be a swordswoman in my little adventuring group. She held the Sword of Dragonslaying, but there weren't many dragons around.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I knew a swordswoman who weilded the power of that very sword!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        //else if (++i == cutsceneStep){
        //    DisplayEnemyTalking("So we sealed away the sword in a temple in the desert. It might even still be there.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("There weren't many dragons around back then, so we sealed the sword away in a temple in the desert.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("For all I know, it might just still be there!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("We have to go and get it!", "Orange Shirt Black Woman No Hat", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Let's go now!", "Blacksmith", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Which way to the desert??", "Redhead Woman", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Now don't be too hasty! It's quite a journey to get there, and they call it the \"Sunscorched Desert\" for a reason!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I can go.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("With my water magic I should be able to survive there.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I'll return with the sword, and then we can fight back against these dragon tyrants!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("If you're sure...", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I am!!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("To get to the desert, you'll have to get to the far side of the enchanted forest, beyond the grasslands to the south.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("And don't worry about us, we will make sure the town stays safe!", "Blacksmith", DialogueStep.Emotion.Excited);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
    }

    private void DoGrasslandsIntroStep(){   
        int i = 0;
        if (++i == cutsceneStep){
            DisplayPlayerTalking("What a great day to begin an adventure!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("The sun is shining, the river is flowing, and the grass is dancing in the wind!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Going out into the grasslands, just me and my... weird magical book...", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){            
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
            PlayAnimation("Player", "Take Out Book");
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("You're a magic book. Can you understand me?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("The magic book says nothing.", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Okay, well your pages fill with random letters when it's magic time. Can you say something to me with them?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("The book's pages remain empty.", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("It'd be nice to have a picnic in the warm grass, reading a book full of magical secrets. Doesn't that sound fun?", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("The magic book continues to be silent.", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Fine, be that way!" , DialogueStep.Emotion.Angry);   
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Miss "+ StaticVariables.playerName + "! Wait a moment!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }      
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Bald Man Blue Shirt", "Walk", -38, 676, 2f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
        }      
        else if (++i == cutsceneStep){
            FlipDirection("Bald Man Blue Shirt");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Are you... arguing with your book?", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){ 
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Does it count as arguing if the thing you are talking to might not even be able to hear you?" , DialogueStep.Emotion.Angry);   
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("What are you doing out here anyway?" , DialogueStep.Emotion.Questioning);   
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I wanted to thank you for offering to heal my hands and bring my magic back!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Actually, about that..." , DialogueStep.Emotion.Normal);   
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I think I should get some more practice with magic before I try it on people!" , DialogueStep.Emotion.Surprised);   
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("That's alright, I'm an old man now. I don't know if I want to weild that power again...", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I do miss cooking though! Before I started adventuring, I was the head chef for the tavern.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("When you come back from your quest, I'll take you up on your offer. You can fix up my hands and I'll cook you a mean nine-course meal!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("That sounds lovely!" , DialogueStep.Emotion.Happy);   
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Oh! But that's not why I came out here! You showed me kindness, and I wanted to give you something in return.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }
        else if (++i == cutsceneStep){ 
            advanceCondition = Cond.Wait;
            //WaitThenAdvance(1.5f);
            WaitThenAdvance(2f);
            PlayAnimation("Bald Man Blue Shirt", "Take Out Bag");
        } 
        else if (++i == cutsceneStep){
            PlayAnimation("Bald Man Blue Shirt", "Idle");
            PlayAnimation("Player", "Idle Holding Bag");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.6f);
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("This is an enchanted pouch that can carry anything and everything you put inside it!", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("I had the children fill it with every single one of your books that survived the flames.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        //else if (++i == cutsceneStep){
        //    DisplayNobodyTalking();
        //    advanceCondition = Cond.Wait;
        //    WaitThenAdvance(0.5f);
        //}    
        //else if (++i == cutsceneStep){
        //    PlayAnimation("Bald Man Blue Shirt", "Idle");
        //    PlayAnimation("Player", "Idle Holding Bag");
        //    advanceCondition = Cond.Wait;
        //    WaitThenAdvance(1.6f);
        //}
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("This is incredible! Thank you!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("You're welcome, Miss " + StaticVariables.playerName + ".", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("You have a long journey ahead of you. Best to not get bored.", "Bald Man Blue Shirt", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("You're right! I'd better get going. Goodbye, Eldric.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }    
        else if (++i == cutsceneStep){
            PlayAnimation("Player", "Put Away Bag");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.6f);
        }
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
            PlayAnimationAndMoveThenIdle("Player", "Walk", 500, 2012, 5f);
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
    }
    
    private void DoGrasslandsOutroStep(){
        int i = 0;
        if (++i == cutsceneStep){
            DisplayPlayerTalking("Alright, book.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("After that crazy fight, there's probably a lot of vague magical whatever energy in the air.", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Maybe you're feeling a little inspired to talk to me?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("For the first time, the book's pages have some clear writing on them.", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("It reads,\n\"ENTER THE CAVE\".", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("No way!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I knew it! You're a talking book! I have so many questions!!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("How old are you? Who made you? Can you see me?", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("The book's text remains unchanged.", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Great. I get to talk with an actual magical book, and you just want me to go into some dark cave instead.", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Ugh, fine. But are you sure we should go in there?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("We need to get through the forest, and there really isn't any time to waste!", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("The ink shifts around, forming new words.", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("\"GO NOW, BEFORE THE CYCLOPS AWAKENS.\"", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("You know what, you're a magic book. You can <water>summon water<>. You can <healing>heal the injured<>.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("And now you can talk!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Maybe I should just take your advice. Let's go!", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){ 
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2f);
            PlayAnimation("Player", "Walk");
            MoveObject("Player", 480, 2280, 5f);
        }
        else if (++i == cutsceneStep){
            StartCutsceneImageTransition(grasslandsOutroBackground2);
            advanceCondition = Cond.BackgroundChange;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I can't see a thing!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            dialogueManager.HideChatheads(dialogueManager.transitionDuration);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        } 
        else if (++i == cutsceneStep){ 
            advanceCondition = Cond.Wait;
            WaitThenAdvance(3f);
            PlayAnimation("Player", "Walk");
            MoveObject("Entrance", -2770, 412, 3f);
        } 
        else if (++i == cutsceneStep){
            StartScreenShake(.2f);
            DisplayEnemyTalking("Bump!", "Dark Object", DialogueStep.Emotion.Normal);
            PlayAnimation("Player", "Idle");
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            DisplayPlayerTalking("Ouch, it looks like I ran into something.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I should take a moment and let my eyes adjust...", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(3f);
            Color c = Color.black;
            c.a = 0;
            GameObject.Find("Darkness").GetComponent<Image>().DOColor(c, 10f);
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Ahh!!! A skeleton!!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Oh, it's dead.", DialogueStep.Emotion.Normal); //relief?
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Well of course it's dead, it's a skeleton!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Come to think of it, I have to be careful. It might come back to life!", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("In every fantasy adventure novel, there's always at least one reanimated skeleton!", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("And my life has certainly been looking like a fantasy adventure lately...", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("What is a skeleton doing here, anyway? The cyclops talked about ecological conservation, not dead bodies.", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("You don't accidentally leave a skeleton in your house.", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Something isn't adding up. I should take a look around.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        } 
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", -441, 1900, 1.5f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Let's see what you've been reading lately...", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Book name, book name...", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("These are history books.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("But they also cover burial practices and enbalming techniques.", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Has he been doing something to these skeletons?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("It's entirely possible that he could be a necromancer...", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("They say the Lich King used magic to make himself undead. Maybe the cyclops is trying to do the same?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        } 
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        } 
        else if (++i == cutsceneStep){
            PlayAnimationAndMoveThenIdle("Player", "Walk", 16, 1675, 2.5f);
            MoveEverythingExceptPlayer(-300, 0, 2.5f);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(2.5f);
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("There's something in the dirt here...", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Hang on a second, these are Lydian Lion coins! Some of the earliest currency in human history!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }        
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("This is an incredibly precious archeological digsite!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Maybe the cyclops is just a normal historian.", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Is this what you wanted me to see?", DialogueStep.Emotion.Questioning);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){ 
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
            PlayAnimation("Player", "Take Out Book");
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("The book reads,\n\"KEEP GOING, AND HURRY UP\".", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Are you serious? You want me to walk right past these Lydian coins?", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Herodotus, the father of history, thought Lydia was the first civilization to ever use metal coins!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("This is probably the coolest thing that I've ever seen!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Well, aside from a magical talking book anyway...", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Fine, I'll move on. But we're going to keep talking about this later!", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){ 
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Wait;
            WaitThenAdvance(3f);
            PlayAnimation("Player", "Walk");
            MoveEverythingExceptPlayer(-591, 0, 3f);
            Color c = Color.black;
            c.a = 0.45f;
            GameObject.Find("Rock Glow Darkness").GetComponent<Image>().DOColor(c, 6f);
        }
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
            PlayAnimation("Player", "Idle");
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Okay, this glowy rock must be what we're here for!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("It looks like there's a book on the table...", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("The...\nNecronomicon???", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("I didn't know that was real!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("That settles it! The cyclops is definitely a necromancer!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }        
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){ 
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
            PlayAnimation("Player", "Take Out Book");
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Alright, book! Tell me what you want me to do, and then let's get out of here!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Ink forms into new words,\n\"TOUCH THE ROCK\".", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Touch it, are you crazy?? It'd turn me into a skeleton or something!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("Words reshape on the book's pages, \n\"YOU WILL BE FINE\".", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Alright, if you're sure...  Here goes nothing!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            //dialogueManager.HideEnemyChathead(dialogueManager.transitionDuration);
            dialogueManager.HideChatheads(dialogueManager.transitionDuration);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1f);
            PlayAnimation("Player", "Book Catch");
        }
        else if (++i == cutsceneStep){
            StartScreenShake(3000);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.8f);
        }
        else if (++i == cutsceneStep){
            //MagicFlash flash = GetAnimatorFromName("Back Table").transform.GetChild(4).GetComponent<MagicFlash>();
            MagicFlash flash = GetAnimatorFromName("Magic Flash").GetComponent<MagicFlash>();
            flash.transform.parent.gameObject.SetActive(true);
            //flash.StartProcess(flash.GetComponent<Image>().color);
            flash.StartProcess(StaticVariables.earthPowerupColor);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(flash.GetTotalTime() - 1f);
        }
        else if (++i == cutsceneStep){
            StopShakeScreen();
            advanceCondition = Cond.Wait;
            PlayAnimation("Player", "Idle Holding Book");
            WaitThenAdvance(1f);
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Somehow, I'm still alive!", DialogueStep.Emotion.Surprised);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("The book's pages are empty aside from a single word,\n\"EARTH\".", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayEnemyTalking("After a moment, the letters reform to say,\n\"YOU SHOULD PROBABLY LEAVE NOW\".", "Magic Book", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("Uhh, yep! That's a great idea.", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayPlayerTalking("That cyclops could wake up any minute! Let's get out of here!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == cutsceneStep){
            DisplayNobodyTalking();
            dialogueManager.HideChatheads(dialogueManager.transitionDuration);
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){ 
            advanceCondition = Cond.Wait;
            WaitThenAdvance(1.5f);
            PlayAnimation("Player", "Put Away Book");
        }
        else if (++i == cutsceneStep){
            FlipDirection("Player");
            advanceCondition = Cond.Wait;
            WaitThenAdvance(0.5f);
        }  
        else if (++i == cutsceneStep){
            //advanceCondition = Cond.Wait;
            //WaitThenAdvance(2f);
            PlayAnimationAndMoveThenIdle("Player", "Walk", -536, 1675, 5f);
            MoveEverythingExceptPlayer(1000, 0, 5f);
            //Color c = Color.black;
            //c.a = 0f;
            //GameObject.Find("Rock Glow Darkness").GetComponent<Image>().DOColor(c, 5f);
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }
        else if (++i == cutsceneStep){
            StaticVariables.hasCompletedStage = true;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }   
    }

    private void MoveEverythingExceptPlayer(float xDistance, float yDistance, float duration){
        foreach (Transform t in backgroundParent){
            if (t.name != "Player (Overworld)"){
                t.DOLocalMove(new Vector2(t.localPosition.x + xDistance, t.localPosition.y + yDistance), duration);
            }
        }
    }

    private void WaitThenAdvance(float duration){
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
    
    private void DisplayPlayerTalking(string s, DialogueStep.Emotion emotion){
        dialogueManager.ShowPlayerTalking(emotion);
        dialogueManager.dialogueTextBox.text = TextFormatter.FormatString(s);
    }

    private void DisplayEnemyTalking(string s, EnemyData enemyData, DialogueStep.Emotion emotion){
        dialogueManager.ShowEnemyTalking(enemyData, emotion);
        dialogueManager.dialogueTextBox.text = TextFormatter.FormatString(s);

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
        print("No animator found with the name [" + name + "]");
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
        GetAnimatorFromName(name).transform.parent.gameObject.SetActive(enabled);
    }
    
    private void StartScreenShake(float duration){
        shakeTimer = duration;
        ShakeScreen();
    }

    private void ShakeScreen(){
        float singleShakeDuration = 0.15f;
        shakeTimer -= singleShakeDuration;
        if (shakeTimer <= 0){
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
        GetAnimatorFromName(objectName).transform.parent.GetComponent<RectTransform>().DOAnchorPos(new Vector2(xPos, yPos), duration);
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
        Transform backgroundPrefab = prefab.transform.GetChild(1).transform;

        foreach (Transform t in backgroundParent)
            Destroy(t.gameObject);
        foreach(Transform t in backgroundPrefab){

            Animator a = t.gameObject.GetComponent<Animator>();
            if (a != null){
                GameObject parent = Instantiate(emptyGameObject, backgroundParent);
                parent.transform.localPosition = t.localPosition;
                //parent.transform.localRotation = t.localRotation;
                parent.name = t.name;
                parent.SetActive(t.gameObject.activeSelf);
                GameObject go = Instantiate(t.gameObject, parent.transform.position, Quaternion.identity, parent.transform);
                go.name = t.name;
                parent.transform.localRotation = t.localRotation;
                go.SetActive(true);
                animatedObjectsInCutscene.Add(go.GetComponent<Animator>());
            }
            else{
                GameObject go = Instantiate(t.gameObject, backgroundParent);
                go.name = t.gameObject.name;
                if (t.name.Contains("Player"))
                    playerAnimator = go.transform.GetChild(0).GetComponent<Animator>();
            }
        }
    }

    private void EndCutsceneImageTransition(){
    }

}


