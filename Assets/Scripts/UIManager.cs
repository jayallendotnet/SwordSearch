using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour {
    private Color textColorForWord = Color.black;
    private Color backgroundColorForWord = Color.grey;
    private Transform enemyObject;
    [HideInInspector]
    public Animator enemyAnimator;
    private float waterDrainDuration;
    private Color waterPowerupStrengthColor;
    private float floodHeight;
    private bool movingBook = false;
    private List<GameObject> animatedObjectsInWindow = new List<GameObject>();

    [Header("Submit Word Button")]
    public Text wordDisplay;
    public Image submitWordButtonImage;
    public Image countdownNumber;
    public GameObject countdownDivider;
    public Image wordStrengthImageSingle;
    public Image wordStrengthImageOnes;
    public Image wordStrengthImageTens;
    public GameObject wordStrengthDivider;
    public Image wordStrengthIcon;


    [Header("Colors")]
    //public Color validWordColor;
    public Color invalidWordColor;
    //public Color validButtonColor;
    public Color invalidButtonColor;
    public Color canRefreshPuzzleColor;
    public List<PowerupDisplayData> powerupDisplayDataList;
    [Header("Numbers")]
    public Sprite[] numberSprites;

    [Header("Player and Enemy")]
    public Animator playerAnimator;
    public Transform playerObject;
    public GameObject playerHealDoubleDigitPrefab;
    public GameObject playerHealSingleDigitPrefab;
    public GameObject playerDamageDoubleDigitPrefab;
    public GameObject playerDamageSingleDigitPrefab;
    public Transform enemyParent;
    public GameObject enemyHealDoubleDigitPrefab;
    public GameObject enemyHealSingleDigitPrefab;
    public GameObject enemyDamageDoubleDigitPrefab;
    public GameObject enemyDamageSingleDigitPrefab;

    [Header("Health Display")]
    public Image playerHP3DigitHundreds;
    public Image playerHP3DigitTens;
    public Image playerHP3DigitOnes;
    public Image playerHP2DigitTens;
    public Image playerHP2DigitOnes;
    public Image playerHP1DigitOnes;
    public Image enemyHP3DigitHundreds;
    public Image enemyHP3DigitTens;
    public Image enemyHP3DigitOnes;
    public Image enemyHP2DigitTens;
    public Image enemyHP2DigitOnes;
    public Image enemyHP1DigitOnes;

    [Header("Status Bar")]
    public Transform enemyTimerBarImageParent;
    public Transform enemyTimerBar;
    public Transform enemyStunBar;
    public Animator enemyStunBarAnimation;
    public GameObject burnDisplay1;
    public GameObject burnDisplay2;
    public GameObject burnDisplay3;
    public GameObject burnDisplay4;
    public GameObject burnDisplay5;
    public GameObject pebbleDisplay1;
    public GameObject pebbleDisplay2;
    public GameObject pebbleDisplay3;
    public GameObject pebbleDisplay4;
    public GameObject pebbleDisplay5;

    [Header("Misc")]
    public BattleManager battleManager;
    public Transform playerAttackAnimationParent;
    public RectTransform waterBuffBottom;
    public RectTransform waterBuffTop;
    public float waterFillDuration = 2f;
    public float waterFloatDuration = 3f;
    public Transform backgroundParent;
    public Transform foregroundParent;
    public RectTransform book;
    public RectTransform pauseArrow;


    public void SetStartingValues(){
        waterPowerupStrengthColor = GetPowerupDisplayDataWithType(BattleManager.PowerupTypes.Water).backgroundColor;
        floodHeight = waterBuffTop.anchoredPosition.y;
        waterBuffTop.anchoredPosition = new Vector2(waterBuffTop.anchoredPosition.x, waterBuffBottom.anchoredPosition.y);
    }


    public void DisplayHealths(int playerHealth, int enemyHealth){
        if (playerHealth < 10){
            playerHP1DigitOnes.sprite = numberSprites[playerHealth];
            playerHP1DigitOnes.transform.parent.gameObject.SetActive(true);
            playerHP2DigitOnes.transform.parent.gameObject.SetActive(false);
            playerHP3DigitOnes.transform.parent.gameObject.SetActive(false);
        }
        else if (playerHealth < 100){
            Vector2 hp = GetTensAndOnes(playerHealth);
            playerHP2DigitOnes.sprite = numberSprites[(int)hp[1]];
            playerHP2DigitTens.sprite = numberSprites[(int)hp[0]];
            playerHP1DigitOnes.transform.parent.gameObject.SetActive(false);
            playerHP2DigitOnes.transform.parent.gameObject.SetActive(true);
            playerHP3DigitOnes.transform.parent.gameObject.SetActive(false);
        }
        else{
            Vector3 hp = GetHundredsTensAndOnes(playerHealth);
            playerHP3DigitOnes.sprite = numberSprites[(int)hp[2]];
            playerHP3DigitTens.sprite = numberSprites[(int)hp[1]];
            playerHP3DigitHundreds.sprite = numberSprites[(int)hp[0]];
            playerHP1DigitOnes.transform.parent.gameObject.SetActive(false);
            playerHP2DigitOnes.transform.parent.gameObject.SetActive(false);
            playerHP3DigitOnes.transform.parent.gameObject.SetActive(true);
        }

        if (enemyHealth < 10){
            enemyHP1DigitOnes.sprite = numberSprites[enemyHealth];
            enemyHP1DigitOnes.transform.parent.gameObject.SetActive(true);
            enemyHP2DigitOnes.transform.parent.gameObject.SetActive(false);
            enemyHP3DigitOnes.transform.parent.gameObject.SetActive(false);
        }
        else if (enemyHealth < 100){
            Vector2 hp = GetTensAndOnes(enemyHealth);
            enemyHP2DigitOnes.sprite = numberSprites[(int)hp[1]];
            enemyHP2DigitTens.sprite = numberSprites[(int)hp[0]];
            enemyHP1DigitOnes.transform.parent.gameObject.SetActive(false);
            enemyHP2DigitOnes.transform.parent.gameObject.SetActive(true);
            enemyHP3DigitOnes.transform.parent.gameObject.SetActive(false);
        }
        else{
            Vector3 hp = GetHundredsTensAndOnes(enemyHealth);
            enemyHP3DigitOnes.sprite = numberSprites[(int)hp[2]];
            enemyHP3DigitTens.sprite = numberSprites[(int)hp[1]];
            enemyHP3DigitHundreds.sprite = numberSprites[(int)hp[0]];
            enemyHP1DigitOnes.transform.parent.gameObject.SetActive(false);
            enemyHP2DigitOnes.transform.parent.gameObject.SetActive(false);
            enemyHP3DigitOnes.transform.parent.gameObject.SetActive(true);
        }
    }

    public void ShowPlayerTakingDamage(int amount, bool stillAlive, bool showDamageAnimation = true){
        if (amount < 1)
            return;
        ShowNumbersAsChild(playerDamageSingleDigitPrefab, playerDamageDoubleDigitPrefab, playerObject, amount);
        if (!showDamageAnimation)
            return;
        if (stillAlive)
            playerAnimator.SetTrigger("TakeDamage");
        else
            playerAnimator.SetTrigger("Die");
    }
    public void ShowPlayerGettingHealed(int amount){
        ShowNumbersAsChild(playerHealSingleDigitPrefab, playerHealDoubleDigitPrefab, playerObject, amount);
    }

    public void ShowEnemyTakingDamage(int amount, bool stillAlive){
        ShowNumbersAsChild(enemyDamageSingleDigitPrefab, enemyDamageDoubleDigitPrefab, enemyObject, amount);
  
        if (!stillAlive)
            enemyAnimator.Play(StaticVariables.GetAnimatorDieStateName(enemyAnimator));
        else if (!StaticVariables.IsAnimatorInDamageState(enemyAnimator))
            enemyAnimator.SetTrigger("TakeDamage");
    }

    public void ShowEnemyGettingHealed(int amount){
        ShowNumbersAsChild(enemyHealSingleDigitPrefab, enemyHealDoubleDigitPrefab, enemyObject, amount);
        
    }

    private void ShowNumbersAsChild(GameObject singleDigitPrefab, GameObject doubleDigitPrefab, Transform parent, int amount){
        if (amount < 10){
            GameObject obj = Instantiate(singleDigitPrefab, parent);
            obj.transform.Find("Ones").GetComponent<Image>().sprite = numberSprites[amount];
        }
        else{
            Vector2 temp = GetTensAndOnes(amount);
            GameObject obj = Instantiate(doubleDigitPrefab, parent);
            obj.transform.Find("Ones").GetComponent<Image>().sprite = numberSprites[(int)temp[1]];
            obj.transform.Find("Tens").GetComponent<Image>().sprite = numberSprites[(int)temp[0]];
        }
    }


    public void StartPlayerCastAnimation(){
        playerAnimator.SetTrigger("StartCast");
    }

    public void StartPlayerHealAnimation(){
        playerAnimator.SetTrigger("StartHeal");
    }

    public void UpdateColorsForWord(string word, BattleManager.PowerupTypes type){
        if (word.Length == 0)
            return;
        foreach (PowerupDisplayData d in powerupDisplayDataList){
            if (d.type == type){
                textColorForWord = d.textColor;
                backgroundColorForWord = d.backgroundColor;
            }
        }

    }

    public void UpdatePowerupIcon(BattleManager.PowerupTypes type){
        PowerupDisplayData d =GetPowerupDisplayDataWithType(type);
        wordStrengthIcon.sprite = d.icon;
    }

    public void UpdateVisualsForLettersInWord(List<LetterSpace> letterSpacesForWord){
        foreach (LetterSpace ls2 in letterSpacesForWord)
            ls2.ShowAsPartOfWord(textColorForWord, backgroundColorForWord);
    }

    public void DisplayWord(string word, bool isValidWord, int countdown, int strength){
        if (isValidWord){
            wordDisplay.text = word;
            wordDisplay.fontSize = 150;
            wordDisplay.color = textColorForWord;
            submitWordButtonImage.color = backgroundColorForWord;
            wordStrengthDivider.SetActive(true);
            countdownDivider.SetActive(true);
            UpdateWordStrengthDisplay(strength);
            UpdateCountdownDisplay(countdown);
        }
        else if ((countdown == 0) && (word.Length == 0)){
            wordDisplay.text = "NEW\nPUZZLE";
            wordDisplay.fontSize = 85;
            wordDisplay.color = Color.white;
            submitWordButtonImage.color = canRefreshPuzzleColor;
            wordStrengthDivider.SetActive(true);
            countdownDivider.SetActive(true);
            UpdateWordStrengthDisplay(strength);
            UpdateCountdownDisplay(countdown);
        }
        else {
            wordDisplay.text = word;
            wordDisplay.fontSize = 150;
            wordDisplay.color = invalidWordColor;
            submitWordButtonImage.color = invalidButtonColor;
            wordStrengthDivider.SetActive(false);
            countdownDivider.SetActive(false);
            UpdateWordStrengthDisplay(strength);
            UpdateCountdownDisplay(countdown);
        }
    }

    public void UpdateWordStrengthDisplay(int strength){
        if (strength < 10){
            wordStrengthImageSingle.sprite = numberSprites[strength];
            wordStrengthImageSingle.gameObject.SetActive(true);
            wordStrengthImageOnes.gameObject.SetActive(false);
            wordStrengthImageTens.gameObject.SetActive(false);
        }
        else{
            Vector2 str = GetTensAndOnes(strength);
            wordStrengthImageOnes.sprite = numberSprites[(int)str[1]];
            wordStrengthImageTens.sprite = numberSprites[(int)str[0]];
            wordStrengthImageSingle.gameObject.SetActive(false);
            wordStrengthImageOnes.gameObject.SetActive(true);
            wordStrengthImageTens.gameObject.SetActive(true);
        }
        Color numberColor = Color.white;
        if ((battleManager.isWaterInPuzzleArea) && (strength > 0))
            numberColor = waterPowerupStrengthColor;
        wordStrengthImageSingle.color = numberColor;
        wordStrengthImageOnes.color = numberColor;
        wordStrengthImageTens.color = numberColor;
    }

    private Vector2 GetTensAndOnes(int val){
        int tens = (val / 10);
        int ones = val - (tens * 10);
        if (val > 99){
            ones = 9;
            tens = 9;
        }
        return new Vector2(tens, ones);
    }    
    
    private Vector3 GetHundredsTensAndOnes(int val){
        int hundreds = val / 100;
        int tens = (val - (hundreds * 100)) / 10;
        int ones = val - ((hundreds * 100) + (tens * 10));
        if (val > 999){
            ones = 9;
            tens = 9;
            hundreds = 9;
        }
        return new Vector3(hundreds, tens, ones);
    }

    public void UpdateCountdownDisplay(int countdown){
        if (countdown > 9)
            countdown = 9;
        countdownNumber.sprite = numberSprites[countdown];
    }


    public PowerupDisplayData GetPowerupDisplayDataWithType(BattleManager.PowerupTypes t){
        foreach (PowerupDisplayData d in powerupDisplayDataList){
            if (d.type == t)
                return d;
        }
        return null;
    }

    public void StopEnemyAttackTimer(){
        enemyTimerBar.localScale = Vector3.one;
        DOTween.Kill(enemyTimerBar);
    }

    public void ActivateEnemyStunBar(float duration){
        enemyStunBar.gameObject.SetActive(true);
        enemyStunBarAnimation.gameObject.SetActive(true);
        enemyStunBar.localScale = Vector3.one;
        DOTween.Kill(enemyStunBar);
        enemyStunBar.DOScale(new Vector3(0,1,1), duration).SetEase(Ease.Linear).OnComplete(battleManager.EnemyStunWearsOff);
    }

    public void DeactivateEnemyStunBar(){
        enemyStunBar.gameObject.SetActive(false);
        enemyStunBarAnimation.SetTrigger("End");
    }

    public void StartEnemyAttackTimer(float duration){
        enemyTimerBar.localScale = Vector3.one;
        enemyTimerBar.DOScale(new Vector3(0,1,1), duration).SetEase(Ease.Linear).OnComplete(battleManager.TriggerEnemyAttack);
    }


    public void AnimateEnemyAttackBarDisappearing(){
        foreach (Transform t in enemyTimerBarImageParent){
            Image im = t.GetComponent<Image>();
            Color newColor = im.color;
            newColor.a = 0;
            im.DOColor(newColor, 1);
        }
    }

    public void StartEnemyAttackAnimation(){
        enemyAnimator.SetTrigger("Attack");
        enemyTimerBar.DOScale(Vector3.one, 1f).SetEase(Ease.Linear);
    }

    public void AddEnemyToScene(GameObject enemyPrefab){
        GameObject newEnemy = Instantiate(enemyPrefab, enemyParent);
        newEnemy.name = enemyPrefab.name;
        enemyObject = newEnemy.transform;
        enemyAnimator = newEnemy.GetComponent<Animator>();
        battleManager.enemyAttackAnimatorFunctions = enemyObject.GetComponent<EnemyAttackAnimatorFunctions>();
        battleManager.enemyAttackAnimatorFunctions.battleManager = battleManager;
        battleManager.enemyData = enemyObject.GetComponent<EnemyData>();
    }

    public void ApplyBackground(GameObject backgroundPrefab){
        animatedObjectsInWindow = new List<GameObject>();
        Transform background = backgroundPrefab.transform.GetChild(0).transform;
        Transform foreground = backgroundPrefab.transform.GetChild(2).transform;
        foreach (Transform t in backgroundParent)
            GameObject.Destroy(t.gameObject);
        foreach (Transform t in foregroundParent)
            GameObject.Destroy(t.gameObject);
        foreach(Transform t in background){
            GameObject go = Instantiate(t.gameObject, backgroundParent);
            if (go.GetComponent<Animator>() != null)
                animatedObjectsInWindow.Add(go);
        }
        foreach(Transform t in foreground){
            GameObject go = Instantiate(t.gameObject, foregroundParent);
            if (go.GetComponent<Animator>() != null)
                animatedObjectsInWindow.Add(go);
        }
    }

    public void ShowBurnCount(){
        int burns = battleManager.enemyAttackAnimatorFunctions.burnDamageQueue.Count;

        burnDisplay1.SetActive(false);
        burnDisplay2.SetActive(false);
        burnDisplay3.SetActive(false);
        burnDisplay4.SetActive(false);
        burnDisplay5.SetActive(false);
        if (burns > 0)
            burnDisplay1.SetActive(true);
        if (burns > 1)
            burnDisplay2.SetActive(true);
        if (burns > 2)
            burnDisplay3.SetActive(true);
        if (burns > 3)
            burnDisplay4.SetActive(true);
        if (burns > 4)
            burnDisplay5.SetActive(true);
    }

    public void ShowPebbleCount(){
        int pebbles = battleManager.playerAnimatorFunctions.pebblesInQueue.Count;

        pebbleDisplay1.SetActive(false);
        pebbleDisplay2.SetActive(false);
        pebbleDisplay3.SetActive(false);
        pebbleDisplay4.SetActive(false);
        pebbleDisplay5.SetActive(false);
        if (pebbles > 0)
            pebbleDisplay1.SetActive(true);
        if (pebbles > 1)
            pebbleDisplay2.SetActive(true);
        if (pebbles > 2)
            pebbleDisplay3.SetActive(true);
        if (pebbles > 3)
            pebbleDisplay4.SetActive(true);
        if (pebbles > 4)
            pebbleDisplay5.SetActive(true);
    }

    public void ThrowPebble(int damage){
        battleManager.playerAnimatorFunctions.pebblesInQueue.RemoveAt(0);
        ShowPebbleCount();
        battleManager.playerAnimatorFunctions.CreateAttackAnimation(BattleManager.PowerupTypes.Pebble, damage, 0);
    }

    public void FillPuzzleAreaWithWater(float totalDuration){
        CancelWaterDrain();
        waterDrainDuration = totalDuration - waterFillDuration - waterFloatDuration;
        waterBuffBottom.DOSizeDelta(new Vector2(waterBuffBottom.sizeDelta.x, (floodHeight - waterBuffBottom.anchoredPosition.y)), waterFillDuration);
        waterBuffTop.DOAnchorPos(new Vector2(0, floodHeight), waterFillDuration).OnComplete(FloatWater);
        waterBuffBottom.gameObject.SetActive(true);
        waterBuffTop.gameObject.SetActive(true);
    }
    
    private void FloatWater(){
        StaticVariables.WaitTimeThenCallFunction(waterFloatDuration, StartDrainingWater);
    }

    public void StartDrainingWater(){
        waterBuffBottom.DOSizeDelta(new Vector2(waterBuffBottom.sizeDelta.x, 0), waterDrainDuration).SetEase(Ease.Linear);
        waterBuffTop.DOAnchorPos(new Vector2(0, (-waterBuffBottom.sizeDelta.y + waterBuffTop.anchoredPosition.y)), waterDrainDuration).SetEase(Ease.Linear).OnComplete(DrainingComplete);
    }

    private void DrainingComplete(){
        waterBuffBottom.gameObject.SetActive(false);
        waterBuffTop.gameObject.SetActive(false);
        battleManager.WaterDrainComplete();
    }

    private void CancelWaterDrain(){
        DOTween.Kill(waterBuffBottom);
        DOTween.Kill(waterBuffTop);
    }

    public void PushedPauseButton(){
        if ((movingBook) || (battleManager.playerHealth == 0) || (battleManager.enemyHealth == 0))
            return;
        movingBook = true;
        book.DOAnchorPos(new Vector2(-book.anchoredPosition.x, book.anchoredPosition.y), 0.5f).OnComplete(MovingBookEnded);
        if (IsPuzzlePageShowing()){
            pauseArrow.DORotate(new Vector3(0,0,270), 0.5f);
            battleManager.PauseEverything();
        }
        else
            pauseArrow.DORotate(new Vector3(0,0,90), 0.5f);
    }

    private void MovingBookEnded(){
        movingBook = false;
        if (IsPuzzlePageShowing())
            battleManager.ResumeEverything();
    }

    private bool IsPuzzlePageShowing(){
        return (book.anchoredPosition.x < 0);
    }

    public void SetAllAnimationStates(bool state){
        ChangeAnimationStateIfObjectIsActive(burnDisplay1, state);
        ChangeAnimationStateIfObjectIsActive(burnDisplay2, state);
        ChangeAnimationStateIfObjectIsActive(burnDisplay3, state);
        ChangeAnimationStateIfObjectIsActive(burnDisplay4, state);
        ChangeAnimationStateIfObjectIsActive(burnDisplay5, state);
        ChangeChildAnimationStateIfObjectIsActive(pebbleDisplay1, state);
        ChangeChildAnimationStateIfObjectIsActive(pebbleDisplay2, state);
        ChangeChildAnimationStateIfObjectIsActive(pebbleDisplay3, state);
        ChangeChildAnimationStateIfObjectIsActive(pebbleDisplay4, state);
        ChangeChildAnimationStateIfObjectIsActive(pebbleDisplay5, state);
        ChangeAnimationStateIfObjectIsActive(waterBuffTop.gameObject, state);
        ChangeAnimationStateIfObjectIsActive(enemyStunBarAnimation.gameObject, state);
        ChangeAnimationStateIfObjectIsActive(battleManager.playerAnimatorFunctions.deathBubble, state);
        foreach(Transform t in playerAttackAnimationParent)
            ChangeAnimationStateIfObjectIsActive(t.gameObject, state);
        foreach(Transform t in playerAnimator.transform.parent)
            ChangeAnimationStateIfObjectIsActive(t.gameObject, state);
        foreach(Transform t in enemyAnimator.transform.parent)
            ChangeAnimationStateIfObjectIsActive(t.gameObject, state);
        foreach (GameObject go in animatedObjectsInWindow)
            ChangeAnimationStateIfObjectIsActive(go, state);
    }

    public void PauseEnemyAttackBar(){
        if (enemyStunBar.gameObject.activeSelf)
            DOTween.Pause(enemyStunBar);
        else
            DOTween.Pause(enemyTimerBar);
    }

    public void ResumeEnemyAttackBar(){
        if (enemyStunBar.gameObject.activeSelf)
            DOTween.Play(enemyStunBar);
        else
            DOTween.Play(enemyTimerBar);
    }

    public void PauseWaterDrain(){
        if (waterBuffTop.gameObject.activeSelf){
            DOTween.Pause(waterBuffBottom);
            DOTween.Pause(waterBuffTop);
        }
    }

    public void ResumeWaterDrain(){
        if (waterBuffTop.gameObject.activeSelf){
            DOTween.Play(waterBuffBottom);
            DOTween.Play(waterBuffTop);
        }
    }

    private void ChangeAnimationStateIfObjectIsActive(GameObject go, bool state){
        if(go.activeSelf)
            go.GetComponent<Animator>().enabled = state;
    }

    private void ChangeChildAnimationStateIfObjectIsActive(GameObject go, bool state){
        if(go.activeSelf)
            go.transform.GetChild(0).GetComponent<Animator>().enabled = state;
    }
}

[System.Serializable]
public class PowerupDisplayData{
    public BattleManager.PowerupTypes type;
    public Color textColor = Color.white;
    public Color backgroundColor = Color.white;
    public Sprite icon;

}
