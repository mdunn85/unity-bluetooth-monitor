using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscoveredListItem : MonoBehaviour
{
    public GameManager gameManager;
    public Button button;

    private string _deviceName;
    void Start()
    {
        gameManager = GameObject.Find(nameof(GameManager)).GetComponent<GameManager>();
        _deviceName = button.GetComponentInChildren<Text>().text;
        button.onClick.AddListener(OnMouseDown);
    }

    public void OnMouseDown()
    {
        gameManager.Connect(_deviceName);
    }
}
