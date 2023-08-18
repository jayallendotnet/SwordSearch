using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public class InteractOverlayManager : MonoBehaviour{

    
    [Header("Overworld Scene")]
    public OverworldSceneManager overworldSceneManager;

   
    [Header("Scene References")]
    public RectTransform interactOverlay;
    public RectTransform battleButton;
    public RectTransform talkButton;
    public RectTransform infoButton;
    public RectTransform cutsceneButton;
    public Text cutsceneText;
    public RectTransform backButton;
    public Text enemyNameText;
    public GameObject clickableBackground;
    public GameObject infoHighlight;
    public RectTransform fullTalkButton;
    public RectTransform scrollableInfoParent;
    public GameObject infoTextPrefab;
    public Image infoTextHider;


    [Header("Configurations")]
    public float transitionDuration = 0.5f;
    public float minHeightAboveInteractOverlay = 200;


    [HideInInspector]
    public bool isInteractOverlayShowing = false;
    [HideInInspector]
    public bool isInfoShowing = false;
    private Vector2 talkButtonStartingPos;
    private Vector2 battleButtonStartingPos;
    private Vector2 infoButtonStartingPos;
    private Vector2 backButtonStartingPos;
    private Vector2 interactOverlayStartingSize;
    private Vector2 enemyNameTextStartingPos;
    private Vector2 fullTalkButtonStartingPos;

    private bool isMovingInteractOverlay = false;
    private readonly string defaultEnemyInfo = "This  enemy  has  no  particular  weaknesses.";

    private readonly string damageKeywordColor = "9C2931";
    private readonly string waterKeywordColor = "0A95D0";


    public void Setup(){
        StartInteractOverlayHidden();
        SetStartingValues();
    }
    
    private void StartInteractOverlayHidden(){
        Vector2 pos = new Vector2(0, -interactOverlay.rect.height);
        interactOverlay.anchoredPosition = pos;
        clickableBackground.SetActive(false);
    }    

    private void SetStartingValues(){
        talkButtonStartingPos = talkButton.localPosition;
        fullTalkButtonStartingPos = fullTalkButton.localPosition;
        battleButtonStartingPos = battleButton.localPosition;
        infoButtonStartingPos = infoButton.localPosition;
        backButtonStartingPos = backButton.localPosition;
        interactOverlayStartingSize = interactOverlay.sizeDelta;
        enemyNameTextStartingPos = enemyNameText.transform.localPosition;
    }

    public void PressedBattleButton(){
        if (isMovingInteractOverlay)
            return;
        if (isInfoShowing)
            return;
        overworldSceneManager.StartBattle();
    }
   
    public void PressedTalkButton(){
        if (isMovingInteractOverlay)
            return;
        if (isInfoShowing)
            return;
        DialogueStep[] steps = overworldSceneManager.currentEnemyData.overworldDialogueSteps;
        BattleData bd = overworldSceneManager.currentPlayerSpace.battleData;
        overworldSceneManager.dialogueManager.Setup(steps, bd, true, (overworldSceneManager.currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Tutorial));
    }

    public void PressedInfoButton(){
        if (isMovingInteractOverlay)
            return;
        if (isInfoShowing)
            return;
        HideOtherButtonsBehindBack(transitionDuration);
        scrollableInfoParent.parent.parent.gameObject.SetActive(true);

        while(scrollableInfoParent.childCount > 0)
            DestroyImmediate(scrollableInfoParent.GetChild(0).gameObject);
        //foreach (Transform t in scrollableInfoParent)
        //    Destroy(t.gameObject);
        EnemyInfoText enemyInfoText = GenerateEnemyInfoText(overworldSceneManager.currentEnemyData);

        for (int i = 0; i<enemyInfoText.summary.Count; i++){
            string s = enemyInfoText.summary[i];
            string d = enemyInfoText.details[i];
            GameObject obj = Instantiate(infoTextPrefab, scrollableInfoParent);
            obj.transform.GetChild(0).GetComponent<Text>().text = s;
            obj.transform.GetChild(2).GetComponent<Text>().text = d;
        }

        if (enemyInfoText.summary.Count == 0){
            ShowDefaultEnemyInfo();
        }

        FadeInInfoText();
        clickableBackground.SetActive(false);
        AdjustHeightsForShowingInfo();

        //StaticVariables.WaitTimeThenCallFunction(0.01f, AdjustHeightsForShowingInfo);

        isInfoShowing = true;
    }

    private void ShowDefaultEnemyInfo(){
        string s = defaultEnemyInfo;
        GameObject obj = Instantiate(infoTextPrefab, scrollableInfoParent);
        obj.transform.GetChild(0).GetComponent<Text>().text = s;
        obj.transform.GetChild(1).gameObject.SetActive(false);
        obj.transform.GetChild(2).gameObject.SetActive(false);
    }


    private void FadeInInfoText(){
        Color c = infoTextHider.color;
        c.a = 1;
        Color c2 = c;
        c2.a = 0;
        infoTextHider.color = c;
        infoTextHider.DOColor(c2, transitionDuration);
    }

    private void FadeOutInfoText(){
        Color c = infoTextHider.color;
        c.a = 1;
        Color c2 = c;
        c2.a = 0;
        infoTextHider.color = c2;
        infoTextHider.DOColor(c, transitionDuration);
    }

    public void PressedCutsceneButton(){
        if (isMovingInteractOverlay)
            return;
        if (isInfoShowing)
            return;
        overworldSceneManager.StartCutscene();
    }

    private void MoveOverworldDownIfRequired(){
        //no idea how this function works lol
        interactOverlay.DOSizeDelta(interactOverlayStartingSize, transitionDuration);
        float diff = interactOverlayStartingSize.y - interactOverlay.sizeDelta.y;
        float temp = overworldSceneManager.overworldView.anchoredPosition.y + diff;
        if (temp < 0)
            temp = 0;
        if (diff < 0)
            overworldSceneManager.overworldView.DOAnchorPosY(temp, transitionDuration);
            
        clickableBackground.SetActive(false);
    }

    private void MoveOverworldUpIfRequired(){
        float playerHasToBeAbove = interactOverlay.rect.height + minHeightAboveInteractOverlay;
        float diff = overworldSceneManager.playerParent.position.y - playerHasToBeAbove;
        if (diff < 0)
            overworldSceneManager.overworldView.DOAnchorPosY(-diff, transitionDuration);
    }

    private void AdjustHeightsForShowingInfo(){
        //print(scrollableInfoParent.rect.height);
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollableInfoParent);
        //print(scrollableInfoParent.rect.height);
        float newHeight = scrollableInfoParent.rect.height + 400;
        if (newHeight > 2400)
            newHeight = 2400;
        if (newHeight < interactOverlayStartingSize.y)
            return;
        Vector2 sd = new Vector2(interactOverlay.sizeDelta.x, newHeight);
        interactOverlay.DOSizeDelta(sd, transitionDuration);

        float playerHasToBeAbove = newHeight + minHeightAboveInteractOverlay;
        float diff = overworldSceneManager.playerParent.position.y - playerHasToBeAbove;
        if (diff < 0)
            overworldSceneManager.overworldView.DOAnchorPosY(overworldSceneManager.overworldView.anchoredPosition.y -diff, transitionDuration);
    }

    private EnemyInfoText GenerateEnemyInfoText(EnemyData enemy){
        List<string> summary = new();
        List<string> details = new();
        if (enemy.isHorde){
            if (StaticVariables.fireActive){
                summary.Add("Horde enemies take more burn damage from fire spells.");
                details.Add("Burn damage from the power of fire is multiplied by the number of enemies in the horde.");
            }
            if (StaticVariables.waterActive){                
                summary.Add("Horde enemies are hit harder from flooded attacks.");
                details.Add("While the book is flooded by the power of water, attacks do +2 damage for each enemy in the horde.");
            }
            if (StaticVariables.lightningActive){
                summary.Add("Horde enemies are stunned for less time from lightning spells.");
                details.Add("The duration of the stun applied by the power of lightning is divided by the number of enemies in the horde.");
            }
        }
        if (enemy.isDraconic){
            if (StaticVariables.swordActive){
                summary.Add("This dragon takes more damage from the sword of slaying.");
                details.Add("Dragons take 5x damage from the power of the sword.");
            }
        }
        if (enemy.isHoly){
            if (StaticVariables.healActive){
                summary.Add("This creature's holy aura amplifies healing magic.");
                details.Add("The health gained by the power of healing is doubled.");
            }
            if (StaticVariables.darkActive){
                summary.Add("This creature's holy aura diminishes the power of darkness.");
                details.Add("The power of darkness deals double damage.");
            }
        }        
        if (enemy.isDark){
            if (StaticVariables.healActive){
                summary.Add("This creature's dark aura dampens healing magic.");
                details.Add("The health gained by the power of healing is halved.");
            }
            if (StaticVariables.darkActive){
                summary.Add("This creature's dark aura bolsters the power of darkness.");
                details.Add("The power of darkness deals half damage.");
            }
        }     
        if (enemy.isNearWater){
            if (StaticVariables.waterActive){
                summary.Add("The nearby river empowers flooded attacks.");
                details.Add("While the book is flooded by the power of water, attacks do +4 damage.");
            
            }
        }

        summary = TextFormatter.FormatStringList(summary);
        details = TextFormatter.FormatStringList(details);

        EnemyInfoText eit = new();
        eit.summary = summary;
        eit.details = details;

        return eit;

    }
    public void PressedBackButton(){
        if (isMovingInteractOverlay)
            return;
        if (isInfoShowing){   
            ReturnButtonsToStartingPositions(transitionDuration);  
            StaticVariables.WaitTimeThenCallFunction(transitionDuration, FinishedShowingInfo);
            FadeOutInfoText();
            MoveOverworldDownIfRequired();
            return;
        }
        if (isInteractOverlayShowing)
            HideInteractOverlay();
    }

    
    private void HideOtherButtonsBehindBack(float duration){
        talkButton.DOLocalMoveY(backButton.localPosition.y, duration);
        battleButton.DOLocalMoveY(backButton.localPosition.y, duration);
        infoButton.DOLocalMoveY(backButton.localPosition.y, duration);
        enemyNameText.transform.DOLocalMoveY(backButton.localPosition.y, duration);
        fullTalkButton.DOLocalMoveY(backButton.localPosition.y, duration);

    }
    private void ReturnButtonsToStartingPositions(float duration){
        talkButton.DOLocalMove(talkButtonStartingPos, duration);
        battleButton.DOLocalMove(battleButtonStartingPos, duration);
        infoButton.DOLocalMove(infoButtonStartingPos, duration);
        backButton.DOLocalMove(backButtonStartingPos, duration);
        enemyNameText.transform.DOLocalMove(enemyNameTextStartingPos, duration);
        fullTalkButton.DOLocalMove(fullTalkButtonStartingPos, duration);
    }

    private void FinishedShowingInfo(){
        isInfoShowing = false;
        scrollableInfoParent.parent.parent.gameObject.SetActive(false);
    }

    private void HideInteractOverlay(){
        interactOverlay.DOAnchorPosY(-interactOverlay.rect.height, transitionDuration);
        clickableBackground.SetActive(false);
        isMovingInteractOverlay = true;
        overworldSceneManager.ShowMapButton(transitionDuration);
        overworldSceneManager.overworldView.DOAnchorPosY(0, transitionDuration).OnComplete(FinishedHidingInteractOverlay);
    }

    private void FinishedHidingInteractOverlay(){
        isInteractOverlayShowing = false;
        isMovingInteractOverlay = false;
    }

    public void ShowInteractOverlay(){
        interactOverlay.DOAnchorPosY(0, transitionDuration).OnComplete(FinishedShowingInteractOverlay);
        isInteractOverlayShowing = true;
        isMovingInteractOverlay = true;
        clickableBackground.SetActive(true);
        overworldSceneManager.HideMapButton(transitionDuration);
        MoveOverworldUpIfRequired();

        SetInteractOptions();
    }

    private void FinishedShowingInteractOverlay(){
        isMovingInteractOverlay = false;
    }

    public void DisplayEnemyName(EnemyData enemy){
        enemyNameText.text = enemy.GetDisplayName().ToUpper();
    }

    public void DisplayInfoHighlightIfAppropriate(EnemyData enemy){
        EnemyInfoText infoTextData = GenerateEnemyInfoText(overworldSceneManager.currentEnemyData);
        if (infoTextData.summary.Count == 0)
            infoHighlight.SetActive(false);
        else
            infoHighlight.SetActive(true);
    }

    private void SetInteractOptions(){
        OverworldSpace.OverworldSpaceType type = overworldSceneManager.currentPlayerSpace.type;
        if ((type == OverworldSpace.OverworldSpaceType.Battle) || (type == OverworldSpace.OverworldSpaceType.Tutorial)){
            battleButton.gameObject.SetActive(true);
            talkButton.gameObject.SetActive(true);
            infoButton.gameObject.SetActive(true);
            cutsceneButton.gameObject.SetActive(false);
            scrollableInfoParent.parent.parent.gameObject.SetActive(false);
            cutsceneText.gameObject.SetActive(false);
            fullTalkButton.gameObject.SetActive(true);
            enemyNameText.gameObject.SetActive(true);
            if (type == OverworldSpace.OverworldSpaceType.Battle)
                fullTalkButton.gameObject.SetActive(false);
            if (type == OverworldSpace.OverworldSpaceType.Tutorial){
                talkButton.gameObject.SetActive(false);
                infoButton.gameObject.SetActive(false);
            }

        }
        else if (type == OverworldSpace.OverworldSpaceType.Cutscene){
            battleButton.gameObject.SetActive(false);
            talkButton.gameObject.SetActive(false);
            infoButton.gameObject.SetActive(false);
            cutsceneButton.gameObject.SetActive(true);
            scrollableInfoParent.parent.parent.gameObject.SetActive(false);
            cutsceneText.gameObject.SetActive(true);
            fullTalkButton.gameObject.SetActive(false);
            enemyNameText.gameObject.SetActive(false);

            cutsceneText.text = TextFormatter.FormatString(overworldSceneManager.currentPlayerSpace.cutsceneDescription);

        }
    }
}

public class EnemyInfoText{
    public List<string> summary;
    public List<string> details;

}


