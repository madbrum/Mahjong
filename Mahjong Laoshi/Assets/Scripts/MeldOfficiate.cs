using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeldOfficiate : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameObject.hideFlags = HideFlags.HideInHierarchy;
        gameObject.SetActive(false);
    }

    public void OnClick()
    {
        int numClicks = gameManager.getClicks();
        Debug.Log("Clicks: " + numClicks);
        if (numClicks <= 4 && numClicks >= 3)
        {
            gameManager.officiate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
