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
        gameObject.GetComponent<TileProperties>().select();
        gameManager.incrementClick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
