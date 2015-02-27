# GameGizmos
Language: C# | Software: Unity3D 5.x | Author: SlateNeon | Website: http://slateneon.github.io/

## Summary
GameGizmos is an utility class for drawing gizmo-like objects during runtime of a game or application. The idea is to emulate Debug.DrawLine/Gizmos.DrawLine for use within game runtime, without requiring Unity editor. This solution does not use GL class to create the lines and instead draws the meshes using line geometry.

## Features
* Lines
* Rectangles
* Cubes

## Usage
Put the 'GameGizmos' folder in your Unity asset folder. To draw a gizmo, simply call one or more of the static functions within the class. No other setup is required.

## Credits
* Vertex color shader [ http://wiki.unity3d.com/index.php/VertexColorUnlit ]
* Vector3 point rotation [ http://answers.unity3d.com/questions/532297/rotate-a-vector-around-a-certain-point.html ]

## Changelog
* [27/02/2015] - (later that day) Added cube shape, changed shader to draw gizmos on top of everything
* [27/02/2015] - Release
