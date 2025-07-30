using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

    [HideInInspector]
    public AttackData inProgressWord;
    [HideInInspector]
    public List<AttackData> attackQueue = new();
    //[HideInInspector]
    //public string word = "";

    [HideInInspector]
    public List<LetterSpace> letterSpacesForWord = new List<LetterSpace>(){};

    private LetterSpace lastLetterSpace = null;
    private LetterSpace secondToLastLetterSpace = null;

    //[HideInInspector]
    //public BattleManager.PowerupTypes powerupTypeForWord;
    //[HideInInspector]
    //public int powerupLevel;
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
    //[HideInInspector]
    //public bool isValidWord = false;
    //private int wordStrength = 0;
    private bool hasSwipedOffALetter = false;
    //private bool waitingForEnemyAttackToFinish = false;
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

        inProgressWord = new(this);
        uiManager.SetStartingValues();
        uiManager.DisplayHealths(playerHealth, enemyHealth);
        uiManager.DisplayWord(inProgressWord, countdownToRefresh);
        StaticVariables.FadeIntoScene();
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, QueueEnemyAttack);
        puzzleGenerator.Setup();
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


    public void AttackHitsEnemy(AttackData attackData){
        if (enemyData.isCopycat){
            if ((copycatBuildup < maxCopycatStacks) && (attackData.type != PowerupTypes.Heal)){
                copycatBuildup ++;
                uiManager.ShowCopycatBuildup();
            }
        }
        switch (attackData.type){
            case PowerupTypes.Water:
                DamageEnemyHealth(attackData.strength);
                ApplyBuffForWaterAttack();
                break;
            case PowerupTypes.Fire:
                DamageEnemyHealth(attackData.strength);
                //ApplyBurnForFireAttack();
                break;
            case PowerupTypes.Lightning:
                DamageEnemyHealth(attackData.strength);
                ApplyEnemyAttackTimeDebuffFromLightning();
                break;
            case PowerupTypes.Dark:
                DoDarkAttack(attackData.strength);
                break;
            case PowerupTypes.Earth:
                DamageEnemyHealth(attackData.strength);
                //ApplyPebblesForEarthAttack();
                break;
            case PowerupTypes.Sword:
                DoSwordAttack(attackData.strength);
                break;
            default: //poweruptypes.heal and none
                DamageEnemyHealth(attackData.strength);
                break;
        }
    }

    private void DoSwordAttack(int strength){
        float mult = swordPowerupDamageMultiplier;
        //if (enemyData.isHorde && firstEnemyInHorde.isDraconic)
        //    mult = swordPowerupDamageMultiplierVsDragons;
        if (enemyData.isDraconic)
            mult = swordPowerupDamageMultiplierVsDragons;
        int enemyDamage = (int)(strength * mult);
        DamageEnemyHealth(enemyDamage);
    }

    public virtual void WaterDrainComplete(){
        isWaterInPuzzleArea = false;
        inProgressWord.RemoveWaterBuff();
        UpdateSubmitVisuals();
    }

    private void ApplyBuffForWaterAttack(){
        if ((enemyHealth <= 0) || (playerHealth <= 0))
            StaticVariables.WaitTimeThenCallFunction(0.6f, uiManager.FadeOutWaterOverlay);
        isWaterInPuzzleArea = true;
        uiManager.FillPuzzleAreaWithWater(StaticVariables.waterFloodDuration);
        inProgressWord.AddWaterBuff();
        UpdateSubmitVisuals();
    }

    private void ApplyEnemyAttackTimeDebuffFromLightning(){
        uiManager.StopEnemyAttackTimer();
        float stunTime = lightningStunDuration;
        if (enemyData.isHorde)
            stunTime /= currentHordeEnemyCount;
        uiManager.ActivateEnemyStunBar(stunTime);
    }

    //private void ApplyBurnForFireAttack(int powerupLevel){
    //    if (enemyData.isHorde)
    //        enemyHordeAttackAnimatorFunctions[0].AddBurnDamageToQueue(powerupLevel, burnDurationFromFireAttack);
    //    else
    //        enemyAttackAnimatorFunctions.AddBurnDamageToQueue(powerupLevel, burnDurationFromFireAttack);
    //    uiManager.ShowBurnCount();
    //}

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

    private void DoDarkAttack(int strength){
        int enemyDamage = (int)(strength * darkPowerupDamageMultiplier);
        if (enemyData.isHoly)
            enemyDamage /= 2;
        if (enemyData.isDark)
            enemyDamage *= 2;
        DamageEnemyHealth(enemyDamage);
    }

    public virtual void ApplyHealToSelf(AttackData attackData){
        int healAmount = attackData.strength * StaticVariables.healMultiplier;
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
        letterSpacesForWord.Add(ls);
        inProgressWord.AddLetter(ls.letter);
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
        letterSpacesForWord.Remove(ls);
        inProgressWord.RemoveLastLetter();
        ls.ShowAsNotPartOfWord();
        if (secondToLastLetterSpace != null){
            secondToLastLetterSpace.nextLetterSpace = null;
            lastLetterSpace.previousLetterSpace = null;
        }
        SetLastTwoLetterSpaces();
        UpdateSubmitVisuals();
    }

    public virtual void UpdateSubmitVisuals(){
        uiManager.powerupType = inProgressWord.type;
        uiManager.UpdateVisualsForLettersInWord(letterSpacesForWord);
        uiManager.DisplayWord(inProgressWord, countdownToRefresh);
        uiManager.ShowPowerupBackground(inProgressWord.type);
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
        inProgressWord.UpdateWord("");
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
        else if ((inProgressWord.word.Length == 1) && (hasSwipedOffALetter) && (CanRemoveLetter(space)))
            RemoveLetter(space);
    }

    public void ProcessTapReleaseOnLetterSpace(LetterSpace space){
        if ((space.wasActiveBeforeFingerDown) && (CanRemoveLetter(space)))
            RemoveLetter(space);
    }

    public virtual void ProcessFingerRelease(){
        if (inProgressWord.isValidWord)
            SubmitWord();
        else
            ClearWord(false);
    }

    public void SubmitWord(){
        if ((playerHealth == 0) || (enemyHealth == 0) || (isGamePaused))
            return;
        if (inProgressWord.isValidWord) {
            attackQueue.Add(inProgressWord);
            inProgressWord = new(this);
            DecrementRefreshPuzzleCountdown();
            ClearWord(true);
            if (isWaterInPuzzleArea && enemyData.canBurn)
                ClearRandomBurnedLetters(2);
            CheckToUseQueuedAttack();
        }
    }
    
    public void CheckToUseQueuedAttack(){
        if (CanQueuedAttackBeUsed())
            AttackWithFirstWordInQueue();
    }
    
    public bool CanQueuedAttackBeUsed(){
        if (attackQueue.Count == 0)
            return false;
        if (enemyHealth <= 0)
            return false;
        if (playerHealth <= 0)
            return false;
        if (enemyData.isHorde) {
                if (!uiManager.enemyHordeAnimators[0].GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                    return false;
            }
            else if (!uiManager.enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                return false;
        return uiManager.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }

    private void AttackWithFirstWordInQueue() {
        if (attackQueue.Count == 0)
            return;
        AttackData attackData = attackQueue[0];
        attackQueue.RemoveAt(0);
        playerAnimatorFunctions.attackInProgress = attackData;
        StartPlayingPlayerAttackAnimation(attackData.type);
    }

    public virtual void PressWordArea() {
        if ((inProgressWord.word.Length == 0) && (countdownToRefresh == 0))
        {
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
        CheckToUseQueuedAttack();
    }
    
    public void PlayerAttackAnimationFinished(GameObject attackObject){
        //destroys the gameobject, then resumes the enemy attack timer
        Destroy(attackObject);
        if (enemyHealth < 1)
            return;
        if (attackQueue.Count > 0)
            CheckToUseQueuedAttack();
        else
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

    public int GetNumberOfEnemies() {
        if (enemyData.isHorde)
            return currentHordeEnemyCount;
        return 1;
    }

}
public class AttackData {
    public BattleManager.PowerupTypes type = BattleManager.PowerupTypes.None;
    public string word = "";
    public bool hasEarthBuff = false;
    public bool hasWaterBuff = false;
    public bool isValidWord = false;
    public int strength = 0;
    private readonly BattleManager battleManager;

    public AttackData(BattleManager battleManager) {
        this.battleManager = battleManager;
        if (battleManager.isWaterInPuzzleArea)
            AddWaterBuff();
        //UpdateWord("");
    }

    public void AddLetter(char letter) {
        UpdateWord(word + letter);
    }

    public void RemoveLastLetter() {
        if (word.Length < 2)
            UpdateWord("");
        else
            UpdateWord(word[..^1]);

    }

    public void UpdateWord(string newWord) {
        word = newWord;
        SetIsValidWord();
        SetPowerupType();
        SetWordStrength();
        //PrintAttackData();
    }

    public void AddWaterBuff() {
        hasWaterBuff = true;
        SetWordStrength();
    }

    public void RemoveWaterBuff() {
        hasWaterBuff = false;
        SetWordStrength();
    }

    public void AddEarthBuff() {
        hasEarthBuff = true;
        SetWordStrength();
    }

    public void RemoveEarthBuff() {
        hasEarthBuff = false;
        SetWordStrength();
    }

    public void SetIsValidWord() {
        if (word.Length < battleManager.minCheckingWordLength)
            isValidWord = false;
        else
            isValidWord = SearchLibraryForWord(word);
    }

    private bool SearchLibraryForWord(string word) {
        //returns true if the library contains the word
        int result = System.Array.BinarySearch<string>(StaticVariables.wordLibraryForChecking, word.ToLower());
        return (result > -1);
    }

    public void SetWordStrength() {
        if (word.Length < battleManager.minCheckingWordLength) {
            strength = 0;
            return;
        }
        int len = word.Length;
        if (hasEarthBuff)
            len += 2;
        int str = Mathf.FloorToInt(Mathf.Pow((len - 2), 2));
        if (hasWaterBuff) {
            str += (StaticVariables.waterFloodDamageBonus * battleManager.GetNumberOfEnemies());
            if (battleManager.enemyData.isNearWater)
                str += StaticVariables.riverDamageBonus;
        }
        strength = str;
    }

    public void SetPowerupType() {
        type = BattleManager.PowerupTypes.None;
        if (battleManager.letterSpacesForWord.Count == 0)
            return;
        foreach (LetterSpace ls in battleManager.letterSpacesForWord) {
            if (ls.powerupType != BattleManager.PowerupTypes.None) {
                type = ls.powerupType;
                return;
            }
        }
    }

    private void PrintAttackData() {
        Debug.Log("attack data- type(" + type + ") str(" + strength + ") length(" + word.Length + ") flooded(" + hasWaterBuff + ") rocked(" + hasEarthBuff + ")");
    }
}
