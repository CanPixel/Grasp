using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {
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
        COM12
    }
    public static Port port;
    private static PlayerInput self;
    public static SerialPort stream;

    public GameObject ConnectionText;
    private float ConnectionScale = 0;

    public bool debug = false;
    public int baudRate;

    private RawImage overlay;
    private float overlayBaseAlpha;

    [System.Serializable]
    public struct ControllerValues {
        public int heartRate;
        public float xAim, yAim;

        public void Print() {
            if(!PlayerInput.IsDebugging()) return;
            Debug.Log("[Heart Rate]: " + heartRate + " | (" + xAim + "," + yAim + ")");
        }

        public void SetValues(string text) {
            int.TryParse(text, out heartRate);
            //this.xAim = xAim;
            //this.yAim = yAim;
        }
    }
    public ControllerValues controllerValues;

    void Start() {
        overlay = GetComponent<RawImage>();
        controllerValues = new ControllerValues();
        overlayBaseAlpha = overlay.color.a;
        self = this;
        StartCoroutine(Connect());
    }

    void Update() {
        ReadSensor();
        controllerValues.Print();
    }

    protected void ReadSensor() {
        string read = PlayerInput.ReadSensors();
        if(read == null) return;
        controllerValues.SetValues(read);
    }

    IEnumerator Connect() {
        if(self == null) yield return new WaitForSeconds(0.5f);
        string[] ports = System.Enum.GetNames(typeof(Port));
        for(int i = 0; i < ports.Length; i++) {
            string port = ports[i];
            string[] portNums = System.Text.RegularExpressions.Regex.Split(port, @"\D+");
            stream = (int.Parse(portNums[1]) > 10) ? new SerialPort("\\\\.\\" + port, baudRate, Parity.None, 8, StopBits.One) : 
            new SerialPort(port.ToString(), baudRate, Parity.None, 8, StopBits.One);

            try {
                stream.Open();
                stream.ReadTimeout = 1;
            } catch(System.IO.IOException) {}
        }
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
        return controllerValues.heartRate > 0;
    }

    void OnGUI() {
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, Mathf.Lerp(overlay.color.a, HasConnection() ? 0 : overlayBaseAlpha, Time.deltaTime * 2));
        if(HasConnection()) ConnectionScale = Mathf.Lerp(ConnectionScale, 0, Time.deltaTime * 3);
        else ConnectionScale = Mathf.Lerp(ConnectionScale, 1, Time.deltaTime * 3);
        ConnectionText.transform.localScale = new Vector3(ConnectionScale, ConnectionScale, ConnectionScale); 

         if(PlayerInput.IsDebugging()) return;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 32;
        GUI.Label(new Rect(10, 10, 100, 100), controllerValues.heartRate.ToString(), style);
    }
}