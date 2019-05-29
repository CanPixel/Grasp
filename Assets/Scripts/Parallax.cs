using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    [System.Serializable]
    public class ParallaxScroll {
        public GameObject texture;
        public float sensitivity;
        private Vector3 originalPos;

        public void Init() {
            originalPos = texture.transform.position;
        }

        public void Scroll(Vector3 pos) {
            Vector3 target = pos / sensitivity + originalPos;
            target.y = originalPos.y;
            texture.transform.position = target;
        }
    }
    public ParallaxScroll[] parallax;

    void Start() {
        foreach(ParallaxScroll i in parallax) i.Init();
    }

    void FixedUpdate() {
        foreach(ParallaxScroll i in parallax) i.Scroll(Camera.main.transform.position);        
    }
}
