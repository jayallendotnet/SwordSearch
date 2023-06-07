using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CutsceneManager : MonoBehaviour{

    public GeneralSceneManager generalSceneManager;
    public DialogueManager dialogueManager;
    public Image cutsceneImage;
    public CutsceneData cutsceneData;
    private int currentStep;
    private CutsceneStep[] steps;
    private bool isTransitioningCutsceneImage = false;

    
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
        SetCutsceneBackground(cutsceneData.startingImage);
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
                    dialogueManager.dialogueTextBox.text = steps[currentStep].description;
                    break;
                case (CutsceneStep.CutsceneType.OtherTalking):
                    dialogueManager.ShowEnemyTalking(steps[currentStep].characterTalking, steps[currentStep].emotion);
                    dialogueManager.dialogueTextBox.text = steps[currentStep].description;
                    break;
                case (CutsceneStep.CutsceneType.ChangeImage):
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
                    PlayAnimation(steps[currentStep].description);
                    break;
            }
        }

        else{
            dialogueManager.dialogueTextBox.text = "No dialogue for this enemy, current talk step is " + currentStep;
            dialogueManager.speakerNameTetxBox.text = "WARNING";
        }
    }

    private void PlayAnimation (string description){
        print("playing animation titled: " + description);
    }

    public void PressedButton(){
        if (isTransitioningCutsceneImage)
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
        SetCutsceneBackground(steps[currentStep].newImage);
        StaticVariables.StartFadeLighten(0.5f);
        StaticVariables.WaitTimeThenCallFunction(0.5f, EndCutsceneImageTransition);
    }

    private void SetCutsceneBackground(Sprite image){
        cutsceneImage.sprite = image;

    }

    private void EndCutsceneImageTransition(){
        isTransitioningCutsceneImage = false;
    }


}


