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
    private bool hurtBeforeStateChange;

    [System.Serializable]
    public enum Behavior {
        MOTH, VAMPIRE
    }
    
    void Awake() {
        animator = GetComponent<Animator>();
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
            animator.SetTrigger("Hurt");
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
        if(Vector3.Distance(transform.position, target.transform.position) > nearDistance) move();
    }

    private void move() {
        if (grounded)
            rb.MovePosition(rb.position + new Vector3((moveSpeed / 10) * dir, 0, 0) / 10f);
    }

    private void CheckGroundedState()
    {
        RaycastHit ground = new RaycastHit();

        grounded = Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out ground, groundCheckDistance, groundLayers);
    }

    private void Animate()
    {
        if (Mathf.Abs(dir) > 0.05f) transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, Vector3.up * (dir > 0 ? 90 : 270), Time.deltaTime * 20 * Mathf.Abs(dir));

        animator.SetBool("Grounded", grounded);
        animator.SetFloat("Speed", Mathf.Abs(targetDir));
    }
}
