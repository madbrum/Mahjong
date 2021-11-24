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

    private GameManager gameManager;

    private List<GameObject> eastTiles = new List<GameObject>();
    private List<GameObject> southTiles = new List<GameObject>();
    private List<GameObject> westTiles = new List<GameObject>();
    private List<GameObject> northTiles = new List<GameObject>();

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void Start()
    {
        TileSprites = Resources.LoadAll<Sprite>("TileSprites");
        //27 normal + 4 directions + 3 dragons 
        for (int i = 0; i < TileSprites.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject single = Instantiate(Tile, new Vector3(0, 0, 0), Quaternion.identity);
                single.GetComponent<Image>().sprite = TileSprites[i];
                single.GetComponent<TileProperties>().setID(i/9);
                single.GetComponent<TileProperties>().setValue((i % 9) + 1);
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
            dealSingle(eastTiles, EastArea);
            dealSingle(southTiles, SouthArea);
            dealSingle(westTiles, WestArea);
            dealSingle(northTiles, NorthArea);
        }
        gameManager.initHand(eastTiles, GameManager.EAST);
        gameManager.initHand(southTiles, GameManager.SOUTH);
        gameManager.initHand(westTiles, GameManager.WEST);
        gameManager.initHand(northTiles, GameManager.NORTH);
        gameManager.initHand(tiles, GameManager.LEFTOVER);

        gameManager.testState();

        GameObject.Destroy(gameObject);
    }

    private void dealSingle(List<GameObject> hand, GameObject area)
    {
        //1=east 2=south 3=west 4=north
        int randomIndex = Random.Range(0, tiles.Count);
        GameObject single = tiles[randomIndex];
        hand.Add(single);
        tiles.RemoveAt(randomIndex);
        single.hideFlags = HideFlags.None;
        single.SetActive(true);
        single.transform.SetParent(area.transform, false);
    }
}
