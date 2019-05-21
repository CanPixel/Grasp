using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;
using BehaviorTree;

[RequireComponent(typeof(AStarUnit))]
public class Enemy : MonoBehaviour {
    public Transform target;
    public float moveSpeed = 1;

    [HideInInspector]
    public BNode lastNode;

    protected BNode behaviorTree;

    protected Rigidbody rb;
    
    void Awake() {
         StartAI();
    }

    protected void StartAI() {
         rb = GetComponent<Rigidbody>();
         behaviorTree = new BehaviorTree.Composite.BSequence(new BNode[]{
                new BAction(this, CheckHealth), 
                new BAction(this, CheckForSynergies),
                new BAction(this, MoveInRangeOfPlayer), 
                new BehaviorTree.Decorator.BTimer(new BehaviorTree.Composite.BSequence(new BNode[] {
                    new BAction(this, RangedCharge), new BAction(this, RangedAttack)
                }), 1)
                });
                //pathfinding.SetTarget(player.transform);
    }

    protected BNode.NodeState CheckHealth() {
        return BNode.NodeState.SUCCESS;
    }

    protected BNode.NodeState CheckForSynergies() {
        return BNode.NodeState.SUCCESS;
    }

    protected BNode.NodeState MoveInRangeOfPlayer() {
        return BNode.NodeState.SUCCESS;
    }

    protected BNode.NodeState RangedCharge() {
        return BNode.NodeState.SUCCESS;
    }

    protected BNode.NodeState RangedAttack() {
        return BNode.NodeState.SUCCESS;
    }

    void FixedUpdate() {

    }

    protected void Move() {

    }
}
