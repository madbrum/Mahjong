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
    private bool halted = false;

    private int clicks = 0;
    private int temp = 4;
    private GameObject questionTile;

    public static GameManager Instance { get; private set; }

    private class TileComparer : IComparer<GameObject>
    {
        public int Compare(GameObject tile1, GameObject tile2)
        {
            int tile1Value = tile1.GetComponent<TileProperties>().getValue();
            int tile2Value = tile2.GetComponent<TileProperties>().getValue();
            int tile1ID = tile1.GetComponent<TileProperties>().getID();
            int tile2ID = tile2.GetComponent<TileProperties>().getID();
            if (tile1ID > tile2ID)
            {
                return 1;
            }
            else if (tile1ID == tile2ID)
            {
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
            Debug.Log("GameManager initialized");
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

    public void incrementClick()
    {
        clicks++;
    }

    public int getClicks()
    {
        return clicks;
    }

    //TODO: fix bug where the discards that were just discarded get flipped 
    public void toggleHide()
    {
        hidden = !hidden;
        foreach (List<GameObject> hand in hands)
        {
            foreach (GameObject tile in hand)
            {
                TileProperties props = tile.GetComponent<TileProperties>();
                if (props.getPlayer() != GameManager.EAST && getPlayerAttribute(tile.transform.parent.gameObject) != GameManager.DISCARD)
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

    public void halt(GameObject haltTile)
    {
        Debug.Log("Current player before halt: " + currentPlayer);
        temp = currentPlayer;
        currentPlayer = 4;
        Debug.Log("Temp is now " + temp);
        Debug.Log("Current player is now " + currentPlayer);
        questionTile = haltTile;
        drawn = false;
        discarded = false;
        halted = true;
    }

    public void unhalt()
    {
        Debug.Log("Temp value is " + temp);
        currentPlayer = temp;
        temp = 4;
        Debug.Log("Unhalted. Current player: " + currentPlayer);
        Debug.Log("Temp is now " + temp);
        halted = false;
    }

    public bool getHalt()
    {
        return halted;
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
        Debug.Log("Origin: "  + origin);
        int destination = getPlayerAttribute(destinationP);
        Debug.Log("Index in origin hand is " + getTileIndex(id, value, origin));
        GameObject tile = hands[origin][getTileIndex(id, value, origin)];
        hands[origin].Remove(tile);
        hands[destination].Add(tile);
        Debug.Log(hands[destination].Count);
    }

    public int getPlayerAttribute(GameObject area)
    {
        return areas.IndexOf(area);
    }

    public void officiate()
    {
        Debug.Log("ID: " + questionTile.GetComponent<TileProperties>().getID() + "Value: " + questionTile.GetComponent<TileProperties>().getValue());
        questionTile.GetComponent<DragDrop>().officiate(checkValidMeld(questionTile, questionTile.transform.parent.gameObject, true));
    }

    //TODO: overload that returns the potential formed melds? we need to know so we can reveal them and change their flags 
    //form two lists for dupes and incs and if there's enough dupes to form a valid meld, return that first. otherwise if the incs are valid, return that. otherwise return empty list 
    public bool checkValidMeld(GameObject tile, GameObject destination, bool playerSelect)
    {
        int player = getPlayerAttribute(destination);
        List<GameObject> destHand = new List<GameObject>(hands[player]);
        destHand.Add(tile);
        destHand.Sort(new TileComparer());


        int tileQV = tile.GetComponent<TileProperties>().getValue();
        int tileQID = tile.GetComponent<TileProperties>().getID();
        int dupeTiles = 0;
        int incTiles = 0;
        int prevValue = 0;
        int prevID = destHand[0].GetComponent<TileProperties>().getID();
        for (int i = 0; i < destHand.Count; i++)
        {
            bool selected = tile.GetComponent<TileProperties>().getSelect();
            if (!playerSelect)
            {
                selected = true;
            }
            int curValue = destHand[i].GetComponent<TileProperties>().getValue();
            int curID = destHand[i].GetComponent<TileProperties>().getID();
            if (selected && curID == tileQID && curValue == tileQV)
            {
                Debug.Log("Selected reading dupe");
                dupeTiles++;
            }
            if (selected && curID == tileQID && curID == prevID && !(tileQID >= 3) && curValue >= tileQV - 2 && curValue <= tileQV + 2)
            {
                Debug.Log("Selected reading count");
                if (prevValue == 0 || curValue == prevValue + 1)
                {
                    incTiles++;
                }
                else
                {
                    incTiles = 1;
                }
                // move outside if statement?
            }
            else
            {
            }

            prevValue = curValue;
            prevID = curID;

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

    public void enableSelection(int player)
    {
        clicks = 0;
        foreach (GameObject tile in hands[player])
        {
            tile.GetComponent<Button>().enabled = true;
        }
    }
    
    public void disableSelection(int player, bool valid)
    {
        clicks = 0;
        foreach (GameObject tile in hands[player])
        {
            tile.GetComponent<Button>().enabled = false;
            if (valid)
            {
                if (tile.GetComponent<TileProperties>().getSelect())
                {
                    tile.GetComponent<TileProperties>().meld();
                }
                tile.GetComponent<TileProperties>().deselect();
            }
        }
        testState();
        printList(hands[player]);
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

    private int[,] buildMatrix(List<GameObject> hand)
    {
        int[,] handMatrix = new int[4,9];
        return handMatrix;
        for (int i = 0; i < handMatrix.Length; i++)
        {
            for (int j = 0; j < handMatrix.GetLength(1); j++)
            {

            }
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
            Debug.Log("Value: " + tiles[i].GetComponent<TileProperties>().getValue() + "ID: " + tiles[i].GetComponent<TileProperties>().getID());
        }
    }
}
