using UnityEngine;
using static Constants;

public abstract class EnemyBehaviour : MonoBehaviour
{
    public float speed;
    public GameObject deadEffect;
    public GeometryFigure figure;
    private Rigidbody2D _rb;

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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        _rb.velocity = transform.right * speed;
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
