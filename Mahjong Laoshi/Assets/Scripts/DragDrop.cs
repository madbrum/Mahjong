using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{
    public GameObject canvas;
    private bool isDragging = false;
    private bool inDropZone = false;
    private GameObject dropZone; 
    private Vector2 startPosition;
    private GameObject startParent;
    private int destPlayerTemp = -1;
    GameManager gameManager;
    private void Awake()
    {
        canvas = GameObject.Find("Main Canvas");
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(canvas.transform, true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        inDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.Equals(dropZone))
        {
            inDropZone = false;
            dropZone = startParent;
        }
    }

    public void startDrag()
    {
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        if (((startParent.Equals(gameManager.getArea(gameManager.getCurrentPlayer())) && !gameManager.discardStatus()) && gameManager.drawStatus()) || (startParent.Equals(gameManager.getArea(GameManager.DISCARD)) && !gameManager.drawStatus() && gameObject.GetComponent<TileProperties>().getPlayer() < 4))
        {
            isDragging = true;
        }
    }

    public void endDrag()
    {
        isDragging = false;
        gameObject.GetComponent<TileProperties>().testTileState();
        if (inDropZone)
        {
            transform.SetParent(dropZone.transform, false);
            if (dropZone.Equals(gameManager.getArea(GameManager.DISCARD)) && !gameObject.GetComponent<TileProperties>().getDiscard())
            {
                gameManager.moveTile(gameObject.GetComponent<TileProperties>().getID(), gameObject.GetComponent<TileProperties>().getValue(), startParent, dropZone);
                gameManager.logDiscard();
                gameObject.GetComponent<TileProperties>().setDiscard(true);
                if (gameObject.GetComponent<TileProperties>().getHidden())
                {
                    gameObject.GetComponent<TileProperties>().toggleHide();
                }
            }
            else if (!dropZone.Equals(gameManager.getArea(GameManager.DISCARD)) && gameObject.GetComponent<TileProperties>().getDiscard() && gameObject.GetComponent<TileProperties>().getPlayer() != gameManager.getPlayerAttribute(dropZone))
            {
                gameObject.GetComponent<Button>().enabled = true;
                gameManager.enableSelection(gameManager.getPlayerAttribute(dropZone));
                gameManager.halt(gameObject);
                Debug.Log("Halted");
                destPlayerTemp = gameManager.getPlayerAttribute(dropZone);
                Debug.Log("Zone: " + gameManager.getPlayerAttribute(dropZone));
            }
            else
            {
                transform.position = startPosition;
                transform.SetParent(startParent.transform, false);
            }
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
    }

    public void officiate(bool valid)
    {
        Debug.Log("\t\tBEGIN: " + this.name + " officiate(bool valid)" + " parameter valid = " + valid);
        Debug.Log("\t\tEssential values:");
        Debug.Log("\t\t\tdestPlayerTemp = " + destPlayerTemp + ", DropZone id is " + gameManager.getPlayerAttribute(dropZone) + ". Should be equal, though DropZone is not used.");
        Debug.Log("This is the discarded tile. Should match with the tile printed in GameManager on officiate. " + gameManager.analyzeTile(gameObject));
        gameManager.testHand(destPlayerTemp);
        gameManager.testHand(GameManager.DISCARD);
        if (valid)
        {
            //is EVERYTHING here being properly reset? 
            gameManager.unhalt();
            gameManager.moveTile(gameObject.GetComponent<TileProperties>().getID(), gameObject.GetComponent<TileProperties>().getValue(), GameManager.DISCARD, destPlayerTemp);
            gameManager.logDraw();
            gameObject.GetComponent<TileProperties>().setPlayer(destPlayerTemp);
            gameObject.GetComponent<TileProperties>().setDiscard(false);
            gameManager.logPlayer(destPlayerTemp);
            Debug.Log("\t\t\tCurrent status: Drawn is " + gameManager.drawStatus() + " and discard status is " + gameManager.discardStatus());
            Debug.Log("Meld is valid and changes have been made. Compare current state of question tile to previous: " + gameManager.analyzeTile(gameObject));
        }
        else
        {
            gameManager.unhalt();
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
            Debug.Log("Meld has been declared invalid. Player is " + gameManager.getCurrentPlayer() + "Current status: Drawn is " + gameManager.drawStatus() + " and discard status is " + gameManager.discardStatus() + ". Compare current state of question tile to previous: " + gameManager.analyzeTile(gameObject));
        }
        gameObject.GetComponent<Button>().enabled = false;
        //bug is probably that discarded tiles aren't being disabled either 
        gameManager.disableSelection(destPlayerTemp, valid);
        gameManager.testHand(destPlayerTemp);
        gameManager.testHand(GameManager.DISCARD);
        Debug.Log("officiate has completed. Compare current state of question tile to previous: " + gameManager.analyzeTile(gameObject));
        destPlayerTemp = -1;
        Debug.Log("\t\tEND: " + this.name + " officiate(bool valid)" + " parameter valid = " + valid);
    }
}
