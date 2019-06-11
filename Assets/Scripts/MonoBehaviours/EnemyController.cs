using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyController : MonoBehaviour
{
    public bool moth = false;
    public enum State { Idle, Detecting, Chasing, Attacking, Hiding }
    public State currentState = State.Idle;
    [Space]
    public float detectionRange = 25f;
    public float stoppingDistance = 2f;
    public float grabRange = 0.45f;
    public float maxSlopeAngle = 45;
    public float maxJumpHeight = 6;
    public float jumpForce = 4;
    public Transform rightHand, leftHand;

    public Transform target { get; set; }
    public float remainingDistance { get; set; }
    public Vector3 direction { get; set; }
    public bool isAnimating { get; set; }
    public Animator animator { get; private set; } = default;

    [SerializeField] float m_GroundCheckDistance = 0.2f;
    [SerializeField] PhysicMaterial m_GroundedMaterial = default, m_AirborneMaterial = default;
    [SerializeField] LayerMask m_GroundLayers = 0;
    [SerializeField] bool m_AllowJumps = true;
    [SerializeField] Vector3 m_CrouchColliderCenter = new Vector3(0, 0.62f, 0);
    [SerializeField] float m_CrouchColliderHeight = 1.27f;

    private float m_StateTimer = 0;
    private float m_ScareTimer = 0;
    private bool m_Grounded = true;
    private CapsuleCollider m_EnemyCollider;
    private Rigidbody m_Rigidbody;
    private PlayerController player;
    private Vector3 m_StartingPosition;
    private Vector3 m_StartingColliderCenter;
    private float m_StartingColliderHeight;

    private EnemySound sounds;

    private void Awake()
    {
        sounds = GetComponent<EnemySound>();
        m_StartingPosition = transform.position;
        player = FindObjectOfType<PlayerController>();
        target = player.transform;
        direction = Vector3.zero;
        animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_EnemyCollider = GetComponent<Collider>() as CapsuleCollider;
        m_StartingColliderCenter = m_EnemyCollider.center;
        m_StartingColliderHeight = m_EnemyCollider.height;
    }

    private void Update()
    {
        AnyState();
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Detecting:
                Detect();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
            case State.Hiding:
                Hide();
                break;
            default:
                TransitionToState(State.Idle);
                break;
        }
    }

    #region States
    private void AnyState()
    {
        if (isAnimating) return;

        CheckGrounded();

        bool crouch = animator.GetBool("Crouch");
        m_EnemyCollider.height = crouch ? m_CrouchColliderHeight : m_StartingColliderHeight;
        m_EnemyCollider.center = crouch ? m_CrouchColliderCenter : m_StartingColliderCenter;
    }

    private void Idle()
    {
        //Wait for player to get in range and player is in front of the enemy, then act.
        if (!player.dead && Vector3.Distance(target.position, transform.position) <= detectionRange)
        {
            TransitionToState(State.Detecting);
        }
        animator.SetFloat("Speed", 0, 1, 0.5f);
    }

    private void Detect()
    {
        //Check if there is no object between the enemy and the target
        Debug.DrawLine(transform.position + Vector3.up, target.position + Vector3.up);
        if (Physics.Linecast(transform.position + Vector3.up, target.position + Vector3.up, out RaycastHit hit) && !hit.collider.CompareTag("Player")) return;
        animator.SetTrigger("Roar");
        if (Countdown(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length))
        {
            if (!player.dead && Vector3.Distance(target.position, transform.position) <= detectionRange)
            {
                    TransitionToState(State.Chasing);
            }
            else
            {
                TransitionToState(State.Idle);
            }
        }
    }

    private void Chase()
    {
        Move();
        if (moth && !Flashlight.IsLightOn())
        {
            animator.SetTrigger("Hurt");
            TransitionToState(State.Hiding);
            return;
        }
        if (remainingDistance <= 0)
        {
            TransitionToState(State.Attacking);
        }
        if (remainingDistance > detectionRange)
        {
            TransitionToState(State.Idle);
        }
    }

    private void Attack()
    {
        if (isAnimating) return;
        sounds.Scream();
        sounds.Attack();
        Move();
        if (player.dead) TransitionToState(State.Detecting);
        if (remainingDistance > stoppingDistance)
        {
            TransitionToState(State.Chasing);
        }
        if (remainingDistance <= stoppingDistance)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void Hide()
    {
        direction = target.position - transform.position;
        if (m_ScareTimer < 1.5f || moth)
        {
            animator.SetFloat("Speed", Mathf.Min(1, Mathf.Abs(direction.x)));
            if (Mathf.Abs(direction.x) > 0.05f)
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (direction.x > 0 ? 270 : 90), Time.deltaTime * 20 * Mathf.Abs(direction.x));
        }
        if (!moth && (direction.magnitude > detectionRange || m_ScareTimer <= 0))
        {
            TransitionToState(State.Chasing);
        }
        m_ScareTimer -= Time.deltaTime;
    }
    #endregion

    public void TransitionToState(State toState)
    {
        if (toState != currentState)
        {
            m_StateTimer = 0;
            currentState = toState;
        }
    }

    public void OnCastLightAt()
    {
        Debug.Log($"OnCastLightAt called by {gameObject.name}.");
        if (player.dead) return;
        if (moth)
        {
            TransitionToState(State.Chasing);
        }
        else
        {
            m_ScareTimer = 2;
            if (currentState != State.Hiding)
            {
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Hurt");
                TransitionToState(State.Hiding);
            }
        }
    }

    private void Move()
    {
        if (!target)
        {
            direction = m_StartingPosition - transform.position;
        }
        else
        {
            direction = target.position - transform.position;
        }
        float distance = Vector3.Distance(transform.position, target.position);
        remainingDistance = Mathf.Max(0, distance - stoppingDistance);
        animator.SetFloat("Speed", Mathf.Min(1, distance));
        if (Mathf.Abs(direction.x) > 0.05f)
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (direction.x > 0 ? 90 : 270), Time.deltaTime * 20 * Mathf.Abs(direction.x));
        if (m_Grounded)
        {
            if (m_AllowJumps && MustJump())
            {
                Jump();
            }
        }
        else
        {
            //Add a small force when in air to control airborne movement.
            m_Rigidbody.AddForce(transform.forward * jumpForce);
        }
    }

    private bool MustJump()
    {
        //If an object is found at foot level, check if its highest surface is reachable with a jump.
        //Do a raycast adjacent to the enemy
        RaycastHit adjacentHit = new RaycastHit();

        if (Physics.Raycast(transform.position + new Vector3(transform.eulerAngles.y == 90 ? 1 : -1, 6, 0), Vector3.down * maxJumpHeight, out adjacentHit, maxJumpHeight, m_GroundLayers))
        {
            if (adjacentHit.point.y <= transform.position.y) return false;

            //Determine if the object adjacent to the foot is too steep to walk and if a jump is required.
            RaycastHit footCast = new RaycastHit();
            Debug.DrawRay(transform.position + Vector3.up * m_GroundCheckDistance, transform.forward);
            if (!Physics.Raycast(transform.position + Vector3.up * m_GroundCheckDistance, transform.forward, out footCast)) return false;
            float slopeAngle = Vector3.Angle(footCast.normal, transform.up);
            if (slopeAngle < maxSlopeAngle)
            {
                Debug.DrawRay(transform.position + new Vector3(direction.x > 0 ? 1.5f : -1.5f, maxJumpHeight, 0), Vector3.down * maxJumpHeight, Color.red);
                return false;
            }

            //Determine if the planned landing area angle is not scalable
            slopeAngle = Vector3.Angle(adjacentHit.normal, transform.up);
            if (slopeAngle > maxSlopeAngle)
            {
                Debug.DrawRay(transform.position + new Vector3(direction.x > 0 ? 1.5f : -1.5f, maxJumpHeight, 0), Vector3.down * maxJumpHeight, Color.red);
                return false;
            }

            //Return if a jump is not too high.
            float height = adjacentHit.point.y - transform.position.y;
            if (height < maxJumpHeight && height > m_GroundCheckDistance)
            {
                Debug.DrawRay(transform.position + new Vector3(direction.x > 0 ? 1.5f : -1.5f, maxJumpHeight, 0), Vector3.down * maxJumpHeight, Color.green);
                return true;
            }
        }
        return false;
    }

    public void Jump()
    {
        m_Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    private void CheckGrounded()
    {
        RaycastHit ground = new RaycastHit();
        if (animator == null) return;
        m_Grounded = animator.applyRootMotion = Physics.Raycast(transform.position + Vector3.up * 0.02f, Vector3.down * m_GroundCheckDistance, out ground, m_GroundCheckDistance, m_GroundLayers);
        m_EnemyCollider.material = m_Grounded ? m_GroundedMaterial : m_AirborneMaterial;
        animator.SetBool("Grounded", m_Grounded);
    }

    //Animator Event function
    public void CheckAttackHit(AnimationInitializer anim)
    {
        //Right hand
        Collider[] hits = Physics.OverlapSphere(rightHand.position, grabRange, 1 << LayerMask.NameToLayer("Player"));
        if (hits.Length > 0 && hits[0].GetComponent<PlayerController>())
        {
            AttackHit(anim);
            return;
        }
        //Left hand
        hits = Physics.OverlapSphere(leftHand.position, grabRange, 1 << LayerMask.NameToLayer("Player"));
        if (hits.Length > 0 && hits[0].GetComponent<PlayerController>())
        {
            AttackHit(anim);
            return;
        }
    }

    private void AttackHit(AnimationInitializer anim)
    {
        if (anim)
        {
            anim.Activate(transform, target.transform);
            target = null;
            TransitionToState(State.Idle);
            animator.SetFloat("Speed", 0);
            isAnimating = true;
        }
        else throw new MissingComponentException("No AnimationInitializer set for \"" + animator.GetCurrentAnimatorClipInfo(0)[0].clip.name + "\".");
    }

    private bool Countdown(float duration) => (m_StateTimer += Time.deltaTime) >= duration;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.9f, 0.11f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, detectionRange);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, stoppingDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rightHand.position, grabRange);
        Gizmos.DrawWireSphere(leftHand.position, grabRange);
    }
}
