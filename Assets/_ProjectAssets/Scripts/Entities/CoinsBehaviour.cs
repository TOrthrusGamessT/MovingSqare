using System;
using UnityEngine;

public class CoinsBehaviour : MonoBehaviour
{

    public static Action<bool> onCoinDestroy;
    public static Action<int> onCoinCollected;
    public static int amount;
    private static int life;

    [SerializeField] private GameObject destroyVFX;
    [SerializeField] private GameObject vfx;
    [SerializeField] private ColectMoneyEffect textEffect;

    public static int Lifetime
    {
        get => life;
        set => life = value;

    }

    private static int _combo = 0;

    // Movement properties
    private Vector2 velocity;
    private bool shouldMove;
    private Rigidbody2D rb;

    private void Start()
    {
        Invoke(nameof(Destroy), life);
        
        // Initialize movement components
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0; // Disable gravity for 2D movement
        rb.linearDamping = 0f; // No linear damping for smooth movement
        rb.angularDamping = 0f; // No angular damping
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Better collision detection
        
        // Ensure we have a collider for ricochet collisions
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
        }
        col.isTrigger = false; // Make sure it's not a trigger so we get collision events
        
        // Create and apply physics material for bouncy behavior
        PhysicsMaterial2D bouncyMaterial = new PhysicsMaterial2D("CoinBouncy");
        bouncyMaterial.friction = 0.1f;    // Low friction
        bouncyMaterial.bounciness = 0.8f;  // High bounciness
        col.sharedMaterial = bouncyMaterial;
        
        // Check conditions for coin movement
        shouldMove = ShouldCoinMove();
        
        if (shouldMove)
        {
            InitializeMovement();
        }
    }
    
    private bool ShouldCoinMove()
    {
        // Check if level > 10 (currentLvlIndex is 0-based, so > 9 means level > 10)
        if (LVLIndexer.currentLvlIndex > 9)
        {
            return true;
        }
        
        // Check if in survival mode and timer > 60 seconds from the survival start
        if (Timer.IsSurviveMode && Timer.ElapsedSeconds > 10)
        {
            return true;
        }
        
        return false;
    }
    
    private void InitializeMovement()
    {
        // Set random initial velocity
        float speed = UnityEngine.Random.Range(1f, 3f);
        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
        rb.linearVelocity = velocity;
    }

    private void Update()
    {
        if (shouldMove)
        {
            CheckBoundariesAndRicochet();
        }
    }
    
    private void CheckBoundariesAndRicochet()
    {
        Vector3 pos = transform.position;
        Vector2 currentVelocity = rb.linearVelocity;
        bool bounced = false;
        
        // Get screen boundaries (using camera bounds)
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;
        float leftBound = -screenWidth / 2f;
        float rightBound = screenWidth / 2f;
        float topBound = screenHeight / 2f;
        float bottomBound = -screenHeight / 2f;
        
        // Check horizontal boundaries
        if (pos.x <= leftBound || pos.x >= rightBound)
        {
            currentVelocity.x = -currentVelocity.x;
            bounced = true;
            
            // Clamp position to stay within bounds
            pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        }
        
        // Check vertical boundaries
        if (pos.y <= bottomBound || pos.y >= topBound)
        {
            currentVelocity.y = -currentVelocity.y;
            bounced = true;
            
            // Clamp position to stay within bounds
            pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        }
        
        if (bounced)
        {
            // Apply the new velocity and position
            rb.linearVelocity = currentVelocity;
            transform.position = pos;
            
            // Optional: Play bounce sound or effect here
            // SoundManager.instance.PlaySoundEffect("CoinBounce");
        }
    }

    private void Destroy()
    {
        Instantiate(destroyVFX, transform.position, Quaternion.identity);
        onCoinDestroy?.Invoke(false);
        ColectMoneyEffect colectMoneyEffect = Instantiate(textEffect, transform.position + Vector3.forward * -9.125f, Quaternion.identity);
        colectMoneyEffect.ShowText(0);
        _combo = 0;
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            _combo++;
            ColectMoneyEffect colectMoneyEffect = Instantiate(textEffect, transform.position + Vector3.forward * -9.125f, Quaternion.identity);
            colectMoneyEffect.ShowText(_combo);
            Instantiate(vfx, transform.position, Quaternion.identity);
            BackgroundParticlesManager.instance.TakeCoinAnimation();
            SoundManager.instance.PlaySoundEffect(Constants.Sounds.PickCoin);
            onCoinDestroy?.Invoke(true);
            onCoinCollected?.Invoke(amount);
            Destroy(gameObject);
        }
        else if (shouldMove && col.gameObject.layer == LayerMask.NameToLayer("MovingZone"))
        {
            // Handle ricochet when hitting MovingZone layer objects
            HandleRicochetCollision(col);
        }
    }
    
    private void HandleRicochetCollision(Collision2D col)
    {
        if (rb == null) return;
        
        // Get the collision normal (direction of the surface we hit)
        Vector2 normal = Vector2.zero;
        if (col.contacts.Length > 0)
        {
            normal = col.contacts[0].normal;
        }
        else
        {
            // Fallback: calculate normal from positions
            Vector2 direction = (transform.position - col.transform.position).normalized;
            normal = direction;
        }
        
        // Get current velocity
        Vector2 currentVelocity = rb.linearVelocity;
        
        // Calculate reflected velocity using Vector2.Reflect
        Vector2 reflectedVelocity = Vector2.Reflect(currentVelocity, normal);
        
        // Add some bounce energy to make the effect more noticeable
        float bounceMultiplier = 1.1f; // Slightly increase energy on bounce
        reflectedVelocity *= bounceMultiplier;
        
        // Ensure minimum speed to prevent coins from getting stuck
        float minSpeed = 1f;
        if (reflectedVelocity.magnitude < minSpeed)
        {
            reflectedVelocity = reflectedVelocity.normalized * minSpeed;
        }
        
        // Apply the reflected velocity
        rb.linearVelocity = reflectedVelocity;
        
        // Optional: Play bounce sound or effect here
        // SoundManager.instance.PlaySoundEffect("CoinBounce");
        
        Debug.Log($"Coin bounced! Normal: {normal}, Original velocity: {currentVelocity}, New velocity: {reflectedVelocity}");
    }
}
