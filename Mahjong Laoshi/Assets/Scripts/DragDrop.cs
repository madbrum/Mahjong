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
                gameObject.GetComponent<TileProperties>().setDiscard(true);
                gameManager.moveTile(gameObject, gameManager.getPlayerAttribute(startParent), gameManager.getPlayerAttribute(dropZone));
                gameManager.logDiscard();
                if (gameObject.GetComponent<TileProperties>().getHidden())
                {
                    gameObject.GetComponent<TileProperties>().toggleHide();
                }
                if (gameManager.getHand(GameManager.DISCARD).Count > 24)
                {
                    int multiplier = gameManager.getHand(GameManager.DISCARD).Count % 24;
                    //dropzone.transform
                }
            }
            else if (!dropZone.Equals(gameManager.getArea(GameManager.DISCARD)) && gameObject.GetComponent<TileProperties>().getDiscard() && gameObject.GetComponent<TileProperties>().getPlayer() != gameManager.getPlayerAttribute(dropZone))
            {
                gameObject.GetComponent<Button>().enabled = true;
                gameManager.enableSelection(gameManager.getPlayerAttribute(dropZone));
                gameManager.halt(gameObject);
                destPlayerTemp = gameManager.getPlayerAttribute(dropZone);
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
            gameManager.moveTile(gameObject, GameManager.DISCARD, destPlayerTemp);
            if (gameManager.getClicks() == 3)
            {
                gameManager.logDraw();
            }
            gameObject.GetComponent<TileProperties>().setPlayer(destPlayerTemp);
            gameObject.GetComponent<TileProperties>().setDiscard(false);
            gameManager.logPlayer(destPlayerTemp);
        }
        else
        {
            gameManager.unhalt();
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
            gameManager.disableSelection(GameManager.DISCARD, valid);
        }
        gameManager.disableSelection(destPlayerTemp, valid);
        destPlayerTemp = -1;
    }
}
