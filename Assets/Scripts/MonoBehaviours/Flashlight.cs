using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {
    public GameObject flashPoint;
    public Light lightPoint;

    private bool lightOn = true;
    private static Flashlight self;

    void Awake() {
        self = this;
    }

    void Update() {
        if(Input.GetMouseButtonDown(2) && !PlayerInput.UsingAlternativeControls()) SwitchLight(!lightOn);
        CastLight();
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
