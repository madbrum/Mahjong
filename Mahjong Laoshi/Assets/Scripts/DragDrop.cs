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
            else if (!dropZone.Equals(gameManager.getArea(GameManager.DISCARD)) && gameObject.GetComponent<TileProperties>().getDiscard() && gameObject.GetComponent<TileProperties>().getPlayer() != gameManager.getPlayerAttribute(dropZone) && gameManager.checkValidMeld(gameObject, dropZone, false))
            {
                gameObject.GetComponent<Button>().enabled = true;
                gameManager.enableSelection(gameManager.getPlayerAttribute(dropZone));
                gameManager.halt(gameObject);
                Debug.Log("Halted");
                destPlayerTemp = gameManager.getPlayerAttribute(dropZone);
                Debug.Log("Zone: " + gameManager.getPlayerAttribute(dropZone));

                //gameManager.moveTile(gameObject.GetComponent<TileProperties>().getID(), gameObject.GetComponent<TileProperties>().getValue(), startParent, dropZone);
                //gameManager.logDraw();
                //int player = gameManager.getPlayerAttribute(dropZone);
                //gameObject.GetComponent<TileProperties>().setPlayer(player);
                //gameObject.GetComponent<TileProperties>().setDiscard(false);
                //gameManager.logPlayer(player);
                //if (gameManager.getHideStatus() && !gameObject.GetComponent<TileProperties>().getHidden())
                //{
                //    gameObject.GetComponent<TileProperties>().toggleHide();
                //}
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
            gameManager.unhalt();
            gameManager.moveTile(gameObject.GetComponent<TileProperties>().getID(), gameObject.GetComponent<TileProperties>().getValue(), startParent, dropZone);
            gameManager.logDraw();
            Debug.Log("DropZone id is " + gameManager.getPlayerAttribute(dropZone));
            Debug.Log("Destination player: " + destPlayerTemp);
            gameObject.GetComponent<TileProperties>().setPlayer(destPlayerTemp);
            gameObject.GetComponent<TileProperties>().setDiscard(false);
            gameManager.logPlayer(destPlayerTemp);
            gameManager.disableSelection(destPlayerTemp);
            if (gameManager.getHideStatus() && !gameObject.GetComponent<TileProperties>().getHidden())
            {
                gameObject.GetComponent<TileProperties>().toggleHide();
            }
            destPlayerTemp = -1;
            Debug.Log("Current status: Drawn is " + gameManager.drawStatus() + " and discard status is " + gameManager.discardStatus());
        }
        else
        {
            Debug.Log("Fail");
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
            destPlayerTemp = -1;
        }
    }
}
