using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public enum MenuItemFunction { START, CONTROLS, QUIT }
    public MenuItemFunction menuItem;
    public Animator leftDoorAnimator, rightDoorAnimator;
    public GameObject uiInstructions;

    public bool active { get; set; } = false;

    private Material thisMat;
    private Color emissionColor;
    private float timer = -1;

    private void Awake()
    {
        thisMat = GetComponent<MeshRenderer>().material;
        emissionColor = thisMat.GetColor("_EmissiveColor");
    }

    public void Update()
    {
        if (timer > -1)
        {
            timer -= Time.deltaTime;
            if (timer != -1 && timer < 0)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Canneke");
            }
        }
        if (!active)
        {
            thisMat.SetColor("_EmissiveColor", Color.black);
        }
        if (active)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SoundManager.PlaySound("Splash1");
                //Debug.Log("A1 formaat is ook een formaat");
                switch (menuItem)
                {
                    case MenuItemFunction.START:
                        leftDoorAnimator.enabled = rightDoorAnimator.enabled = true;
                        FindObjectOfType<PlayerController>().locked = false;
                        timer = 2;
                        break;
                    case MenuItemFunction.CONTROLS:
                        uiInstructions.SetActive(!uiInstructions.activeSelf);
                        break;
                    case MenuItemFunction.QUIT:
                        Application.Quit();
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#endif
                        break;
                    default:
                        break;
                }
            }
            active = false;
        }
    }

    public void OnCastLightAt()
    {
        //Debug.Log(name);
        thisMat.SetColor("_EmissiveColor", Color.white);
        active = true;
    }
}
