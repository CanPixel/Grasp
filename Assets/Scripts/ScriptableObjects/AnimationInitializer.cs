using UnityEngine;

[CreateAssetMenu(menuName = "Animation Initializer")]
public class AnimationInitializer : ScriptableObject
{
    public string clipName = "Animation Clip";
    public TransformSet transform;
}

[System.Serializable]
public struct TransformSet
{
    public Vector3 localPosition;
    public Vector3 localEulerAngles;
    public Vector3 localScale;
}
