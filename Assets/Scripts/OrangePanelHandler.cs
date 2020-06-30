using UnityEngine;
using UnityEngine.UI;

public class OrangePanelHandler : MonoBehaviour
{
    public Text heartRateTextSubTitle;
    public Text heartRateText;
    public Text bmp;

    public GameObject heartBeat;
    public GameObject heartBeatSmall;

    /**
     * Hide and show different UI element for the 3rd screen
     */
    public void ShowHeartRate()
    {
        heartBeat.SetActive(false);
        heartBeatSmall.SetActive(true);
        bmp.gameObject.SetActive(true);
        heartRateText.gameObject.SetActive(true);
        heartRateTextSubTitle.text = "PAIRED";
    }

    /**
     * Update the heart rate UI
     */
    public void UpdateHeartRate(double heartRate)
    {
        heartRateText.text = heartRate.ToString();
    }
}
