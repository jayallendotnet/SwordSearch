using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour{

    public GeneralSceneManager generalSceneManager;
    public DialogueManager dialogueManager;
    public Transform backgroundParent;
    public CutsceneData cutsceneData;
    private int currentStep;
    private CutsceneStep[] steps;
    private bool isTransitioningCutsceneImage = false;
    private List<Animator> animatedObjectsInCutscene = new List<Animator>();

    
    void Start(){
        steps = cutsceneData.cutsceneSteps;
        //cutsceneImage.sprite = cutsceneData.startingImage;
        generalSceneManager.Setup();
        //StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, StartCutscene);
        dialogueManager.buttonText.text = "NEXT";
        dialogueManager.isInBattle = false;
        dialogueManager.isInOverworld = false;
        dialogueManager.isInCutscene = true;
        dialogueManager.cutsceneManager = this;
        currentStep = 0;
        StartCutscene();

    }

    private void StartCutscene(){
        //ShowCurrentCutsceneStage();
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
                    break;
                case (CutsceneStep.CutsceneType.GoToBattle):
                    print("going to battle");
                    break;
                case (CutsceneStep.CutsceneType.PlayAnimation):
                    PlayAnimation();
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

    private void PlayAnimation (){
        string characterName = steps[currentStep].characterToAnimate;
        string animationName = steps[currentStep].animationName;

        foreach (Animator anim in animatedObjectsInCutscene){
            if (anim.gameObject.name == characterName){
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
                        anim.transform.DOLocalMoveX(newPosX, durationOfAnimation);
                    if (newPosY != -12345)
                        anim.transform.DOLocalMoveY(newPosY, durationOfAnimation);
                }
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
            GameObject go = Instantiate(t.gameObject, backgroundParent);
            go.name = t.gameObject.name;
            Animator anim = go.GetComponent<Animator>();
            if (anim != null)
                animatedObjectsInCutscene.Add(anim);
        }
    }

    private void EndCutsceneImageTransition(){
        isTransitioningCutsceneImage = false;
    }

}


