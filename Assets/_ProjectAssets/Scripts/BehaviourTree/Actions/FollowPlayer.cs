using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : StateMachineBehaviour
{

    public float speed;
    public Transform player;
    public Rigidbody2D rb;
    public float maxXValue;
    public float minXValue;
    
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        rb = animator.GetComponent<Rigidbody2D>();
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPosition =  Vector2.MoveTowards(rb.position,target, speed * Time.deltaTime);

        newPosition.x = Mathf.Clamp(newPosition.x, minXValue, maxXValue);
        rb.MovePosition(newPosition);
        
    }

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
    
}
