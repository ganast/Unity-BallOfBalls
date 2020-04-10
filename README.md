# Ball of Balls

Version 1.0, April 11, 2020

Copyright (c) 2020 by George Anastassakis (ganast@ganast.com)

## About

A simple demonstration of the performance difference when using baseline scenegraph (gameobject) vs. DOTS (ECS) techniques in the Unity engine to render and manage large numbers of physics-enabled colliding objects.

## Running

Adjust the following settings:

- NS: Number of balls
- U0: Initial speed (direction is random)
- FF: Force factor for keyboard-based manipulation
- SE: Spawn extents (around world origin)
- BS: Ball scale

Press reset to reset the scene by spawining balls as defined in the settings.

Toggle bounding box on and off in real-time.

Change between scenegraph and DOTS mode in real-time. In each case, each ball is converted on-the-fly to a gameobject or entity, respectively, with the same translation, rotation and velocity vectors, so that the effect continues seamlessly.

Left mouse drag to orbit camera. Mouse wheel to adjust viewing distance.

Note that conversion is seamless but with a slight delay depending on the number of objects converted, since all objects are converted in a single frame. This is intentional and used instead of a job-based conversion over multiple frames so that the simulation proceeds based on either gameobject or entity physics and not a mix of the two, which would lead to unpredictable -and potentially observably erratic- results.

Take extra caution with the various settings. The program does not check the values you provide in any way. Make small, progressive changes, especially to the number of balls and the ball scale settings, to avoid stressing your system or rendering it completely unresponsive due to, e.g., too many balls or an infinite collision-resolution loop caused by too many or too large balls confined within the bounding box.

## Preview package dependencies

- com.unity.dots.editor 0.3.0-preview
- com.unity.entities 0.6.0-preview.24
- com.unity.physics 0.3.0-preview.1
- com.unity.rendering.hybrid 0.3.4-preview.24

## Disclaimer

This demo does not mean to demonstrate optimal or otherwise preferred programming techniques -especially in relation with entity management and conversion and the DOTS framework in general- and should not be relied-upon as such.

Having said that, comments and suggestions regarding errors and improvements are more than welcome.

## Contact

ganast@ganast.com
