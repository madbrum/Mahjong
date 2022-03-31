using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DrawCards : MonoBehaviour
{
    public GameObject Tile;

    public GameObject EastWall;
    public GameObject SouthWall;
    public GameObject WestWall;
    public GameObject NorthWall;

    public GameObject EastArea;
    public GameObject SouthArea;
    public GameObject WestArea;
    public GameObject NorthArea;
    public GameObject DealArea;

    public GameObject EastName;
    public GameObject SouthName;
    public GameObject WestName;
    public GameObject NorthName;

    private List<GameObject> tiles = new List<GameObject>();
    private Sprite[] TileSprites;

    private GameManager gameManager;

    private List<GameObject> eastTiles = new List<GameObject>();
    private List<GameObject> southTiles = new List<GameObject>();
    private List<GameObject> westTiles = new List<GameObject>();
    private List<GameObject> northTiles = new List<GameObject>();

    public GameObject toggleButton;
    public GameObject OKButton;

    //private void Awake()
    //{
    //    gameManager = GameManager.Instance;
    //    Debug.Log("GameManager instance set in DrawCards");
    //}

    void Start()
    {
        gameManager = GameManager.Instance;
        TileSprites = Resources.LoadAll<Sprite>("TileSprites");
        //27 normal + 4 directions + 3 dragons 
        int keyCount = 0;
        for (int i = 0; i < TileSprites.Length; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject single = Instantiate(Tile, new Vector3(0, 0, 0), Quaternion.identity);
                single.GetComponent<Button>().enabled = false;
                single.GetComponent<Image>().sprite = TileSprites[i];
                single.GetComponent<TileProperties>().setID(i/9);
                single.GetComponent<TileProperties>().setValue((i % 9) + 1);
                single.GetComponent<TileProperties>().setKey(keyCount);
                single.name = "Tile ID: " + single.GetComponent<TileProperties>().getID() + ", Tile Value: " + single.GetComponent<TileProperties>().getValue();
                single.hideFlags = HideFlags.HideInHierarchy;
                single.SetActive(false);
                tiles.Add(single);
                keyCount++;
            }
        }
    }

    public void OnClick()
    {
        readTest();
        for (int i = 0; i < 13; i++)
        {
            dealSingle(eastTiles, EastArea, GameManager.EAST);
            dealSingle(southTiles, SouthArea, GameManager.SOUTH);
            dealSingle(westTiles, WestArea, GameManager.WEST);
            dealSingle(northTiles, NorthArea, GameManager.NORTH);
        }
        dealSingle(eastTiles, EastArea, GameManager.EAST);
        gameManager.logDraw();

        gameManager.initHand(eastTiles, GameManager.EAST);
        gameManager.initHand(southTiles, GameManager.SOUTH);
        gameManager.initHand(westTiles, GameManager.WEST);
        gameManager.initHand(northTiles, GameManager.NORTH);
        gameManager.initHand(tiles, GameManager.LEFTOVER);

        gameManager.initArea(EastArea);
        gameManager.initArea(SouthArea);
        gameManager.initArea(WestArea);
        gameManager.initArea(NorthArea);
        gameManager.initArea(DealArea);

        gameManager.initTitle(EastName);
        EastName.GetComponent<Image>().sprite = Resources.Load<Sprite>("Titles/4east");
        gameManager.initTitle(SouthName);
        gameManager.initTitle(WestName);
        gameManager.initTitle(NorthName);

        EastWall.GetComponent<WallProperties>().setID(GameManager.EAST);
        EastWall.hideFlags = HideFlags.None;
        EastWall.SetActive(true);
        SouthWall.GetComponent<WallProperties>().setID(GameManager.SOUTH);
        SouthWall.hideFlags = HideFlags.None;
        SouthWall.SetActive(true);
        WestWall.GetComponent<WallProperties>().setID(GameManager.WEST);
        WestWall.hideFlags = HideFlags.None;
        WestWall.SetActive(true);
        NorthWall.GetComponent<WallProperties>().setID(GameManager.NORTH);
        NorthWall.hideFlags = HideFlags.None;
        NorthWall.SetActive(true);

        toggleButton.hideFlags = HideFlags.None;
        toggleButton.SetActive(true);
        gameManager.setButton(OKButton);

        GameObject.Destroy(gameObject);
    }

    private void dealSingle(List<GameObject> hand, GameObject area, int player)
    {
        //1=east 2=south 3=west 4=north
        int randomIndex = Random.Range(0, tiles.Count);
        GameObject single = tiles[randomIndex];
        hand.Add(single);
        tiles.RemoveAt(randomIndex);
        single.hideFlags = HideFlags.None;
        single.SetActive(true);
        single.GetComponent<TileProperties>().setPlayer(player);
        single.transform.SetParent(area.transform, false);
    }

    private string[] readTest()
    {
        string[] txthands = System.IO.File.ReadAllLines(@"C:\Users\s-brumleyma\Documents\GitHub\Mahjong\Mahjong Laoshi\Assets\TEST file input.txt");
        return txthands;
    }

    private void instantiateTest(string[] txthands)
    {
        for (int i = 0; i < txthands[0].Length; i++)
        {
            string[] handTilesTxt = txthands[0].Split(",".ToCharArray());
            for (int j = 0; j < handTilesTxt.Length; j++)
            {
                int id = int.Parse(handTilesTxt[j].Substring(0, 1));
                int value = int.Parse(handTilesTxt[j].Substring(2));
                int index = (id * 9) + (value - 1);
                //GameObject single = tiles[randomIndex];
                //hand.Add(single);
                //tiles.RemoveAt(randomIndex);
                //single.hideFlags = HideFlags.None;
                //single.SetActive(true);
                //single.GetComponent<TileProperties>().setPlayer(player);
                //single.transform.SetParent(area.transform, false);
                //id * 9 + (value - 1) 
            }
        }
    }

    private int getTileIndex(int id, int value, int origin)
    {
        int tileIndex = -1;
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            TileProperties props = tiles[i].GetComponent<TileProperties>();
            if (props.getID() == id && props.getValue() == value)
            {
                if (origin != GameManager.DISCARD)
                {
                    tileIndex = i;
                    break;
                }
                else if (props.getSelect())
                {
                    tileIndex = i;
                    break;
                }
            }
        }
        return tileIndex;
    }
}
