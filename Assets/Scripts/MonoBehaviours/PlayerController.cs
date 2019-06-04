using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveDampTime, moveDeltaTime, idleTimer;
    public Transform ikTarget;
    //Speed for IK smoothness
    public float ikAdaptSpeed = 2;
    public float maxXSpeed = 5;
    public float airControlForce = 4;

    public bool isGrabbing { get; set; }
    public bool isCrouching { get; private set; }
    public float ikWeight { get; set; }
    public float ikLookWeight { get; set; }
    public Vector3 desiredIKPosition { get; set; }
    public float desiredIKWeight { get; set; }
    public float desiredIKWeightLeftHand { get; set; }
    public Animator animator { get; private set; }
    public
#if UNITY_EDITOR
        new
#endif
        Rigidbody rigidbody { get; private set; }
    public Transform overrideIKTarget { get; set; }
    public bool dead { get; set; }

    [SerializeField] float m_JumpHeightMultiplier = 1.8f;
    [SerializeField] float m_GroundCheckDistance = 0.02f;
    [SerializeField] LayerMask m_GroundLayers = 0;
    [SerializeField] PhysicMaterial m_GroundedMaterial = null, m_AirborneMaterial = null;
    [SerializeField] float m_ColliderCrouchSize = 1.2f;
    [SerializeField] Vector3 m_ColliderCrouchCenter = new Vector3(0, 0.6f, 0);

    [Space]
    public bool locked;

    private Vector3 m_Move;
    private Vector3 m_VelocityBuffer;
    private float m_IdleTimer;
    private bool m_Grounded;
    private CapsuleCollider m_PlayerCollider;
    private float m_OriginalColliderSize;
    private Vector3 m_OriginalColliderCenter;

    private float jumpDelay = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        m_PlayerCollider = GetComponent<CapsuleCollider>();
        m_OriginalColliderCenter = m_PlayerCollider.center;
        m_OriginalColliderSize = m_PlayerCollider.height;

        desiredIKWeight = 1;
    }

    private void FixedUpdate()
    {
        //Physics stuff, grounded check etc.
        CheckGrounded();
        if (!m_Grounded)
        {
            //Enable player to control air movement slightly
            if (rigidbody.velocity.x < maxXSpeed) rigidbody.AddForce(new Vector3(m_Move.x * airControlForce, 0, 0), ForceMode.Acceleration);
        }
        //Turning
        if (!isGrabbing && Mathf.Abs(m_Move.x) > 0.05f) transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (m_Move.x > 0 ? 90 : 270), Time.deltaTime * 20 * Mathf.Abs(m_Move.x));
    }

    private void Update() 
    {
        if (dead) enabled = false;
        if (jumpDelay > 0) jumpDelay -= Time.deltaTime;

        //Input
        m_Move.x = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift)) m_Move.x *= 0.2f;
        m_Move.y = Input.GetAxisRaw("Vertical");
        if(Input.GetKey(KeyCode.Space)) m_Move.y = 1;

        if (locked) m_Move = Vector3.zero;

        //Grounded Controls
        if (m_Grounded) {
            if (m_Move.y > 0 && !isCrouching)
            {
                //Jump. Convert current input to velocity and disable Root Motion controls
                animator.applyRootMotion = false;
                m_Grounded = false;
                rigidbody.velocity += Vector3.Scale(m_Move, Vector3.up * m_JumpHeightMultiplier * (Time.deltaTime * 90));
                if(jumpDelay <= 0) JumpSound();
            }
            m_PlayerCollider.center = !isCrouching ? m_OriginalColliderCenter : m_ColliderCrouchCenter;
            m_PlayerCollider.height = !isCrouching ? m_OriginalColliderSize : m_ColliderCrouchSize;
            if (m_Move.y < 0)
            {
                m_Move.x = Mathf.Min(m_Move.x, 0.2f);
                //Crouch
                isCrouching = true;
            }
            if (isCrouching && m_Move.y >= 0)
            {
                //Standing after crouch. Requires check if possible.
                isCrouching = Physics.Raycast(transform.position, transform.up, m_OriginalColliderSize, m_GroundLayers);
            }
        }

        Animate();

        IKAim();
    }

    private void JumpSound() {
        jumpDelay = 0.5f;
        SoundManager.PlaySoundAt("Jump", transform.position, 0.05f, Random.Range(0.9f, 1f));
    }

    private void Animate()
    {
        animator.SetFloat("Speed", Mathf.Abs(m_Move.x), moveDampTime, moveDeltaTime);
        animator.SetFloat("JumpCrouch", m_Move.y);
        animator.SetBool("Grounded", m_Grounded);
        animator.SetBool("Grabbing", isGrabbing);
        animator.SetBool("Crouching", isCrouching);

        if (Mathf.Approximately(animator.GetFloat("Speed"), 0) && animator.GetCurrentAnimatorStateInfo(0).IsTag("Active"))
        {
            if (m_IdleTimer > 0)
            {
                m_IdleTimer -= Time.deltaTime;
            }
            else
            {
                m_IdleTimer = idleTimer;
                animator.SetTrigger("Idle");
            }
        }
        else
        {
            m_IdleTimer = idleTimer;
        }

        //Enable/Disable ik weights during special animations
        ikLookWeight = Mathf.MoveTowards(ikLookWeight, animator.GetCurrentAnimatorStateInfo(0).IsTag("Active") ? 1 : 0, Time.deltaTime * 5);
        desiredIKWeightLeftHand = Mathf.Lerp(desiredIKWeightLeftHand, isGrabbing ? 1 : 0, Time.deltaTime * ikAdaptSpeed * 2);

        if (!m_Grounded) animator.SetFloat("Impact", Mathf.Abs(rigidbody.velocity.y));
    }

    private void CheckGrounded()
    {
        //Check if the player is currently on the ground.
        RaycastHit ground = new RaycastHit();

        m_Grounded = Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out ground, m_GroundCheckDistance, m_GroundLayers);
        animator.applyRootMotion = m_Grounded;
        m_PlayerCollider.material = m_Grounded ? m_GroundedMaterial : m_AirborneMaterial;
    }

    private void IKAim() {
        if(PlayerInput.UsingAlternativeControls()) desiredIKPosition = new Vector3(Camera.main.transform.position.x + -PlayerInput.GetControllerValues().xAim * 10, Camera.main.transform.position.y + PlayerInput.GetControllerValues().yAim * 10, -0.1f);
        else {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);

            //Sets the desired IK Target to Mouse Position. Add a tiny z offset to prevent the hand from going through the body.
            desiredIKPosition = new Vector3(ray.origin.x + ray.direction.x * 10, ray.origin.y + ray.direction.y * 10, -0.1f);
        }
    }

    public Transform SetIKOverrideTarget(Transform target)
    {
        overrideIKTarget = target;
        return overrideIKTarget;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        ikWeight = Mathf.Lerp(ikWeight, desiredIKWeight, Time.deltaTime * ikAdaptSpeed);
        Vector3 ikPosition = overrideIKTarget == null ? desiredIKPosition : overrideIKTarget.position;
        ikTarget.position = Vector3.Lerp(ikTarget.position, ikPosition, Time.deltaTime * ikAdaptSpeed);

        //Right Hand IK
        animator.SetIKPosition(AvatarIKGoal.RightHand, ikTarget.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);

        //Left Hand IK
        animator.SetIKPosition(AvatarIKGoal.LeftHand, ikPosition);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, desiredIKWeightLeftHand);

        //Right Hand Rotation IK
        Vector3 offset = ikTarget.position - animator.GetBoneTransform(HumanBodyBones.RightHand).position;
        Quaternion rotation = Quaternion.LookRotation(offset, Vector3.up);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rotation);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);

        //Head Look IK
        animator.SetLookAtPosition(ikTarget.position);
        animator.SetLookAtWeight(ikLookWeight, 0.1f, 0.2f);
    }
}
