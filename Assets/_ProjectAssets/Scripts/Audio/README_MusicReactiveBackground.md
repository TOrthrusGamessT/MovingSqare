# Music-Reactive Background Documentation

This system provides three different scripts for creating music-reactive background color changes that respond to beat detection in your Unity project.

## Scripts Overview

### 1. MusicReactiveBackground.cs (Advanced)
**Use Case**: Full-featured beat detection with multiple analysis methods and extensive customization.

**Features**:
- Multiple beat detection algorithms (Energy Threshold, Frequency Analysis, Amplitude Spikes, Spectrum Peaks)
- Frequency band analysis (Bass, Low-Mid, High-Mid, Treble)
- Color cycling and pulse effects
- Integration with camera shake
- Extensive debug information
- Works with Camera backgrounds and SpriteRenderer

### 2. SimpleMusicReactiveBackground.cs (Simple)
**Use Case**: Easy-to-use version with minimal configuration required.

**Features**:
- Simple energy-based beat detection
- Three customizable beat colors
- Auto-detection of AudioSource
- Works with Camera, SpriteRenderer, and BackGroundImageController
- Runtime parameter adjustment

### 3. MusicReactiveBackgroundManager.cs (Recommended)
**Use Case**: Production-ready system that integrates seamlessly with existing game systems.

**Features**:
- Auto-setup with intelligent component detection
- Integration with SoundManager and MenuManager
- Respects music on/off settings
- Works with Camera, SpriteRenderer, and UI Image
- Multiple background target support
- Camera shake integration

## Quick Setup Guide

### Option 1: Recommended Setup (MusicReactiveBackgroundManager)

1. **Add to Scene**:
   - Create an empty GameObject in your scene
   - Add the `MusicReactiveBackgroundManager` component
   - Leave "Auto Setup" enabled

2. **Configuration**:
   - The script will automatically find your AudioSource, Camera, and background objects
   - Adjust `Beat Sensitivity` (0.08 is a good starting point)
   - Customize colors in the `Music Colors` array

3. **Integration**:
   - Works automatically with existing SoundManager and MenuManager
   - Respects music on/off settings
   - Integrates with CameraShaking if present

### Option 2: Simple Setup (SimpleMusicReactiveBackground)

1. **Add to Scene**:
   - Add `SimpleMusicReactiveBackground` to any GameObject
   - Enable "Auto Find Audio Source"

2. **Choose Target**:
   - Assign either `Target Camera`, `Target Sprite Renderer`, or `Background Controller`
   - The script will change the color of whichever target you assign

3. **Adjust Settings**:
   - `Sensitivity`: How easily beats are detected (0.1 = more sensitive)
   - `Color Speed`: How fast colors change
   - `Return Speed`: How fast it returns to base color

### Option 3: Advanced Setup (MusicReactiveBackground)

1. **Setup**:
   - Add `MusicReactiveBackground` to a GameObject
   - Assign `Audio Source` manually
   - Choose `Use Camera Background` or assign `Background Sprite Renderer`

2. **Beat Detection**:
   - Choose `Detection Method`:
     - `Energy Threshold`: Most reliable, good for most music
     - `Frequency Analysis`: Better for bass-heavy music
     - `Amplitude Spikes`: Good for electronic music
     - `Spectrum Peaks`: Best for complex compositions

3. **Advanced Features**:
   - Enable `Use Frequency Bands` for more sophisticated analysis
   - Enable `Pulse Effect` for additional visual feedback
   - Enable `Color Cycling` for continuous color changes

## Configuration Tips

### Beat Detection Sensitivity
- **Too sensitive**: Colors change constantly, even during quiet parts
- **Not sensitive enough**: Only very loud beats trigger color changes
- **Sweet spot**: Colors change on clear beats but stay stable during verses

**Recommended Values**:
- Electronic/Dance music: 0.06 - 0.08
- Rock/Pop music: 0.08 - 0.12
- Classical/Ambient: 0.10 - 0.15

### Color Configuration
- **Base/Default Color**: The color shown when no beat is detected
- **Beat Colors**: Colors that flash on beat detection
- **Color Intensity**: How strong the color change is (0.7 is usually good)
- **Transition Speed**: How quickly colors change (3.0 is a good balance)

### Performance Considerations
- All scripts are optimized for real-time use
- Audio analysis runs at 60fps without significant performance impact
- The Advanced script uses more CPU due to frequency analysis

## Integration with Existing Systems

### With SoundManager
```csharp
// The scripts automatically detect and work with your SoundManager
// They respect the musicOn setting
```

### With MenuManager
```csharp
// Works with MenuManager's music toggle functionality
// Automatically finds the AudioSource component
```

### With CameraShaking
```csharp
// MusicReactiveBackgroundManager automatically triggers camera shake on beats
// Integrates with your existing CameraShaking script
```

### With BackGroundImageController
```csharp
// SimpleMusicReactiveBackground can target your existing background system
// Assign the BackGroundImageController to the Background Controller field
```

## Scene-Specific Usage

### Main Menu
- Use **MusicReactiveBackgroundManager** for best integration
- Set colors to match your UI theme
- Lower sensitivity for more subtle effects

### Gameplay Scenes
- Use **MusicReactiveBackgroundManager** with "Enable Only In Gameplay" option
- Higher sensitivity for more dynamic effects
- Consider enabling camera shake integration

### Survival Mode
- More intense colors and faster transitions
- Higher sensitivity to match fast-paced gameplay
- Consider using pulse effects

## Troubleshooting

### No Color Changes
1. Check that an AudioSource is assigned and playing
2. Verify that music settings allow music to play
3. Increase beat sensitivity
4. Check that a background target is assigned

### Colors Change Too Often
1. Decrease beat sensitivity
2. Increase beat cooldown time
3. Check audio levels (very loud audio can cause constant triggers)

### Colors Don't Return to Base
1. Increase "Return Speed" values
2. Check that base color is different from beat colors
3. Ensure audio is not constantly triggering beats

### Performance Issues
1. Use SimpleMusicReactiveBackground instead of the advanced version
2. Reduce sample data length in advanced settings
3. Disable frequency band analysis
4. Disable debug mode

## Runtime Control

All scripts provide public methods for runtime control:

```csharp
// Get the component
var musicReactive = FindObjectOfType<MusicReactiveBackgroundManager>();

// Adjust sensitivity
musicReactive.SetBeatSensitivity(0.1f);

// Change default color
musicReactive.SetDefaultColor(Color.blue);

// Add new beat color
musicReactive.AddMusicColor(Color.purple);

// Enable/disable the system
musicReactive.SetActive(false);
```

## Best Practices

1. **Start Simple**: Begin with MusicReactiveBackgroundManager for most cases
2. **Test with Different Music**: Different genres may need different sensitivity settings
3. **Consider Your Art Style**: Subtle effects often work better than dramatic ones
4. **Respect Player Preferences**: Always integrate with existing music settings
5. **Performance**: Use the simplest script that meets your needs

## Future Enhancements

The system is designed to be extensible. You can:
- Add new beat detection algorithms
- Create custom color patterns
- Integrate with other visual effects
- Add UI controls for player customization
- Sync with game events (level completion, power-ups, etc.)
