using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    private int tileID;
    private int tileValue;

    public void setID(int ID)
    {
        tileID = ID;
    }

    public void setValue(int value)
    {
        tileValue = value;
    }

    public int getID()
    {
        return tileID;
    }

    public int getValue()
    {
        return tileValue;
    }
}
