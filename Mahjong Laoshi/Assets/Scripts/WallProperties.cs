using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallProperties : MonoBehaviour
{
    private int player;

    public void setID(int id)
    {
        player = id;
    }

    public int getID()
    {
        return player;
    }
}
