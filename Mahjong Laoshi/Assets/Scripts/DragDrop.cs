using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    public GameObject canvas;
    private bool isDragging = false;
    private bool inDropZone = false;
    private GameObject dropZone; 
    private Vector2 startPosition;
    private GameObject startParent;
    GameManager gameManager = GameManager.Instance;
    private void Awake()
    {
        canvas = GameObject.Find("Main Canvas");
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
        Debug.Log("Entering " + gameManager.getPlayerAttribute(dropZone));
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Leaving " + gameManager.getPlayerAttribute(dropZone));
        Debug.Log(collision.gameObject);
        if (!collision.gameObject.Equals(dropZone) && gameManager.getPlayerAttribute(collision.gameObject) > -1)
        {
            inDropZone = true;
            dropZone = collision.gameObject;
        }
        else
        {
            inDropZone = false;
            dropZone = null;
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
        //Debug.Log(gameManager.getPlayerAttribute(dropZone));
        Debug.Log(inDropZone);
        if (inDropZone)
        {
            Debug.Log("Drop Zone is " + gameManager.getPlayerAttribute(dropZone));
            transform.SetParent(dropZone.transform, false);
            if (dropZone.Equals(gameManager.getArea(GameManager.DISCARD)) && !gameObject.GetComponent<TileProperties>().getDiscard())
            {
                gameManager.moveTile(gameObject.GetComponent<TileProperties>().getID(), gameObject.GetComponent<TileProperties>().getValue(), startParent, dropZone);
                gameManager.logDiscard();
                gameObject.GetComponent<TileProperties>().setDiscard(true);
            }
            else if (!dropZone.Equals(gameManager.getArea(GameManager.DISCARD)) && gameObject.GetComponent<TileProperties>().getDiscard() && gameObject.GetComponent<TileProperties>().getPlayer() != gameManager.getPlayerAttribute(dropZone) && gameManager.checkValidMeld(gameObject, dropZone))
            {
                gameManager.moveTile(gameObject.GetComponent<TileProperties>().getID(), gameObject.GetComponent<TileProperties>().getValue(), startParent, dropZone);
                gameManager.logDraw();
                int player = gameManager.getPlayerAttribute(dropZone);
                gameObject.GetComponent<TileProperties>().setPlayer(player);
                gameObject.GetComponent<TileProperties>().setDiscard(false);
                gameManager.logPlayer(player);
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
}
