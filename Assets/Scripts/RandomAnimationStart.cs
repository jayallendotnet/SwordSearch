using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomAnimationStart : MonoBehaviour{

    void Start(){
        Animator anim = GetComponent<Animator>();
        //print(anim.runtimeAnimatorController.name);
        anim.Play(anim.runtimeAnimatorController.name, 0, (float)StaticVariables.rand.NextDouble());
        Destroy(this);
    }

}
