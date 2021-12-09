using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    private int tileID;
    private int tileValue;
    private int originPlayer;

    public void setID(int ID)
    {
        tileID = ID;
    }

    public void setValue(int value)
    {
        tileValue = value;
    }
    
    public void setPlayer(int player)
    {
        originPlayer = player;
    }

    public int getID()
    {
        return tileID;
    }

    public int getValue()
    {
        return tileValue;
    }

    public int getPlayer()
    {
        return originPlayer;
    }
}
