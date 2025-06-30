# Music-Reactive Background Setup for Survive Mode

## Quick Setup Instructions

### Option 1: Automatic Setup (Recommended)
The music-reactive background system will automatically set itself up when you start the Survive scene.

1. **Play the Survive scene** - The GameManager will automatically create and configure the music-reactive background
2. **Assign music** - Add an AudioClip to any AudioSource in the scene for music playback
3. **Enjoy** - The background will automatically react to beats in the music!

### Option 2: Manual Setup

1. **Add the Component**:
   - Create an empty GameObject in the Survive scene
   - Name it "Music Reactive Background"
   - Add the `SurviveMusicReactive` component

2. **Configure Audio**:
   - If you have an AudioSource in the scene, it will be auto-detected
   - Or assign your AudioSource manually in the component
   - Make sure the AudioSource has a music clip assigned and is playing

3. **Configure Camera**:
   - The Main Camera will be auto-detected
   - Or assign your camera manually in the component

### Settings Configuration

#### Beat Detection
- **Beat Sensitivity**: Lower = more sensitive to beats (0.05-0.3)
  - For electronic/dance music: 0.06-0.08
  - For rock/pop music: 0.08-0.12
  - For ambient music: 0.10-0.15

#### Colors
- **Base Color**: Background color when no beats are detected
- **Beat Colors**: Array of colors that flash on beat detection
- **Color Transition Speed**: How fast colors change (default: 8)

#### Advanced Settings
- **Beat Cooldown**: Minimum time between beat detections (default: 0.1s)

## Integration with Existing Systems

### With SoundManager
The system automatically finds and uses your existing SoundManager's AudioSource.

### With GameManager
- Automatically starts when Survive mode begins
- Stops when game over occurs
- Restarts when player revives

### With Camera Shake
If you have additional camera effects, they can be integrated separately from the music-reactive background system.

## Troubleshooting

### No Color Changes
1. Check that an AudioSource is assigned and playing music
2. Verify the AudioSource has an audio clip assigned
3. Try increasing beat sensitivity
4. Make sure the component is enabled and active

### Colors Change Too Often
1. Decrease beat sensitivity
2. Increase beat cooldown time
3. Check audio levels aren't too high

### Performance Issues
1. Use fewer beat colors
2. Reduce color transition speed
3. Disable camera shake if not needed

## Runtime Control

You can control the music-reactive background through code:

```csharp
// Get the component
SurviveMusicReactive musicReactive = FindObjectOfType<SurviveMusicReactive>();

// Adjust sensitivity
musicReactive.SetBeatSensitivity(0.1f);

// Change base color
musicReactive.SetBaseColor(Color.blue);

// Add new beat color
musicReactive.AddBeatColor(Color.purple);

// Enable/disable
musicReactive.SetActive(false);

// Start/stop for Survive mode
musicReactive.StartSurviveMode();
musicReactive.StopSurviveMode();
```

## GameManager Integration

The GameManager has been updated with music-reactive background support:

- **Enable Music Reactive Background**: Toggle in GameManager inspector
- **Music Reactive Background**: Reference to the component (auto-found if empty)

## Files Added

1. `SurviveMusicReactive.cs` - Main music-reactive background component
2. `SurviveMusicReactiveSetup.cs` - Alternative setup helper (if needed)
3. Updates to `GameManager.cs` for automatic integration

## Best Practices

1. **Start Simple**: Use the automatic setup first, then customize
2. **Test Different Music**: Different genres may need different sensitivity settings
3. **Match Your Art Style**: Choose colors that complement your game's visual style
4. **Consider Player Preferences**: Integrate with your music on/off settings
5. **Performance**: The system is optimized but monitor performance on lower-end devices

Enjoy your music-reactive Survive mode! 🎵🎮
