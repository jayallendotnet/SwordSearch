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
    public GameObject bookDescriptionPrefab;
    private GameObject bookDescription;
    public float transitionDuration;
    public InteractOverlayManager interactOverlayManager;

    public void PressedButton(){
        if (interactOverlayManager.CanMakeBookSelection())
            return;
        if (StaticVariables.buffedType == powerupType)
            return;
        StaticVariables.buffedType = powerupType;
        FindObjectOfType<InteractOverlayManager>().UpdateBookSelection();
        interactOverlayManager.isMovingBookDescriptions = true;
    }

    private void ShowActive(bool b){
        inactiveBackground.SetActive(!b);
    }

    public void ShowActiveIfMatchingType(BattleManager.PowerupTypes type){
        ShowActive(type == powerupType);
    }

    private void AddDescriptionBelow(){
        //add the book description text
        bookDescription = Instantiate(bookDescriptionPrefab, transform.parent);
        RectTransform t = bookDescription.GetComponent<RectTransform>();
        t.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        t.GetChild(0).GetComponent<Text>().text = bookData.description.ToUpper().Replace(" ", "  ");
        t.GetChild(2).GetComponent<Text>().text = TextFormatter.FormatString(GetPowerupSummary());

        //start the description small then grow to full size
        float width = t.sizeDelta.x;
        float height = t.sizeDelta.y;
        t.localScale = new Vector3(1,0,1);
        t.sizeDelta = new Vector3(width,-30,1);
        t.DOScaleY(1, transitionDuration);
        t.DOSizeDelta(new Vector3(width,height,1), transitionDuration);

    }

    private void DeleteDescription(){
        //shrink the book description, then delete it
        RectTransform t = bookDescription.GetComponent<RectTransform>();
        float width = t.sizeDelta.x;
        t.DOScaleY(0, transitionDuration);
        t.DOSizeDelta(new Vector3(width, -30, 1), transitionDuration).OnComplete(FinishShrinkingDescription);

        //fade out the book description text
        Color c = t.GetChild(0).GetComponent<Text>().color;
        c.a = 0;
        Color c2 = t.GetChild(1).GetChild(0).GetComponent<Image>().color;
        c2.a = 0;
        t.GetChild(0).GetComponent<Text>().DOColor(c, transitionDuration);
        t.GetChild(1).GetChild(0).GetComponent<Image>().DOColor(c2, transitionDuration);
        t.GetChild(2).GetComponent<Text>().DOColor(c, transitionDuration);
    }

    private void FinishShrinkingDescription(){
        GameObject.DestroyImmediate(bookDescription);
        interactOverlayManager.isMovingBookDescriptions = false;
    }

    public void ShowOrHideDescription(){
        if (inactiveBackground.activeSelf){
            if (bookDescription != null)
                DeleteDescription();
        }
        else{
            if (bookDescription == null)
                AddDescriptionBelow();
        }
    }

    public void ShowBookName(){
        bookName.text = bookData.name.ToUpper();
    }

    private string GetPowerupSummary(){
        string powerupName = (powerupType + "").ToLower();
        if (powerupType == BattleManager.PowerupTypes.Heal)
            powerupName = "healing";
        else if (powerupType == BattleManager.PowerupTypes.Dark)
            powerupName = "darkness";
        return ("The <" + powerupName + ">power of " + powerupName + "<> will now appear much more frequently in battle!");
    }

}


