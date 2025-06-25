using UnityEngine;
using static Constants;

public abstract class EnemyBehaviour : MonoBehaviour
{
    public float speed;
    public GameObject deadEffect;
    public GeometryFigure figure;
    private Rigidbody2D _rb;

    [SerializeField]
    private ParticleSystem _trail;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    public virtual void Start()
    {
        transform.right = GameManager.instance.PlayerPosition - (Vector2)transform.position;
    }

    public void SetTransformRight()
    {
        transform.rotation = Quaternion.identity;
        transform.right = (Vector2)transform.position + new Vector2(-90, 0);
    }


    public abstract void UpdateSpeedBasedOnFigure(float speed);

    public void SetColor(Color color)
    {
        // Set the sprite renderer color
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = color;
        }

        // Set the particle system trail color
        if (_trail != null)
        {
            var mainModule = _trail.main;
            mainModule.startColor = color;
        }
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("DeadZone"))
        {
            SoundManager.instance.PlaySoundEffect(Constants.Sounds.DestroyEnemy);
            Destroy(gameObject);
            Instantiate(deadEffect, new Vector3(col.contacts[0].point.x,
                col.contacts[0].point.y, -8), Quaternion.identity);
        }
    }
}
