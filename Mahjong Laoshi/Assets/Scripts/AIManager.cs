using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }
    private GameManager gameManager;
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
                int random = Random.Range(0, hand.Count);
                GameObject tile = gameManager.moveTileAtIndex(random, currentPlayer, GameManager.DISCARD);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
