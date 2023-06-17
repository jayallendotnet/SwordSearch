using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour{

    public GeneralSceneManager generalSceneManager;
    public DialogueManager dialogueManager;
    public RectTransform backgroundParent;
    public CutsceneData cutsceneData;
    private int currentStep;
    private CutsceneStep[] steps;
    private bool isTransitioningCutsceneImage = false;
    private List<Animator> animatedObjectsInCutscene = new List<Animator>();
    private float shakeTimer = 0f;
    public float screenShakeSegment = 0.1f;
    public GameObject emptyGameObject;
    private Animator playerAnimator;
    public int startAtStep = 0;

    
    void Start(){
        steps = cutsceneData.cutsceneSteps;
        generalSceneManager.Setup();
        dialogueManager.buttonText.text = "NEXT";
        dialogueManager.isInBattle = false;
        dialogueManager.isInOverworld = false;
        dialogueManager.isInCutscene = true;
        dialogueManager.cutsceneManager = this;
        //currentStep = 0;
        currentStep = startAtStep;
        StartCutscene();

    }

    private void StartCutscene(){
        SetCutsceneBackground(cutsceneData.startingBackground);
        dialogueManager.ClearDialogue();
        dialogueManager.SetStartingValues();
        dialogueManager.TransitionToShowing();
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, ShowCurrentCutsceneStage);
    }


    private void ShowCurrentCutsceneStage(){
        if (currentStep < steps.Length){
            switch (steps[currentStep].type){
                case (CutsceneStep.CutsceneType.PlayerTalking):
                    dialogueManager.ShowPlayerTalking(steps[currentStep].emotion);
                    dialogueManager.dialogueTextBox.text = steps[currentStep].dialogue;
                    break;
                case (CutsceneStep.CutsceneType.OtherTalking):
                    dialogueManager.ShowEnemyTalking(steps[currentStep].characterTalking, steps[currentStep].emotion);
                    dialogueManager.dialogueTextBox.text = steps[currentStep].dialogue;
                    break;
                case (CutsceneStep.CutsceneType.ChangeBackground):
                    StartCutsceneImageTransition();
                    break;
                case (CutsceneStep.CutsceneType.GoToOverworld):
                    StaticVariables.FadeOutThenLoadScene("World 1 - Grasslands");
                    break;
                case (CutsceneStep.CutsceneType.GoToTutorial):
                    print("going to tutorial");
                    StaticVariables.FadeOutThenLoadScene("Map Scene");
                    //StaticVariables.FadeOutThenLoadScene("Cutscene");
                    break;
                case (CutsceneStep.CutsceneType.GoToBattle):
                    print("going to battle");
                    break;
                case (CutsceneStep.CutsceneType.PlayAnimation):
                    PlayAnimation();
                    break;
                case (CutsceneStep.CutsceneType.ShakeScreen):
                    ShakeScreen();
                    break;
                case (CutsceneStep.CutsceneType.ShowHideObject):
                    ShowHideObject();
                    break;
            }
        }

        else{
            dialogueManager.dialogueTextBox.text = "No dialogue for this enemy, current talk step is " + currentStep;
            dialogueManager.speakerNameTetxBox.text = "WARNING";
        }

        if (steps[currentStep].advanceAutomatically)
            StaticVariables.WaitTimeThenCallFunction(steps[currentStep].advanceTime, AdvanceCutsceneStage);
    }

    private void ShowHideObject(){
        string objectName = steps[currentStep].objectName;
        bool visibility = steps[currentStep].isShowing;
        foreach (Transform t in backgroundParent){
            if (t.name == objectName)
                t.gameObject.SetActive(visibility);
        }
    }

    private void StartScreenShake(){
        shakeTimer = -screenShakeSegment;
        ShakeScreen();
    }

    private void ShakeScreen(){
        float duration = screenShakeSegment;
        shakeTimer += duration;
        if (shakeTimer >= steps[currentStep].shakeDuration){
            backgroundParent.DOAnchorPos(Vector2.zero, duration);
            return;
        }
        Vector2 newSpot = new Vector2 (StaticVariables.rand.Next(-50, 50), StaticVariables.rand.Next(-50, 50));
        backgroundParent.DOAnchorPos(newSpot, duration).OnComplete(ShakeScreen);
    }

    private void PlayAnimation (){
        string characterName = steps[currentStep].characterToAnimate;
        string animationName = steps[currentStep].animationName;

        if (characterName == "Player")
            PlayAnimationForCharacter(playerAnimator, animationName);

        foreach (Animator anim in animatedObjectsInCutscene){
            if (anim.gameObject.name == characterName)
                PlayAnimationForCharacter(anim, animationName);
        }
    }

    private void PlayAnimationForCharacter(Animator anim, string animationName){
        if (animationName != "")
            anim.Play(animationName);

        if (steps[currentStep].alsoMoveCharacter){
            float durationOfAnimation = 0;
            AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
            foreach(AnimationClip clip in clips){
                if (clip.name.Contains(animationName))
                    durationOfAnimation = clip.length;
            }
            float newPosX = steps[currentStep].newPosX;
            float newPosY = steps[currentStep].newPosY;
            if (newPosX != -12345)
                anim.transform.parent.GetComponent<RectTransform>().DOAnchorPosX(newPosX, durationOfAnimation);
            if (newPosY != -12345)
                anim.transform.parent.GetComponent<RectTransform>().DOAnchorPosY(newPosY, durationOfAnimation);
            if (steps[currentStep].changeFacing){
                anim.transform.parent.localScale = new Vector2(anim.transform.parent.localScale.x * -1, anim.transform.parent.localScale.y);
            }
        }
    }

    public void PressedButton(){
        if (isTransitioningCutsceneImage)
            return;
        if (steps[currentStep].advanceAutomatically)
            return;
        AdvanceCutsceneStage();
    }

    private void AdvanceCutsceneStage(){
        currentStep ++;
        if (currentStep >= steps.Length){
            print("reached end of cutscene");
            return;
        }
        ShowCurrentCutsceneStage();
    }

    private void StartCutsceneImageTransition(){
        isTransitioningCutsceneImage = true;
        StaticVariables.StartFadeDarken(0.5f);
        StaticVariables.WaitTimeThenCallFunction(0.5f, MidCutsceneImageTransition);
    }

    private void MidCutsceneImageTransition(){
        SetCutsceneBackground(steps[currentStep].newBackground);
        StaticVariables.StartFadeLighten(0.5f);
        StaticVariables.WaitTimeThenCallFunction(0.5f, EndCutsceneImageTransition);
    }

    private void SetCutsceneBackground(GameObject prefab){

        animatedObjectsInCutscene = new List<Animator>();
        Transform backgroundPrefab = prefab.transform.GetChild(0).transform;

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
                //Animator anim = go.GetComponent<Animator>();
                //if (anim != null)
                //    animatedObjectsInCutscene.Add(anim);
            }
        }
    }

    private void EndCutsceneImageTransition(){
        isTransitioningCutsceneImage = false;
    }

}


