using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneTreeMimic : MonoBehaviour{

    public Transform originalTree;

    void Update(){
        transform.localPosition = originalTree.localPosition;
        //print("copying tree position: " + originalTree.localPosition.x);
    }

    public void DestroyScript(){
        Destroy(this);
    }
}
