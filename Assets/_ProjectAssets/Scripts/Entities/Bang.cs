using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bang : MonoBehaviour
{
    private Rigidbody2D _rb;
    // Start is called before the first frame update
    void Start()
    {
        transform.right = GameManager.instance.PlayerPosition - (Vector2)transform.position;
    }
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        _rb.linearVelocity = transform.right * 2;
    }
}
