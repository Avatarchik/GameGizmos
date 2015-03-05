# GameGizmos
Language: C# | Software: Unity3D 5.x | Author: [SlateNeon](https://github.com/SlateNeon) | Website: http://slateneon.github.io/

## About
GameGizmos is a tool written in C# for Unity3D engine. The purpose of this tool is to draw gizmo-like lines and shapes during runtime.
Unlike Debug or Gizmos class in Unity3D, this tool does not require editor to see the gizmos it produces.
This implementation does not use GL lines class and instead creates the lines by using meshes with MeshTopology set to lines.

### Shapes
* Line
* Rectangle
* Cube
* Circle
* Sphere


## Installation
Place GameGizmos.cs inside your project folder

## Usage
-TODO-

## Platforms
* [YES] Desktop Windows
* [???] Desktop Linux
* [???] Desktop OSX
* [YES] WebPlayer
* [???] Mobile Android
* [???] Mobile iOS

## Credits
* [Vertex color shader](http://wiki.unity3d.com/index.php/VertexColorUnlit)
* [Vector3 point rotation](http://answers.unity3d.com/questions/532297/rotate-a-vector-around-a-certain-point.html)

## Changelog
* [05/03/15] - (after sleep) Integrated shader into script itself. Otherwise shader would have to be manually included which makes script less portable
* [05/03/15] - Fixed sphere gizmo
* [28/02/15] - (12 hours later) A small rewrite, added circle and sphere shape, added basic commentary(lets play GameGizmos), further optimizations are planned
* [28/02/15] - (soon after) A bit more optimization, little update (like a small puppy)
* [28/02/15] - Made stuff a little more GC friendly, thus increasing performance
* [27/02/15] - (later that day) Added cube shape, changed shader to draw gizmos on top of everything
* [27/02/15] - Release