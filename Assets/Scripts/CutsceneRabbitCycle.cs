using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneRabbitCycle : MonoBehaviour{
    public float endDestination = 1000;
    public float moveSpeed = 700;
    public float spawnPoint = -1000;
    //private Vector2 initialPosition;
    //public 

    void Start(){
        Vector2 startingPoint = transform.localPosition;
        float distanceToTravel = MathF.Abs(endDestination - startingPoint.x);
        float timeToTravel = distanceToTravel / moveSpeed;

        transform.localPosition = startingPoint;
        transform.DOLocalMoveX(endDestination, timeToTravel).SetEase(Ease.Linear).OnComplete(StartMove);

        //initialPosition = transform.localPosition;
        //StartMove();
    }

    private void StartMove(){
        Vector2 startingPoint = new Vector2(spawnPoint, transform.localPosition.y);
        float distanceToTravel = MathF.Abs(endDestination - startingPoint.x);
        float timeToTravel = distanceToTravel / moveSpeed;

        transform.localPosition = startingPoint;
        transform.DOLocalMoveX(endDestination, timeToTravel).SetEase(Ease.Linear).OnComplete(StartMove);
    }
    
}
