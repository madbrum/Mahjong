using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static int EAST { get; private set; } = 0;
    public static int SOUTH { get; private set; } = 1;
    public static int WEST { get; private set; } = 2;
    public static int NORTH { get; private set; } = 3;
    public static int DISCARD { get; private set; } = 4;
    public static int LEFTOVER { get; private set; } = 5;

    private List<GameObject>[] hands = new List<GameObject>[6];
    private List<GameObject> areas = new List<GameObject>();
    private List<GameObject> titles = new List<GameObject>();
    private Sprite[] titleImgs = new Sprite[8];

    private int currentPlayer = EAST;
    private bool drawn = false;
    private bool discarded = false;

    private bool hidden = false;

    public static GameManager Instance { get; private set; }

    private class TileComparer : IComparer<GameObject>
    {
        public int Compare(GameObject tile1, GameObject tile2)
        {
            int tile1Value = tile1.GetComponent<TileProperties>().getValue();
            int tile2Value = tile2.GetComponent<TileProperties>().getValue();
            if (tile1Value > tile2Value)
            {
                return 1;
            }
            else if (tile1Value == tile2Value)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Warning: multiple " + this + " in scene!");
        }
        hands[DISCARD] = new List<GameObject>();
        titleImgs = Resources.LoadAll<Sprite>("Titles");
    }

    public int getCurrentPlayer()
    {
        return currentPlayer;
    }

    public GameObject getArea(int player)
    {
        return areas[player];
    }

    public List<GameObject> getHand(int player)
    {
        return hands[player];
    }

    public void logDiscard()
    {
        discarded = true;
    }

    public void logDraw()
    {
        drawn = true;
    }

    public void toggleHide()
    {
        hidden = !hidden;
        foreach (List<GameObject> hand in hands)
        {
            foreach (GameObject tile in hand)
            {
                TileProperties props = tile.GetComponent<TileProperties>();
                if (props.getPlayer() != GameManager.EAST && props.getPlayer() != GameManager.DISCARD)
                {
                    props.toggleHide();
                }
            }
        }

    }

    public bool drawStatus()
    {
        return drawn;
    }

    public bool discardStatus()
    {
        return discarded;
    }

    public bool getHideStatus()
    {
        return hidden;
    }

    public void logPlayer(int player)
    {    
        titles[currentPlayer].GetComponent<Image>().sprite = titleImgs[currentPlayer];
        currentPlayer = player;
        titles[currentPlayer].GetComponent<Image>().sprite = titleImgs[currentPlayer + 4];
    }

    public void initHand(List<GameObject> hand, int player)
    {
        hands[player] = hand;
    }
    public void initArea(GameObject area)
    {
        //areas[player] = area;
        areas.Add(area);
    }

    public void initTitle(GameObject title)
    {
        titles.Add(title);
    }

    public void moveTile(int id, int value, GameObject originP, GameObject destinationP)
    {
        int origin = getPlayerAttribute(originP);
        int destination = getPlayerAttribute(destinationP);
        GameObject tile = hands[origin][getTileIndex(id, value, origin)];
        hands[origin].Remove(tile);
        hands[destination].Add(tile);
    }

    public int getPlayerAttribute(GameObject area)
    {
        return areas.IndexOf(area);
    }

    public bool checkValidMeld(GameObject tile, GameObject destination)
    {
        int player = getPlayerAttribute(destination);
        List<GameObject> destHand = new List<GameObject>(hands[player]);
        List<GameObject> tilesOfSuit = new List<GameObject>();
        for (int i = 0; i < destHand.Count; i++)
        {
            if(destHand[i].GetComponent<TileProperties>().getID() == tile.GetComponent<TileProperties>().getID())
            {
                tilesOfSuit.Add(destHand[i]);
            }
        }
        tilesOfSuit.Add(tile);
        tilesOfSuit.Sort(new TileComparer());
        printList(tilesOfSuit);
        int tileQV = tile.GetComponent<TileProperties>().getValue();
        int dupeTiles = 0;
        int incTiles = 0;
        int prevValue = 0;
        for (int i = 0; i < tilesOfSuit.Count; i++)
        {
            int curValue = tilesOfSuit[i].GetComponent<TileProperties>().getValue();
            Debug.Log(dupeTiles + " Current tile: " + curValue);
            Debug.Log(incTiles);
            Debug.Log(prevValue);
            if (curValue == tileQV)
            {
                dupeTiles++;
            }
            //how do we make it read for this tile specifically?
            if (curValue >= tileQV - 2 && curValue <= tileQV + 2)
            {
                if (prevValue == 0 || curValue == prevValue + 1)
                {
                    incTiles++;
                }
                else
                {
                    incTiles = 1;
                }
                // move outside if statement?
                prevValue = curValue;
            }

            if (dupeTiles >= 3)
            {
                Debug.Log("Valid Draw");
                return true;
            }
            if (incTiles >= 3 && getPlayerAttribute(destination) == currentPlayer)
            {
                Debug.Log("Valid Draw");
                return true;
            }
        }
        Debug.Log("Illegal Draw");
        return false;
    }

    private int getTileIndex(int id, int value, int origin)
    {
        //if -1 returns that means the tile isn't present in this hand . probably need to add checks for this later
        int tileIndex = -1;
        for (int i = 0; i < hands[origin].Count; i++)
        {
            TileProperties props = hands[origin][i].GetComponent<TileProperties>();
            if (props.getID() == id && props.getValue() == value)
            {
                tileIndex = i;
                break;
            }
        }
        return tileIndex;
    }

    private void Update()
    {
        if (drawn && discarded)
        {
            titles[currentPlayer].GetComponent<Image>().sprite = titleImgs[currentPlayer];
            currentPlayer = (currentPlayer + 1) % 4;
            titles[currentPlayer].GetComponent<Image>().sprite = titleImgs[currentPlayer + 4];
            drawn = false;
            discarded = false;
        }
    }

    public void testState()
    {
        Debug.Log("East Size equals " + hands[EAST].Count);
        Debug.Log("South Size equals " + hands[SOUTH].Count);
        Debug.Log("West Size equals " + hands[WEST].Count);
        Debug.Log("North Size equals " + hands[NORTH].Count);
        Debug.Log("Drawn? " + drawn);
        Debug.Log("Discarded? " + discarded);
        Debug.Log("Current player is " + currentPlayer);
    }

    public void printList(List<GameObject> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Debug.Log(tiles[i].GetComponent<TileProperties>().getValue());
        }
    }
}
