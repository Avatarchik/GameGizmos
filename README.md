# GameGizmos
Language: C# | Software: Unity3D 5.x | Author: [SlateNeon](https://github.com/SlateNeon) | Website: http://slateneon.github.io/

## Summary
GameGizmos is an utility class for drawing gizmo-like objects during runtime of a game or application. The idea is to emulate Debug.DrawLine/Gizmos.DrawLine for use within game runtime, without requiring Unity editor. This solution does not use GL class to create the lines and instead draws the meshes using line topology.

## Features
* Lines
* Rectangles
* Cubes
* Circles
* Spheres

## Usage
Put the 'GameGizmos' folder in your Unity asset folder. To draw a gizmo, simply call one or more of the static functions within the class. No other setup is required.

**Example:**
```csharp
GameGizmos.DrawLine(transform.position, Vector3.zero, Color.red);
GameGizmos.DrawCube(transform.position, transform.localScale, Color.blue, transform.localRotation);
```

## Credits
* [Vertex color shader](http://wiki.unity3d.com/index.php/VertexColorUnlit)
* [Vector3 point rotation](http://answers.unity3d.com/questions/532297/rotate-a-vector-around-a-certain-point.html)

## Changelog
* [28/02/2015] - (12 hours later) A small rewrite, added circle and sphere shape, added basic commentary(lets play GameGizmos), further optimizations are planned
* [28/02/2015] - (soon after) A bit more optimization, little update (like a small puppy)
* [28/02/2015] - Made stuff a little more GC friendly, thus increasing performance
* [27/02/2015] - (later that day) Added cube shape, changed shader to draw gizmos on top of everything
* [27/02/2015] - Release