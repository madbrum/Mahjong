using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileProperties : MonoBehaviour
{
    private GameManager gameManager = GameManager.Instance;
    private int tileKey;
    private int tileID;
    private int tileValue;
    private int originPlayer;
    private bool discarded = false;
    private bool hidden = false;
    private bool melded = false;
    private bool selected = false;
    private Sprite previousSprite;

    private void Awake()
    {
        previousSprite = Resources.Load<Sprite>("1_blank");
    }

    public void setID(int ID)
    {
        tileID = ID;
    }

    public void setValue(int value)
    {
        tileValue = value;
    }

    public void setKey(int key)
    {
        tileKey = key;
    }
    
    public void setPlayer(int player)
    {
        originPlayer = player;
    }

    public void setDiscard(bool status)
    {
        discarded = status;
    }

    public void toggleHide()
    {
        if (!melded)
        { 
            hidden = !hidden;
            Sprite current = gameObject.GetComponent<Image>().sprite;
            gameObject.GetComponent<Image>().sprite = previousSprite;
            previousSprite = current;
        }
    }

    public void meld()
    {
        if (hidden)
        {
            toggleHide();
        }
        melded = true;
    }

    public void select()
    {
        selected = true;
    }

    public void deselect()
    {
        selected = false;
    }

    public int getID()
    {
        return tileID;
    }

    public int getValue()
    {
        return tileValue;
    }

    public int getKey()
    {
        return tileKey;
    }

    public int getPlayer()
    {
        return originPlayer;
    }

    public bool getDiscard()
    {
        return discarded;
    }

    public bool getHidden()
    {
        return hidden;
    }

    public bool getSelect()
    {
        return selected;
    }

    public bool getMeld()
    {
        return melded;
    }

    private void Update()
    {
        //the idea is this: you're only able to pick up the tile if it's 1) already discarded and 2) if it's the tile of the player who JUST went. after that, it's over.
        //ids are irrelevant once you're officially unable to pick them up again, they're completely out of play at that point.
        //so you only keep them as long as you need for comparison to the turn number. then ids change when they switch hands, ect...

        //plus 2 because + 1 means it's just the next player after the discarder, and if they haven't done anything yet this is the only chance you have to steal discard 
        if (discarded && gameManager.getCurrentPlayer() >= ((originPlayer + 1) % 4) && gameManager.drawStatus())
        {
            originPlayer = GameManager.DISCARD;
        }
    }

    public void testTileState()
    {
        Debug.Log("Discarded : " + discarded);
        Debug.Log("Hidden: " + hidden);
        Debug.Log("Melded: " + melded);
        Debug.Log("Selected: " + selected);
}
}
