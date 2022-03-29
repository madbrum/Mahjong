using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeldSelect : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void OnClick()
    {
        Debug.Log("BEGIN: " + this.name + " OnClick()");
        gameObject.GetComponent<TileProperties>().select();
        gameManager.testHand(GameManager.DISCARD);
        Debug.Log(gameObject.transform.parent.name);
        gameManager.incrementClick();
        Debug.Log("END: " + this.name + " OnClick()");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
