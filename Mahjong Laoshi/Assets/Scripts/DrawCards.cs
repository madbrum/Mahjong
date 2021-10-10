using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : MonoBehaviour
{
    public GameObject Dot1;
    public GameObject Bam8;
    public GameObject PlayerArea;
    public GameObject OpponentArea;
    public GameObject DealArea;

    List<GameObject> tiles = new List<GameObject>();

    void Start()
    {
        tiles.Add(Dot1);
        tiles.Add(Bam8);
    }

    public void OnClick()
    {
        for (int i = 0; i < 13; i++)
        {
            GameObject playerTile = Instantiate(tiles[Random.Range(0, tiles.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            playerTile.transform.SetParent(PlayerArea.transform, false);

            GameObject opponentTile = Instantiate(tiles[Random.Range(0, tiles.Count)], new Vector3(0, 0, 0), Quaternion.identity);
            opponentTile.transform.SetParent(OpponentArea.transform, false);
        }
    }
}
