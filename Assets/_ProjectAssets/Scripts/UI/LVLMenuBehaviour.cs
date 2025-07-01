using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LVLMenuBehaviour : MonoBehaviour
{
    public RectTransform lvlWrapper;
    public RectTransform content;
    public GameObject linePrefab; // Assign a UI Image prefab for the line
    public Color unlockedLineColor = Color.green;
    public Color lockedLineColor = Color.gray;
    public float lineWidth = 100f;
    public float lineHeight = 4f;
    public float horizontalLineMultiplier = 0.9f; // Longer lines for horizontal connections
    public float diagonalLineMultiplier = 0.7f;   // Shorter lines for diagonal connections
    
    [Header("Float Animation")]
    public bool enableFloatEffect = true;
    public float floatAmplitude = 10f;     // Base amplitude for floating
    public float floatSpeed = 2f;          // Base speed of floating animation
    public float floatRandomOffset = 2f;   // Random timing offset between buttons
    public float amplitudeVariation = 0.5f; // How much amplitude can vary (0-1)
    public float speedVariation = 0.3f;    // How much speed can vary (0-1)
    
    private int maxLvlReached;
    private List<GameObject> lines = new List<GameObject>();
    private List<RectTransform> buttons = new List<RectTransform>();
    private List<Vector3> originalButtonPositions = new List<Vector3>();
    private List<float> buttonFloatOffsets = new List<float>();
    private List<float> buttonAmplitudes = new List<float>();
    private List<float> buttonSpeeds = new List<float>();

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("MaxLvlReached"))
        {
            PlayerPrefs.SetInt("MaxLvlReached", 0);
        }
        else
        {
            maxLvlReached = PlayerPrefs.GetInt("MaxLvlReached", 0);
        }

    }


    private void OnEnable()
    {
        // Clear existing lines
        ClearLines();
        
        // Initialize buttons
        for (int i = 0; i <= maxLvlReached; i++)
        {
            LVLButtonBehaviour lvlButtonBehaviour = content.GetChild(i).GetComponent<LVLButtonBehaviour>();
            lvlButtonBehaviour.Init((i + 1).ToString(), true);
        }

        for (int i = maxLvlReached + 1; i < 20; i++)
        {
            LVLButtonBehaviour lvlButtonBehaviour = content.GetChild(i).GetComponent<LVLButtonBehaviour>();
            lvlButtonBehaviour.Init((i + 1).ToString(), false);
        }
        
        // Create lines after buttons are initialized
        StartCoroutine(CreateLinesAfterFrame());
        
        // Initialize floating animation
        if (enableFloatEffect)
        {
            InitializeFloatAnimation();
        }
    }
    
    private void Update()
    {
        if (enableFloatEffect && buttons.Count > 0)
        {
            UpdateFloatAnimation();
            UpdateLinePositions();
        }
    }
    
    private void InitializeFloatAnimation()
    {
        buttons.Clear();
        originalButtonPositions.Clear();
        buttonFloatOffsets.Clear();
        buttonAmplitudes.Clear();
        buttonSpeeds.Clear();
        
        // Collect all buttons and their original positions
        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform buttonRect = content.GetChild(i) as RectTransform;
            if (buttonRect != null && buttonRect.GetComponent<LVLButtonBehaviour>() != null)
            {
                buttons.Add(buttonRect);
                originalButtonPositions.Add(buttonRect.localPosition);
                
                // Generate independent random values for each button
                buttonFloatOffsets.Add(UnityEngine.Random.Range(0f, floatRandomOffset));
                
                // Vary amplitude for each button
                float amplitudeMultiplier = 1f + UnityEngine.Random.Range(-amplitudeVariation, amplitudeVariation);
                buttonAmplitudes.Add(floatAmplitude * amplitudeMultiplier);
                
                // Vary speed for each button
                float speedMultiplier = 1f + UnityEngine.Random.Range(-speedVariation, speedVariation);
                buttonSpeeds.Add(floatSpeed * speedMultiplier);
            }
        }
    }
    
    private void UpdateFloatAnimation()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != null)
            {
                // Use individual parameters for each button
                float timeOffset = buttonFloatOffsets[i];
                float amplitude = buttonAmplitudes[i];
                float speed = buttonSpeeds[i];
                
                float floatY = Mathf.Sin((Time.time + timeOffset) * speed) * amplitude;
                Vector3 newPos = originalButtonPositions[i];
                newPos.y += floatY;
                buttons[i].localPosition = newPos;
            }
        }
    }
    
    private void UpdateLinePositions()
    {
        for (int i = 0; i < lines.Count && i < buttons.Count - 1; i++)
        {
            if (lines[i] != null && buttons[i] != null && buttons[i + 1] != null)
            {
                RectTransform lineRect = lines[i].GetComponent<RectTransform>();
                RectTransform currentButton = buttons[i];
                RectTransform nextButton = buttons[i + 1];
                
                // Get current button positions
                Vector3 currentPos = currentButton.localPosition;
                Vector3 nextPos = nextButton.localPosition;
                
                // Calculate line properties
                Vector2 direction = nextPos - currentPos;
                float distance = direction.magnitude;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                // Determine if line is horizontal vs diagonal
                bool isHorizontal = Mathf.Abs(direction.y) < Mathf.Abs(direction.x) * 0.3f;
                float lineMultiplier = isHorizontal ? horizontalLineMultiplier : diagonalLineMultiplier;
                
                // Update line position and rotation
                Vector2 linePosition = (Vector2)currentPos + direction * 0.5f;
                lineRect.localPosition = linePosition;
                lineRect.sizeDelta = new Vector2(distance * lineMultiplier, lineHeight);
                lineRect.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
    
    private IEnumerator CreateLinesAfterFrame()
    {
        // Wait one frame to ensure all UI elements are properly positioned
        yield return null;
        CreateLinesBetweenButtons();
    }
    
    private void ClearLines()
    {
        foreach (GameObject line in lines)
        {
            if (line != null)
            {
                DestroyImmediate(line);
            }
        }
        lines.Clear();
    }
    
    private void CreateLinesBetweenButtons()
    {
        if (linePrefab == null)
        {
            Debug.LogError("Line prefab is not assigned!");
            return;
        }
        
        // Use the buttons list if it's already populated, otherwise collect them
        List<RectTransform> buttonsToUse = buttons.Count > 0 ? buttons : new List<RectTransform>();
        
        if (buttonsToUse.Count == 0)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                RectTransform buttonRect = content.GetChild(i) as RectTransform;
                if (buttonRect != null && buttonRect.GetComponent<LVLButtonBehaviour>() != null)
                {
                    buttonsToUse.Add(buttonRect);
                }
            }
        }
        
        // Create lines between buttons
        for (int i = 0; i < Mathf.Min(19, buttonsToUse.Count - 1); i++)
        {
            RectTransform currentButton = buttonsToUse[i];
            RectTransform nextButton = buttonsToUse[i + 1];
            
            if (currentButton == null || nextButton == null) continue;
            
            // Create line
            GameObject line = Instantiate(linePrefab, content);
            RectTransform lineRect = line.GetComponent<RectTransform>();
            Image lineImage = line.GetComponent<Image>();
            
            if (lineImage == null)
            {
                Debug.LogError("Line prefab must have an Image component!");
                continue;
            }
            
            // Set anchor and pivot FIRST before positioning
            lineRect.anchorMin = new Vector2(0.5f, 0.5f);
            lineRect.anchorMax = new Vector2(0.5f, 0.5f);
            lineRect.pivot = new Vector2(0.5f, 0.5f);
            
            // Get button positions using localPosition
            Vector3 currentPos = currentButton.localPosition;
            Vector3 nextPos = nextButton.localPosition;
            
            // Calculate line properties
            Vector2 direction = nextPos - currentPos;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Determine if line is horizontal (or nearly horizontal) vs diagonal
            bool isHorizontal = Mathf.Abs(direction.y) < Mathf.Abs(direction.x) * 0.3f; // If Y change is less than 30% of X change
            float lineMultiplier = isHorizontal ? horizontalLineMultiplier : diagonalLineMultiplier;
            
            // Set line position to center between buttons
            Vector2 linePosition = (Vector2)currentPos + direction * 0.5f;
            
            // Configure line transform - use localPosition instead of anchoredPosition
            lineRect.localPosition = linePosition;
            lineRect.sizeDelta = new Vector2(distance * lineMultiplier, lineHeight);
            lineRect.localRotation = Quaternion.Euler(0, 0, angle);
            
            // Set line color based on unlock status
            bool isUnlocked = i < maxLvlReached;
            lineImage.color = isUnlocked ? unlockedLineColor : lockedLineColor;
            
            // Add to lines list
            lines.Add(line);
            
            // Set line as sibling index to appear behind buttons
            line.transform.SetSiblingIndex(0);
        }
    }
    
    public void UpdateLinesForNewLevel(int newMaxLevel)
    {
        maxLvlReached = newMaxLevel;
        
        // Update line colors
        for (int i = 0; i < lines.Count && i < lines.Count; i++)
        {
            if (lines[i] != null)
            {
                Image lineImage = lines[i].GetComponent<Image>();
                if (lineImage != null)
                {
                    bool isUnlocked = i < maxLvlReached;
                    lineImage.color = isUnlocked ? unlockedLineColor : lockedLineColor;
                }
            }
        }
    }
    
}
