using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

    [HideInInspector]
    public string word = "";
    
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
    [HideInInspector]
    public int countdownToRefresh;
    public enum PowerupTypes{None, Water, Fire, Heal, Dark, Earth, Lightning, Pebble, Sword};
    [HideInInspector]
    public bool isValidWord = false;
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
    private int enemyAttackIndex = 0;
    [HideInInspector]
    public int copycatBuildup = 0;
    private int maxBurnedLetters = 20;


    [Header("Game Variables")]
    private readonly int startingPlayerHealth = 100;
    public int maxHealth = 999; //for display purposes
    public int minCheckingWordLength = 3;
    public int maxPuzzleCountdown = 3;
    public int selfDamageFromDarkAttack = 5;
    public int burnDurationFromFireAttack = 5;
    public float timeBetweenBurnHits = 3f;
    public int pebbleCountFromEarthAttack = 3;
    public float pebbleDamageMultiplier = 0.33f;
    public float lightningStunDuration = 15f;
    public float darkPowerupDamageMultiplier = 2.5f;
    public float swordPowerupDamageMultiplier = 2f;
    public float swordPowerupDamageMultiplierVsDragons = 5f;
    public BattleData defaultBattleData;
    public int selfDamageFromBurnedLetters = 2;
    public int maxCopycatStacks = 5;

    [Header("Scripts")]
    public UIManager uiManager;
    public PuzzleGenerator puzzleGenerator;
    public PlayerAnimatorFunctions playerAnimatorFunctions;
    public GeneralSceneManager setup;



    public virtual void Start(){
        setup.Setup();
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
        if (StaticVariables.difficultyMode == StaticVariables.DifficultyMode.Story)
            enemyHealth = 1;
        else if (StaticVariables.difficultyMode == StaticVariables.DifficultyMode.Puzzle)
            enemyHealth *= 10;
        playerHealth = startingPlayerHealth;
        uiManager.ApplyBackground(StaticVariables.battleData.backgroundPrefab);

        uiManager.SetStartingValues();
        uiManager.DisplayHealths(playerHealth, enemyHealth);
        uiManager.DisplayWord(word, isValidWord, countdownToRefresh, wordStrength);
        StaticVariables.FadeIntoScene();
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, QueueEnemyAttack);
        puzzleGenerator.Setup();
    }
    
    public void SetIsValidWord(){
        if (word.Length < minCheckingWordLength)
            isValidWord = false;
        else
            isValidWord = SearchLibraryForWord(word);
    }

    private bool SearchLibraryForWord(string word){
        //returns true if the library contains the word
        int result = System.Array.BinarySearch<string>(StaticVariables.wordLibraryForChecking, word.ToLower());
        return (result > -1);
    }

    public void CalcWordStrength(){
        if (word.Length < minCheckingWordLength)
            wordStrength = 0;
        else{
            wordStrength =  Mathf.FloorToInt(Mathf.Pow((word.Length - 2), 2));
            if (isWaterInPuzzleArea){
                if (enemyData.isHorde)
                    wordStrength += (StaticVariables.waterFloodDamageBonus * currentHordeEnemyCount);
                else
                    wordStrength += StaticVariables.waterFloodDamageBonus;
                if (enemyData.isNearWater) //if near water add +1x to the water flood bonus. added at the end to work with hordes near water
                    wordStrength += StaticVariables.riverDamageBonus;
                    
            }
        }
    }

    public virtual void DamageEnemyHealth(int amount){
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
            //uiManager.FadeOutWaterOverlay();
            uiManager.PauseBoulderDebuff();
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

    public virtual void ApplyEnemyAttackDamage(int amount) {
        if (StaticVariables.difficultyMode == StaticVariables.DifficultyMode.Story)
            DamagePlayerHealth(1);
        else if (StaticVariables.difficultyMode == StaticVariables.DifficultyMode.Puzzle)
            DamagePlayerHealth(0);
        else
            DamagePlayerHealth(amount);
    }

    public virtual void DamagePlayerHealth(int amount) {
        playerHealth -= amount;
        if (playerHealth < 0)
            playerHealth = 0;
        uiManager.ShowPlayerTakingDamage(amount, playerHealth > 0);
        uiManager.DisplayHealths(playerHealth, enemyHealth);
        if (playerHealth == 0)
        {
            uiManager.PauseEnemyAttackBar();
            uiManager.PauseWaterDrain();
            //uiManager.FadeOutWaterOverlay();
            uiManager.PauseBoulderDebuff();
            uiManager.PausePageTurn();
            ClearWord(false);
        }
    }

    public void PauseEverything(){
        isGamePaused = true;
        uiManager.PauseEnemyAttackBar();
        uiManager.PauseWaterDrain();
        uiManager.PauseBoulderDebuff();
        uiManager.SetAllAnimationStates(false);
    }


    public void ResumeEverything(){
        isGamePaused = false;
        uiManager.ResumeEnemyAttackBar();
        uiManager.ResumeWaterDrain();
        uiManager.ResumeBoulderDebuff();
        uiManager.SetAllAnimationStates(true);
    }

    private void StartPlayingPlayerAttackAnimation(PowerupTypes type) {
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
        if (powerupTypeForWord == PowerupTypes.Heal)
            playerAnimatorFunctions.CreateAttackAnimation(PowerupTypes.None, wordStrength, 0);
    }

    public void DoAttackEffect(PowerupTypes type, int strength, int powerupLevel){
        switch (type){
            case PowerupTypes.Heal:
                ApplyHealToSelf(strength, powerupLevel);
                ClearDebuffsViaHealing();
                break;
            default:
                ApplyAttackToEnemy(type, strength, powerupLevel);
                break;
        }
    }

    public void DoEnemyAttackEffect(EnemyAttackAnimatorFunctions enemy){
        EnemyAttack ea = null;
        if (enemyData.isHorde){
            if (enemy.data == firstEnemyInHorde)
                ea = firstEnemyInHorde.attackOrder.Value[enemyAttackIndex];
        }
        else
            ea = enemyData.attackOrder.Value[enemyAttackIndex];


        if (ea == null)
            return;
        if (enemyData.isHorde)
            ApplyEnemyAttackDamage(ea.attackDamage * currentHordeEnemyCount);
        else if (enemyData.isCopycat) {
            int newDamage = ea.attackDamage * copycatBuildup; //at 0 stacks, copycat does 0 damage
            if (newDamage < 0)
                newDamage = 0;
            ApplyEnemyAttackDamage(newDamage);
            print("copycat is attacking! base damage is " + ea.attackDamage + ", buildup is " + copycatBuildup + ", so total damage is " + newDamage);
        }
        else if (ea.isSpecial && ea.specialType == EnemyAttack.EnemyAttackTypes.HealsSelf) {
            int amount = playerHealth;
            ApplyEnemyAttackDamage(ea.attackDamage);
            amount -= playerHealth;
            if (amount > 0)
                HealEnemyHealth(amount);
        }

        else
            ApplyEnemyAttackDamage(ea.attackDamage);


        if (ea.isSpecial){
            switch (ea.specialType){
                case EnemyAttack.EnemyAttackTypes.ThrowRocks:
                    uiManager.CoverPageWithBoulders();
                    //print("rocks should now cover some of the screen.");
                    break;
                case EnemyAttack.EnemyAttackTypes.BurnLetters:
                    BurnRandomLetters(3);
                    break;
            }

        }


        IncrementAttackIndex();

    }

    private void IncrementAttackIndex(){
        enemyAttackIndex = GetNextAttackIndex();
    }

    private int GetNextAttackIndex(){
        int val = enemyAttackIndex;
        val ++;
        int attackCount;
        if (enemyData.isHorde)
            attackCount = firstEnemyInHorde.attackOrder.Value.Length;
        else
            attackCount = enemyData.attackOrder.Value.Length;
        if (val >= attackCount)
            val = 0;
        return val;
    }


    public void ApplyAttackToEnemy(PowerupTypes type, int strength, int powerupLevel){
        if (enemyData.isCopycat){
            if ((copycatBuildup < maxCopycatStacks) && (type != PowerupTypes.Heal) && (type != PowerupTypes.Pebble)){
                copycatBuildup ++;
                uiManager.ShowCopycatBuildup();
            }
        }
        if (powerupLevel < 1)
                DamageEnemyHealth(strength);
            else {
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
        //if (enemyData.isHorde && firstEnemyInHorde.isDraconic)
        //    mult = swordPowerupDamageMultiplierVsDragons;
        if (enemyData.isDraconic)
            mult = swordPowerupDamageMultiplierVsDragons;
        int enemyDamage = (int)(strength * (powerupLevel * mult));
        DamageEnemyHealth(enemyDamage);
    }

    public virtual void WaterDrainComplete(){
        isWaterInPuzzleArea = false;
        CalcWordStrength();
        UpdateSubmitVisuals();
    }

    private void ApplyBuffForWaterAttack(int powerupLevel){
        if ((enemyHealth <= 0) || (playerHealth <= 0))
            StaticVariables.WaitTimeThenCallFunction(0.6f, uiManager.FadeOutWaterOverlay);
        isWaterInPuzzleArea = true;
        float duration = powerupLevel * StaticVariables.waterFloodDuration;
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

    public virtual void ApplyHealToSelf(int strength, int powerupLevel){
        int healAmount = strength * StaticVariables.healMultiplier;
        if (enemyData.isHoly)
            healAmount *= 2;
        if (enemyData.isDark)
            healAmount /= 2;
        HealPlayerHealth(healAmount);
    }
        
    public void BurnRandomLetters(int amount){
        for (int i = 0; i < amount; i++){
            if (puzzleGenerator.burnedLetters.Count >= maxBurnedLetters)
                return;
            LetterSpace toBurn = puzzleGenerator.PickRandomSpaceWithoutPowerupOrBurn();
            if(toBurn != null)
                toBurn.ApplyBurn();
        }
    }
    
    public void ClearRandomBurnedLetters(int amount){
        for (int i = 0; i < amount; i++){
            LetterSpace toClear = puzzleGenerator.PickRandomBurnedSpace();
            if (toClear != null)
                toClear.RemoveBurn();
        }
    }

    private void ClearAllBurnedLetters() {
        List<LetterSpace> letters = new(puzzleGenerator.burnedLetters); //we dont want to modify the list as we are iterating on it, so we make a copy
        foreach (LetterSpace ls in letters)
            ls.RemoveBurn();
    }

    public void ClearDebuffsViaHealing(){
        if (uiManager.shownBoulders != null)
            uiManager.ClearBouldersOnPage();
        if (enemyData.isCopycat) {
            copycatBuildup -= 3;
            if (copycatBuildup < -1)
                copycatBuildup = -1; //its ok to set to -1, because a normal damaging attack happens right after, bringing the buildup back to 0.
        }
        //if (puzzleGenerator.burnedLetters.Count > 0)
        //    ClearRandomBurnedLetters(5);
    }
    
    public void HideAllDebuffVisuals(){
        if (uiManager.shownBoulders != null)
            uiManager.ClearBouldersOnPage();
        if (puzzleGenerator.burnedLetters.Count > 0)
            ClearAllBurnedLetters();
    }

    private void HealPlayerHealth(int amount) {
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

    public virtual void AddLetter(LetterSpace ls) {
        word += ls.letter;
        SetIsValidWord();
        CalcWordStrength();
        letterSpacesForWord.Add(ls);
        if (lastLetterSpace != null) {
            lastLetterSpace.nextLetterSpace = ls;
            ls.previousLetterSpace = lastLetterSpace;
        }
        SetLastTwoLetterSpaces();
        UpdateSubmitVisuals();
        if (ls.isBurned)
            DamagePlayerHealth(selfDamageFromBurnedLetters);

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

    public virtual void UpdateSubmitVisuals(){
        UpdatePowerupTypeAndLevel();
        uiManager.powerupType = powerupTypeForWord;
        //uiManager.SetPowerupBackgroundColor(powerupTypeForWord);
        //uiManager.UpdatePowerupIcon(powerupTypeForWord, powerupLevel);
        uiManager.UpdateVisualsForLettersInWord(letterSpacesForWord);
        uiManager.DisplayWord(word, isValidWord, countdownToRefresh, wordStrength);
        uiManager.ShowPowerupBackground(powerupTypeForWord);
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

    public virtual bool CanAddLetter(LetterSpace letterSpace){
        if (letterSpace.letter == '=')
            return false;
        if (letterSpace.letter == '-')
            return false;
        if (letterSpace.letter == ' ')
            return false;
        if ((playerHealth == 0) || (enemyHealth == 0) || (isGamePaused))
            return false;
        if (letterSpace.hasBeenUsedInAWordAlready)
            return false;
        if (letterSpacesForWord.Contains(letterSpace))
            return false;
        //print(uiManager.shownBoulders);
        if ((uiManager.shownBoulders != null) && (uiManager.shownBoulders.coveredLetters.Contains(letterSpace)))
            return false;
        if (letterSpacesForWord.Count == 0)
            return true;
        if (letterSpacesForWord.Count >= 15) //hard limit of 15 letters per word, for UI reasons
            return false;
        if (letterSpace.IsAdjacentToLetterSpace(lastLetterSpace))
            return true;
        return false;
    }

    public virtual bool CanRemoveLetter(LetterSpace letterSpace){
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

    public virtual void ProcessFingerRelease(){
        if (isValidWord)
            SubmitWord();
        else
            ClearWord(false);
    }

    public void SubmitWord(){
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

            if (isWaterInPuzzleArea && enemyData.canBurn)
                ClearRandomBurnedLetters(2);
        }
    }

    public virtual void PressWordArea(){
        if ((word.Length == 0) && (countdownToRefresh == 0)){
            puzzleGenerator.GenerateNewPuzzle();
            countdownToRefresh = maxPuzzleCountdown;
            ClearWord(true);  
            uiManager.ShowPageTurn();         
        }
    }

    

    private void SetLetterSpaceActiveBeforeFingerDown(){
        foreach (LetterSpace ls in puzzleGenerator.letterSpaces){
            ls.wasActiveBeforeFingerDown = false;
            if (letterSpacesForWord.Contains(ls))
                ls.wasActiveBeforeFingerDown = true;
        }
    }

    public virtual void QueueEnemyAttack(){
        if (playerHealth != 0){
            EnemyAttack ea;
            if (enemyData.isHorde)
                ea = firstEnemyInHorde.attackOrder.Value[enemyAttackIndex];    
            else
                ea = enemyData.attackOrder.Value[enemyAttackIndex];
            if (ea.isSpecial)
                uiManager.StartEnemyAttackTimer(ea.attackSpeed, ea.specialColor);
            else
                uiManager.StartEnemyAttackTimer(ea.attackSpeed);
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

    public virtual void TriggerEnemyAttack(){
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

    public virtual void EnemyDeathAnimationFinished(){
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

    public virtual void TurnPageEnded(){

    }

    public virtual void WaterReachedTopOfPage(){

    }

}
 
