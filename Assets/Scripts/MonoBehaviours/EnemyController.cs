using UnityEngine;

[DisallowMultipleComponent]
public sealed class EnemyController : MonoBehaviour
{
    public enum State { Idle, Detecting, Chasing, Attacking, Hiding }
    public State currentState = State.Idle;
    [Space]
    public float detectionRange = 25f;
    public float stoppingDistance = 2f;
    public float grabRange = 0.45f;
    public Transform rightHand, leftHand;

    public Transform target { get; set; }
    public float remainingDistance { get; set; }
    public Vector3 direction { get; set; }

    [SerializeField] float m_GroundCheckDistance = 0.2f;
    [SerializeField] PhysicMaterial m_GroundedMaterial = null, m_AirborneMaterial = null;
    [SerializeField] LayerMask m_GroundLayers = 0;

    private float m_StateTimer = 0;
    private Animator m_Animator = null;
    private bool m_Grounded = true;
    private CapsuleCollider m_EnemyCollider;

    private void Awake()
    {
        target = FindObjectOfType<PlayerController>().transform;
        direction = Vector3.zero;
        m_Animator = GetComponent<Animator>();
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
                break;
            case State.Hiding:
                break;
            default:
                TransitionToState(State.Idle);
                break;
        }
    }

    #region States
    private void AnyState()
    {
        CheckGrounded();
    }

    private void Idle()
    {
        //Wait for player to get in range and player is in front of the enemy, then act.
        if (Vector3.Distance(target.position, transform.position) <= detectionRange)
        {
            TransitionToState(State.Detecting);
        }
    }

    private void Detect()
    {
        if (Countdown(m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length))
        {
            TransitionToState(State.Chasing);
        }
    }

    private void Chase()
    {
        //Move and rotate
        direction = target.position - transform.position;
        m_Animator.SetFloat("Speed", Mathf.Min(1, Mathf.Abs(direction.x)));
        if (Mathf.Abs(direction.x) > 0.05f)
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (direction.x > 0 ? 90 : 270), Time.deltaTime * 20 * Mathf.Abs(direction.x));
    }

    private void Attack()
    {

    }

    private void Hide()
    {
        //Move and rotate, but this time facing away from the target
        direction = target.position - transform.position;
        m_Animator.SetFloat("Speed", Mathf.Min(1, Mathf.Abs(direction.x)));
        if (Mathf.Abs(direction.x) > 0.05f)
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (direction.x > 0 ? 270 : 90), Time.deltaTime * 20 * Mathf.Abs(direction.x));
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

    private void CheckGrounded()
    {
        Debug.Log("Check Ground");
        RaycastHit ground = new RaycastHit();
        if (m_Animator == null) return;
        Debug.DrawRay(transform.position + Vector3.up * 0.02f, Vector3.down * m_GroundCheckDistance, Color.red);
        m_Grounded = m_Animator.applyRootMotion = Physics.Raycast(transform.position + Vector3.up * 0.02f, Vector3.down, out ground, m_GroundCheckDistance, m_GroundLayers);
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
        if (anim) anim.Activate(transform, target.transform);
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
