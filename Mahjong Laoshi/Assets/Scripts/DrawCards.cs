using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCards : MonoBehaviour
{
    public GameObject Tile;
    public GameObject EastArea;
    public GameObject SouthArea;
    public GameObject WestArea;
    public GameObject NorthArea;
    public GameObject DealArea;

    private List<GameObject> tiles = new List<GameObject>();
    private Sprite[] TileSprites;

    void Start()
    {
        TileSprites = Resources.LoadAll<Sprite>("TileSprites");
        for (int i = 0; i < TileSprites.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject single = Instantiate(Tile, new Vector3(0, 0, 0), Quaternion.identity);
                single.GetComponent<Image>().sprite = TileSprites[i];
                single.hideFlags = HideFlags.HideInHierarchy;
                single.SetActive(false);
                tiles.Add(single);
            }
        }
    }

    public void OnClick()
    {
        for (int i = 0; i < 13; i++)
        {
            int randomIndex = Random.Range(0, tiles.Count);
            GameObject eastTile = tiles[randomIndex];
            tiles.RemoveAt(randomIndex);
            eastTile.hideFlags = HideFlags.None;
            eastTile.SetActive(true);
            eastTile.transform.SetParent(EastArea.transform, false);


            int randomOppIndex = Random.Range(0, tiles.Count);
            GameObject westTile = tiles[randomOppIndex];
            tiles.RemoveAt(randomOppIndex);
            westTile.hideFlags = HideFlags.None;
            westTile.SetActive(true);
            westTile.transform.SetParent(WestArea.transform, false);

            randomOppIndex = Random.Range(0, tiles.Count);
            GameObject southTile = tiles[randomOppIndex];
            tiles.RemoveAt(randomOppIndex);
            southTile.hideFlags = HideFlags.None;
            southTile.SetActive(true);
            southTile.transform.SetParent(SouthArea.transform, false);

            randomOppIndex = Random.Range(0, tiles.Count);
            GameObject northTile = tiles[randomOppIndex];
            tiles.RemoveAt(randomOppIndex);
            northTile.hideFlags = HideFlags.None;
            northTile.SetActive(true);
            northTile.transform.SetParent(NorthArea.transform, false);
        }
    }
}
