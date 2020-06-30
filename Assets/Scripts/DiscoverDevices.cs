using UnityEngine;

public class DiscoverDevices : MonoBehaviour
{
    public GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find(nameof(GameManager)).GetComponent<GameManager>();
    }

    /**
     * Start scanning for devices on click
     */
    private void OnMouseDown()
    {
        gameManager.Scan();
    }
}
