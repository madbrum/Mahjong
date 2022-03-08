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
        gameManager.incrementClick();
        if (gameManager.getClicks() >= 3)
        {
            gameManager.officiate();
        }
        Debug.Log("END: " + this.name + " OnClick()");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
