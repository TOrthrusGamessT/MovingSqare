using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private static GameObject attachedObject;

    void Awake()
    {
        attachedObject = gameObject;
    }

    private void OnEnable()
    {
        Boomerang.onBoomerangHit += Shake;
    }

    private void OnDisable()
    {
        Boomerang.onBoomerangHit -= Shake;
    }

    public static void Shake()
    {
        LeanTween.moveX(attachedObject, 0.3f, 0.03f).setEasePunch().setOnComplete(() =>
        {
            LeanTween.moveY(attachedObject, 0.3f, 0.03f).setEasePunch().setOnComplete(() =>
            {
                LeanTween.moveX(attachedObject, -0.3f, 0.03f).setEasePunch().setOnComplete(() =>
                {
                    LeanTween.moveY(attachedObject, -0.3f, 0.03f).setEasePunch();
                });
            });
        });
    }
}
