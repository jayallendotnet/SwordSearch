using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class StaticVariables
{
    static public Transform tweenDummy;
    static public System.Random rand = new System.Random();
    static public BattleData battleData = null;
    static private string sceneName = "";
    static public Image fadeImage;
    static public float sceneFadeDuration = 0.5f;


    static public void WaitTimeThenCallFunction(float delay, TweenCallback function) {
        tweenDummy.DOLocalMove(tweenDummy.transform.localPosition, delay, false).OnComplete(function);
    }

    static public bool IsAnimatorInIdleState(Animator animator){
        //requires idle animation is named "GoblinIdle" or "OgreIdle", etc
        //also requires parent is named "Goblin" or "Ogre", etc
        string stateName = animator.gameObject.name + "Idle";
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    static public bool IsAnimatorInDamageState(Animator animator){
        //requires idle animation is named "GoblinDamage" or "OgreDamage", etc
        //also requires parent is named "Goblin" or "Ogre", etc
        string stateName = animator.gameObject.name + "Damage";
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    static public string GetAnimatorDieStateName(Animator animator){
        string stateName = animator.gameObject.name + "Die";
        return stateName;
    }

    static public void FadeOutThenLoadScene(string name){
        sceneName = name;
        Color currentColor = Color.black;
        currentColor.a = 0;
        fadeImage.color = currentColor;
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOColor(Color.black, sceneFadeDuration).OnComplete(LoadScene);
    }

    static public void FadeIntoScene(){
        Color nextColor = Color.black;
        nextColor.a = 0;
        fadeImage.color = Color.black;
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOColor(nextColor, sceneFadeDuration).OnComplete(HideFadeObject);
    }

    static private void HideFadeObject(){
        fadeImage.gameObject.SetActive(false);
    }

    static private void LoadScene(){
        SceneManager.LoadScene(sceneName);
    }

}