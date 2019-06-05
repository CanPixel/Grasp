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
    public bool scared { get; private set; }

    [SerializeField] float m_GroundCheckDistance = 0.2f;
    [SerializeField] PhysicMaterial m_GroundedMaterial = default, m_AirborneMaterial = default;
    [SerializeField] LayerMask m_GroundLayers = 0;

    private float m_StateTimer = 0;
    private Animator m_Animator = default;
    private bool m_Grounded = true;
    private CapsuleCollider m_EnemyCollider;
    private Rigidbody m_Rigidbody;
    private PlayerController player;
    private Vector3 m_StartingPosition;

    private void Awake()
    {
        m_StartingPosition = transform.position;
        player = FindObjectOfType<PlayerController>();
        target = player.transform;
        direction = Vector3.zero;
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_EnemyCollider = GetComponent<Collider>() as CapsuleCollider;
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
        if (!isAnimating)
            CheckGrounded();
    }

    private void Idle()
    {
        //Wait for player to get in range and player is in front of the enemy, then act.
        if (!player.dead && Vector3.Distance(target.position, transform.position) <= detectionRange)
        {
            TransitionToState(State.Detecting);
        }
        m_Animator.SetFloat("Speed", 0, 1, 0.5f);
    }

    private void Detect()
    {
        if (Countdown(m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length))
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
        Move();
        if (remainingDistance > stoppingDistance)
        {
            TransitionToState(State.Chasing);
        }
        if (remainingDistance <= stoppingDistance)
        {
            m_Animator.SetTrigger("Attack");
        }
    }

    private void Hide()
    {
        direction = target.position - transform.position;
        m_Animator.SetFloat("Speed", Mathf.Min(1, Mathf.Abs(direction.x)));
        if (Mathf.Abs(direction.x) > 0.05f)
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (direction.x > 0 ? 270 : 90), Time.deltaTime * 20 * Mathf.Abs(direction.x));
        if (direction.magnitude > detectionRange)
        {
            TransitionToState(State.Chasing);
        }
    }
    #endregion

    public void TransitionToState(State toState)
    {
        if (toState != currentState)
        {
            m_StateTimer = 0;
            currentState = toState;
        }
        else
        {
            Debug.LogWarning("Transition to active state is redundant. Cancelling.", gameObject);
        }
    }

    public void OnCastLightAt()
    {
        if (player.dead) return;
        if (moth)
        {
            //Extra behaviour
        }
        else
        {
            if (currentState != State.Hiding && !scared)
            {
                m_Animator.ResetTrigger("Attack");
                m_Animator.SetTrigger("Hurt");
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
        remainingDistance = Mathf.Max(0, direction.magnitude - stoppingDistance);
        m_Animator.SetFloat("Speed", Mathf.Min(1, Mathf.Abs(direction.x) - remainingDistance));
        if (Mathf.Abs(direction.x) > 0.05f)
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (direction.x > 0 ? 90 : 270), Time.deltaTime * 20 * Mathf.Abs(direction.x));
        if (m_Grounded)
        {
            if (MustJump())
            {
                m_Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
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
            //Determine if its angle is not scalable
            Debug.Log(Vector3.Angle(adjacentHit.normal, transform.up));
            float slopeAngle = Vector3.Angle(adjacentHit.normal, transform.up);
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

    private void CheckGrounded()
    {
        RaycastHit ground = new RaycastHit();
        if (m_Animator == null) return;
        m_Grounded = m_Animator.applyRootMotion = Physics.Raycast(transform.position + Vector3.up * 0.02f, Vector3.down * m_GroundCheckDistance, out ground, m_GroundCheckDistance, m_GroundLayers);
        m_EnemyCollider.material = m_Grounded ? m_GroundedMaterial : m_AirborneMaterial;
        m_Animator.SetBool("Grounded", m_Grounded);
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
            m_Animator.SetFloat("Speed", 0);
            isAnimating = true;
        }
        else throw new MissingComponentException("No AnimationInitializer set for \"" + m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name + "\".");
    }

    private bool Countdown(float duration) => (m_StateTimer += Time.deltaTime) >= duration;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.9f, 0.11f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, detectionRange);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, stoppingDistance);
    }
}
