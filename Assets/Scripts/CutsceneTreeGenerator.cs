using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneTreeGenerator : MonoBehaviour{

    //private bool keepThrowing = true;
    public Sprite treeSprite;
    public List<GameObject> treePrefabs;
    public List<Color> treeColors;

    //public Sprite magicBook;
    //public List<Sprite> normalBooks;

    public float timeBetweenTrees = 0.5f;
    public float treeMoveDistance = -500;
    public float treeMoveTime = 4f;

    //public GameObject tossedBookPrefab;

    void Start(){
        CreateTree();
        //create all the initial trees
        //wait time then create next tree
    }

    private void CreateTree(){
        print("creating tree");
        int treeNum = StaticVariables.rand.Next(0, treePrefabs.Count);
        treeNum = 0;
        GameObject prefab = treePrefabs[treeNum];
        Color color = treeColors[StaticVariables.rand.Next(0,treeColors.Count)];
        //GameObject prefab = treePrefabs[0];
        GameObject tree = GameObject.Instantiate(prefab, transform);
        tree.GetComponent<Image>().color = color;
        tree.transform.localPosition = Vector3.zero;
        tree.transform.DOLocalMoveX(tree.transform.localPosition.x + treeMoveDistance, treeMoveTime, true).SetEase(Ease.Linear);
        if (treeNum == 0)
            StaticVariables.WaitTimeThenCallFunction(timeBetweenTrees, CreateTree);
        else
            StaticVariables.WaitTimeThenCallFunction(timeBetweenTrees * 0.7f, CreateTree);
        StaticVariables.WaitTimeThenCallFunction(treeMoveTime, GameObject.Destroy, tree);

    }

    /*
    public void StartThrow(){
        keepThrowing = true;
        ThrowRandomBook();
        StaticVariables.WaitTimeThenCallFunction(timeBetweenBooks, ThrowRandomBook);
        //the magic book is always the third book thrown out
        StaticVariables.WaitTimeThenCallFunction(timeBetweenBooks * 2, ThrowMagicBook);
        //then, start the process that continuously throws out random books until stopped
        StaticVariables.WaitTimeThenCallFunction(timeBetweenBooks * 3, ThrowBookAndQueueNextBook);
    }

    public void StopThrow(){
        keepThrowing = false;
    }

    private void ThrowRandomBook(){
        ThrowBook(normalBooks[StaticVariables.rand.Next(normalBooks.Count)]);
        //pick a random book
        //throw the book
    }

    private void ThrowBook(Sprite bookSprite){
        GameObject newBook = Instantiate(tossedBookPrefab, transform);
        //newBook.transform.SetParent(transform);
        newBook.GetComponent<Image>().sprite = bookSprite;
    }

    private void ThrowMagicBook(){
        ThrowBook(magicBook);
    }

    private void ThrowBookAndQueueNextBook(){
        if (!keepThrowing)
            return;
        ThrowRandomBook();
        StaticVariables.WaitTimeThenCallFunction(timeBetweenBooks, ThrowBookAndQueueNextBook);
    }
    */

}
