using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneTreeFinalSynchronizer : MonoBehaviour{
    private float treeMoveDistance = -3000;
    private float treeMoveTime = 2.5f;
    private List<Transform> clusters = new();
    private Transform standaloneTree;
    private float standaloneTreeDestination = -1500;

    public GameObject mimicTreeScript;


    public void AnotherClusterStartedMoving(Transform cluster){
        clusters.Add(cluster);
        if (clusters.Count == 6){
            SynchronizeClusters();
        }
    }

    public void StandaloneTreeStartedMoving(Transform tree){
        standaloneTree = tree;
    }

    private void SynchronizeClusters(){
        //sync clusters so the standalone tree is in the middle of the screen by the end of it,
        //while maintaining that all clusters are moving at the same speed for the same time
        DOTween.Kill(standaloneTree);
        float standaloneTreeCurrentPosition = standaloneTree.localPosition.x;
        float standaloneTreeDistanceRemaining = standaloneTreeCurrentPosition - standaloneTreeDestination;
        float timeToMove = treeMoveTime * -(standaloneTreeDistanceRemaining/treeMoveDistance);

        standaloneTree.DOLocalMoveX(standaloneTree.localPosition.x -standaloneTreeDistanceRemaining, timeToMove).SetEase(Ease.Linear);

        foreach (Transform cluster in clusters){
            DOTween.Kill(cluster);
            cluster.DOLocalMoveX(cluster.localPosition.x -standaloneTreeDistanceRemaining, timeToMove).SetEase(Ease.Linear);
        }
        
        FindObjectOfType<CutsceneManager>().ExternalTrigger(timeToMove);
    }

    private void DeleteMimicTreeScript(){
        Destroy(mimicTreeScript);
    }
}
