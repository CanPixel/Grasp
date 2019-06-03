using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {
    public GameObject flashPoint;
    public GameObject narrowBeam;
    public Light lightPoint;

    private bool lightOn = true;
    private static Flashlight self;

    private float beamDelay = 0;
    private bool castBeam = false;

    [Header("Light Beam Settings")]
    public float castingSpeed = 8;
    public float castingLength = 2;
    public float holdTillBeam = 0.5f;

    void Awake() {
        self = this;
        narrowBeam.SetActive(false);
    }

    void Update() {
        if(Input.GetMouseButtonDown(2) && !PlayerInput.UsingAlternativeControls()) SwitchLight(!lightOn);

        Debug.Log(Input.GetMouseButtonDown(2) + " | " + Input.GetMouseButton(2));

        if (Input.GetMouseButton(2))
        {
            beamDelay += Time.deltaTime;
            if (beamDelay > holdTillBeam) ActivateBeam(true);
        }
        else ActivateBeam(false);

        if (narrowBeam.activeSelf && castBeam) narrowBeam.transform.localScale = new Vector3(narrowBeam.transform.localScale.x, narrowBeam.transform.localScale.y, Mathf.Lerp(narrowBeam.transform.localScale.z, castingLength, Time.deltaTime * castingSpeed));

        CastLight();
    }

    private void ActivateBeam(bool i)
    {
        narrowBeam.SetActive(i);
        lightPoint.enabled = !i;
        if (!i)
        {
            beamDelay = 0;
            castBeam = false;
        }
        else if (!castBeam)
        {
            narrowBeam.transform.localScale = new Vector3(1, 1, 0);
            castBeam = true;
        }
    }

    public static bool IsLightOn() {
        return self.lightOn;
    }

    protected void CastLight() {
        RaycastHit hit;
        bool rayHit = Physics.Raycast(lightPoint.transform.position, lightPoint.transform.forward, out hit, 20);
        if(rayHit && hit.collider.gameObject != null) {
            if(lightOn && hit.collider.gameObject.tag == "Vampire") hit.collider.gameObject.GetComponent<Enemy>().Scare(transform.position);
            if(lightOn && hit.collider.gameObject.tag != "Light") flashPoint.transform.position = hit.point;
        }
        if(!rayHit || !lightOn) flashPoint.transform.position = new Vector3(0, -100, 0);
    }

    public void SwitchLight() {
        SwitchLight(!lightOn);
    }

    protected void SwitchLight(bool i) {
        lightOn = i;
        lightPoint.enabled = i;
        SoundManager.PlaySoundAt("Flashlight", transform.position, SoundManager.PLAYER_VOLUME, 1.2f);
    }
}
