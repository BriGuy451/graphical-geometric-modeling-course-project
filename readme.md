# Visual Effects in Unity (WIP)

To-do:
- add itch.io link to executable.

## Overview
This is a repository for the visual effects I developed in 6991 3D Graphical and Geometrical Modeling course. This is a standard Unity project with it's standard file architecture. All of the essential code exists within the Assets folder. The primary bulk of the visual effects are stored in the Shaders folder (Assets -> Shaders). Inside the Shaders directory, the the sub-directories map to the Visual Effect scenes. So if you would like to know what went into making the effect you can look in those directories.

### Vertex Dissolve Scene
A vertex based dissolve that collapses a vertex to a central position depending on it's place in relation to the mesh's bounds. Dissolve effect goes from top to bottom, but this could be easily modified to move in any arbitrary direction with some modification.

![Vertex Dissolve Scene](./Images/vertex_dissolve.png)

### Fragment Dissolve Scene
This is the same as the vertex dissolve except it is implemented in the fragment shader creating a smooth dissolve.

![Fragment Dissolve Scene](./Images/fragment_dissolve.png)


### Lightning Strike Scene
This was a more involved scene that I tried to make, inspired from a [Game Developer Conference](https://www.youtube.com/watch?v=KaNDezgsg4M) presentation by Simon Trumpler. He created a much better lightning effect but this was a good challenge and way to build some fundamental understanding about making visual effects. There are a decent amount of moving pieces from this but I essentially followed his steps while learning some good tips and techniques. There a multiple effects including erosion of the cloud texture, emission glow for lightning build-up, using gradient texture for unraveling of lightning, and a splash effect.

![Lightning Strike Scene](./Images/lightning_vfx.png)


### Material Panner Scene
This is a simple texture panner. It scrolls the texture that is attached to the mesh over time.

![Material Dissolve Scene](./Images/material_dissolve.png)

### Explosion Scene
This scene implements multiple flipbook shaders to give the effect of fire and the explosion. The flipbooks used were obtained from a Unity [blog](https://unity.com/blog/engine-platform/free-vfx-image-sequences-flipbooks) on visual effect design. This requires sequence of playing flipbooks, disabling mesh renderers, and applying forces for debris scatter.

![Explosion Scene](./Images/explosion_vfx.png)

### Implosion Scene
This was a pretty cool effect I liked, took me longer to create than it should have but I was messing around trying to create a morph visual effect. That morph code is still in this repository but doesn't work exactly. For the implosion effect, the mesh is sucked into a central point. Groups of vertices move to the central point at different intervals, creating an effect that shows the mesh being sucked into a central point until it is nothing.

![Implosion Scene](./Images/vertex_implosion.png)


### Aura Scene


![Aura Scene](./Images/aura_scene.png)




## Reference Websites for understanding Shaders and Graphics programming:
- Catlike Code
- OpenGL Book
- Procedural Texturing
- Unity Shader Graph Link
- Unity shader documentation