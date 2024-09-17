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

    public void PressedButton(){
        StaticVariables.buffedType = powerupType;
        FindObjectOfType<InteractOverlayManager>().UpdateBookSelection();
    }

    private void ShowActive(bool b){
        inactiveBackground.SetActive(!b);
    }

    public void ShowActiveIfMatchingType(BattleManager.PowerupTypes type){
        ShowActive(type == powerupType);
    }

}


