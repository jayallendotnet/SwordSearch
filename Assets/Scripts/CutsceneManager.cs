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
    public enum Cutscene{Opening, SavedTown};
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

    public GameObject openingBackground1;
    public GameObject openingBackground2;
    public GameObject openingBackground3;
    

    public void Start() {
        //manually start at a later cutscene step (put 1 less than the step you actually want to start on)
        //cutsceneStep = 48;
        SetCutsceneID();
        switch (cutsceneID){
            case (Cutscene.Opening):
                SetupOpening();
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

    private void SetupOpening(){
        SetCutsceneBackground(openingBackground1);

    }

    private void AdvanceCutsceneStep(){
        cutsceneStep ++;
        switch (cutsceneID){
            case (Cutscene.Opening):
                DoOpeningStep();
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

    private void DoOpeningStep(){    
        switch (cutsceneStep){
            case (1):
                DisplayEnemyTalking("Miss Player! Miss Player!", "Child 1", DialogueStep.Emotion.Excited);
                PlayAnimationAndMoveThenIdle("Child 1", "Walk", 75, 449, 2f);
                PlayAnimationAndMoveThenIdle("Child 2", "Walk", 134, 408, 2.3f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(2.3f);
                break;
            case (2):
                advanceCondition = Cond.Click;
                break;
            case (3):
                DisplayPlayerTalking("What is it, children?", DialogueStep.Emotion.Happy);
                advanceCondition = Cond.Click;
                break;
            case (4):
                DisplayEnemyTalking("A newspaper just came in! Look!", "Child 2", DialogueStep.Emotion.Excited);
                PlayAnimation("Child 2", "Show Newspaper");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(3.2f);
                break;
            case (5):
                advanceCondition = Cond.Click;
                break;
            case (6):
                DisplayEnemyTalking("Can you read it? It looks important!", "Child 1", DialogueStep.Emotion.Excited);
                advanceCondition = Cond.Click;
                break;
            case (7):
                DisplayEnemyTalking("You're the only person in the whole town who knows how to read!", "Child 2", DialogueStep.Emotion.Excited);
                advanceCondition = Cond.Click;
                break;
            case (8):
                DisplayPlayerTalking("This does look important...", DialogueStep.Emotion.Worried);
                advanceCondition = Cond.Click;
                break;
            case (9):
                DisplayPlayerTalking("'Kingdom under new rule! The Lich King has been driven into hiding...'", DialogueStep.Emotion.Normal);
                advanceCondition = Cond.Click;
                break;
            case (10):
                DisplayPlayerTalking("Well that's great! I never liked His Lichness anyway.", DialogueStep.Emotion.Happy);
                advanceCondition = Cond.Click;
                break;
            case (11):
                DisplayPlayerTalking("There's more here...\n'The king has been driven away by --'", DialogueStep.Emotion.Questioning);
                advanceCondition = Cond.Click;
                break;
            case (12):
                DisplayEnemyTalking("ROOOOOOOAR!!!!", "Mystery Dragon", DialogueStep.Emotion.Angry);
                PlayAnimation("Child 2", "Put Away Newspaper");
                advanceCondition = Cond.Click;
                break;
            case (13):
                PlayAnimation("Red Dragon", "Fly");
                MoveObject("Red Dragon", 1200, 1177, 2f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(1f);
                break;
            case (14):
                FlipDirection("Child 1");
                FlipDirection("Child 2");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(1f);
                break;
            case (15):
                MoveObject("Red Dragon", 521, 850, 2f);
                FlipDirection("Red Dragon");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(2f);
                break;
            case (16):
                PlayAnimation("Red Dragon", "Land");
                MoveObject("Red Dragon", 521, 800, 1.1f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(0.6f);
                break;
            case (17):
                StartScreenShake(1f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(1f);
                break;
            case (18):
                DisplayPlayerTalking("'-- a group of dragons!'", DialogueStep.Emotion.Worried);
                advanceCondition = Cond.Click;
                break;
            case (19):
                DisplayEnemyTalking("Humans...\nTiny, tasty, delectable humans...", "Red Dragon", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (20):
                DisplayEnemyTalking("Come out little humans, and meet your fate!", "Red Dragon", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (21):
                PlayAnimationAndMoveThenIdle("Blacksmith", "Walk", -562, 310, 1.3f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(0.2f);
                break;
            case (22):
                PlayAnimationAndMoveThenIdle("Redhead Woman", "Walk", -493, 740, 1.3f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(0.1f);
                break;
            case (23):
                PlayAnimationAndMoveThenIdle("Orange Shirt Black Woman", "Walk", 196, 859, 1.3f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(1.3f);
                break;
            case (24):
                FlipDirection("Orange Shirt Black Woman");
                DisplayEnemyTalking("This town now belongs to the mighty King Dragon! Bow down to him and all of dragonkind!", "Red Dragon", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (25):
                DisplayEnemyTalking("Surrender all of your valuables, or be brought to ruin!", "Red Dragon", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (26):
                DisplayEnemyTalking("No! We won't be bossed around by some overgrown lizard!", "Blacksmith", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (27):
                DisplayEnemyTalking("We're not afraid of you!", "Redhead Woman", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (28):
                DisplayEnemyTalking("You're not afraid of me? Let me give you a little taste of fear!", "Red Dragon", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (29):
                PlayAnimation("Red Dragon", "Prolonged Attack - Start");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(1.5f);
                break;
            case (30):StartCutsceneImageTransition(openingBackground2);
                advanceCondition = Cond.BackgroundChange;
                break;
            case (31):
                PlayAnimation("Red Dragon", "Prolonged Attack - End");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(1.5f);
                break;
            case (32):
                DisplayEnemyTalking("Ha ha ha... You humans are powerless!", "Red Dragon", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (33):
                DisplayEnemyTalking("Just try to protect your homes now... Goblins!", "Red Dragon", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (34):
                PlayAnimationAndMoveThenIdle("Wolf Rider", "Walk", 575, 454, 2f);
                PlayAnimationAndMoveThenIdle("Goblin 1", "Walk", 316, 368, 2f);
                PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", 366, 204, 2f);
                PlayAnimationAndMoveThenIdle("Goblin 3", "Walk", 396, 540, 2f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(2f);
                break;
            case (35):
                DisplayEnemyTalking("Yes master?", "Wolf Rider", DialogueStep.Emotion.Normal);
                advanceCondition = Cond.Click;
                break;
            case (36):
                DisplayEnemyTalking("Kill them! And take everything they have!", "Red Dragon", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (37):
                DisplayEnemyTalking("Ha ha ha!!!", "Red Dragon", DialogueStep.Emotion.Angry);
                PlayAnimationAndMoveThenIdle("Red Dragon", "Fly", -1500, 1400, 2f);
                PlayAnimationAndMoveThenIdle("Goblin 1", "Walk", -324, 688, 2f);
                PlayAnimationAndMoveThenIdle("Goblin 2", "Walk", -366, 260, 2f);
                PlayAnimationAndMoveThenIdle("Goblin 3", "Walk", 396, 804, 2f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(2f);
                break;
            case (38):
                DisplayPlayerTalking("I don't know anything about fighting... I'll have to leave the town defense to the professionals.", DialogueStep.Emotion.Worried);
                advanceCondition = Cond.Click;
                break;
            case (39):
                DisplayPlayerTalking("AH! The library is on fire!", DialogueStep.Emotion.Angry);
                advanceCondition = Cond.Click;
                break;
            case (40):
                DisplayPlayerTalking("All of our precious books are burning!", DialogueStep.Emotion.Worried);
                advanceCondition = Cond.Click;
                break;
            case (41):
                DisplayPlayerTalking("I'll have to toss some of the books into the sacred town well... I hope the goddess doesn't mind.", DialogueStep.Emotion.Questioning);
                FlipDirection("Player");
                advanceCondition = Cond.Click;
                break;
            case (42):
                PlayAnimation("Player", "Walk");
                MoveObject("Player", -33, 2023, 1.5f);
                FlipDirection("Player");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(1.5f);
                break;
            case (43):
                ToggleObject("Player", false);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(0.5f);
                break;
            case (44):
                ToggleObject("Tossing Books", true);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(5f);
                break;
            case (45):
                StartScreenShake(1.66f + 8 + 0.5f - 0.15f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(1.66f);
                break;
            case (46):                
                DisplayPlayerTalking("What's happening now??", DialogueStep.Emotion.Questioning);
                ToggleObject("Player", true);
                ToggleObject("Tossing Books", false);
                PlayAnimationAndMoveThenIdle("Player", "Walk", -138, 1735, 1.5f);
                FlipDirection("Player");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(2f);
                break;
            case (47):                
                ToggleObject("Water Spray", true);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(3f);
                break;
            case (48):                
                ToggleObject("Rain", true);
                MoveObject("Rain", -33, 630, 1.5f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(3f);
                break;
            case (49):StartCutsceneImageTransition(openingBackground3);
                advanceCondition = Cond.BackgroundChange;
                break;
            case (50):
                PlayAnimation("Water Spray", "Sustained Spray");
                DisplayPlayerTalking("There's a book coming out of the water!", DialogueStep.Emotion.Happy);
                advanceCondition = Cond.Click;
                break;
            case (51):
                PlayAnimation("Water Spray", "End Spray");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(0.7f);
                break;
            case (52):
                PlayAnimation("Player", "Walk");
                MoveObject("Player", 300, 1735, 1.1f);
                advanceCondition = Cond.Wait;
                WaitThenAdvance(0.6f);
                break;
            case (53):
                PlayAnimation("Player", "Book Catch");
                advanceCondition = Cond.Wait;
                WaitThenAdvance(0.67f);
                break;
            case (54):
                ToggleObject("Water Spray", false);
                DisplayPlayerTalking("Got it!", DialogueStep.Emotion.Happy);
                advanceCondition = Cond.Click;
                break;
            case (55):
                DisplayPlayerTalking("Hey, I remember this book! It was always empty, and no ink would stick to the pages...", DialogueStep.Emotion.Questioning);
                advanceCondition = Cond.Click;
                break;
            case (56):
                DisplayPlayerTalking("But now the pages have writing in them?", DialogueStep.Emotion.Questioning);
                advanceCondition = Cond.Click;
                break;
            case (57):
                DisplayPlayerTalking("And the letters react to my touch!", DialogueStep.Emotion.Happy);
                advanceCondition = Cond.Click;
                break;
            case (58):
                StaticVariables.currentBattleWorld = 0;
                StaticVariables.currentBattleLevel = 1;
                StaticVariables.beatCurrentBattle = true;
                StaticVariables.FadeOutThenLoadScene(StaticVariables.GetCurrentWorldName());
                break;
        }
    }

    private void WaitThenAdvance(float duration){
        StaticVariables.WaitTimeThenCallFunction(duration, AdvanceCutsceneStep);
    }

    
    

    private void ToggleButton(bool value){
        dialogueManager.buttonText.transform.parent.gameObject.SetActive(value);
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
        dialogueManager.dialogueTextBox.text = s;
    }

    private void DisplayEnemyTalking(string s, EnemyData enemyData, DialogueStep.Emotion emotion){
        dialogueManager.ShowEnemyTalking(enemyData, emotion);
        dialogueManager.dialogueTextBox.text = s;

    }

    private void DisplayEnemyTalking(string s, string enemyName, DialogueStep.Emotion emotion){
        DisplayEnemyTalking(s, GetAnimatorFromName(enemyName).GetComponent<EnemyData>(), emotion);
    }

    private Animator GetAnimatorFromName(string name){
        if (name == "Player")
            return playerAnimator;
        foreach (Animator anim in animatedObjectsInCutscene){
            if (anim.gameObject.name == name)
                return anim;
        }
        print("No animator found with the name [" + name + "]");
        return null;
    }
    

    private void ButtonText(string s){
        dialogueManager.buttonText.text = s;
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
                parent.name = t.name;
                parent.SetActive(t.gameObject.activeSelf);
                GameObject go = Instantiate(t.gameObject, parent.transform.position, Quaternion.identity, parent.transform);
                go.name = t.name;
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


