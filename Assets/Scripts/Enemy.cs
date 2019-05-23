using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class Enemy : MonoBehaviour {
    public BNode lastNode;

    [HideInInspector]
    public GameObject target;
    public float moveSpeed = 1;
    private float dir = 0, targetDir = 1;

    public float targetRange = 10;
    public float nearDistance = 3;
    private float baseNear;

    protected BNode behaviorTree;
    protected Rigidbody rb;
    public GameObject player, playerLightPoint;

    public Behavior behavior;    

    [System.Serializable]
    public enum Behavior {
        MOTH, VAMPIRE
    }
    
    void Awake() {
         StartAI();
    }

    protected void StartAI() {
        baseNear = nearDistance;
        rb = GetComponent<Rigidbody>();
        switch(behavior) {
            default:
            case Behavior.MOTH:
                behaviorTree = new BehaviorTree.Composite.BSequence(new BNode[]{
                        //new BAction(this, OnScreen), 
                        new BAction(this, CheckTarget), 
                        new BAction(this, Walk),
                    //    new BehaviorTree.Decorator.BTimer(new BehaviorTree.Composite.BSequence(new BNode[] {
                    //       new BAction(this, RangedCharge), new BAction(this, RangedAttack)
                    //  }), 1)
                        });
                break;
            case Behavior.VAMPIRE:
                behaviorTree = new BehaviorTree.Composite.BSequence(new BNode[]{
                        //new BAction(this, OnScreen), 
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
                if(Flashlight.IsLightOn()) {
                    float lightX = Mathf.Abs(playerLightPoint.transform.position.x);
                    float posX = Mathf.Abs(transform.position.x);
                    float playerX = Mathf.Abs(player.transform.position.x);
                    targetDir = -1;
                } else{
                    targetDir = 1;
                }
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

    void FixedUpdate() {
        behaviorTree.Run();
    }

    protected void MoveToTarget(GameObject target) {
        Vector3 dest = transform.position - target.transform.position;
        if(dest.x > 0) dir = -1 * targetDir;
        if(dest.x < 0) dir = 1 * targetDir;
        if(Vector3.Distance(transform.position, target.transform.position) > nearDistance) move();
    }

    private void move() {
        rb.MovePosition(rb.position + new Vector3(moveSpeed * dir, 0, 0) / 10f);
    }
}
