using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneBranchFall : MonoBehaviour{

    public float dropHeight;
    private float dropSpeed = 300f;
    public float delay;

    public void QueueFall(){
        StaticVariables.WaitTimeThenCallFunction(delay, StartFall);
    }

    private void StartFall(){
        float totalDuration = dropHeight / dropSpeed;
        //print(totalDuration);
        transform.DOMoveY(transform.position.y - dropHeight, totalDuration).SetEase(Ease.Linear);

    }

    //void Update(){
    //    print(transform.position);
   // }





    /*
    private bool keepThrowing = true;

    public Sprite magicBook;
    public List<Sprite> normalBooks;

    public float timeBetweenBooks = 0.5f;

    public GameObject tossedBookPrefab;

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
