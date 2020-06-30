using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    private AndroidJavaObject _bluetoothHelpersObject;
    private AndroidJavaObject _androidActivityContext;
    private bool _isConnecting;

    public GameObject orangePanel;
    public GameObject discoveryPanel;
    
    private DeviceDiscoveryHandler _deviceDiscoveryHandler;
    private OrangePanelHandler _orangePanelHandler;
    private DevicesDto _devicesDto;

    public Transform scrollView;
    void Start()
    {
        //Request permissions
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation))
        {
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
#endif
//Get panel scripts
        _deviceDiscoveryHandler = discoveryPanel.GetComponent<DeviceDiscoveryHandler>();
        _orangePanelHandler = orangePanel.GetComponent<OrangePanelHandler>();
    }

    /**
     * Setup android libraries
     */
    private void SetupLibraries()
    {
        //Get android activity 
        using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            _androidActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
        }
        //Get bluetooth library class
        AndroidJavaClass blueToothHelpersClass = new AndroidJavaClass("com.mattoverflow.andriod.bluetoothhelpers.Helpers");
        //Get an instance of the bluetooth library class
        _bluetoothHelpersObject = blueToothHelpersClass.CallStatic<AndroidJavaObject>("instance");
        //Set the android activity to instance of the bluetooth
        _bluetoothHelpersObject.Call("setContext", _androidActivityContext);
    }

    /**
     * Scan for new devices 
     */
    public void Scan()
    {
        //Init android libraries
        SetupLibraries();
        //Scan for bluetooth devices
        _bluetoothHelpersObject.Call("discover");
        
        //Hide first panel
        orangePanel.SetActive(false);
        //Set header and animate with 3 dots
        _deviceDiscoveryHandler.SetHeader("Searching",true);
        //Show second panel
        discoveryPanel.SetActive(true);
        
        //Search for devices for 12 seconds
        InvokeRepeating(nameof(GetDevices), 1f, 1f);
        Invoke(nameof(StopDiscovery),12f);
    }
    
    /**
     * Stop searching for new devices
     */
    private void StopDiscovery()
    {
        _bluetoothHelpersObject.Call("stop");
    }
    
    /**
     * Check bluetooth is still searching for devices
     */

    private bool IsDiscovering()
    {
        return _bluetoothHelpersObject.Call<bool>("isDiscovering");
    }
    
    /**
     * Get devices from the android library
     */
    private void GetDevices()
    {
        if (IsDiscovering())
        {
            //Get the found devices from the android library
            string deviceJson = _bluetoothHelpersObject.Call<string>("getDevices");
            _devicesDto = JsonUtility.FromJson<DevicesDto>(deviceJson);
            //Get the button prefab
            GameObject resource = Resources.Load("DeviceButton") as GameObject;
            if (resource != null)
            {
                //Add new devices if they don't exist
                var currentDeviceNames = scrollView.GetComponentsInChildren<Text>().Select(s => s.text);
                foreach (Device device in _devicesDto.devices.Where(d => !currentDeviceNames.Contains(d.name)))
                {
                    resource.GetComponentsInChildren<Text>().First().text = device.name;
                    Instantiate(resource, scrollView);
                }
            }
        }
        else
        {
            //If not discovering cancel task and display text if device isn't being connected to
            CancelInvoke(nameof(GetDevices));
            if (!_isConnecting)
            {
                _deviceDiscoveryHandler.SetHeader("Discovered devices");
            }
        }
    }

    /**
     * Connect to bluetooth device
     * Expects the name of the device to connect to - ID would be better for uniqueness
     */
    public void Connect(string deviceName)
    {
        _isConnecting = true;
        if (IsDiscovering())
        {
            StopDiscovery();
        }
        //Get device and connect
        Device device = _devicesDto.devices.FirstOrDefault(d => d.name == deviceName);
        if (device != null)
        {
            _bluetoothHelpersObject.Call("connect", device.id);
            _deviceDiscoveryHandler.SetHeader("Connecting",true);
            InvokeRepeating(nameof(CheckIsConnecting), 1f, 1f);
        }
    }

    /**
     * Check the device is connecting and there is a positive heart rate
     */
    private void CheckIsConnecting()
    {
        if (_bluetoothHelpersObject.Call<bool>("isConnected") && GetHeartRate() > 0)
        {
            CancelInvoke(nameof(CheckIsConnecting));
            ShowHeartRate();
        }
    }

    /**
     * Show the 3rd screen with heart rate data
     */
    private void ShowHeartRate()
    {
        _deviceDiscoveryHandler.StopHeaderAnimation();
        _orangePanelHandler.ShowHeartRate();
        orangePanel.SetActive(true);
        discoveryPanel.SetActive(false);
        InvokeRepeating(nameof(UpdateHeartRate), 1f, 1f);
    }

    /**
     * Gets the heart rate from the android library
     */
    private double GetHeartRate()
    {
        return _bluetoothHelpersObject.Call<double>("getHeartRate");
    }

    /**
     * Updates the heart rate in the UI
     */
    private void UpdateHeartRate()
    {
        if (_bluetoothHelpersObject.Call<bool>("isConnected"))
        {
            double heartRate = GetHeartRate();
            _orangePanelHandler.UpdateHeartRate(heartRate);
        }
    }
}
