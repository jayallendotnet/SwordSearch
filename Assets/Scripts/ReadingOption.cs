using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;
using MyBox;

public class ReadingOption : MonoBehaviour{

    [HideInInspector]
    public BattleManager.PowerupTypes powerupType;
    public Image bookImage;
    public GameObject inactiveBackground;
    public Text bookName;
    public BookData bookData;

    public void PressedButton(){
        StaticVariables.buffedType = powerupType;
        FindObjectOfType<InteractOverlayManager>().UpdateBookSelection();
        ShowBookDescription();
    }

    private void ShowActive(bool b){
        inactiveBackground.SetActive(!b);
    }

    public void ShowActiveIfMatchingType(BattleManager.PowerupTypes type){
        ShowActive(type == powerupType);
    }

    public void ShowBookName(){
        bookName.text = bookData.name.ToUpper();
    }

    private void ShowBookDescription(){
        string s = bookData.description.ToUpper();
        string powerupName = powerupType + "";
        if (powerupType == BattleManager.PowerupTypes.Heal)
            powerupName = "Healing";
        else if (powerupType == BattleManager.PowerupTypes.Dark)
            powerupName = "Darkness";
        s += "\n" + "The power of " + powerupName + " will now appear much more frequently in battle!";
        print(s);
    }

}


