# DistortPlane
Generate plane/grid mesh with configurable number of segments. Use Handles on the four corners to distort!

Created with Unity 2018.1.0f2
On OSX, Metal throws annoying Assertion errors when using mesh.RecalculateBounds(). This seems to me a Metal thing, as it doesn't happen in OpenGL. 

Untested on any other platform.
 
### Usage: 
* Add "DistortPlane" script to any added 3D object in your scene - though for consistency, I'd suggest adding a Plane primitive GameObject.
* Script will automatically create a new Mesh object and replace the one on your [required] MeshFilter componenent with it. 
* When selected in the Hierarchy, use the Handles on each corner to push/pull etc. 
* Use H/V Segments sliders to set number of subdivisions in either direction. 

#### Options:
* "Reset Corners" resets the positions of the corner Handles so that the overall width/height = 1 unit. However, if "Use Texture For Initial Position" is turned on, script will query the Material for a texture set to "_MainTex", commonly known as the Albedo map texture on the Standard Shader.
* "Show Bounds" will draw the bounding box. 



![GIF](https://i.imgur.com/WDjYKPD.gif)
