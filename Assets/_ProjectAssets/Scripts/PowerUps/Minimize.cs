using System.Collections;
using UnityEngine;


public class Minimize :  PowerUpBehaviour
{
    private Vector3 originalScale;

    [ContextMenu("Minimize test")]
    public override void Effect()
    {
        Transform player=  GameManager.instance.Player;
        originalScale = player.localScale; // Store the original scale


        float minimizedScale = player.localScale.x - CalculatePercentage(item.GetEffect(item.effects[0].name), player.localScale.x);

        LeanTween.value(player.localScale.x, minimizedScale, 0.2f)
            .setOnUpdate(value =>
            {
                player.localScale = Vector3.one * value;
            }).setEaseInCubic();
        
        StartCoroutine(DestroyMinimiseEffect());

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        
    }
    
    
    private IEnumerator DestroyMinimiseEffect()
    {
        yield return new WaitForSeconds(effectTime);
        Transform player=  GameManager.instance.Player;

        // Restore to the original scale
        LeanTween.value(player.localScale.x, originalScale.x, 0.2f)
            .setOnUpdate(value =>
            {
                player.localScale = Vector3.one * value;
            }).setEaseInCubic();
        
        Destroy(gameObject);
        
    }
    
    private float CalculatePercentage(float percentage,float max)
    {
        float soum = max * percentage;
        return soum / 100;
    }

    
    
    
}
