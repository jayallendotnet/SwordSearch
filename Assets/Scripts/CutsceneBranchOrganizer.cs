using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneBranchOrganizer : MonoBehaviour{

    public float dropSpeed = 1200f;
    public float timeBetweenFalls = 0.2f;
    public List<CutsceneBranchData> branches;
    private int index = 0;

    public void StartDrops(){
        //print("starting drops");
        index = 0;
        DropNextBranch();
    }

    private void DropNextBranch(){
        //print(index);
        //print(branches.Count);
        if (index < branches.Count){
            //print("dropping branch #" + branches[index]);
            DropBranch(branches[index]);
            index ++;
            StaticVariables.WaitTimeThenCallFunction(timeBetweenFalls, DropNextBranch);
        }
    }

    private void DropBranch(CutsceneBranchData branch){
        float totalDistance = (branch.droppedPosition - new Vector2(branch.transform.localPosition.x, branch.transform.localPosition.y)).magnitude;
        float totalDuration = totalDistance/dropSpeed;
        branch.transform.DOLocalMove(branch.droppedPosition, totalDuration).SetEase(Ease.Linear);
    }


    /*
    public void QueueFall(){
        StaticVariables.WaitTimeThenCallFunction(delay, StartFall);
    }

    private void StartFall(){
        float totalDistance = (droppedPosition - new Vector2(transform.localPosition.x, transform.localPosition.y)).magnitude;
        float totalDuration = totalDistance/dropSpeed;
        transform.DOLocalMove(droppedPosition, totalDuration).SetEase(Ease.Linear);
        //float totalDuration = dropHeight / dropSpeed;
        //print(totalDuration);
        //transform.DOMoveY(transform.position.y - dropHeight, totalDuration).SetEase(Ease.Linear);

    }

    //void Update(){
    //    print(transform.position);
   // }
   */
}
