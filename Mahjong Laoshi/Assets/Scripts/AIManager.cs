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

    private void drawDisc(int player, List<GameObject> meld, GameObject tileP)
    {
        StartCoroutine(discDrawCrt(player, meld, tileP));
    }

    private IEnumerator discDrawCrt(int player, List<GameObject> meld, GameObject tileP)
    {
        yield return new WaitForSeconds((float)1.5);
        if (!gameManager.drawStatus())
        {
            tileP.transform.SetParent(gameManager.getArea(player).transform, false);
            gameManager.moveTile(tileP, GameManager.DISCARD, player);
            gameManager.logPlayer(player);
            tileP.GetComponent<TileProperties>().setPlayer(player);
            gameManager.logDraw(tileP.GetComponent<TileProperties>().getID(), tileP.GetComponent<TileProperties>().getValue());
            tileP.GetComponent<TileProperties>().setDiscard(false);
            foreach (GameObject tile in meld)
            {
                tile.GetComponent<TileProperties>().meld();
                Transform parent = tile.transform.parent;
                tile.transform.SetParent(null, false);
                tile.transform.SetParent(parent, false);
            }
        }
        gameManager.testState();
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
                    int[,] hypoHand = buildMatrixConcealed(gameManager.getHand(i));
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
            Debug.Log(maxPlayer);
            drawDisc(maxPlayer, maxMeld, tile);
            return true;
        }
        else
        {
            Debug.Log("no steals possible");
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
                int index = getWorstIndex(currentPlayer, hand);
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

    private int getWorstIndex(int player, List<GameObject> hand)
    {
        int[,] handMatrix = buildMatrixConcealed(gameManager.getHand(player));
        int[,] weightMatrix = new int[4, 9];
        int min = int.MaxValue;
        int minID = -1;
        int minValue = -1;
        for (int i = 0; i < handMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < handMatrix.GetLength(1); j++)
            {
                int weightedValue = weightSingle(handMatrix, i, j);
                weightMatrix[i, j] = weightedValue;
                if (weightedValue != 0 && weightedValue <= min)
                {
                    int tileIndex = getTileIndex(i, j + 1, player);
                    if (tileIndex != -1)
                    {
                        min = weightedValue;
                        minID = i;
                        minValue = j + 1;
                    }
                }
            }
        }
        //returns index of worst tile 
        for (int i = 0; i < weightMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < weightMatrix.GetLength(1); j++)
            {
                Debug.Log("At " + i + " " + (j+1) + " weight is " + weightMatrix[i, j]);
            }
        }
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
            if (neighbors[0] == 1 && neighbors[2] == 1)
            {
                weightedValue += 4;
            }
            else if (neighbors[0] < 2)
            {
                weightedValue += neighbors[0];
            }
            else if (neighbors[2] < 2)
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
            if (props.getID() == id && props.getValue() == value && !props.getMeld())
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
        if (incTiles >= 3 && incs.Contains(tile))
        {
            if (destPlayer == gameManager.getCurrentPlayer())
            {
                return incs;
            }
            else if (checkPotential(destPlayer, tile))
            {
                return incs;
            }
        }
        return null;
    }

    public bool checkPotential(int player, GameObject tile)
    {
        int[,] handMatrix = gameManager.buildMatrix(gameManager.getHand(player));
        TileProperties props = tile.GetComponent<TileProperties>();
        handMatrix[props.getID(), props.getValue() - 1]++;
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
                        if (handMatrix[i, j + 1] == 1 && handMatrix[i, j + 2] == 1)
                        {
                            melds++;
                            j += 2;
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

    public int[,] buildMatrixConcealed(List<GameObject> hand)
    {
        int[,] handMatrix = new int[4, 9];
        for (int i = 0; i < hand.Count; i++)
        {
            TileProperties tileP = hand[i].GetComponent<TileProperties>();
            if (!tileP.getMeld())
            {
                int id = tileP.getID();
                int valueIndex = tileP.getValue() - 1;
                handMatrix[id, valueIndex]++;
            }
            
        }
        return handMatrix;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
