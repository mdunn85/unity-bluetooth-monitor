using UnityEngine;
using UnityEngine.UI;

public class DeviceDiscoveryHandler : MonoBehaviour
{
    private int _headerDots;
    private string _headerText;
    private bool _isHeaderAnimating;
    public Text headerText;

    /**
     * Set the header text and animate with dots if requested
     */
    public void SetHeader(string text, bool animateDots = false)
    {
        StopHeaderAnimation();
        _headerText = text;
        _headerDots = 1;
        _isHeaderAnimating = animateDots;
        InvokeRepeating(nameof(SetHeaderText),0.5f,0.5f);
    }

    /**
     * Stop the header animation 
     */
    public void StopHeaderAnimation()
    {
        CancelInvoke(nameof(SetHeaderText));
    }
    
    /**
     * Set the header text with dots
     */
    private void SetHeaderText()
    {
        string text = _headerText;
        if (_isHeaderAnimating)
        {
             text += AnimateHeaderDots();
        }
        headerText.text = text;
    }
    
    /**
     * Animate the dots in the title
     */
    private string AnimateHeaderDots()
    {
        string dots = "";
        for (int i = 0; i < _headerDots; i++)
        {
            dots += ".";
        }

        _headerDots++;
        if (_headerDots > 3)
        {
            _headerDots = 1;
        }

        return dots;
    }
}
