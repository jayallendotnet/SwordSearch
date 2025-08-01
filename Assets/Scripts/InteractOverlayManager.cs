using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;
using MyBox;

public class InteractOverlayManager : MonoBehaviour{

    
    [Header("Overworld Scene")]
    public OverworldSceneManager overworldSceneManager;

   
    [Header("Scene References")]
    public RectTransform interactOverlay;
    public RectTransform battleButton;
    public RectTransform infoButton;
    public GameObject infoPresent;
    public GameObject infoAbsent;
    public GameObject infoLocked;
    public RectTransform readButton;
    public Image readingBook;
    public GameObject readingLocked;
    public RectTransform talkButton;
    public RectTransform backButton;
    public GameObject cutsceneStuff;
    public Text cutsceneTitle;
    public Text cutsceneText;
    public Text enemyNameText;
    public GameObject clickableBackground;
    public GameObject infoHighlight;
    public RectTransform scrollableInfoParent;
    public GameObject infoTextPrefab;
    public Image infoTextHider;
    public RectTransform readOptionsParent;
    public GameObject bookOptionPrefab;
    //public GameObject bookDescriptionPrefab;
    public Image readTextHider;
    public RectTransform topOfBox;

    [Header("Book Sprites")]
    public Sprite waterBook;
    public Sprite healingBook;
    public Sprite earthBook;
    public Sprite fireBook;
    public Sprite lightningBook;
    public Sprite darknessBook;
    public Sprite swordBook;


    [Header("Configurations")]
    public float transitionDuration = 0.5f;

    [HideInInspector]
    public BookData chosenWaterBook;
    [HideInInspector]
    public BookData chosenHealingBook;
    [HideInInspector]
    public BookData chosenEarthBook;
    [HideInInspector]
    public BookData chosenFireBook;
    [HideInInspector]
    public BookData chosenLightningBook;
    [HideInInspector]
    public BookData chosenDarknessBook;
    [HideInInspector]
    public BookData chosenSwordBook;


    [HideInInspector]
    public bool isInteractOverlayShowing = false;
    [HideInInspector]
    public bool isInfoShowing = false;
    [HideInInspector]
    public bool isReadShowing = false;
    private Vector2 talkButtonStartingPos;
    private Vector2 battleButtonStartingPos;
    private Vector2 infoButtonStartingPos;
    private Vector2 backButtonStartingPos;
    private Vector2 readButtonStartingPos;
    private Vector2 interactOverlayStartingSize;
    private Vector2 enemyNameTextStartingPos;
    private float topOfBoxStartingPos;
    private bool isMovingInteractOverlay = false;
    [HideInInspector]
    public bool isMovingBookDescriptions = false;
    private readonly string defaultEnemyInfo = "This  enemy  has  no  particular  weaknesses.";
    


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
        battleButtonStartingPos = battleButton.localPosition;
        infoButtonStartingPos = infoButton.localPosition;
        backButtonStartingPos = backButton.localPosition;
        readButtonStartingPos = readButton.localPosition;
        interactOverlayStartingSize = interactOverlay.sizeDelta;
        enemyNameTextStartingPos = enemyNameText.transform.localPosition;
        topOfBoxStartingPos = topOfBox.anchoredPosition.y;
    }

    public bool CanMakeBookSelection(){ //same as isinterfacebusy, except showing the books is not a limiter
        return (isMovingInteractOverlay || isInfoShowing || isMovingBookDescriptions); 
    }

    private bool IsInterfaceBusy(){
        return (isMovingInteractOverlay || isInfoShowing || isReadShowing || isMovingBookDescriptions);
    }

    public void PressedBattleButton(){
        if (IsInterfaceBusy()) return;
        overworldSceneManager.StartBattle();
    }
   
    public void PressedTalkButton(){
        if (IsInterfaceBusy()) return;
        DialogueStep[] steps = overworldSceneManager.currentEnemyData.overworldDialogueSteps;
        BattleData bd = overworldSceneManager.currentPlayerSpace.battleData;
        overworldSceneManager.dialogueManager.Setup(steps, bd, true); 
    }

    public void PressedInfoButton(){
        if (IsInterfaceBusy()) return;
        HideOtherButtonsBehindBack(transitionDuration);
        scrollableInfoParent.parent.parent.gameObject.SetActive(true);
        readOptionsParent.parent.gameObject.SetActive(false);

        while(scrollableInfoParent.childCount > 0)
            DestroyImmediate(scrollableInfoParent.GetChild(0).gameObject);
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

    private void FadeInReadText(){
        Color c = readTextHider.color;
        c.a = 1;
        Color c2 = c;
        c2.a = 0;
        readTextHider.color = c;
        readTextHider.DOColor(c2, transitionDuration);
    }

    private void FadeOutReadText(){
        Color c = readTextHider.color;
        c.a = 1;
        Color c2 = c;
        c2.a = 0;
        readTextHider.color = c2;
        readTextHider.DOColor(c, transitionDuration);

    }

    public void PressedCutsceneButton(){
        if (IsInterfaceBusy()) return;
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
        topOfBox.anchoredPosition = new(0, topOfBoxStartingPos);
    }

    private void MoveOverworldUpIfRequired(){
        float worldPosOfTopOfBox = topOfBox.position.y;
        float worldPosOfBottomOfPlayer = overworldSceneManager.playerParent.GetChild(1).position.y;
        float diff = worldPosOfBottomOfPlayer - worldPosOfTopOfBox;
        float newWorldPos = overworldSceneManager.overworldView.position.y - diff;
        if (diff < 0)
            overworldSceneManager.overworldView.DOMoveY(newWorldPos, transitionDuration);
    }

    private void AdjustHeightsForShowingInfo(){
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollableInfoParent);
        float newHeight = scrollableInfoParent.rect.height + 400;
        if (newHeight > 2400)
            newHeight = 2400;
        if (newHeight < interactOverlayStartingSize.y)
            return;
        Vector2 sd = new Vector2(interactOverlay.sizeDelta.x, newHeight);
        interactOverlay.DOSizeDelta(sd, transitionDuration);

        //set the "top of box" position to where it will be after the overlay has finished expanding, then move the overworld if necessary
        topOfBox.anchoredPosition = new(0, newHeight);
        MoveOverworldUpIfRequired();
    }

    private void AdjustHeightsForShowingRead(){
        LayoutRebuilder.ForceRebuildLayoutImmediate(readOptionsParent);
        float bookDescriptionHeight = bookOptionPrefab.GetComponent<ReadingOption>().bookDescriptionPrefab.GetComponent<RectTransform>().sizeDelta.y;
        float newHeight = readOptionsParent.rect.height + 630 + bookDescriptionHeight;
        if (newHeight < interactOverlayStartingSize.y)
            return;
        Vector2 sd = new Vector2(interactOverlay.sizeDelta.x, newHeight);
        interactOverlay.DOSizeDelta(sd, transitionDuration);

        //set the "top of box" position to where it will be after the overlay has finished expanding, then move the overworld if necessary
        topOfBox.anchoredPosition = new(0, newHeight);
        MoveOverworldUpIfRequired();
    }

    private EnemyInfoText GenerateEnemyInfoText(EnemyData enemy){
        List<string> summary = new();
        List<string> details = new();
        if (enemy.isHorde){
            summary.Add("This enemy is a horde.");
            details.Add("A horde is made up of <damage>multiple enemies<>. As you deal damage, the number of enemies left in the horde will decrease, and the horde will do <damage>less damage<> when it attacks.");
            if (StaticVariables.fireActive){
                summary.Add("Horde enemies take more <fire>burn damage<> from <fire>fire spells<>.");
                details.Add("<fire>Burn damage<> from the <fire>power of fire<> is multiplied by the number of enemies in the horde.");
            }
            if ((StaticVariables.waterActive) && (!enemy.isWaterDangerous)){                
                summary.Add("Horde enemies are hit harder from <water>flooded attacks<>.");
                details.Add("The usual <damage>+" + StaticVariables.waterFloodDamageBonus + " damage bonus<> while the book is <water>flooded<> is dealt to every enemy remaining in the horde.");
            }
            if (StaticVariables.lightningActive){
                summary.Add("Horde enemies are <lightning>stunned<> for less time from <lightning>lightning spells<>.");
                details.Add("The duration of the <lightning>stun<> applied by the <lightning>power of lightning<> is divided by the number of enemies in the horde.");
            }
        }
        if (enemy.isDraconic){
            if (StaticVariables.swordActive){
                summary.Add("This dragon takes more damage from the <sword>sword of dragonslaying<>.");
                details.Add("Dragons take <damage>5x damage<> from the <sword>power of the sword<>.");
            }
        }
        //if (enemy.isHoly){
        //    if (StaticVariables.healActive){
        //        summary.Add("This creature's holy aura amplifies <healing>healing magic<>.");
        //        details.Add("The <healing>health<> gained by the <healing>power of healing<> is <damage>doubled<>.");
        //    }
        //    if (StaticVariables.darkActive){
        //        summary.Add("This creature's holy aura diminishes the <dark>power of darkness<>.");
        //        details.Add("The <dark>power of darkness<> deals <damage>half damage<>.");
        //    }
        //}        
        //if (enemy.isDark){
        //    if (StaticVariables.healActive){
        //        summary.Add("This creature's dark aura dampens <healing>healing magic<>.");
        //        details.Add("The <healing>health<> gained by the <healing>power of healing<> is <damage>halved<>.");
        //    }
        //    if (StaticVariables.darkActive){
        //        summary.Add("This creature's dark aura bolsters the <dark>power of darkness<>.");
        //        details.Add("The <dark>power of darkness<> deals <damage>double damage<>.");
        //    }
        //}     
        if (enemy.isNearWater){
            if ((StaticVariables.waterActive) && (!enemy.isWaterDangerous)){
                summary.Add("The nearby river empowers <water>flooded attacks<>.");
                details.Add("While the book is <water>flooded<> by the <water>power of water<>, attacks do an additional <damage>+" + StaticVariables.riverDamageBonus + " damage<>.");
            
            }
        }  
        if (enemy.throwsRocks){
            if (StaticVariables.healActive){
                summary.Add("This enemy's attacks cover the spellbook in debris.");
                details.Add("Some of this enemy's attacks temporarily cover up some letters, reducing your ability to make words! The effect will dissipate over time, or you can use the <healing>power of healing<> to clear it immediately.");
            
            }
        }  
        if (enemy.isWaterDangerous){
            if (StaticVariables.waterActive){
                summary.Add("It wouldn't be safe to use <water>water spells<> here.");
                details.Add("The <water>power of water<> is not available for this battle.");
            
            }
        }  
        if (enemy.isCopycat){
            summary.Add("This enemy gets stronger with every attack you make.");
            details.Add("The enemy's <damage>damage<> builds every time you make an <damage>attack<>. Using the <healing>power of healing<> reduces the buildup.");
        }
        if (enemy.healsSelf){
            summary.Add("The enemy <damage>steals<> your health.");
            details.Add("Whenever this enemy deals <damage>damage<> to you, it <healing>heals<> an equal amount.");
        }
        if (enemy.canBurn){
            summary.Add("This enemy can <fire>burn<> the spellbook!");
            details.Add("The enemy's attacks will <fire>scorch<> some letters. You can still use a <fire>burned letter<>, but you get hurt in the process. Making an attack while the book is <water>flooded<> will <water>douse<> some flames.");
        }

        summary = TextFormatter.FormatStringList(summary);
        details = TextFormatter.FormatStringList(details);

        EnemyInfoText eit = new();
        eit.summary = summary;
        eit.details = details;

        return eit;

    }

    public void PressedReadButton(){
        if (IsInterfaceBusy()) return;

        HideOtherButtonsBehindBack(transitionDuration);
        readOptionsParent.parent.gameObject.SetActive(true);
        scrollableInfoParent.parent.parent.gameObject.SetActive(false);
        while(readOptionsParent.childCount > 0)
            DestroyImmediate(readOptionsParent.GetChild(0).gameObject);
        if (overworldSceneManager.waterActive)
            CreateBookOption(BattleManager.PowerupTypes.Water);
        if (overworldSceneManager.healActive)
            CreateBookOption(BattleManager.PowerupTypes.Heal);
        if (overworldSceneManager.earthActive)
            CreateBookOption(BattleManager.PowerupTypes.Earth);
        if (overworldSceneManager.fireActive)
            CreateBookOption(BattleManager.PowerupTypes.Fire);
        if (overworldSceneManager.lightningActive)
            CreateBookOption(BattleManager.PowerupTypes.Lightning);
        if (overworldSceneManager.darkActive)
            CreateBookOption(BattleManager.PowerupTypes.Dark);
        if (overworldSceneManager.swordActive)
            CreateBookOption(BattleManager.PowerupTypes.Sword);
        
        FadeInReadText();
        clickableBackground.SetActive(false);
        //AdjustHeightsForShowingRead();
        isReadShowing = true;
        UpdateBookSelection();
        AdjustHeightsForShowingRead();
    }

    private void CreateBookOption(BattleManager.PowerupTypes type){
        GameObject obj = Instantiate(bookOptionPrefab, readOptionsParent);
        ReadingOption ro = obj.GetComponent<ReadingOption>();
        ro.powerupType = type;
        ro.bookImage.sprite = GetBookSpriteForType(type);
        ro.transitionDuration = transitionDuration;
        ro.interactOverlayManager = this;
        if (type == BattleManager.PowerupTypes.Water)
            ro.bookData = chosenWaterBook;
        if (type == BattleManager.PowerupTypes.Heal)
            ro.bookData = chosenHealingBook;
        if (type == BattleManager.PowerupTypes.Earth)
            ro.bookData = chosenEarthBook;
        if (type == BattleManager.PowerupTypes.Fire)
            ro.bookData = chosenFireBook;
        if (type == BattleManager.PowerupTypes.Lightning)
            ro.bookData = chosenLightningBook;
        if (type == BattleManager.PowerupTypes.Dark)
            ro.bookData = chosenDarknessBook;
        if (type == BattleManager.PowerupTypes.Sword)
            ro.bookData = chosenSwordBook;
        ro.ShowBookName();
    }

    public void UpdateBookSelection(){
        foreach (Transform t in readOptionsParent){
            ReadingOption ro = t.GetComponent<ReadingOption>();
            if (ro != null){
                ro.ShowActiveIfMatchingType(StaticVariables.buffedType);
                ro.ShowOrHideDescription();
            }
        }
    }

    public void PressedBackButton(){
        if (isMovingInteractOverlay)
            return;
        if (isMovingBookDescriptions)
            return;
        if (isInfoShowing){   
            ReturnButtonsToStartingPositions(transitionDuration);  
            StaticVariables.WaitTimeThenCallFunction(transitionDuration, FinishedShowingInfo);
            FadeOutInfoText();
            MoveOverworldDownIfRequired();
            return;
        }
        if (isReadShowing){
            ReturnButtonsToStartingPositions(transitionDuration);  
            StaticVariables.WaitTimeThenCallFunction(transitionDuration, FinishedShowingRead);
            FadeOutReadText();
            MoveOverworldDownIfRequired();
            UpdateReadingButtonBook();
            return;
        }
        if (isInteractOverlayShowing)
            HideInteractOverlay();
    }

    
    private void HideOtherButtonsBehindBack(float duration){
        talkButton.DOLocalMoveY(backButton.localPosition.y, duration);
        battleButton.DOLocalMoveY(backButton.localPosition.y, duration);
        infoButton.DOLocalMoveY(backButton.localPosition.y, duration);
        readButton.DOLocalMoveY(backButton.localPosition.y, duration);
        backButton.DOLocalMoveX(battleButton.localPosition.x, duration);
        backButton.DOSizeDelta(battleButton.sizeDelta, duration);
        enemyNameText.transform.DOLocalMoveY(backButton.localPosition.y, duration);
    }
    private void ReturnButtonsToStartingPositions(float duration){
        talkButton.DOLocalMove(talkButtonStartingPos, duration);
        battleButton.DOLocalMove(battleButtonStartingPos, duration);
        infoButton.DOLocalMove(infoButtonStartingPos, duration);
        readButton.DOLocalMove(readButtonStartingPos, duration);
        backButton.DOLocalMove(backButtonStartingPos, duration);
        backButton.DOSizeDelta(readButton.sizeDelta, duration);
        enemyNameText.transform.DOLocalMove(enemyNameTextStartingPos, duration);
    }

    private void FinishedShowingInfo(){
        isInfoShowing = false;
        scrollableInfoParent.parent.parent.gameObject.SetActive(false);
    }

    private void FinishedShowingRead(){
        isReadShowing = false;
        readOptionsParent.parent.gameObject.SetActive(false);
    }

    private void HideInteractOverlay(){
        interactOverlay.DOAnchorPosY(-interactOverlay.rect.height, transitionDuration);
        clickableBackground.SetActive(false);
        isMovingInteractOverlay = true;
        overworldSceneManager.ShowSceneHeader(transitionDuration);
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
        overworldSceneManager.HideSceneHeader(transitionDuration);
        MoveOverworldUpIfRequired();
        SetInteractOptions();
    }

    private void FinishedShowingInteractOverlay(){
        isMovingInteractOverlay = false;
    }

    public void DisplayEnemyName(EnemyData enemy){
        enemyNameText.text = enemy.GetDisplayName().ToUpper();
    }

    public void ConfigureInfoButton(EnemyData enemy){
        if (StaticVariables.ShouldStageShowInfoButton(enemy)){
            infoLocked.SetActive(false);
            infoPresent.SetActive(false);
            infoAbsent.SetActive(false);
            EnemyInfoText infoTextData = GenerateEnemyInfoText(overworldSceneManager.currentEnemyData);
            if (infoTextData.summary.Count == 0)
                infoAbsent.SetActive(true);
            else
                infoPresent.SetActive(true);
        }
        else
            infoLocked.SetActive(true);
    }

    private Sprite GetBookSpriteForType(BattleManager.PowerupTypes powerupType){
        return powerupType switch{
            BattleManager.PowerupTypes.Water => waterBook,
            BattleManager.PowerupTypes.Heal => healingBook,
            BattleManager.PowerupTypes.Earth => earthBook,
            BattleManager.PowerupTypes.Fire => fireBook,
            BattleManager.PowerupTypes.Lightning => lightningBook,
            BattleManager.PowerupTypes.Dark => darknessBook,
            _ => swordBook,
        };
    }

    public void ConfigureReadButton(EnemyData enemy){
        if (StaticVariables.IsReadingEnabledForStage(enemy)){
            readingLocked.SetActive(false);
            UpdateReadingButtonBook();
        }
        else
            readingLocked.SetActive(true);
    }

    private void UpdateReadingButtonBook(){
        readingBook.sprite = GetBookSpriteForType(StaticVariables.buffedType);
    }

    private void SetInteractOptions(){
        OverworldSpace.OverworldSpaceType type = overworldSceneManager.currentPlayerSpace.type;
        if ((type == OverworldSpace.OverworldSpaceType.Battle) || (type == OverworldSpace.OverworldSpaceType.Tutorial)){
            battleButton.gameObject.SetActive(true);
            talkButton.gameObject.SetActive(true);
            infoButton.gameObject.SetActive(true);
            readButton.gameObject.SetActive(true);
            cutsceneStuff.SetActive(false);
            scrollableInfoParent.parent.parent.gameObject.SetActive(false);
            cutsceneTitle.gameObject.SetActive(false);
            cutsceneText.gameObject.SetActive(false);
            enemyNameText.gameObject.SetActive(true);
        }
        else if (type == OverworldSpace.OverworldSpaceType.Cutscene){
            battleButton.gameObject.SetActive(false);
            talkButton.gameObject.SetActive(false);
            infoButton.gameObject.SetActive(false);
            readButton.gameObject.SetActive(false);
            cutsceneStuff.SetActive(true);
            scrollableInfoParent.parent.parent.gameObject.SetActive(false);
            cutsceneTitle.gameObject.SetActive(true);
            cutsceneText.gameObject.SetActive(true);
            enemyNameText.gameObject.SetActive(false);
            cutsceneTitle.text = TextFormatter.FormatString(overworldSceneManager.currentPlayerSpace.cutsceneTitle);
            cutsceneText.text = TextFormatter.FormatString(overworldSceneManager.currentPlayerSpace.cutsceneDescription);
        }
    }
}

public class EnemyInfoText{
    public List<string> summary;
    public List<string> details;

}


