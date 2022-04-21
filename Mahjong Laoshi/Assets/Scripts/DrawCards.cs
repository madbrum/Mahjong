using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    private List<GameObject>[] hands;
    private List<GameObject> areas;

    public GameObject toggleButton;
    public GameObject OKButton;

    public GameObject instructions;

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
        hands = new List<GameObject>[]{ eastTiles, southTiles, westTiles, northTiles };
        areas = new List<GameObject> { EastArea, SouthArea, WestArea, NorthArea };
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
        string[] test = readTest();
        if (test == null)
        {
            for (int i = 0; i < 13; i++)
            {
                dealSingle(eastTiles, EastArea, GameManager.EAST);
                dealSingle(southTiles, SouthArea, GameManager.SOUTH);
                dealSingle(westTiles, WestArea, GameManager.WEST);
                dealSingle(northTiles, NorthArea, GameManager.NORTH);
            }
            dealSingle(eastTiles, EastArea, GameManager.EAST);
        }
        else
        {
            instantiateTest(test);
        }

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

        instructions.SetActive(false);
        gameManager.giveText(instructions);

        gameManager.logDraw();

        gameObject.hideFlags = HideFlags.HideInHierarchy;
        gameObject.SetActive(false);
    }

    private void dealSingle(List<GameObject> hand, GameObject area, int player)
    {
        int randomIndex = Random.Range(0, tiles.Count);
        GameObject single = tiles[randomIndex];
        hand.Add(single);
        tiles.RemoveAt(randomIndex);
        single.hideFlags = HideFlags.None;
        single.SetActive(true);
        single.GetComponent<TileProperties>().setPlayer(player);
        single.transform.SetParent(area.transform, false);
    }

    private void dealSingle(List<GameObject> hand, GameObject area, int index, int player)
    {
        GameObject single = tiles[index];
        hand.Add(single);
        tiles.RemoveAt(index);
        single.hideFlags = HideFlags.None;
        single.SetActive(true);
        single.GetComponent<TileProperties>().setPlayer(player);
        single.transform.SetParent(area.transform, false);
    }

    private string[] readTest()
    {
        TextAsset testInput = Resources.Load<TextAsset>("TEST file input");
        string[] txthands = Regex.Split(testInput.ToString(), "\r\n|\r|\n");
        if (txthands[0].Equals("empty"))
        {
            return null;
        }
        else
        {
            return txthands;
        }
        
    }

    private void instantiateTest(string[] txthands)
    {
        List<int> randoms = new List<int>();
        for (int i = 0; i < txthands.Length; i++)
        {
            if (txthands[i].Equals("random"))
            {
                randoms.Add(i);
            }
            else
            {
                string[] handTilesTxt = txthands[i].Split(",".ToCharArray());
                for (int j = 0; j < handTilesTxt.Length; j++)
                {
                    int id = int.Parse(handTilesTxt[j].Substring(0, 1));
                    int value = int.Parse(handTilesTxt[j].Substring(2));
                    int index = getTileIndex(id, value);
                    dealSingle(hands[i], areas[i], index, i);
                }
            }
        }
        for (int m = 0; m < randoms.Count; m++)
        {
            for (int j = 0; j < 13; j++)
            {
                dealSingle(hands[randoms[m]], areas[randoms[m]], randoms[m]);
            }
            if (randoms[m] == GameManager.EAST)
            {
                dealSingle(hands[randoms[m]], areas[randoms[m]], randoms[m]);
            }
        }
    }

    private int getTileIndex(int id, int value)
    {
        int tileIndex = -1;
        for (int i = 0; i < tiles.Count; i++)
        {
            TileProperties props = tiles[i].GetComponent<TileProperties>();
            if (props.getID() == id && props.getValue() == value)
            {
                tileIndex = i;
                break;
            }
        }
        return tileIndex;
    }
}
