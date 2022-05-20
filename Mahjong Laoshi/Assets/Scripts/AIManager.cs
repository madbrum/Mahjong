using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }
    private GameManager gameManager;

    private int[] filter = { 2, 3, 2 };
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("AIManager initialized");
        }
        else
        {
            Debug.Log("Warning: multiple " + this + " in scene!");
        }
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        Debug.Log("Instance of GM in AM initialized");
    }

    public void draw()
    {
        StartCoroutine(drawCrt());
    }

    private IEnumerator drawCrt()
    {
        int currentPlayer = gameManager.getCurrentPlayer();
        if (currentPlayer != 0 && currentPlayer != 4)
        {
            gameManager.getHand(currentPlayer);
            if (!gameManager.drawStatus())
            {
                //yield return new WaitForSeconds((float) 0.5);
                Button wall = gameManager.getWall(currentPlayer).GetComponent<Button>();
                Sprite original = wall.GetComponent<Image>().sprite;
                wall.GetComponent<Image>().CrossFadeColor(wall.colors.pressedColor, wall.colors.fadeDuration, true, true);
                yield return new WaitForSeconds((float)0.25);
                wall.GetComponent<Image>().CrossFadeColor(wall.colors.normalColor, wall.colors.fadeDuration, true, true);
                gameManager.getWall(currentPlayer).GetComponent<DrawSingle>().dealSingle();
            }
        }
        yield return null;
    }

    private IEnumerator discDrawCrt(int player, List<GameObject> meld)
    {
        yield return null;
    }

    public bool scanDiscarded()
    {
        List<GameObject> discards = gameManager.getHand(GameManager.DISCARD);
        GameObject tile = discards[discards.Count - 1];
        int maxValue = 0;
        int maxPlayer = -1;
        List<GameObject> maxMeld = null;
        for (int i = 1; i < 4; i++)
        {
            if (i != gameManager.getCurrentPlayer()-1)
            {
                List<GameObject> meld = getValidMeld(tile, i);
                if (meld != null)
                {
                    //get value by building matrix and taking center tile of meld, store in max
                    int centerId = meld[1].GetComponent<TileProperties>().getID();
                    int centerValue = meld[1].GetComponent<TileProperties>().getValue();
                    int[,] hypoHand = gameManager.buildMatrix(gameManager.getHand(i));
                    hypoHand[centerId, centerValue - 1]++;
                    int value = weightSingle(hypoHand, centerId, centerValue-1);
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxPlayer = i;
                        maxMeld = meld;
                    }
                }
            }
        }
        //if there is a max, call the draw function from discard and pass in meld so you can reveal it and do everything you do in dragdrop 
        if (maxValue != 0)
        {
            //draw with maxplayer and return true. UPDATE CURRENTPLAYER 
            discDrawCrt(maxPlayer, maxMeld);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void discard()
    {
        StartCoroutine(discardCrt());
    }

    private IEnumerator discardCrt()
    {
        int currentPlayer = gameManager.getCurrentPlayer();
        if (currentPlayer != 0 && currentPlayer != 4)
        {
            List<GameObject> hand = gameManager.getHand(currentPlayer);
            if (!gameManager.discardStatus())
            {
                //yield return new WaitForSeconds((float)0.5);
                int index = getWorstIndex(currentPlayer);
                Debug.Log("Index: " + index);
                GameObject tile = gameManager.moveTileAtIndex(index, currentPlayer, GameManager.DISCARD);
                tile.transform.SetParent(gameManager.getArea(GameManager.DISCARD).transform, false);
                tile.GetComponent<TileProperties>().setDiscard(true);
                gameManager.logDiscard();
                if (tile.GetComponent<TileProperties>().getHidden())
                {
                    tile.GetComponent<TileProperties>().toggleHide();
                }
                //yield return new WaitForSeconds((float)0.5);
            }
        }
        yield return null;
    }

    private int getWorstIndex(int player)
    {
        int[,] handMatrix = gameManager.buildMatrix(gameManager.getHand(player));
        int[,] weightMatrix = new int[4, 9];
        int min = int.MaxValue;
        int minID = -1;
        int minValue = -1;
        for (int i = 0; i < handMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < handMatrix.GetLength(1); j++)
            {
                int weightedValue = weightSingle(handMatrix, i, j);
                if (weightedValue != 0 && weightedValue <= min)
                {
                    min = weightedValue;
                    minID = i;
                    minValue = j + 1;
                }
                Debug.Log("ID: " + i + ", Value: " + (j + 1) + ", Weighted: " + weightedValue);
            }
        }
        //returns index of worst tile 
        return getTileIndex(minID, minValue, player);
    }

    private int weightSingle(int[,] handMatrix, int id, int value)
    {

        int[] neighbors = { 0, handMatrix[id, value], 0 };
        if (value - 1 >= 0)
        {
            neighbors[0] = handMatrix[id, value - 1];
        }
        if (value + 1 < handMatrix.GetLength(1))
        {
            neighbors[2] = handMatrix[id, value + 1];
        }
        int weightedValue = neighbors[1];
        if (id != 3 && neighbors[1] == 1)
        {
            if (neighbors[0] < 2)
            {
                weightedValue += neighbors[0];
            }
            if (neighbors[2] < 2)
            {
                weightedValue += neighbors[2];
            }
        }
        else if (neighbors[1] >= 2)
        {
            weightedValue = neighbors[1] * 2;
        }
        return weightedValue; 
    }

    private int getTileIndex(int id, int value, int player)
    {
        //if -1 returns that means the tile isn't present in this hand . probably need to add checks for this later
        int tileIndex = -1;
        for (int i = 0; i < gameManager.getHand(player).Count; i++)
        {
            TileProperties props = gameManager.getHand(player)[i].GetComponent<TileProperties>();
            if (props.getID() == id && props.getValue() == value)
            {
                tileIndex = i;
                break;
            }
        }
        return tileIndex;
    }

    private List<GameObject> getValidMeld(GameObject tile, int destPlayer)
    {
        List<GameObject> destHand = new List<GameObject>(gameManager.getHand(destPlayer));
        destHand.Add(tile);
        destHand.Sort(new GameManager.TileComparer());

        int tileQV = tile.GetComponent<TileProperties>().getValue();
        int tileQID = tile.GetComponent<TileProperties>().getID();
        int dupeTiles = 0;
        int incTiles = 0;
        int prevValue = 0;
        List<GameObject> dupes = new List<GameObject>();
        List<GameObject> incs = new List<GameObject>();
        for (int i = 0; i < destHand.Count; i++)
        {
            int curValue = destHand[i].GetComponent<TileProperties>().getValue();
            int curID = destHand[i].GetComponent<TileProperties>().getID();
            bool melded = destHand[i].GetComponent<TileProperties>().getMeld();
            if (!melded && curID == tileQID && curValue == tileQV)
            {
                dupeTiles++;
                dupes.Add(destHand[i]);
            }
            if (!melded && tileQID < 3 && curID == tileQID && curValue >= tileQV - 2 && curValue <= tileQV + 2)
            {
                if ((prevValue == 0 || curValue == prevValue + 1) && prevValue != curValue)
                {
                    incTiles++;
                    incs.Add(destHand[i]);
                    prevValue = curValue;
                }
                else if (prevValue != curValue)
                {
                    prevValue = curValue;
                    incs = new List<GameObject>();
                    incs.Add(destHand[i]);
                    incTiles = 1;
                }
            }
        }
        if (dupeTiles >= 3)
        {
            return dupes;
        }
        if (incTiles >= 3 && incs.Contains(tile) && destPlayer == gameManager.getCurrentPlayer())
        {
            return incs;
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
