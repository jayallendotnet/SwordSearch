using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour {
    //to be attached to the camera in the combat scene
    //detects the player's touch or mouse inputs, and calls the touch function for the appropriate object
 
    //variables used to detect if the player is tapping the screen
    private Vector2 startingFingerPlacement;
    private bool tapping = false; //set to true when the player taps the screen. changed to false when the player moves their finger more than the tap radius

    private LetterSpace spaceUnderFinger;

    public BattleManager battleManager;

    //the following declarations let us choose the size of the tap radius from the inspector. It is a percentage of either screen width or screen height
    public enum wh { Width, Height};
    [Header("Tap Radius in terms of screen dimensions")]
    public wh widthOrHeight = wh.Width;
    public float percent = 2f; //% of the width/height of screen that the player can move their finger by to register as a tap




    private void Update() {
        //every frame, check for any touch inputs

        //process a tap with the finger
        if (Input.GetMouseButtonDown(0)) {
            //register the current finger position for the purpose of detecting a tap
            startingFingerPlacement = Input.mousePosition;
            tapping = true;
            SetLetterSpaceUnderFinger();
        }
        else if (Input.GetMouseButton(0)){
            bool wasTapping = tapping;
            if (tapping)
                tapping = StillTapping();
            if (!tapping){
                LetterSpace prevLetterSpace = spaceUnderFinger;
                SetLetterSpaceUnderFinger();
                if (prevLetterSpace != spaceUnderFinger){
                    ProcessSwipeOffLetterSpace(prevLetterSpace);
                    ProcessSwipeOnLetterSpace(spaceUnderFinger);
                }
                else if (wasTapping){
                    ProcessBeginSwipeOnLetterSpace(spaceUnderFinger); 
                }          
            }
        }
        else if (Input.GetMouseButtonUp(0)){
            if (tapping){
                ProcessTapReleaseOnLetterSpace(spaceUnderFinger);
            }
            else{
                ProcessSwipeReleaseOnLetterSpace(spaceUnderFinger);
            }
        }
    }

    private void ProcessBeginSwipeOnLetterSpace(LetterSpace space){
        if (space == null)
            return;
        battleManager.ProcessBeginSwipeOnLetterSpace(space);
    }

    private void ProcessSwipeOffLetterSpace(LetterSpace space){
        if (space == null)
            return;
        battleManager.ProcessSwipeOffLetterSpace(space);
    }

    private void ProcessSwipeOnLetterSpace(LetterSpace space){
        if (space == null)
            return;
        battleManager.ProcessSwipeOnLetterSpace(space);
    }

    private void ProcessSwipeReleaseOnLetterSpace(LetterSpace space){
        if (space == null)
            return;
        battleManager.ProcessSwipeReleaseOnLetterSpace(space);
    }

    private void ProcessTapReleaseOnLetterSpace(LetterSpace space){
        if (space == null)
            return;
        battleManager.ProcessTapReleaseOnLetterSpace(space);
    }


    private void SetLetterSpaceUnderFinger(){
        spaceUnderFinger = null;
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++) {
            //print(raycastResultList[i].gameObject.name);
            if (raycastResultList[i].gameObject.name == "TouchDetection")
                spaceUnderFinger = raycastResultList[i].gameObject.transform.parent.GetComponent<LetterSpace>();
        }
    }    
    
    private bool StillTapping() {
        //returns true if the touch start position and current touch position are identical, or very close
        float dist = Vector2.Distance(Input.mousePosition, startingFingerPlacement);

        //set the tap radius based on values provided in the inspector
        float tapRadius; //the max distance the start and release positions can be for the touch to register as a tap
        tapRadius = percent * 0.01f; //start with the provided percentage scalar
        //then multiply it by either the screen width or screen height
        if (widthOrHeight == wh.Width) { tapRadius *= Screen.width; }
        else { tapRadius *= Screen.height; }

        //check if the touch position is smaller than the tap radius
        if (dist <= tapRadius) { return true; }
        return false;
    }
}
