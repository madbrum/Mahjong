using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private GameObject OKButton;
    private GameObject instructions;

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
        bool won = checkMahjong(currentPlayer);
        if (won)
        {
            win(currentPlayer);
        }
        drawn = true;
    }

    public void logDraw(int id, int value)
    {
        bool win = checkMahjong(currentPlayer);
        int[,] handMatrix = buildMatrix(hands[currentPlayer]);
        if (handMatrix[id,value-1] != 4)
        {
            drawn = true;
        }
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

    public void setButton(GameObject button)
    {
        OKButton = button;
    }

    public void halt(GameObject haltTile)
    {
        OKButton.hideFlags = HideFlags.None;
        OKButton.SetActive(true);
        temp = currentPlayer;
        currentPlayer = 4;
        questionTile = haltTile;
        drawn = false;
        discarded = false;
        halted = true;
    }

    public void unhalt()
    {
        OKButton.hideFlags = HideFlags.HideInHierarchy;
        OKButton.SetActive(false);
        currentPlayer = temp;
        temp = 4;
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
        areas.Add(area);
    }

    public void initTitle(GameObject title)
    {
        titles.Add(title);
    }

    public void giveText(GameObject text)
    {
        instructions = text;
    }

    public void moveTile(GameObject tileP, int originID, int destinationID)
    {
        int tileIndex = getTileIndex(tileP, originID);
        GameObject tile = hands[originID][tileIndex];
        hands[originID].RemoveAt(tileIndex);
        hands[destinationID].Add(tile);
    }

    public int getPlayerAttribute(GameObject area)
    {
        return areas.IndexOf(area);
    }

    public void officiate(bool check)
    {
        if (check)
        {
            if (checkMahjong(getPlayerAttribute(questionTile.transform.parent.gameObject)))
            {
                questionTile.GetComponent<DragDrop>().officiate(true);
            }
            else
            {
                questionTile.GetComponent<DragDrop>().officiate(checkValidMeld(questionTile, questionTile.transform.parent.gameObject));
            }
        }
        else
        {
            questionTile.GetComponent<DragDrop>().officiate(check);
        }
    }

    //will be replaced but exists as a way to verify function outputs 
    public bool checkValidMeld(GameObject tile, GameObject destination)
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
        bool qSelected = false;
        for (int i = 0; i < destHand.Count; i++)
        {
            bool selected = destHand[i].GetComponent<TileProperties>().getSelect();
            int curValue = destHand[i].GetComponent<TileProperties>().getValue();
            int curID = destHand[i].GetComponent<TileProperties>().getID();
            bool melded = destHand[i].GetComponent<TileProperties>().getMeld();
            if (selected && destHand[i].Equals(questionTile))
            {
                qSelected = true;
            }
            if (selected && curID == tileQID && curValue == tileQV)
            {
                dupeTiles++;
            }
            if (selected && tileQID < 3 && curID == tileQID && curValue >= tileQV - 2 && curValue <= tileQV + 2)
            {
                if ((prevValue == 0 || curValue == prevValue + 1) && prevValue != curValue)
                {
                    incTiles++;
                    prevValue = curValue;
                }
                else if (prevValue != curValue)
                {
                    prevValue = curValue;
                    incTiles = 1;
                }
                if (melded)
                {
                    return false;
                }
            }

            if (dupeTiles >= clicks && qSelected)
            {
                return true;
            }
            if (clicks == 3 && incTiles == 3 && getPlayerAttribute(destination) == temp && qSelected)
            {
                return true;
            }
        }
        return false;
    }

    public bool checkMahjong(int player)
    {
        int[,] handMatrix = buildMatrix(hands[player]);
        if (halted)
        {
            handMatrix[questionTile.GetComponent<TileProperties>().getID(), questionTile.GetComponent<TileProperties>().getValue() - 1]++;
        }
        int melds = 0;
        int eyes = 0;
        for (int i = 0; i < handMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < handMatrix.GetLength(1); j++)
            {
                if (handMatrix[i, j] >= 3)
                {
                    melds++;
                }
                else if (handMatrix[i, j] == 2)
                {
                    if (eyes < 1)
                    {
                        eyes++;
                    }
                }
                else if (handMatrix[i, j] == 1)
                {
                    if (j + 2 < handMatrix.GetLength(1))
                    {
                        if (handMatrix[i, j+1] == 1 && handMatrix[i, j+2] == 1)
                        {
                            melds++;
                            j += 3;
                        }
                    }
                }
            }
        }
        if (melds == 4 & eyes == 1)
        {
            Debug.Log("win!");
            return true;
        }
        else
        {
            Debug.Log("lose");
            return false;
        }
    }

    private void win(int winner)
    {
        string winStr = "";
        if (winner == EAST)
        {
            winStr = "EAST";
        }
        if (winner == SOUTH)
        {
            winStr = "SOUTH";
        }
        if (winner == WEST)
        {
            winStr = "WEST";
        }
        if (winner == NORTH)
        {
            winStr = "NORTH";
        }
        instructions.SetActive(true);
        instructions.GetComponent<Text>().text = winStr + " WINS!";
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
                    melds++;
                    tile.GetComponent<TileProperties>().meld();
                    Transform parent = tile.transform.parent;
                    tile.transform.SetParent(null, false);
                    tile.transform.SetParent(parent, false);
                }
            }
            tile.GetComponent<TileProperties>().deselect();
        }
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

    private void Update()
    {
        if (drawn && discarded)
        {
            checkMahjong(currentPlayer);
            titles[currentPlayer].GetComponent<Image>().sprite = titleImgs[currentPlayer];
            currentPlayer = (currentPlayer + 1) % 4;
            titles[currentPlayer].GetComponent<Image>().sprite = titleImgs[currentPlayer + 4];
            drawn = false;
            discarded = false;
        }
    }

    private int[,] buildMatrix(List<GameObject> hand)
    {
        int[,] handMatrix = new int[4, 9];
        for (int i = 0; i < hand.Count; i++)
        { 
            TileProperties tileP = hand[i].GetComponent<TileProperties>();
            int id = tileP.getID();
            int valueIndex = tileP.getValue() - 1;
            handMatrix[id,valueIndex]++;
        }
        return handMatrix;
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
