using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public enum MenuItemFunction { START, CONTROLS, QUIT }
    public MenuItemFunction menuItem;

    public bool active { get; set; } = false;

    private Material thisMat;
    private Color emissionColor;

    private void Awake()
    {
        thisMat = GetComponent<MeshRenderer>().material;
        emissionColor = thisMat.GetColor("_EmissiveColor");
    }

    public void OnCastLightAt()
    {
        Debug.Log(name);
        thisMat.SetColor("_EmissiveColor", Color.white);
        active = true;
    }

    public void Update()
    {
        thisMat.SetColor("_EmissiveColor", Color.black);

        if (active)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("A1 formaat is ook een formaat");
                switch (menuItem)
                {
                    case MenuItemFunction.START:

                        break;
                    case MenuItemFunction.CONTROLS:
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


}
