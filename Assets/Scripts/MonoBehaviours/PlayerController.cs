using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveDampTime, moveDeltaTime, idleTimer;
    public Transform ikTarget;
    //Speed for IK smoothness
    public float ikAdaptSpeed = 2;

    public float ikWeight { get; set; }
    public float ikLookWeight { get; set; }
    public Vector3 desiredIKPosition { get; set; }
    public float desiredIKWeight { get; set; }

    [SerializeField] float m_JumpHeightMultiplier = 1.8f;
    [SerializeField] float m_GroundCheckDistance = 0.02f;
    [SerializeField] LayerMask m_GroundLayers = 0;
    [SerializeField] PhysicMaterial m_GroundedMaterial = null, m_AirborneMaterial = null;

    private Vector3 m_Move;
    private Vector3 m_VelocityBuffer;
    private Animator m_Animator;
    private float m_IdleTimer;
    private bool m_Grounded;
    private Rigidbody m_Rigidbody;
    private Collider m_PlayerCollider;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_PlayerCollider = GetComponent<Collider>();

        desiredIKWeight = 1;
    }

    private void FixedUpdate()
    {
        //Physics stuff, grounded check etc.
        CheckGrounded();
        if (!m_Grounded)
        {
            //Enable player to control air movement slightly
            m_Rigidbody.AddForce(new Vector3(m_Move.x, 0, 0));
        }
    }

    private void Update()
    {
        m_Move.x = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            m_Move.x *= 0.2f;
        }
        m_Move.y = Input.GetAxisRaw("Vertical");
        if (m_Grounded && m_Move.y > 0)
        {
            //Jump. Convert current input to velocity and disable Root Motion controls
            m_Animator.applyRootMotion = false;
            m_Grounded = false;
            m_Rigidbody.velocity += Vector3.Scale(m_Move, Vector3.up * m_JumpHeightMultiplier);
        }

        Animate();

        IKAim();
    }

    private void Animate()
    {
        m_Animator.SetFloat("Speed", m_Move.x, moveDampTime, moveDeltaTime);
        m_Animator.SetFloat("JumpCrouch", m_Move.y);
        m_Animator.SetBool("Grounded", m_Grounded);

        if (Mathf.Approximately(m_Animator.GetFloat("Speed"), 0) && m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Active"))
        {
            if (m_IdleTimer > 0)
            {
                m_IdleTimer -= Time.deltaTime;
            }
            else
            {
                m_IdleTimer = idleTimer;
                m_Animator.SetTrigger("Idle");
            }
        }
        else
        {
            m_IdleTimer = idleTimer;
        }

        //Enable/Disable ik Look Target Weight during special idle animations
        ikLookWeight = Mathf.MoveTowards(ikLookWeight, m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Ative") ? 1 : 0, Time.deltaTime * 5);

        if (!m_Grounded)
        {
            m_Animator.SetFloat("Impact", Mathf.Abs(m_Rigidbody.velocity.y));
        }
    }

    private void CheckGrounded()
    {
        //Check if the player is currently on the ground.
        RaycastHit ground = new RaycastHit();

        m_Grounded = Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out ground, m_GroundCheckDistance, m_GroundLayers);
        m_Animator.applyRootMotion = m_Grounded;
        m_PlayerCollider.material = m_Grounded ? m_GroundedMaterial : m_AirborneMaterial;
    }

    private void IKAim()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        //Sets the desired IK Target to Mouse Position. Add a tiny z offset to prevent the hand from going through the body.
        desiredIKPosition = new Vector3(ray.origin.x + ray.direction.x * 10, ray.origin.y + ray.direction.y * 10, -0.1f);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        ikWeight = Mathf.Lerp(ikWeight, desiredIKWeight, Time.deltaTime * ikAdaptSpeed);
        ikTarget.position = Vector3.Lerp(ikTarget.position, desiredIKPosition, Time.deltaTime * ikAdaptSpeed);

        //Right Hand IK
        m_Animator.SetIKPosition(AvatarIKGoal.RightHand, ikTarget.position);
        m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);

        //Right Hand Rotation IK
        Vector3 offset = ikTarget.position - m_Animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        Quaternion rotation = Quaternion.LookRotation(offset, Vector3.up);
        m_Animator.SetIKRotation(AvatarIKGoal.RightHand, rotation);
        m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);

        //Head Look IK
        m_Animator.SetLookAtPosition(ikTarget.position);
        m_Animator.SetLookAtWeight(ikLookWeight);
    }
}
