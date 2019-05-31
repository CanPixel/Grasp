using UnityEngine;

[CreateAssetMenu(menuName = "Animation Initializer")]
public class AnimationInitializer : ScriptableObject
{
    public string clipName = "Animation Clip";
    public TransformSet transform;

    public void Activate(Transform user, Transform target)
    {
        //Set right transforms
        Vector3 localPosition = user.LocalPosition(transform.offsetPosition);
        target.position = localPosition;
        target.eulerAngles = user.eulerAngles + transform.offsetEulerAngles;
        target.localScale += transform.offsetScale;

        //Animation Handling
        target.GetComponent<Rigidbody>().isKinematic = true;
        user.GetComponent<Rigidbody>().isKinematic = true;
        target.GetComponent<Animator>().Play(clipName);
        user.GetComponent<Animator>().Play(clipName);
    }
}

[System.Serializable]
public struct TransformSet
{
    public Vector3 offsetPosition;
    public Vector3 offsetEulerAngles;
    public Vector3 offsetScale;
}
