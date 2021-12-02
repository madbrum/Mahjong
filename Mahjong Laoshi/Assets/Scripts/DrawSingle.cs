using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSingle : MonoBehaviour
{
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        gameObject.hideFlags = HideFlags.HideInHierarchy;
        gameObject.SetActive(false);
    }
    
    public void OnClick()
    {
        Debug.Log("Clicked!");
        dealSingle();
        gameManager.logDraw();
        gameManager.testState();
    }

    private void dealSingle()
    {
        int currentPlayer = gameManager.getCurrentPlayer();
        GameObject area = gameManager.getArea(currentPlayer);
        List<GameObject> leftovers = gameManager.getHand(GameManager.LEFTOVER);
        //1=east 2=south 3=west 4=north
        int randomIndex = Random.Range(0, leftovers.Count);
        GameObject single = leftovers[randomIndex];
        gameManager.getHand(currentPlayer).Add(single);
        leftovers.RemoveAt(randomIndex);
        single.hideFlags = HideFlags.None;
        single.SetActive(true);
        single.transform.SetParent(area.transform, false);
    }

}
