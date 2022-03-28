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
                if ((props.getPlayer() != GameManager.EAST && getPlayerAttribute(props.transform.parent.gameObject) != GameManager.EAST) && getPlayerAttribute(tile.transform.parent.gameObject) != GameManager.DISCARD)
                {
                    if (!halted || (halted && !tile.Equals(questionTile)))
                    {
                        props.toggleHide();
                    }
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
        Debug.Log("\t\t\tBEGIN: " + this.name + " unhalt()");
        Debug.Log("\t\t\t\tTemp value is " + temp);
        currentPlayer = temp;
        temp = 4;
        Debug.Log("\t\t\t\tUnhalted. Current player: " + currentPlayer);
        Debug.Log("\t\t\t\tTemp is now " + temp);
        halted = false;
        Debug.Log("\t\t\tEND: " + this.name + " unhalt()");
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

    public void moveTile(GameObject tileP, int originID, int destinationID)
    {
        Debug.Log("\t\t\tBEGIN: " + this.name + " moveTile(int id, int value, int originID, int destinationID) with parameters: tileP = " + tileP.name + " OriginID = " + originID + " DestinationID = " + destinationID);
        testHand(originID);
        testHand(destinationID);
        int tileIndex = getTileIndex(tileP, originID);
        Debug.Log("Tile Index is " + tileIndex);
        GameObject tile = hands[originID][tileIndex];
        hands[originID].RemoveAt(tileIndex);
        hands[destinationID].Add(tile);
        testHand(originID);
        testHand(destinationID);
        Debug.Log("Moved tile from " + originID + " to " + destinationID + ". Id and Value are " + tileP.name);
    }

    public int getPlayerAttribute(GameObject area)
    {
        return areas.IndexOf(area);
    }

    public void officiate()
    {
        Debug.Log("\tBEGIN: " + this.name + " officiate()");
        Debug.Log("\tTile in question: " + analyzeTile(questionTile));
        testHand(GameManager.DISCARD);
        Debug.Log(questionTile.transform.parent.name);
        questionTile.GetComponent<DragDrop>().officiate(checkValidMeld(questionTile, questionTile.transform.parent.gameObject, true));
        Debug.Log("\tEND: " + this.name + " officiate()");
    }

    //TODO: overload that returns the potential formed melds? we need to know so we can reveal them and change their flags 
    //form two lists for dupes and incs and if there's enough dupes to form a valid meld, return that first. otherwise if the incs are valid, return that. otherwise return empty list 
    public bool checkValidMeld(GameObject tile, GameObject destination, bool playerSelect)
    {
        int player = getPlayerAttribute(destination);
        List<GameObject> destHand = new List<GameObject>(hands[player]);
        destHand.Add(tile);
        destHand.Sort(new TileComparer());
        printList(destHand);
        printList(hands[GameManager.DISCARD]);

        int tileQV = tile.GetComponent<TileProperties>().getValue();
        int tileQID = tile.GetComponent<TileProperties>().getID();
        int dupeTiles = 0;
        int incTiles = 0;
        int prevValue = 0;
        int prevID = destHand[0].GetComponent<TileProperties>().getID();
        for (int i = 0; i < destHand.Count; i++)
        {
            bool selected = destHand[i].GetComponent<TileProperties>().getSelect();
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
            Debug.Log("Test is as follows: ");
            Debug.Log("selected && tileQID < 3 && curID == tileQID && curValue >= tileQV - 2 && curValue <= tileQV + 2");
            Debug.Log(selected + " " + (tileQID < 3) + " " + (curID == tileQID) + " " + (curValue >= tileQV - 2) + " " + (curValue <= tileQV + 2));
            Debug.Log("curID = " + curID + ", tileQID = " + tileQID + ", prevID = " + prevID + ", curValue = " + curValue + ", tileQV = " + tileQV + " incTiles = " + incTiles + " prevValue = " + prevValue);
            if (selected && tileQID < 3 && curID == tileQID && curValue >= tileQV - 2 && curValue <= tileQV + 2)
            {
                Debug.Log("Selected reading count");
                if ((prevValue == 0 || curValue == prevValue + 1) && prevValue != curValue)
                {
                    incTiles++;
                    prevValue = curValue;
                    Debug.Log("IncTiles increased. Now " + incTiles);
                }
                else if (prevValue != curValue)
                {
                    prevValue = curValue;
                    incTiles = 1;
                }
            }
            //else
            //{
            //    incTiles = 0;
            //    prevValue = 0;
            //}
            prevID = curID;

            if (dupeTiles >= 3)
            {
                Debug.Log("Valid Draw");
                return true;
            }
            Debug.Log("Test: incTiles >= 3 && getPlayerAttribute(destination) == temp");
            Debug.Log((incTiles >= 3) + " " + getPlayerAttribute(destination) + " == " + temp + " " + (getPlayerAttribute(destination) == temp));
            if (playerSelect)
            {
                if (incTiles >= 3 && getPlayerAttribute(destination) == temp)
                {
                    Debug.Log("Valid Draw");
                    return true;
                }
            }
            else
            {
                if (incTiles >= 3 && getPlayerAttribute(destination) == currentPlayer)
                {
                    Debug.Log("Valid Draw");
                    return true;
                }
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
        Debug.Log("\t\t\tBEGIN: " + this.name + " disableSelection(int player, bool valid)" + " parameters: player = " + player + ", valid = " + valid);
        testHand(player);
        clicks = 0;
        int test = 0;
        int melds = 0;
        foreach (GameObject tile in hands[player])
        {
            tile.GetComponent<Button>().enabled = false;
            if (valid)
            {
                test++;
                if (tile.GetComponent<TileProperties>().getSelect())
                {
                    Debug.Log("Tile is being melded. Tile properties: " + analyzeTile(tile));
                    melds++;
                    tile.GetComponent<TileProperties>().meld();
                    Transform parent = tile.transform.parent;
                    tile.transform.SetParent(null, false);
                    tile.transform.SetParent(parent, false);
                    Debug.Log("Tile after melding: " + analyzeTile(tile));
                }
            }
            tile.GetComponent<TileProperties>().deselect();
            Debug.Log("Tile after running through disableSelection. Selected must be false now. " + analyzeTile(tile));
        }
        Debug.Log("Number of valid counts : " + test + ", number of tiles that were melded: " + melds);
        testState();
        printList(hands[player]);
        Debug.Log("\t\t\tEND: " + this.name + " disableSelection(int player, bool valid)" + " parameters: player = " + player + ", valid = " + valid);
    }

    private int getTileIndex(GameObject tile, int origin)
    {
        int tileIndex = -1;
        for (int i = 0; i < hands[origin].Count; i++)
        {
            TileProperties props = hands[origin][i].GetComponent<TileProperties>();
            if (props.getKey() == tile.GetComponent<TileProperties>().getKey())
            {
                tileIndex = i;
                break;
            }
        }
        return tileIndex;
    }

    private int getTileIndex(int id, int value, int origin)
    {
        int tileIndex = -1;
        for (int i = hands[origin].Count - 1; i >= 0; i--)
        {
            TileProperties props = hands[origin][i].GetComponent<TileProperties>();
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

    //private int[,] buildMatrix(List<GameObject> hand)
    //{
    //    int[,] handMatrix = new int[4,9];
    //    return handMatrix;
    //    for (int i = 0; i < handMatrix.Length; i++)
    //    {
    //        for (int j = 0; j < handMatrix.GetLength(1); j++)
    //        {

    //        }
    //    }
    //}

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

    public void testHand(int player)
    {
        Debug.Log("Hand " + player + " with Size " + hands[player].Count);
        printList(hands[player]);
    }

    public void printList(List<GameObject> tiles)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Debug.Log("\tTile " + i + ": ");
            Debug.Log("\t" + analyzeTile(tiles[i]));
        }
    }

    public string analyzeTile(GameObject tile)
    {
        TileProperties props = tile.GetComponent<TileProperties>();
        return "ID: " + props.getID() + ", Value: " + props.getValue() + ", Player ID: " + props.getPlayer() + ", Discarded? " + props.getDiscard() + ", Hidden? " + props.getHidden() + ", Selected? " + props.getSelect() + ", Melded? " + props.getMeld();
    }
}
