using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleHide : MonoBehaviour
{
    private string toggledMsg;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        toggledMsg = "SHOW";
        gameObject.hideFlags = HideFlags.HideInHierarchy;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        string temp = gameObject.GetComponentInChildren<Text>().text;
        gameObject.GetComponentInChildren<Text>().text = toggledMsg;
        toggledMsg = temp;
        gameManager.toggleHide();
    }
}
