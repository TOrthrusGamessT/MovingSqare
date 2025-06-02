
using UnityEngine;

public class BackGroundImageController : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetBkSize();
    }

    private void SetBkSize()
    {

        this.transform.localScale = new Vector3(1, 1, 1);

        var width = spriteRenderer.sprite.bounds.size.x;
        var height = spriteRenderer.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        this.transform.localScale = new Vector2((float)worldScreenWidth / width,
            (float)worldScreenHeight / height);

    }
}
