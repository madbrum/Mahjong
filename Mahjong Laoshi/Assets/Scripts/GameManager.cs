using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<GameObject> eastTiles = new List<GameObject>();
    private List<GameObject> southTiles = new List<GameObject>();
    private List<GameObject> westTiles = new List<GameObject>();
    private List<GameObject> northTiles = new List<GameObject>();

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
        if (player.Equals("east"))
        {
            eastTiles = hand;
        }
        if (player.Equals("south"));
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
    }

    public void testState()
    {
        Debug.Log("East Size equals " + eastTiles.Count);
        Debug.Log("South Size equals " + southTiles.Count);
        Debug.Log("West Size equals " + westTiles.Count);
        Debug.Log("North Size equals " + northTiles.Count);
    }
}
