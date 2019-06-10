using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class EnemyLandmark : MonoBehaviour
{
    public enum MovementType { Stand, Crouch, Jump };
    public MovementType movementType;

    private void OnTriggerEnter(Collider other)
    {
        EnemyController ec = other.GetComponent<EnemyController>();

        if (other.CompareTag("Moth") || other.CompareTag("Vampire"))
        {
            switch (movementType)
            {
                case MovementType.Stand:
                    ec.animator.SetBool("Crouch", false);
                    break;
                case MovementType.Crouch:
                    ec.animator.SetBool("Crouch", true);
                    break;
                case MovementType.Jump:
                    ec.Jump();
                    break;
                default:
                    goto case MovementType.Stand;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.55f, 0.69f, 0.54f);
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
    }
}
