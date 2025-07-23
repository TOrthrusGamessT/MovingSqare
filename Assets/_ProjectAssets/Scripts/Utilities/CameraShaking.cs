using UnityEngine;

public class CameraShaking : MonoBehaviour
{
    private static GameObject attachedObject;
    private static Vector3 originalPosition;

    void Awake()
    {
        attachedObject = gameObject;
        originalPosition = transform.position;
    }


    public static void Shake()
    {
        // Cancel any existing shake animations
        LeanTween.cancel(attachedObject);
        
        // Store the current position as the base position
        Vector3 basePosition = attachedObject.transform.position;
        
        // Perform shake sequence with relative movements
        LeanTween.moveX(attachedObject, basePosition.x + 0.5f, 0.05f).setEasePunch().setOnComplete(() =>
        {
            LeanTween.moveY(attachedObject, basePosition.y + 0.5f, 0.05f).setEasePunch().setOnComplete(() =>
            {
                LeanTween.moveX(attachedObject, basePosition.x - 0.5f, 0.05f).setEasePunch().setOnComplete(() =>
                {
                    LeanTween.moveY(attachedObject, basePosition.y - 0.5f, 0.05f).setEasePunch().setOnComplete(() =>
                    {
                        // Return to original position
                        LeanTween.move(attachedObject, basePosition, 0.1f).setEase(LeanTweenType.easeOutQuad);
                    });
                });
            });
        });
    }
}
