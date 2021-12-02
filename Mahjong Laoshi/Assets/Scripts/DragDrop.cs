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
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        inDropZone = false;
        dropZone = null;
    }

    public void startDrag()
    {
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
        isDragging = true;
    }

    public void endDrag()
    {
        isDragging = false;
        if (inDropZone)
        {
            transform.SetParent(dropZone.transform, false);
            Debug.Log("ID of Tile is " + gameObject.GetComponent<TileProperties>().getID() + ", Value of Tile is " + gameObject.GetComponent<TileProperties>().getValue());
            string origin = startParent.name;
            string destination = dropZone.transform.name;
            //this really ugly code assumes the convention that the player area names always end with the player number
            int originHand = int.Parse(origin.Substring(origin.Length - 2)) - 1;
            int destHand = int.Parse(destination.Substring(destination.Length - 2)) - 1;
            gameManager.moveTile(gameObject.GetComponent<TileProperties>().getID(), gameObject.GetComponent<TileProperties>().getValue(), originHand, destHand);
            if (destHand == GameManager.DISCARD)
            {
                gameManager.logDiscard();
            }
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
        gameManager.testState();
    }
}
