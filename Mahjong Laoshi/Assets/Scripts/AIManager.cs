using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void bonk()
    {
        int currentPlayer = gameManager.getCurrentPlayer();
        if (currentPlayer != 0 && currentPlayer != 4)
        {
            gameManager.getHand(currentPlayer);
            if (!gameManager.drawStatus())
            {
                gameManager.getWall(currentPlayer).GetComponent<DrawSingle>().dealSingle();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
