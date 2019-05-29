using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Enemy : MonoBehaviour {
    public BNode lastNode;

    [HideInInspector]
    public GameObject target;

    [Range(0.1f, 8)]
    public float moveSpeed = 1;
    public float jumpForce = 4;
    public float maxJumpHeight = 4;
    [Range(0, 90)]
    public float maxSlopeAngle = 45;
    private float dir = 0, targetDir = 1;

    public float targetRange = 10;
    public float nearDistance = 3;
    private float baseNear;

    protected BNode behaviorTree;
    protected Rigidbody rb;
    public GameObject player, playerLightPoint;

    private float scareDelay = 0;

    public Behavior behavior;

    private Animator animator;
    private bool grounded;
    [SerializeField] float groundCheckDistance = 0.02f;
    [SerializeField] LayerMask groundLayers = 0;
    [SerializeField] PhysicMaterial groundedMaterial = null, airborneMaterial = null;
    private bool hurtBeforeStateChange;
    private CapsuleCollider enemyCollider;

    [System.Serializable]
    public enum Behavior {
        MOTH, VAMPIRE
    }
    
    void Awake() {
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<CapsuleCollider>();
        StartAI();
    }

    protected void StartAI() {
        baseNear = nearDistance;
        rb = GetComponent<Rigidbody>();
        switch(behavior) {
            default:
            case Behavior.MOTH:
                behaviorTree = new BehaviorTree.Composite.BSequence(new BNode[]{
                        new BAction(this, CheckTarget), 
                        new BAction(this, Walk),
                    //    new BehaviorTree.Decorator.BTimer(new BehaviorTree.Composite.BSequence(new BNode[] {
                    //       new BAction(this, RangedCharge), new BAction(this, RangedAttack)
                    //  }), 1)
                        });
                break;
            case Behavior.VAMPIRE:
                behaviorTree = new BehaviorTree.Composite.BSequence(new BNode[]{
                        new BAction(this, CheckTarget), 
                        new BAction(this, Walk),
                    //    new BehaviorTree.Decorator.BTimer(new BehaviorTree.Composite.BSequence(new BNode[] {
                    //       new BAction(this, RangedCharge), new BAction(this, RangedAttack)
                    //  }), 1)
                        });
                break;
        }
    }

    protected BNode.NodeState Walk() {
        if(target != null) MoveToTarget(target);
        return BNode.NodeState.SUCCESS;
    }

    protected BNode.NodeState CheckTarget() {
        float lightDist = Vector3.Distance(transform.position, playerLightPoint.transform.position);
        float playerDist = Vector3.Distance(transform.position, player.transform.position);
        switch(behavior) {
            default:
            case Behavior.MOTH:
                if(lightDist < targetRange) target = playerLightPoint;
                else {
                    if(playerDist < targetRange) target = player;
                    else target = null;
                }
                break;
            case Behavior.VAMPIRE:
                if(playerDist < targetRange) target = player;
                else target = null;
                break;
        }
        return BNode.NodeState.SUCCESS;
    }

    protected BNode.NodeState OnScreen() {
        return BNode.NodeState.SUCCESS;
    }

    protected BNode.NodeState RangedCharge() {
        return BNode.NodeState.SUCCESS;
    }

    protected BNode.NodeState RangedAttack() {
        return BNode.NodeState.SUCCESS;
    }

    public void Scare(Vector3 origin) {
        if (!hurtBeforeStateChange) {
            if(animator != null) animator.SetTrigger("Hurt");
            hurtBeforeStateChange = true;
        }
        if(scareDelay > 0) return;
        scareDelay = 2;
        float posX = transform.position.x;  
    }

    void FixedUpdate() {
        behaviorTree.Run();
        if(scareDelay > 0) {
            scareDelay -= Time.deltaTime;
            targetDir = Mathf.Lerp(targetDir, -1, Time.deltaTime * 2);
        }
        else targetDir = 1;

        CheckGroundedState();
    }

    void Update()
    {
        Animate();
    }

    protected void MoveToTarget(GameObject target) {
        Vector3 dest = transform.position - target.transform.position;
        if(dest.x > 0) dir = -1 * targetDir;
        if(dest.x < 0) dir = 1 * targetDir;
        if(Vector3.Distance(transform.position, target.transform.position) > nearDistance) Move();
    }

    private void Move() {
        if (grounded)
        {
            rb.MovePosition(rb.position + new Vector3((moveSpeed / 10) * dir, 0, 0) / 10f);
            //Check objects at foot level
            if (Physics.Raycast(transform.position + Vector3.up * 0.05f, dir > 0 ? Vector3.right : Vector3.left, 1, groundLayers))
            {
                if (MustJump())
                {
                    //Jump
                    rb.AddForce((Vector3.up + transform.forward.normalized / 4)* jumpForce, ForceMode.VelocityChange);
                }
            }
        }
    }

    private bool MustJump()
    {
        //If an object is found at foot level, check if its highest surface is reachable with a jump.
        //Do a raycast adjacent to the enemy
        RaycastHit adjacentHit = new RaycastHit();

        Debug.DrawRay(transform.position + new Vector3(dir > 0 ? 1 : -1, 100, 0), Vector3.down * 100);
        if (Physics.Raycast(transform.position + new Vector3(transform.eulerAngles.y == 90 ? 1 : -1, 100, 0), Vector3.down * 100, out adjacentHit, 100, groundLayers))
        {
            //Determine if its angle is not scalable
            Debug.Log(Vector3.Angle(adjacentHit.normal, transform.up));
            if (Vector3.Angle(adjacentHit.normal, transform.up) < maxSlopeAngle)
            {
                return false;
            }
            //Return if a jump is not too high.
            float height = adjacentHit.point.y - transform.position.y;
            if (height < 4 && height > 0.01f)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckGroundedState()
    {
        RaycastHit ground = new RaycastHit();
        if(animator == null) return;
        grounded = animator.applyRootMotion = Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out ground, groundCheckDistance, groundLayers);
        enemyCollider.material = grounded ? groundedMaterial : airborneMaterial;
    }

    private void Animate()
    {
        if (Mathf.Abs(dir) > 0.05f) transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (dir > 0 ? 90 : 270), Time.deltaTime * 20 * Mathf.Abs(dir));
        if(animator == null) return;
        animator.SetBool("Grounded", grounded);
        animator.SetFloat("Speed", Mathf.Abs(dir));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.9f, 0.11f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, nearDistance);
        Gizmos.DrawWireSphere(transform.position + Vector3.up, targetRange);
        Gizmos.color = new Color(1f, 0.21f, 0.2f, 0.8f);
        Gizmos.DrawRay(transform.position, Vector3.up * maxJumpHeight);
    }
}
