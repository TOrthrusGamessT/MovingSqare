using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed;
    
    private float maxX = 2.17f;
    private float minX = -2.17f;
    private float maxY = 4.82f;
    private float minY = -4.82f;
    
    private bool isMoving;
    private Vector2 currentInputValue;
    private Vector2 smoothInputValue;
    private Touch _touch;
    private bool alive = true;

    // Public property for other components to access movement state
    public bool IsMoving => isMoving;

    private void OnEnable()
    {
        PlayerLife.onPlayerDie += SetAliveFalse;
        AdsManager.onReviveADFinish += SetAliveTrue;
    }

    private void OnDisable()
    {
        PlayerLife.onPlayerDie -= SetAliveFalse;
        AdsManager.onReviveADFinish -= SetAliveTrue;
    }

    public void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        if (Input.touchCount > 0 && alive)
        {
            _touch = Input.GetTouch(0);

            if (_touch.phase == TouchPhase.Moved)
            {
                Vector2 nextPosition = new Vector2(
                    transform.position.x + _touch.deltaPosition.x * speed,
                    transform.position.y + _touch.deltaPosition.y * speed);
                
                nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
                nextPosition.y = Mathf.Clamp(nextPosition.y, minY, maxY);
                
                transform.position = nextPosition;
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
        }
    }

    private void SetAliveTrue()
    {
        alive = true;
    }

    private void SetAliveFalse()
    {
        alive = false;
    }
}