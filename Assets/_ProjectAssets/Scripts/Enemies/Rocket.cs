using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{

    public float speed;
    public float speedRotation;
    
    private Transform _player;
    private Rigidbody2D _rb;
    
    private void Start()
    {
        _player = GameManager.instance.Player;
        _rb = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        Vector2 direction = ((Vector2)_player.position - _rb.position).normalized;

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        _rb.angularVelocity = -rotateAmount * speedRotation; 

        _rb.linearVelocity = transform.up * speed;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(gameObject);
    }
}
