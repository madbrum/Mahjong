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

    private int currentPlayer = EAST;
    private bool drawn = false;
    private bool discarded = false;

    public GameObject currentPlayerIndicator;

    public static GameManager Instance { get; private set; }
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

    public bool drawStatus()
    {
        return drawn;
    }

    public bool discardStatus()
    {
        return discarded;
    }

    public void logPlayer(int player)
    {
        currentPlayer = player;
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
            currentPlayer = (currentPlayer + 1) % 4;
            drawn = false;
            discarded = false;
            string txt = "";
            if (currentPlayer == EAST)
            {
                txt = "EAST";
            }
            if (currentPlayer == SOUTH)
            {
                txt = "SOUTH";
            }
            if (currentPlayer == WEST)
            {
                txt = "WEST";
            }
            if (currentPlayer == NORTH)
            {
                txt = "NORTH";
            }
            currentPlayerIndicator.GetComponent<Text>().text = "Current Player: " + txt;
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
}
