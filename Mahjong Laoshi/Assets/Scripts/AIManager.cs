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

    public 

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

        // Update is called once per frame
        void Update()
    {
        
    }
}
