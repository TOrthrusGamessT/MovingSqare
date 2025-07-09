using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameAnimation : MonoBehaviour
{
    public GameObject player;
    [SerializeField]
    private TextMeshProUGUI timerText;

    [Header("Win Animation Settings")]
    public int gridSize = 5; // 5x5 grid around player
    public float squareSize = 0.3f;
    public float spacing = 0.4f;
    public float animationDuration = 0.6f;
    public float delayBetweenSquares = 0.05f;
    public float scaleMultiplier = 1.5f;
    public float rotationAmount = 180f;
     private float originalTimerFontSize;


    private List<GameObject> animationSquares = new List<GameObject>();

    private void Start()
    {
        // Store original timer font size
        if (timerText != null)
        {
            originalTimerFontSize = timerText.fontSize;
        }
    }

    [ContextMenu("Start Win Animation")]
    public void StartWinAnimation()
    {
        StartCoroutine(PlayWinAnimationCoroutine());
    }

   
    private IEnumerator PlayWinAnimationCoroutine()
    {
        // Clear any existing animation squares
        ClearAnimationSquares();

        // Start timer text attention animation
        StartTimerAttentionAnimation();

        Vector3 playerPos = player.transform.position;
        
        // Create grid of white squares around the player
        CreateSquareGrid(playerPos);

        // Animate all squares scaling up and rotating
        yield return StartCoroutine(AnimateSquaresIn());

        // Wait a moment at full scale
        yield return new WaitForSeconds(0.3f);

        // Animate squares scaling down and rotating back
        yield return StartCoroutine(AnimateSquaresOut());

        // Clean up
        ClearAnimationSquares();

        // Bounce out hard and reset timerText font size and rotation
        if (timerText != null)
        {
            LeanTween.cancel(timerText.gameObject); // Cancel ongoing animations
            LeanTween.value(timerText.gameObject, (float val) => { timerText.fontSize = val; }, timerText.fontSize, originalTimerFontSize, 0.25f)
                .setEase(LeanTweenType.easeInBounce);
            LeanTween.rotateZ(timerText.gameObject, 0f, 0.2f).setEase(LeanTweenType.easeInOutSine);
            
            // Reset position to original
            Vector3 currentPos = timerText.transform.position;
            LeanTween.moveY(timerText.gameObject, currentPos.y + 50f, 0.3f)
                .setEase(LeanTweenType.easeOutBack);
        }
    }

    private void CreateSquareGrid(Vector3 centerPosition)
    {
        int halfSize = gridSize / 2;
        
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int y = -halfSize; y <= halfSize; y++)
            {
                // Skip the center position (where the player is)
                if (x == 0 && y == 0) continue;

                Vector3 squarePosition = centerPosition + new Vector3(x * spacing, y * spacing, 0);
                GameObject square = CreateWhiteSquare(squarePosition);
                animationSquares.Add(square);
            }
        }
    }

    private GameObject CreateWhiteSquare(Vector3 position)
    {
        // Create a new GameObject for the square
        GameObject square = new GameObject("WinAnimationSquare");
        square.transform.position = position;
        
        // Add SpriteRenderer component
        SpriteRenderer spriteRenderer = square.AddComponent<SpriteRenderer>();
        
        // Create a white square sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        
        Sprite squareSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        spriteRenderer.sprite = squareSprite;
        spriteRenderer.color = Color.white;
        
        // Set initial scale
        square.transform.localScale = Vector3.zero;
        
        // Set sorting order to be in front of other objects
        spriteRenderer.sortingOrder = 10;
        
        return square;
    }

    private IEnumerator AnimateSquaresIn()
    {
        // Group squares by their distance from center (in rings/layers)
        Vector3 playerPos = player.transform.position;
        Dictionary<float, List<GameObject>> ringGroups = new Dictionary<float, List<GameObject>>();
        
        // Group squares by distance (rounded to avoid floating point precision issues)
        foreach (GameObject square in animationSquares)
        {
            float distance = Vector3.Distance(square.transform.position, playerPos);
            float roundedDistance = Mathf.Round(distance * 10f) / 10f; // Round to 1 decimal place
            
            if (!ringGroups.ContainsKey(roundedDistance))
            {
                ringGroups[roundedDistance] = new List<GameObject>();
            }
            ringGroups[roundedDistance].Add(square);
        }
        
        // Sort rings by distance (farthest first for outside-in effect)
        var sortedRings = ringGroups.OrderByDescending(kvp => kvp.Key).ToList();
        
        // Animate each ring with a delay between rings
        for (int ringIndex = 0; ringIndex < sortedRings.Count; ringIndex++)
        {
            var ring = sortedRings[ringIndex];
            
            // Animate all squares in this ring simultaneously (no delay between squares in the same ring)
            foreach (GameObject square in ring.Value)
            {
                // Scale animation
                LeanTween.scale(square, Vector3.one * squareSize * scaleMultiplier, animationDuration)
                    .setEase(LeanTweenType.easeOutBack);
                
                // Rotation animation
                LeanTween.rotateZ(square, rotationAmount, animationDuration)
                    .setEase(LeanTweenType.easeOutBack);
            }
            
            // Wait for the ring delay before starting next ring
            yield return new WaitForSeconds(delayBetweenSquares * 3f);
        }
        
        // Wait for the last ring's animation to complete fully
        // This ensures all squares have finished their entrance animation
        yield return new WaitForSeconds(animationDuration);
    }

    private IEnumerator AnimateSquaresOut()
    {
        // Group squares by their distance from center (in rings/layers)
        Vector3 playerPos = player.transform.position;
        Dictionary<float, List<GameObject>> ringGroups = new Dictionary<float, List<GameObject>>();
        
        // Group squares by distance (rounded to avoid floating point precision issues)
        foreach (GameObject square in animationSquares)
        {
            float distance = Vector3.Distance(square.transform.position, playerPos);
            float roundedDistance = Mathf.Round(distance * 10f) / 10f; // Round to 1 decimal place
            
            if (!ringGroups.ContainsKey(roundedDistance))
            {
                ringGroups[roundedDistance] = new List<GameObject>();
            }
            ringGroups[roundedDistance].Add(square);
        }
        
        // Sort rings by distance (closest first for inside-out effect)
        var sortedRings = ringGroups.OrderBy(kvp => kvp.Key).ToList();
        
        // Animate each ring with a delay between rings
        for (int ringIndex = 0; ringIndex < sortedRings.Count; ringIndex++)
        {
            var ring = sortedRings[ringIndex];
            
            // Animate all squares in this ring simultaneously (no delay between squares in the same ring)
            foreach (GameObject square in ring.Value)
            {
                // Scale down animation
                LeanTween.scale(square, Vector3.zero, animationDuration * 0.8f)
                    .setEase(LeanTweenType.easeInBack);
                
                // Rotation animation (rotate further)
                LeanTween.rotateZ(square, rotationAmount * 2, animationDuration * 0.8f)
                    .setEase(LeanTweenType.easeInBack);
            }
            
            // Wait for the ring delay before starting next ring
            yield return new WaitForSeconds(delayBetweenSquares * 2f);
        }
        
        // Wait for all animations to complete
        yield return new WaitForSeconds(animationDuration * 0.8f);
    }

    private void ClearAnimationSquares()
    {
        foreach (GameObject square in animationSquares)
        {
            if (square != null)
            {
                // Cancel any ongoing tweens on this object
                LeanTween.cancel(square);
                
                // Destroy the sprite texture to prevent memory leaks
                SpriteRenderer sr = square.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null && sr.sprite.texture != null)
                {
                    DestroyImmediate(sr.sprite.texture);
                    DestroyImmediate(sr.sprite);
                }
                
                DestroyImmediate(square);
            }
        }
        animationSquares.Clear();
    }

    private void OnDestroy()
    {
        // Clean up when the component is destroyed
        ClearAnimationSquares();
    }

    private void StartTimerAttentionAnimation()
    {
        if (timerText == null) return;

        // Cancel any existing animations on the timer text
        LeanTween.cancel(timerText.gameObject);

        // Scale up to 150 font size with bounce effect
        LeanTween.value(timerText.gameObject, (float val) => { timerText.fontSize = val; }, 
            timerText.fontSize, 150f, 0.8f)
            .setEase(LeanTweenType.easeOutBounce);

        // Move timer text down a bit during the animation
        Vector3 originalPosition = timerText.transform.position;
        LeanTween.moveY(timerText.gameObject, originalPosition.y - 1f, 0.5f)
            .setEase(LeanTweenType.easeOutBack);

        // Shake with rotation - continuous shaking
        StartCoroutine(ShakeTimerText());

        // Continuous bouncing scale effect
        StartCoroutine(BounceTimerText());
    }

    private IEnumerator ShakeTimerText()
    {
        if (timerText == null) yield break;

        float shakeIntensity = 25f; // Big degrees for shaking
        float shakeSpeed = 0.1f;

        // Shake for the duration of the grid creation and animation
        float totalShakeTime = (delayBetweenSquares * 3f * gridSize) + animationDuration + 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < totalShakeTime)
        {
            if (timerText == null) yield break;

            float randomRotation = Random.Range(-shakeIntensity, shakeIntensity);
            LeanTween.rotateZ(timerText.gameObject, randomRotation, shakeSpeed)
                .setEase(LeanTweenType.easeInOutSine);

            yield return new WaitForSeconds(shakeSpeed);
            elapsedTime += shakeSpeed;
        }
    }

    private IEnumerator BounceTimerText()
    {
        if (timerText == null) yield break;

        float bounceScale = 0.35f; // Bigger scale variation for bouncing (increased from 0.15f)
        float bounceSpeed = 0.7f; // Slower bouncing (increased from 0.3f)
        float baseFontSize = 150f;

        // Bounce for the duration of the grid creation and animation
        float totalBounceTime = (delayBetweenSquares * 3f * gridSize) + animationDuration + 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < totalBounceTime)
        {
            if (timerText == null) yield break;

            // Bounce up
            LeanTween.value(timerText.gameObject, (float val) => { timerText.fontSize = val; }, 
                baseFontSize, baseFontSize + (baseFontSize * bounceScale), bounceSpeed / 2f)
                .setEase(LeanTweenType.easeOutSine);

            yield return new WaitForSeconds(bounceSpeed / 2f);

            // Bounce down
            LeanTween.value(timerText.gameObject, (float val) => { timerText.fontSize = val; }, 
                baseFontSize + (baseFontSize * bounceScale), baseFontSize, bounceSpeed / 2f)
                .setEase(LeanTweenType.easeInSine);

            yield return new WaitForSeconds(bounceSpeed / 2f);

            elapsedTime += bounceSpeed;
        }
    }
}
