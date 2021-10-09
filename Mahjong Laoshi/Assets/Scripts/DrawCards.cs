using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameObject OneDotTile;
    public GameObject EightBambooTile;
    public GameObject PlayerArea;
    public GameObject OpponentArea;
    public GameObject DealArea;

    void Start()
    {
        
    }

    public void OnClick()
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject playerTile = Instantiate(OneDotTile, new Vector3(0, 0, 0), Quaternion.identity);
            playerTile.transform.SetParent(PlayerArea.transform, false);
        }
    }
}
