using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneTreeGenerator : MonoBehaviour{
    public GameObject bigTreePrefab;
    public GameObject smallTreePrefab;
    public List<Color> treeColors;
    public float timeBetweenSmallTrees = 0.3f;
    public float timeBetweenBigTrees = 0.5f;
    public float timeBetweenBigAndSmallTrees = 0.4f;
    public float treeMoveDistance = -3000;
    public float treeMoveTime = 2.5f;
    private bool isNextTreeBig = false;
    private bool slowDown = false;
    public Transform finalTreeCluster;
    public bool isFirstFinalTreeBig = true;
    //public Transform otherStuffForFinalMove;
    public bool tellSynchronizerToStartMovingTree = false;
    public CutsceneTreeFinalSynchronizer synchronizer;

    void Start(){
        CreateTree();
    }

    private void CreateTree(){
        bool isCurrentTreeBig = isNextTreeBig;
        isNextTreeBig = StaticVariables.rand.Next(0,2) > 0;
        GameObject currentTreePrefab = smallTreePrefab;
        if (isCurrentTreeBig)
            currentTreePrefab = bigTreePrefab;
        Color color = treeColors[StaticVariables.rand.Next(0,treeColors.Count)];
        GameObject tree = GameObject.Instantiate(currentTreePrefab, transform);
        tree.GetComponent<Image>().color = color;
        tree.transform.localPosition = Vector3.zero;
        tree.transform.DOLocalMoveX(tree.transform.localPosition.x + treeMoveDistance, treeMoveTime).SetEase(Ease.Linear);
        StaticVariables.WaitTimeThenCallFunction(treeMoveTime, GameObject.Destroy, tree);
        if (slowDown){
            float timeUntilNextTree;
            isNextTreeBig = isFirstFinalTreeBig;
            if (isCurrentTreeBig && isNextTreeBig)
                timeUntilNextTree = timeBetweenBigTrees;
            else if (!isCurrentTreeBig  && !isNextTreeBig)
                timeUntilNextTree = timeBetweenSmallTrees;
            else
                timeUntilNextTree = timeBetweenBigAndSmallTrees;
            StaticVariables.WaitTimeThenCallFunction(timeUntilNextTree, StartFinalClusterMoving);
        }
        else{
            float timeUntilNextTree;
            if (isCurrentTreeBig && isNextTreeBig)
                timeUntilNextTree = timeBetweenBigTrees;
            else if (!isCurrentTreeBig  && !isNextTreeBig)
                timeUntilNextTree = timeBetweenSmallTrees;
            else
                timeUntilNextTree = timeBetweenBigAndSmallTrees;
            StaticVariables.WaitTimeThenCallFunction(timeUntilNextTree, CreateTree);
        }
    }

    public void BeginSlowdown(){
        slowDown = true;
    }

    private void StartFinalClusterMoving(){
        if (tellSynchronizerToStartMovingTree){
            synchronizer.StartMovingTree();
            //print(otherStuffForFinalMove.localPosition.x);
            //otherStuffForFinalMove.DOLocalMoveX(otherStuffForFinalMove.localPosition.x + treeMoveDistance, treeMoveTime).SetEase(Ease.Linear);
            //synchronizer.StandaloneTreeStartedMoving(otherStuffForFinalMove);
        }
        finalTreeCluster.DOLocalMoveX(finalTreeCluster.localPosition.x + treeMoveDistance, treeMoveTime).SetEase(Ease.Linear);
        synchronizer.AnotherClusterStartedMoving(finalTreeCluster);

    }
}
