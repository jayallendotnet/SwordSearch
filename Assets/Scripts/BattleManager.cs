using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

    private string word = "";
    
    [HideInInspector]
    public List<LetterSpace> letterSpacesForWord = new List<LetterSpace>(){};

    private LetterSpace lastLetterSpace = null;
    private LetterSpace secondToLastLetterSpace = null;

    [HideInInspector]
    public BattleManager.PowerupTypes powerupTypeForWord;
    [HideInInspector]
    public int powerupLevel;
    [HideInInspector]
    public int enemyHealth = 0;
    [HideInInspector]
    public int hordeStartingHealth = 0;
    [HideInInspector]
    public int startingHordeEnemyCount = 0;
    [HideInInspector]
    public int currentHordeEnemyCount = 0;
    [HideInInspector]
    public EnemyData firstEnemyInHorde;
    [HideInInspector]
    public int playerHealth = 0;
    private string[] wordLibraryForChecking;
    private int countdownToRefresh;
    public enum PowerupTypes{None, Water, Fire, Heal, Dark, Earth, Lightning, Pebble, Sword};
    private bool isValidWord = false;
    private int wordStrength = 0;
    private bool hasSwipedOffALetter = false;
    private bool waitingForEnemyAttackToFinish = false;
    private bool stopNextAttack = false;
    [HideInInspector]
    public EnemyData enemyData;
    [HideInInspector]
    public EnemyAttackAnimatorFunctions enemyAttackAnimatorFunctions;
    [HideInInspector]
    public List<EnemyAttackAnimatorFunctions> enemyHordeAttackAnimatorFunctions;
    [HideInInspector]
    public bool isWaterInPuzzleArea = false;
    private bool isGamePaused = false;


    [Header("Game Variables")]
    public int startingPlayerHealth = 30;
    public int maxHealth = 999; //for display purposes
    public int powerupsPerPuzzle = 4;
    public int minCheckingWordLength = 3;
    public int maxPuzzleCountdown = 3;
    public int selfDamageFromDarkAttack = 5;
    public int burnDurationFromFireAttack = 5;
    public float timeBetweenBurnHits = 3f;
    public int pebbleCountFromEarthAttack = 3;
    public float pebbleDamageMultiplier = 0.33f;
    public float lightningStunDuration = 15f;
    public float darkPowerupDamageMultiplier = 2.5f;
    public float waterFloodDuration = 10f;
    public int waterFloodDamageBonus = 1;
    public float swordPowerupDamageMultiplier = 2f;
    public float swordPowerupDamageMultiplierVsDragons = 5f;
    public BattleData defaultBattleData;

    [Header("Scripts")]
    public UIManager uiManager;
    public PuzzleGenerator puzzleGenerator;
    public PlayerAnimatorFunctions playerAnimatorFunctions;
    public GeneralSceneManager setup;

    [Header("Libraries")]
    public TextAsset wordLibraryForGenerationFile; //all words that can be used to generate the puzzle
    public TextAsset wordLibraryForCheckingFile; //all words that can be considered valid, even if they are not in the generating list
    public TextAsset randomLetterPoolFile;

    void Start(){
        setup.Setup();
        wordLibraryForChecking = wordLibraryForCheckingFile.text.Split("\r\n");
        countdownToRefresh = maxPuzzleCountdown;
        if (StaticVariables.battleData == null)
            StaticVariables.battleData = defaultBattleData;
        uiManager.AddEnemyToScene(StaticVariables.battleData.enemyPrefab);
        if (enemyData.isHorde){
            startingHordeEnemyCount = enemyHordeAttackAnimatorFunctions.Count;
            currentHordeEnemyCount = startingHordeEnemyCount;
            firstEnemyInHorde = enemyHordeAttackAnimatorFunctions[0].GetComponent<EnemyData>();
            hordeStartingHealth = firstEnemyInHorde.startingHealth * startingHordeEnemyCount;
            enemyHealth = hordeStartingHealth;
        }
        else{
            enemyHealth = enemyData.startingHealth;
        }
        playerHealth = startingPlayerHealth;
        uiManager.ApplyBackground(StaticVariables.battleData.backgroundPrefab);

        uiManager.SetStartingValues();
        uiManager.DisplayHealths(playerHealth, enemyHealth);
        uiManager.DisplayWord(word, isValidWord, countdownToRefresh, wordStrength);
        StaticVariables.FadeIntoScene();
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, QueueEnemyAttack);
    }
    
    public void SetIsValidWord(){
        if (word.Length < minCheckingWordLength)
            isValidWord = false;
        else
            isValidWord = SearchLibraryForWord(word);
    }

    private bool SearchLibraryForWord(string word){
        //returns true if the library contains the word
        int result = System.Array.BinarySearch<string>(wordLibraryForChecking, word.ToLower());
        return (result > -1);
    }

    public void CalcWordStrength(){
        if (word.Length < minCheckingWordLength)
            wordStrength = 0;
        else{
            wordStrength =  Mathf.FloorToInt(Mathf.Pow((word.Length - 2), 2));
            if (isWaterInPuzzleArea)
                wordStrength += waterFloodDamageBonus;
        }
    }

    public void DamageEnemyHealth(int amount){
        enemyHealth -= amount;
        if (enemyHealth < 0)
            enemyHealth = 0;
        CalculateHordeEnemiesLeft();
        uiManager.ShowEnemyTakingDamage(amount, enemyHealth > 0);
        uiManager.DisplayHealths(playerHealth, enemyHealth);
        if (enemyHealth == 0){
            stopNextAttack = true;
            uiManager.PauseEnemyAttackBar();
            uiManager.PauseWaterDrain();
            uiManager.PausePageTurn();
            //uiManager.SetAllAnimationStates(false);
            ClearWord(false);
        }
    }

    private void CalculateHordeEnemiesLeft(){
        if (enemyHealth == 0){
            currentHordeEnemyCount = 0;
            return;
        }
        currentHordeEnemyCount = 1;
        for (int i = 1; i < startingHordeEnemyCount; i++){
            if (enemyHealth > i * firstEnemyInHorde.startingHealth)
                currentHordeEnemyCount ++;
        }
    }

    private void DamagePlayerHealth(int amount){
        playerHealth -= amount;
        if (playerHealth < 0)
            playerHealth = 0;
        uiManager.ShowPlayerTakingDamage(amount, playerHealth > 0);
        uiManager.DisplayHealths(playerHealth, enemyHealth);
        if (playerHealth == 0){
            uiManager.PauseEnemyAttackBar();
            ClearWord(false);
        }
            
    }

    public void PressSubmitWordButton(){
        if ((playerHealth == 0) || (enemyHealth == 0) || (isGamePaused))
            return;
        if (isValidWord){
            bool startNow = false;
            if (enemyData.isHorde)
                startNow = uiManager.enemyHordeAnimators[0].GetCurrentAnimatorStateInfo(0).IsName("Idle");
            else
                startNow = uiManager.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
            if (startNow)
                StartPlayingPlayerAttackAnimation(powerupTypeForWord);
            else
                PlayPlayerAttackAnimationAfterEnemyFinishes();
            SetCurrentAttackData();
            DecrementRefreshPuzzleCountdown();
            ClearWord(true);

        }
        else if ((word.Length == 0) && (countdownToRefresh == 0)){
            puzzleGenerator.GenerateNewPuzzle();
            countdownToRefresh = maxPuzzleCountdown;
            ClearWord(true);  
            uiManager.ShowPageTurn();         
        }
    }

    public void PauseEverything(){
        isGamePaused = true;
        uiManager.PauseEnemyAttackBar();
        uiManager.PauseWaterDrain();
        uiManager.SetAllAnimationStates(false);
    }


    public void ResumeEverything(){
        isGamePaused = false;
        uiManager.ResumeEnemyAttackBar();
        uiManager.ResumeWaterDrain();
        uiManager.SetAllAnimationStates(true);
    }

    private void StartPlayingPlayerAttackAnimation(PowerupTypes type){
        uiManager.PauseEnemyAttackBar();
        if (type == PowerupTypes.Heal)
            uiManager.StartPlayerHealAnimation();
        else if (type == PowerupTypes.Sword)
            uiManager.StartPlayerSwordAnimation();
        else
            uiManager.StartPlayerCastAnimation();
    }

    private void PlayPlayerAttackAnimationAfterEnemyFinishes(){
        waitingForEnemyAttackToFinish = true;
    }

    private void SetCurrentAttackData(){
        playerAnimatorFunctions.CreateAttackAnimation(powerupTypeForWord, wordStrength, powerupLevel);
    }

    public void DoAttackEffect(PowerupTypes type, int strength, int powerupLevel){
        switch (type){
            case PowerupTypes.Heal:
                ApplyHealToSelf(strength, powerupLevel);
                break;
            default:
                ApplyAttackToEnemy(type, strength, powerupLevel);
                break;
        }
    }

    public void DoEnemyAttackEffect(EnemyAttackAnimatorFunctions enemy){
        if (enemyData.isHorde){
            if (enemy.data == firstEnemyInHorde)
                DamagePlayerHealth(firstEnemyInHorde.attackDamage * currentHordeEnemyCount);
        }
        else
            DamagePlayerHealth(enemyData.attackDamage);
    }


    public void ApplyAttackToEnemy(PowerupTypes type, int strength, int powerupLevel){
        if (powerupLevel < 1)
            DamageEnemyHealth(strength);
        else{
            switch (type){
                case PowerupTypes.Water:
                    DamageEnemyHealth(strength);
                    ApplyBuffForWaterAttack(powerupLevel);
                    break;
                case PowerupTypes.Fire:
                    ApplyBurnForFireAttack(powerupLevel);
                    DamageEnemyHealth(strength);
                    break;
                case PowerupTypes.Lightning:
                    ApplyEnemyAttackTimeDebuffFromLightning(powerupLevel);
                    DamageEnemyHealth(strength);
                    break;
                case PowerupTypes.Dark:
                    DoDarkAttack(strength, powerupLevel);
                    break;
                case PowerupTypes.Earth:
                    DamageEnemyHealth(strength);
                    ApplyPebblesForEarthAttack(powerupLevel);
                    break;
                case PowerupTypes.Pebble:
                    DamageEnemyHealth(strength);
                    break;
                case PowerupTypes.Sword:
                    DoSwordAttack(strength, powerupLevel);
                    break;
            }
        }
    }

    private void DoSwordAttack(int strength, int powerupLevel){
        float mult = swordPowerupDamageMultiplier;
        if (enemyData.isHorde && firstEnemyInHorde.isDraconic)
            mult = swordPowerupDamageMultiplierVsDragons;
        if (enemyData.isDraconic)
            mult = swordPowerupDamageMultiplierVsDragons;
        int enemyDamage = (int)(strength * (powerupLevel * mult));
        DamageEnemyHealth(enemyDamage);
    }

    public void WaterDrainComplete(){
        isWaterInPuzzleArea = false;
        CalcWordStrength();
        UpdateSubmitVisuals();
    }

    private void ApplyBuffForWaterAttack(int powerupLevel){
        isWaterInPuzzleArea = true;
        float duration = powerupLevel * waterFloodDuration;
        uiManager.FillPuzzleAreaWithWater(duration);
        CalcWordStrength();
        UpdateSubmitVisuals();
    }

    public void ThrowPebbleIfPossible(int attackStrength){
        if (playerAnimatorFunctions.pebblesInQueue.Count > 0){
            float multiplier = playerAnimatorFunctions.pebblesInQueue[0];
            int pebbleDamage = ((int)(attackStrength * multiplier));
            if (pebbleDamage < 1)
                pebbleDamage = 1;
            uiManager.ThrowPebble(pebbleDamage);
        }
    }

    private void ApplyPebblesForEarthAttack(int powerupLevel){
        playerAnimatorFunctions.AddPebblesToQueue((pebbleDamageMultiplier * powerupLevel), pebbleCountFromEarthAttack);
        uiManager.ShowPebbleCount();
    }

    private void ApplyEnemyAttackTimeDebuffFromLightning(int powerupLevel){
        uiManager.StopEnemyAttackTimer();
        float stunTime = lightningStunDuration * powerupLevel;
        if (enemyData.isHorde)
            stunTime /= currentHordeEnemyCount;
        uiManager.ActivateEnemyStunBar(stunTime);
    }

    private void ApplyBurnForFireAttack(int powerupLevel){
        if (enemyData.isHorde)
            enemyHordeAttackAnimatorFunctions[0].AddBurnDamageToQueue(powerupLevel, burnDurationFromFireAttack);
        else
            enemyAttackAnimatorFunctions.AddBurnDamageToQueue(powerupLevel, burnDurationFromFireAttack);
        uiManager.ShowBurnCount();
    }

    public void DamagePlayerForDarkAttack(){
        //doesn't play animation for self damage
        //self damage can't kill you
        int prevHP = playerHealth;
        playerHealth -= selfDamageFromDarkAttack;
        if (playerHealth < 1)
            playerHealth = 1;
        uiManager.ShowPlayerTakingDamage((prevHP - playerHealth), playerHealth > 0, false);
        uiManager.DisplayHealths(playerHealth, enemyHealth);
    }

    private void DoDarkAttack(int strength, int powerupLevel){
        int enemyDamage = (int)(strength * (powerupLevel * darkPowerupDamageMultiplier));
        if (enemyData.isHoly)
            enemyDamage /= 2;
        if (enemyData.isDark)
            enemyDamage *= 2;
        DamageEnemyHealth(enemyDamage);
    }

    public void ApplyHealToSelf(int strength, int powerupLevel){
        int healAmount = strength * 3;
        if (enemyData.isHoly)
            healAmount *= 2;
        if (enemyData.isDark)
            healAmount /= 2;
        HealPlayerHealth(healAmount);
    }

    private void HealPlayerHealth(int amount){
        playerHealth += amount;
        if (playerHealth > maxHealth)
            playerHealth = maxHealth;
        uiManager.ShowPlayerGettingHealed(amount);
        uiManager.DisplayHealths(playerHealth, enemyHealth);
    }

    private void HealEnemyHealth(int amount){
        enemyHealth += amount;
        if (enemyHealth > maxHealth)
            enemyHealth = maxHealth;
        uiManager.ShowEnemyGettingHealed(amount);
        uiManager.DisplayHealths(playerHealth, enemyHealth);
    }

    public void DecrementRefreshPuzzleCountdown(){
        countdownToRefresh --;
        if (countdownToRefresh < 0)
            countdownToRefresh = 0;
    }

    public void AddLetter(LetterSpace ls){
        word += ls.letter;
        SetIsValidWord();
        CalcWordStrength();
        letterSpacesForWord.Add(ls);
        if (lastLetterSpace != null){
            lastLetterSpace.nextLetterSpace = ls;
            ls.previousLetterSpace = lastLetterSpace;
        }
        SetLastTwoLetterSpaces();
        UpdateSubmitVisuals();
    }

    public void RemoveLetter(LetterSpace ls){
        //assumes the last letter in the list is the provided letter
        if (word.Length < 2)
            word = "";
        else
            word = word.Substring(0, (word.Length - 1));
        SetIsValidWord();
        CalcWordStrength();
        letterSpacesForWord.Remove(ls);
        ls.ShowAsNotPartOfWord();
        if (secondToLastLetterSpace != null){
            secondToLastLetterSpace.nextLetterSpace = null;
            lastLetterSpace.previousLetterSpace = null;
        }
        SetLastTwoLetterSpaces();
        UpdateSubmitVisuals();
    }

    private void UpdateSubmitVisuals(){
        UpdatePowerupTypeAndLevel();
        uiManager.UpdateColorsForWord(word, powerupTypeForWord);
        uiManager.UpdatePowerupIcon(powerupTypeForWord);
        uiManager.UpdateVisualsForLettersInWord(letterSpacesForWord);
        uiManager.DisplayWord(word, isValidWord, countdownToRefresh, wordStrength);
    }


    private void UpdatePowerupTypeAndLevel(){
        powerupTypeForWord = BattleManager.PowerupTypes.None;
        powerupLevel = 0;        
        if (letterSpacesForWord.Count == 0)
            return;
        foreach (LetterSpace ls in letterSpacesForWord){
            if (ls.powerupType != BattleManager.PowerupTypes.None){
                powerupLevel++;
                if (powerupLevel == 1)
                    powerupTypeForWord = ls.powerupType;
            }

        }
    }

    private void SetLastTwoLetterSpaces(){
        lastLetterSpace = null;
        secondToLastLetterSpace = null;
        if (letterSpacesForWord.Count > 0)
            lastLetterSpace = letterSpacesForWord[letterSpacesForWord.Count - 1];
        if (letterSpacesForWord.Count > 1)
            secondToLastLetterSpace = letterSpacesForWord[letterSpacesForWord.Count - 2];
    }

    public bool CanAddLetter(LetterSpace letterSpace){
        if ((playerHealth == 0) || (enemyHealth == 0) || (isGamePaused))
            return false;
        if (letterSpace.hasBeenUsedInAWordAlready)
            return false;
        if (letterSpacesForWord.Contains(letterSpace))
            return false;
        if (letterSpacesForWord.Count == 0)
            return true;
        if (letterSpacesForWord.Count > 8) //decide on some limit, based on screen / text size?
            return false;
        if (letterSpace.IsAdjacentToLetterSpace(lastLetterSpace))
            return true;
        return false;
    }

    public bool CanRemoveLetter(LetterSpace letterSpace){
        if (letterSpacesForWord.Count == 0)
            return false;
        return (lastLetterSpace == letterSpace);
    }

    public void ClearWord(bool markLettersAsUsed){
        foreach (LetterSpace ls in letterSpacesForWord){
            ls.previousLetterSpace = null;
            ls.nextLetterSpace = null;
            if (markLettersAsUsed)
                ls.hasBeenUsedInAWordAlready = true;
            ls.ShowAsNotPartOfWord();
        }
        letterSpacesForWord = new List<LetterSpace>();
        SetLastTwoLetterSpaces();
        word = "";
        isValidWord = false;
        CalcWordStrength();
        UpdateSubmitVisuals();
    }

    public void ProcessFingerDown(){
        SetLetterSpaceActiveBeforeFingerDown();
        hasSwipedOffALetter = false;
    }

    public void ProcessBeginSwipe(){
    }

    public void ProcessBeginSwipeOnLetterSpace(LetterSpace space){
        if (CanAddLetter(space))
            AddLetter(space);
    }

    public void ProcessSwipeOffLetterSpace(LetterSpace space){      
        hasSwipedOffALetter = true;  
        if (CanAddLetter(space))
            AddLetter(space);
    }

    public void ProcessSwipeOnLetterSpace(LetterSpace space){
        if (CanAddLetter(space))
            AddLetter(space);
        else if (space == secondToLastLetterSpace){
            if (CanRemoveLetter(lastLetterSpace))
                RemoveLetter(lastLetterSpace);
        }
    }

    public void ProcessFingerDownOnLetter(LetterSpace space){
        if (CanAddLetter(space))
            AddLetter(space);
    }

    public void ProcessSwipeReleaseOnLetterSpace(LetterSpace space){
        if ((space.wasActiveBeforeFingerDown) && (CanRemoveLetter(space)) && (!hasSwipedOffALetter))
            RemoveLetter(space);
        else if ((word.Length == 1) && (hasSwipedOffALetter) && (CanRemoveLetter(space)))
            RemoveLetter(space);
    }

    public void ProcessTapReleaseOnLetterSpace(LetterSpace space){
        if ((space.wasActiveBeforeFingerDown) && (CanRemoveLetter(space)))
            RemoveLetter(space);
    }

    private void SetLetterSpaceActiveBeforeFingerDown(){
        foreach (LetterSpace ls in puzzleGenerator.letterSpaces){
            ls.wasActiveBeforeFingerDown = false;
            if (letterSpacesForWord.Contains(ls))
                ls.wasActiveBeforeFingerDown = true;
        }
    }

    public void QueueEnemyAttack(){
        if (playerHealth != 0){
            if (enemyData.isHorde)
                uiManager.StartEnemyAttackTimer(firstEnemyInHorde.attackSpeed);
            else
                uiManager.StartEnemyAttackTimer(enemyData.attackSpeed);
        }
    }

    public void EnemyReturnedToIdle(){
        if (waitingForEnemyAttackToFinish){
            waitingForEnemyAttackToFinish = false;
            if (playerAnimatorFunctions.attacksInProgress.Count == 1){
                StartPlayingPlayerAttackAnimation(playerAnimatorFunctions.attacksInProgress[0].GetComponent<AttackAnimatorFunctions>().type);
            }
                
            else if (playerAnimatorFunctions.attacksInProgress.Count > 1){
                StartPlayingPlayerAttackAnimation(playerAnimatorFunctions.attacksInProgress[0].GetComponent<AttackAnimatorFunctions>().type);
                PlayNextAttackAfterBriefPause();
            }
        }
    }

    private void PlayNextAttackAfterBriefPause(){
        StaticVariables.WaitTimeThenCallFunction(0.5f, PlayNextAttack);
    }

    private void PlayNextAttack(){
        if (enemyHealth == 0)
            return;
        if (playerAnimatorFunctions.attacksInProgress.Count == 1){
                StartPlayingPlayerAttackAnimation(playerAnimatorFunctions.attacksInProgress[0].GetComponent<AttackAnimatorFunctions>().type);
        }
        else if (playerAnimatorFunctions.attacksInProgress.Count > 1){
            StartPlayingPlayerAttackAnimation(playerAnimatorFunctions.attacksInProgress[0].GetComponent<AttackAnimatorFunctions>().type);
            PlayNextAttackAfterBriefPause();
        }
    }
    
    public void PlayerAttackAnimationFinished(GameObject attackObject){
        //destroys the gameobject
        //then resumes the enemy attack timer, if there are no non-pebble animations left
        Destroy(attackObject);
        if (enemyHealth < 1)
            return;
        bool anyAnimationsInProgress = false;
        foreach (Transform t in uiManager.playerAttackAnimationParent){
            if (t.gameObject != attackObject){
                if (!t.name.Contains("Pebble"))
                    anyAnimationsInProgress = true;
            }
        }
        if (!anyAnimationsInProgress)
            uiManager.ResumeEnemyAttackBar();
    }

    public void TriggerEnemyAttack(){
        if (!stopNextAttack){
            DecrementRefreshPuzzleCountdown();
            UpdateSubmitVisuals();
            uiManager.StartEnemyAttackAnimation();
        }
    }

    public void EnemyStunWearsOff(){
        uiManager.DeactivateEnemyStunBar();
        if (!stopNextAttack){
            QueueEnemyAttack();
        }
    }

    public void EnemyDeathAnimationFinished(){
        if (enemyData.isHorde){
            if (enemyHealth == 0)
                uiManager.ShowVictoryPage();
            else{
                for (int i = currentHordeEnemyCount; i < startingHordeEnemyCount; i++){
                    if (enemyHordeAttackAnimatorFunctions[i].gameObject.activeSelf){
                        enemyHordeAttackAnimatorFunctions[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        else{
            uiManager.ShowVictoryPage();
        }
    }

}
 
