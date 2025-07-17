using System;
using UnityEngine;


public class FullScreenLine : MonoBehaviour
{
    [SerializeField]
    [ColorUsage(true, true)]
    private Color neutralColor;


    private Collider2D laserCollider;
    private int initIdLeanTween;

    // Start is called before the first frame update
    void Awake()
    {
        laserCollider = GetComponent<Collider2D>();
        Init();
    }

    void Init()
    {
        Color c = GetComponent<SpriteRenderer>().color;

       initIdLeanTween = LeanTween.value(0, 1, 1f).setOnUpdate(value =>
        {
            try
            {
                GetComponent<SpriteRenderer>().color = Color.Lerp(c, neutralColor, value);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                LeanTween.cancel(initIdLeanTween);
            }
        }).id;
    }

    [ContextMenu("Test Line")]
    public void Activate()
    {
        Color c = GetComponent<SpriteRenderer>().color;
        Color randomActiveColor = Constants.brightColorPalette[UnityEngine.Random.Range(0, Constants.brightColorPalette.Count)];

        LeanTween.value(0, 1, 0.3f).setOnUpdate(value =>
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(c, randomActiveColor, value);
        }).setOnComplete(() => laserCollider.enabled = true);
    }

    public void DestroyLaser()
    {
        LeanTween.cancel(gameObject);

        Color c = GetComponent<SpriteRenderer>().color;
        laserCollider.enabled = false;

        LeanTween.value(0, 1, 0.3f).setOnUpdate(value =>
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(c, new Vector4(0, 0, 0, 0), value);
        }).setOnComplete(() => Destroy(gameObject));
    }

    private void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }
}