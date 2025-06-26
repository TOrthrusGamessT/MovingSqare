using UnityEngine;

/// <summary>
/// Example of how to connect SquareAnimator with other visual systems
/// This makes particles react to the square's animation state
/// </summary>
public class AnimationReactiveEffects : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem[] excitementParticles;
    [SerializeField] private float particleIntensityMultiplier = 2f;
    
    [Header("Background Effects")]
    [SerializeField] private bool useBackgroundParticles = true;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioSource movementAudioSource;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.5f;
    
    private SquareAnimator squareAnimator;
    private float lastExcitementLevel = 0f;

    private void Awake()
    {
        squareAnimator = GetComponent<SquareAnimator>();
    }

    private void OnEnable()
    {
        // Subscribe to animation events
        SquareAnimator.OnExcitementChanged += HandleExcitementChanged;
        SquareAnimator.OnMovementStarted += HandleMovementStarted;
        SquareAnimator.OnMovementStopped += HandleMovementStopped;
        SquareAnimator.OnDirectionChanged += HandleDirectionChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe from animation events
        SquareAnimator.OnExcitementChanged -= HandleExcitementChanged;
        SquareAnimator.OnMovementStarted -= HandleMovementStarted;
        SquareAnimator.OnMovementStopped -= HandleMovementStopped;
        SquareAnimator.OnDirectionChanged -= HandleDirectionChanged;
    }

    private void HandleExcitementChanged(float excitementLevel)
    {
        // Update particle systems based on excitement
        if (excitementParticles != null)
        {
            foreach (var particles in excitementParticles)
            {
                if (particles != null)
                {
                    var emission = particles.emission;
                    emission.rateOverTime = excitementLevel * particleIntensityMultiplier;
                    
                    // Change particle color based on excitement
                    var colorOverLifetime = particles.colorOverLifetime;
                    colorOverLifetime.enabled = true;
                    
                    Gradient gradient = new Gradient();
                    gradient.SetKeys(
                        new GradientColorKey[] { 
                            new GradientColorKey(Color.white, 0.0f), 
                            new GradientColorKey(Color.Lerp(Color.white, Color.red, excitementLevel), 1.0f) 
                        },
                        new GradientAlphaKey[] { 
                            new GradientAlphaKey(1.0f, 0.0f), 
                            new GradientAlphaKey(0.0f, 1.0f) 
                        }
                    );
                    
                    colorOverLifetime.color = gradient;
                }
            }
        }

        // Update audio pitch based on excitement
        if (movementAudioSource != null && squareAnimator.IsAnimationInitialized)
        {
            movementAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, excitementLevel);
            
            // Play sound if excitement increased significantly
            if (excitementLevel > lastExcitementLevel + 0.2f && !movementAudioSource.isPlaying)
            {
                movementAudioSource.Play();
            }
        }

        lastExcitementLevel = excitementLevel;
    }

    private void HandleMovementStarted()
    {
        // Trigger background particle animation when movement starts
        if (useBackgroundParticles && BackgroundParticlesManager.instance != null)
        {
            // You can create a custom method in BackgroundParticlesManager for movement effects
            // BackgroundParticlesManager.instance.TriggerMovementAnimation();
        }

        // Start movement audio loop
        if (movementAudioSource != null && !movementAudioSource.isPlaying)
        {
            movementAudioSource.Play();
        }

        // Trigger special animation effect
        if (squareAnimator != null)
        {
            squareAnimator.TriggerSpecialAnimation("excitement", 0.3f);
        }
    }

    private void HandleMovementStopped()
    {
        // Stop movement audio
        if (movementAudioSource != null && movementAudioSource.isPlaying)
        {
            movementAudioSource.Stop();
        }

        // Reduce particle intensity when stopped
        if (excitementParticles != null)
        {
            foreach (var particles in excitementParticles)
            {
                if (particles != null)
                {
                    var emission = particles.emission;
                    emission.rateOverTime = 0;
                }
            }
        }
    }

    private void HandleDirectionChanged(Vector2 newDirection)
    {
        // Create special effect when direction changes quickly
        if (squareAnimator != null)
        {
            squareAnimator.TriggerSpecialAnimation("bounce", 0.8f);
        }

        // Maybe spawn a directional particle burst
        if (excitementParticles != null && excitementParticles.Length > 0)
        {
            var burstParticles = excitementParticles[0];
            if (burstParticles != null)
            {
                burstParticles.Emit(5); // Quick burst of particles
            }
        }
    }
}
