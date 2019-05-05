using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour {
    private Vector3 baseCamPos;

    public GameObject playerLight;
    private Light shine;
    private float lightRotation;
    private float dir = 1;

    private PlayerInput.ControllerValues controller;
    private ThirdPersonCharacter character;

    void Start() {
        baseCamPos = Camera.main.transform.position;
        shine = playerLight.GetComponentInChildren<Light>();
        character = GetComponent<ThirdPersonCharacter>();

        controller = PlayerInput.GetControllerValues();
        PlayerInput.AddClickEvent(Click);
    }

    void FixedUpdate() {
        Camera.main.transform.position = new Vector3(Mathf.Lerp(Camera.main.transform.position.x, transform.position.x, Time.deltaTime * 2), baseCamPos.y, baseCamPos.z);
    
        transform.localRotation = Quaternion.Euler(0, Mathf.LerpAngle(transform.localEulerAngles.y, dir * 90, Time.deltaTime * 8), 0);

        RotateLight();
    }

    protected void RotateLight() {
        float xRad = controller.xAim;
        float yRad = controller.yAim;
        if(Util.InRange(xRad, 0, 11) && Util.InRange(yRad, 0, 11)) lightRotation = Mathf.LerpAngle(lightRotation, 0, Time.deltaTime * 4);
        else lightRotation = Mathf.LerpAngle(lightRotation, 180 - Mathf.Atan2(xRad, yRad) * Mathf.Rad2Deg - 90, Time.deltaTime * 8);
        playerLight.transform.rotation = Quaternion.Euler(lightRotation, 90, 0);

        Debug.Log(xRad);

        if(xRad < 0) dir = -1;//character.Move(new Vector3(-1, 0, 0), false, false);
        else dir = 1;
    }

    public void Click() {
        shine.enabled = !shine.enabled;
    }
}
