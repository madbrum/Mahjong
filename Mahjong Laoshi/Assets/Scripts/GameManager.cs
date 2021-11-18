using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<GameObject> leftovers = new List<GameObject>();
    private List<GameObject> eastTiles = new List<GameObject>();
    private List<GameObject> southTiles = new List<GameObject>();
    private List<GameObject> westTiles = new List<GameObject>();
    private List<GameObject> northTiles = new List<GameObject>();
    private List<GameObject> discardPile = new List<GameObject>();
    //hash table? so you don't have to convert to string every time? idk

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
    }

    public void initHand(List<GameObject> hand, string player)
    {
        //List<GameObject> destination = determineHand(player);
        //destination = hand;

        if (player.Equals("east"))
        {
            eastTiles = hand;
        }
        if (player.Equals("south"))
        {
            southTiles = hand;
        }
        if (player.Equals("west"))
        {
            westTiles = hand;
        }
        if (player.Equals("north"))
        {
            northTiles = hand;
        }
        if (player.Equals("leftover"))
        {
            leftovers = hand;
        }
    }

    public void switchHand(int id, int value, string originP, string destinationP)
    {
        List<GameObject> origin = determineHand(originP);
        List<GameObject> destination = determineHand(destinationP);
        GameObject tile = origin[getTileIndex(id, value, origin)];
        origin.Remove(tile);
        destination.Add(tile);
    }

    private int getTileIndex(int id, int value, List<GameObject> hand)
    {
        int tileIndex = 0;
        for (int i = 0; i < hand.Count; i++)
        {
            TileProperties props = hand[i].GetComponent<TileProperties>();
            if (props.getID() == id && props.getValue() == value)
            {
                tileIndex = i;
                break;
            }
        }
        return tileIndex;
    }

    
    private List<GameObject> determineHand(string player)
    {
        if (player.Equals("east"))
        {
            return eastTiles;
        }
        if (player.Equals("south"))
        {
            return southTiles;
        }
        if (player.Equals("west"))
        {
            return westTiles;
        }
        if (player.Equals("north"))
        {
            return northTiles;
        }
        if (player.Equals("leftover"))
        {
            return leftovers;
        }
        else
        {
            return discardPile;
        }
    }
   

    public void testState()
    {
        Debug.Log("East Size equals " + eastTiles.Count);
        Debug.Log("South Size equals " + southTiles.Count);
        Debug.Log("West Size equals " + westTiles.Count);
        Debug.Log("North Size equals " + northTiles.Count);
        Debug.Log("Leftover Tiles Size equals " + leftovers.Count);
    }
}
