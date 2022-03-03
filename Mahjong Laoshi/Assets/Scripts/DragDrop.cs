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
        if (valid)
        {
            //is EVERYTHING here being properly reset? 
            gameManager.unhalt();
            gameManager.moveTile(gameObject.GetComponent<TileProperties>().getID(), gameObject.GetComponent<TileProperties>().getValue(), startParent, dropZone);
            gameManager.logDraw();
            Debug.Log("DropZone id is " + gameManager.getPlayerAttribute(dropZone));
            Debug.Log("Destination player: " + destPlayerTemp);
            gameObject.GetComponent<TileProperties>().setPlayer(destPlayerTemp);
            gameObject.GetComponent<TileProperties>().setDiscard(false);
            gameManager.logPlayer(destPlayerTemp);
            Debug.Log("Current status: Drawn is " + gameManager.drawStatus() + " and discard status is " + gameManager.discardStatus());
            gameObject.GetComponent<TileProperties>().testTileState();
        }
        else
        {
            Debug.Log("Fail");
            gameManager.unhalt();
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
            Debug.Log("Player is " + gameManager.getCurrentPlayer() + "Current status: Drawn is " + gameManager.drawStatus() + " and discard status is " + gameManager.discardStatus());
        }
        gameObject.GetComponent<Button>().enabled = false;
        gameManager.disableSelection(destPlayerTemp, valid);
        gameObject.GetComponent<TileProperties>().testTileState();
        destPlayerTemp = -1;
    }
}
