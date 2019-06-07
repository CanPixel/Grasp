﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour {
    public GameObject flashPoint;
    public GameObject narrowBeam;
    public Light lightPoint;

    private bool lightOn = true, beamLightOn = false;
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

        if (Input.GetMouseButton(2) || (PlayerInput.UsingAlternativeControls() && PlayerInput.GetControllerValues().click == 0)) {
            beamDelay += Time.deltaTime;
            if (beamDelay > holdTillBeam) ActivateBeam(true);
        }
        if(Input.GetMouseButtonUp(2) || (PlayerInput.UsingAlternativeControls() && PlayerInput.GetControllerValues().click == 1)) ActivateBeam(false);

        if (narrowBeam.activeSelf && castBeam) narrowBeam.transform.localScale = new Vector3(narrowBeam.transform.localScale.x, narrowBeam.transform.localScale.y, Mathf.Lerp(narrowBeam.transform.localScale.z, castingLength, Time.deltaTime * castingSpeed));

        CastLight();
    }

    private void ActivateBeam(bool i) {
        if(i && !narrowBeam.activeSelf) SoundManager.PlaySoundAt("BeamLight", transform.position, SoundManager.PLAYER_VOLUME, 1.2f);
        narrowBeam.SetActive(i);
        if(narrowBeam.activeSelf) {
            lightOn = false;
            lightPoint.enabled = false;
            beamLightOn = true;
        }
        else beamLightOn = false;
        if (!i) {
            beamDelay = 0;
            castBeam = false;
        } else if (!castBeam) {
            narrowBeam.transform.localScale = new Vector3(1, 1, 0);
            castBeam = true;
        }
    }

    public static bool IsLightOn() {
        return self.lightOn;
    }

    public static bool isBeamLight() {
        return self.beamLightOn;
    }

    protected void CastLight() {
        RaycastHit hit;
        bool rayHit = Physics.Raycast(lightPoint.transform.position, lightPoint.transform.forward, out hit, 20);
        if(rayHit && hit.collider.gameObject != null) {
            if(lightOn && (hit.collider.gameObject.CompareTag("Vampire") || hit.collider.gameObject.CompareTag("Moth")))
                hit.collider.gameObject.GetComponent<EnemyController>().OnCastLightAt();
            if((beamLightOn || lightOn) && !hit.collider.gameObject.CompareTag("Light")) flashPoint.transform.position = hit.point;
        }
        if(!rayHit || (!lightOn && !beamLightOn)) flashPoint.transform.position = new Vector3(0, -100, 0);
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
