# Advanced Glow Setup Instructions

## Overview
The advanced glow system provides a beautiful, shader-based glow effect for interactable objects that's much more visually appealing than simple outline highlighting.

## Setup Steps

### 1. Create Glow Material
1. In the Project window, right-click and select `Create > Material`
2. Name the material "SignGlow" (or similar)
3. In the Material Inspector:
   - Change the Shader to "Custom/SpriteGlow" (the shader we just created)
   - Set the Glow Color to your desired color (e.g., bright yellow or white)
   - Set Glow Intensity to around 1.5-2.0
   - Set Brightness to around 1.2-1.5

### 2. Configure SignController
1. Select your Sign GameObject in the hierarchy
2. In the SignController component:
   - Check "Use Advanced Glow"
   - Set "Glow Intensity" to 1.5-2.0
   - Set "Glow Pulse Speed" to 2.0-3.0
   - Check "Enable Pulse" for animated glow effect

### 3. Test the Effect
- Run the game and approach the sign
- You should see a smooth glow effect instead of the outline
- The glow should pulse if "Enable Pulse" is checked

## Customization Options

### Glow Intensity
- Controls how bright the glow appears
- Range: 0.5 (subtle) to 3.0 (very bright)
- Recommended: 1.5-2.0

### Pulse Speed
- Controls how fast the glow pulses
- Range: 1.0 (slow) to 5.0 (fast)
- Recommended: 2.0-3.0

### Glow Color
- Set in the material properties
- Bright colors work best (yellow, white, cyan, etc.)
- Avoid dark colors as they won't be visible

## Alternative: Using Built-in Shaders

If you prefer to use Unity's built-in shaders instead of the custom shader:

1. Create a material using "Sprites/Default" shader
2. In SignController, the system will fall back to color multiplication
3. Increase the "Glow Intensity" value to compensate (try 2.0-3.0)

## Troubleshooting

### Glow Not Visible
- Check that "Use Advanced Glow" is enabled
- Ensure the material is assigned and has appropriate glow settings
- Try increasing Glow Intensity values

### Performance Issues
- Disable "Enable Pulse" if you have many glowing objects
- Reduce Glow Intensity to lower values
- Consider using the outline method for mobile platforms

### Glow Too Bright/Dark
- Adjust the Glow Intensity in SignController
- Modify the Glow Color in the material properties
- Adjust Brightness value in the material

## Extending to Other Objects

The same system can be applied to:
- ChestController (for chest glow effects)
- DoorController (for door highlighting)
- PropsAltar (for altar glow effects)

Simply copy the advanced glow code sections from SignController to these other scripts and add the same inspector properties.