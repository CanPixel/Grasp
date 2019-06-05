using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour {
    public bool alternativeControls = true;

    public enum Port {
        COM1,
        COM2,
        COM3,
        COM4,
        COM5,
        COM6,
        COM7,
        COM8,
        COM9,
        COM10,
        COM11,
        COM12,
        COM13,
        COM14,
        COM15,
        COM16,
        COM17,
        COM18,
        COM19
    }
    public static Port port;
    private static PlayerInput self;
    public static SerialPort stream;

    public Text HeartRateBPM;
    public GameObject ConnectionText;
    private float ConnectionScale = 0;
    public RawImage overlay;
    private float overlayBaseAlpha;

    public bool debug = false;
    public int baudRate;

    [Header("Heartrate Color Stages")]
    public Gradient heartRange;

    [System.Serializable]
    public class ControllerValues {
        public int heartRate;
        public int xAim, yAim, click;

        private int[] lastRate = new int[20];
        private int lastRateIndex = 0;

        private int lastClick;

        public UnityEvent onClick;

        [HideInInspector]
        public bool updated = false;

        public void Print() {
            if(!PlayerInput.IsDebugging()) return;
            Debug.Log("[Heart Rate]: " + heartRate + " | (" + xAim + "," + yAim + ") " + click);
        }

        public void SetValues(string text) {
            updated = false;
            string[] splitter = text.Split(':');
            string[] splitText = splitter[0].Split('|');
            int tempHeart = 0;
            int.TryParse(splitText[0].Trim(), out tempHeart);
            if(tempHeart > 0) lastRate[lastRateIndex] = tempHeart;
            if(lastRateIndex < lastRate.Length - 1) lastRateIndex++;
            else lastRateIndex = 0;
            try {
                string[] joyStick = splitText[1].Replace('(', ' ').Replace(')', ' ').Trim().Split(',');
                int.TryParse(joyStick[0], out xAim);
                int.TryParse(joyStick[1], out yAim);
                int.TryParse(splitter[1], out click);
            } catch(System.IndexOutOfRangeException) {}

            if(lastRateIndex == 0) {
                int sum = 0;
                for(int i = 0; i < lastRate.Length; i++) sum += lastRate[i];
                heartRate = (sum) / lastRate.Length;
            }
            updated = true;
        }

        public void Tick() {
            if(lastClick != click && click == 0 && lastClick == 1) onClick.Invoke();
            lastClick = click;
        }
    }
    public ControllerValues controllerValues;

    void Awake() {
        overlayBaseAlpha = overlay.color.a;
        self = this;
        if(alternativeControls) StartCoroutine(Connect());
        else DisableUI();
    }

    void Update() {
        ReadSensor();
        controllerValues.Tick();
        controllerValues.Print();
    }

    void LateUpdate() {
        if(!controllerValues.updated || !alternativeControls) return;
        HeartRateBPM.text = "<color=#" + GetHeartRateColor(controllerValues.heartRate) + ">" + controllerValues.heartRate.ToString() + "</color> BPM";
        overlay.enabled = !PlayerInput.IsDebugging();
        if(!PlayerInput.IsDebugging()) HeartRateBPM.enabled = HasConnection();
    }

    protected string GetHeartRateColor(int rate) {
        string endColor = "fff";
        if(rate >= 100) return ColorUtility.ToHtmlStringRGB(heartRange.colorKeys[heartRange.colorKeys.Length - 1].color);
        for(int i = 0; i < heartRange.colorKeys.Length; i++) {
            if(heartRange.colorKeys[i].time >= rate / 100f) {
                endColor = ColorUtility.ToHtmlStringRGB(heartRange.colorKeys[i].color);
                break;
            } 
        }
        return endColor;
    }

    protected void ReadSensor() {
        string read = PlayerInput.ReadSensors();
        if(read == null) return;
        controllerValues.SetValues(read);
    }

    IEnumerator Connect() {
        if(self == null) yield return new WaitForSeconds(0.5f);
        string[] ports = System.Enum.GetNames(typeof(Port));
        bool connected = false;
        for(int i = 0; i < ports.Length; i++) {
            string port = ports[i];
            string[] portNums = System.Text.RegularExpressions.Regex.Split(port, @"\D+");
            stream = (int.Parse(portNums[1]) > 10) ? new SerialPort("\\\\.\\" + port, baudRate, Parity.None, 8, StopBits.One) : 
            new SerialPort(port.ToString(), baudRate, Parity.None, 8, StopBits.One);
            try {
                stream.Open();
                stream.ReadTimeout = 1;
                alternativeControls = true;
                Cursor.visible = false;
                connected = true;
                yield break;
            } catch(System.IO.IOException) {}
        }
        if(!connected) {
            Debug.LogWarning("Couldn't find heartbeat + joystick configuration; Disabling alt controls.");
            alternativeControls = false;
            DisableUI();
        }
    }

    private void DisableUI() {
        ConnectionText.SetActive(false);
        HeartRateBPM.enabled = false;
        overlay.enabled = false;
    }

    public static string ReadSensors(int timeout = 1) {
        if(stream == null) return null;
        stream.ReadTimeout = timeout;
        try {return stream.ReadLine();}
        catch(System.Exception) {}
        return null;
    }

    public static bool IsDebugging() {
        return self.debug;
    }

    public bool HasConnection() {
        return controllerValues.heartRate > 1;
    }

    public static ControllerValues GetControllerValues() {
        return self.controllerValues;
    }

    public static bool UsingAlternativeControls() {
        return self.alternativeControls;
    }

    void OnGUI() {
        if(!alternativeControls) return;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, Mathf.Lerp(overlay.color.a, HasConnection() ? 0 : overlayBaseAlpha, Time.deltaTime * 2));
        if(HasConnection()) ConnectionScale = Mathf.Lerp(ConnectionScale, 0, Time.deltaTime * 3);
        else {
            ConnectionScale = Mathf.Lerp(ConnectionScale, 1, Time.deltaTime * 3);
            HeartRateBPM.enabled = false;
        }
        ConnectionText.transform.localScale = new Vector3(ConnectionScale, ConnectionScale, ConnectionScale); 

    }
}