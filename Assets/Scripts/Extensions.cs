using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Converts a world position to the local position of the transform.
    /// </summary>
    public static Vector3 LocalPosition(this Transform transform, Vector3 value)
    {
        return transform.position + (transform.right * value.x + transform.up * value.y + transform.forward * value.z);
    }
}
