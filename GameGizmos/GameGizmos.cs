using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// GameGizmos.cs
// https://github.com/SlateNeon/GameGizmos
// Last Edit: 2015-03-08

public class GameGizmos : MonoBehaviour
{
    #region SINGLETON
    private static GameGizmos _I;
    /// <summary>
    /// Singleton instance
    /// </summary>
    public static GameGizmos I {
        get {
            if (_I != null) return _I;
            _I = FindObjectOfType<GameGizmos>() ?? (new GameObject("GameGizmos", typeof(GameGizmos))).GetComponent<GameGizmos>();
            return _I;
        }
    }
    #endregion

    /// <summary>
    /// The gizmo color if no other color is provided
    /// </summary>
    public Color32 DefaultColor = Color.white;

    private Transform _t;
    private Mesh _mesh;
    private MeshFilter _mFilter;
    private MeshRenderer _mRenderer;

    private readonly List<Vector3> _dVerts = new List<Vector3>();
    private readonly List<int> _dIndices = new List<int>();
    private readonly List<Color32> _dColors32 = new List<Color32>();

    public readonly List<Vector3> PutListVerts = new List<Vector3>();
    public readonly List<Color32> PutListColors32 = new List<Color32>();

    /// <summary>
    /// Initialize empty GameObject with necessary data
    /// </summary>
    private void Init()
    {
        _t = transform;
        _t.name = "GameGizmos";
        _mFilter = gameObject.AddComponent<MeshFilter>();
        _mRenderer = gameObject.AddComponent<MeshRenderer>();
        _mesh = new Mesh { name = "GameGizmos_Mesh" };
        _mFilter.mesh = _mesh;
        _mesh.MarkDynamic();
        _mRenderer.material = new Material(ShaderText);
    }
    /// <summary>
    /// Populates mesh with data from temporary storage
    /// </summary>
    private void PopulateMesh()
    {
        _mesh.vertices = _dVerts.ToArray();
        _mesh.SetIndices(_dIndices.ToArray(), MeshTopology.Lines, 0);
        _mesh.colors32 = _dColors32.ToArray();
    }
    /// <summary>
    /// Clears all mesh data, both actual and temp
    /// </summary>
    private void ClearEverything()
    {
        _mesh.Clear();
        _dVerts.Clear();
        _dIndices.Clear();
        _dColors32.Clear();
    }
    /// <summary>
    /// Puts transform into its natural zero state
    /// </summary>
    private void ZeroTransform()
    {
        _t.position = Vector3.zero;
        _t.rotation = Quaternion.identity;
        _t.localScale = Vector3.one;
    }

    /// <summary>
    /// Adds data from collections into temp storage that will be used to render gizmo
    /// </summary>
    /// <param name="verts">Vertex positions</param>
    /// <param name="colors">Colors for vertices, must match verts count</param>
    /// <param name="loop">Does this shape loop? If so, connect 1st and last point I.E. circle</param>
    public void Put(List<Vector3> verts, List<Color32> colors, bool loop = false)
    {
        if (!enabled) return;   //if gizmos are off, dont put anything

        if (verts.Count < 3)    // if two points, just a straight line
        {
            foreach (var v in verts)
            {
                
                _dIndices.Add(_dVerts.Count);
                _dVerts.Add(v);
            }

            foreach (var c in colors)
            {
                _dColors32.Add(c);
            }
        }
        else // a line strip
        {
            var startIndex = _dVerts.Count;
            for (var i = 1; i < verts.Count; i++)
            {
                if (i == 1) // 1st line segment
                {
                    _dIndices.Add(_dVerts.Count);
                    _dVerts.Add(verts[i - 1]);
                    _dIndices.Add(_dVerts.Count);
                    _dVerts.Add(verts[i]);
                      
                }
                else // all other line segments
                {
                    _dIndices.Add(_dVerts.Count - 1);
                    _dIndices.Add(_dVerts.Count);
                    _dVerts.Add(verts[i]);
                }
            }
            if (loop) // close off loop if needed
            {
                
                _dIndices.Add(_dIndices[_dIndices.Count - 1]);
                _dIndices.Add(startIndex);
            }
            foreach (var c in colors)
            {
                _dColors32.Add(c);
            }
        }
    }

    #region Unity Callbacks
    void Awake()
    {
        Init();
    }
    private bool _wasRendered = false;
    private void OnBecameVisible()
    {
        ClearEverything();  // clear everything when object first becomes visible, prevents overflow bug and toggle flicker
    }
    private void OnWillRenderObject()
    {
        if(_wasRendered) return;
        PopulateMesh();
        ZeroTransform();
        _wasRendered = true;
    }
    private void OnRenderObject()
    {
        if(Camera.current != Camera.main) return;   //if the camera that rendered wasn't main, keep mesh until main renders it (this is done to prevent flicker)

        ClearEverything(); //clears data after render is complete
        _wasRendered = false;
    }
    #endregion

    #region SHADER
    // Shader embedded in script because otherwise manual inclusion was required for shader to work in builds
    private const string ShaderText = "Shader \"Unlit/VertexColor\" {\n" +
        "\n" +
        "Category {\n" +
        "\t\n" +
        "\tLighting Off\n" +
        "\tZWrite Off\n" + 
        "\tBindChannels {\n" +
        "\t\tBind \"Color\", color\n" +
        "\t\tBind \"Vertex\", vertex\n" +
        "\t\tBind \"TexCoord\", texcoord\n" +
        "\t}\n" +
        "\n" +
        "\tSubShader {\n" +
        "\t\tZTest Always\n" + //remove this for depth support(so it doesn't draw on top)
        "\t\tTags { \"Queue\"=\"Overlay\"}\n" +
        "\t\tPass {\n" +
        "\t\t\tSetTexture [_MainTex] {\n" +
        "\t\t\t\tcombine primary\n" +
        "\t\t\t}\n" +
        "\t\t}\n" +
        "\t}\n" +
        "}\n" +
        "}";
    #endregion
}

public static class GizmoShapes
{
    #region LINES
    /// <summary>
    /// Draws a line "from" to "to" with default color
    /// </summary>
    /// <param name="from">Line's starting position</param>
    /// <param name="to">Line's ending position</param>
    public static void DrawLine(Vector3 from, Vector3 to)
    {
        DrawLine(from, to, GameGizmos.I.DefaultColor);
    }
    /// <summary>
    /// Draws a line "from" to "to" with color
    /// </summary>
    /// <param name="from">Line's starting position</param>
    /// <param name="to">Line's ending position</param>
    /// <param name="color">Color of the line</param>
    public static void DrawLine(Vector3 from, Vector3 to, Color32 color)
    {
        GameGizmos.I.PutListVerts.Clear(); GameGizmos.I.PutListVerts.Add(from); GameGizmos.I.PutListVerts.Add(to);
        GameGizmos.I.PutListColors32.Clear(); GameGizmos.I.PutListColors32.Add(color); GameGizmos.I.PutListColors32.Add(color);
        GameGizmos.I.Put(GameGizmos.I.PutListVerts, GameGizmos.I.PutListColors32);
    }

    public static void DrawLine(List<Vector3> points, bool loop = false)
    {
        DrawLine(points, GameGizmos.I.DefaultColor, loop);
    }

    public static void DrawLine(List<Vector3> points, Color32 color, bool loop = false)
    {
        List<Color32> colors = new List<Color32>();
        points.ForEach(x => colors.Add(color));
        GameGizmos.I.Put(points, colors, loop);
    }

    #endregion

    #region RECTANGLES
    public static void DrawRect(Vector3[] points, Color32 color)
    {
        //Vector3[] points = new Vector3[4];
        //points[0] = new Vector3(pos.x + size.x / 2, pos.y, pos.z + size.y / 2);
        //points[1] = new Vector3(pos.x + size.x / 2, pos.y, pos.z - size.y / 2);
        //points[2] = new Vector3(pos.x - size.x / 2, pos.y, pos.z + size.y / 2);
        //points[3] = new Vector3(pos.x - size.x / 2, pos.y, pos.z - size.y / 2);

        //points[0] = RotatePoint(points[0], pos, angle);
        //points[1] = RotatePoint(points[1], pos, angle);
        //points[2] = RotatePoint(points[2], pos, angle);
        //points[3] = RotatePoint(points[3], pos, angle);

        DrawLine(new List<Vector3>() { points[0], points[1], points[2], points[3] }, color, true);
    }

    public static void DrawRect(Vector3 pos, Vector3 size, Vector3 angle, Color32 color)
    {
        Vector3[] points = new Vector3[4];
        points[0] = new Vector3(pos.x + size.x / 2, pos.y, pos.z + size.y / 2);
        points[1] = new Vector3(pos.x + size.x / 2, pos.y, pos.z - size.y / 2);
        points[2] = new Vector3(pos.x - size.x / 2, pos.y, pos.z + size.y / 2);
        points[3] = new Vector3(pos.x - size.x / 2, pos.y, pos.z - size.y / 2);

        points[0] = RotatePoint(points[0], pos, angle);
        points[1] = RotatePoint(points[1], pos, angle);
        points[2] = RotatePoint(points[2], pos, angle);
        points[3] = RotatePoint(points[3], pos, angle);

        //DrawLine(points[1], points[2]);
        DrawLine(new List<Vector3>() { points[0], points[1], points[3], points[2] }, color, true);
    }
    #endregion

    #region CUBES
    public static void DrawCube(Vector3 pos, Vector3 size, Vector3 angle, Color32 color)
    {
        Vector3[] points = new Vector3[8];  // 8 points for a cube
        points[0] = new Vector3(pos.x + size.x / 2, pos.y + size.y / 2, pos.z + size.z / 2);
        points[1] = new Vector3(pos.x + size.x / 2, pos.y + size.y / 2, pos.z - size.z / 2);
        points[2] = new Vector3(pos.x - size.x / 2, pos.y + size.y / 2, pos.z + size.z / 2);
        points[3] = new Vector3(pos.x - size.x / 2, pos.y + size.y / 2, pos.z - size.z / 2);

        points[4] = new Vector3(pos.x + size.x / 2, pos.y - size.y / 2, pos.z + size.z / 2);
        points[5] = new Vector3(pos.x + size.x / 2, pos.y - size.y / 2, pos.z - size.z / 2);
        points[6] = new Vector3(pos.x - size.x / 2, pos.y - size.y / 2, pos.z + size.z / 2);
        points[7] = new Vector3(pos.x - size.x / 2, pos.y - size.y / 2, pos.z - size.z / 2);

        // Rotate the points based on angle
        points[0] = RotatePoint(points[0], pos, angle);
        points[1] = RotatePoint(points[1], pos, angle);
        points[2] = RotatePoint(points[2], pos, angle);
        points[3] = RotatePoint(points[3], pos, angle);
        points[4] = RotatePoint(points[4], pos, angle);
        points[5] = RotatePoint(points[5], pos, angle);
        points[6] = RotatePoint(points[6], pos, angle);
        points[7] = RotatePoint(points[7], pos, angle);

        // Draw the lines
        //DrawLine(points[0], points[1], color);
        //DrawLine(points[0], points[2], color);
        //DrawLine(points[2], points[3], color);
        //DrawLine(points[3], points[1], color);
        DrawRect(new[] { points[0], points[1], points[3], points[2] }, color);


        //DrawLine(points[4], points[5], color);
        //DrawLine(points[4], points[6], color);
        //DrawLine(points[6], points[7], color);
        //DrawLine(points[7], points[5], color);

        DrawRect(new[] { points[4], points[5], points[7], points[6] }, color);

        DrawLine(points[0], points[4], color);
        DrawLine(points[1], points[5], color);
        DrawLine(points[2], points[6], color);
        DrawLine(points[3], points[7], color);

    }

    #endregion

    #region CIRCLES

    public static void DrawCircle(Vector3 pos, float diameter, Vector3 angle, Color32 color, int detail = 8)
    {
        float step = 360f/detail;
        Vector3[] points = new Vector3[detail];
        for (var i = 0; i < detail; i++)
        {
            points[i] = RotatePoint(new Vector3(pos.x, pos.y, pos.z + (diameter/2)), pos, new Vector3(0, step*i, 0));
            points[i] = RotatePoint(points[i], pos, angle);
        }
        DrawLine(points.ToList(), color, true);
    }

    #endregion

    #region SPHERES
    public static void DrawSphere(Vector3 pos, float diameter, Vector3 angle, Color32 color, int detail = 8)
    {
        Vector3 n = Quaternion.Euler(angle) * Vector3.forward;  //normal direction of angle for 3rd axis
        DrawCircle(pos, diameter, angle, color, detail);
        DrawCircle(pos, diameter, new Vector3(angle.x, angle.y, angle.z + 90), color, detail);
        DrawCircle(pos, diameter, Quaternion.FromToRotation(Vector3.up, n).eulerAngles, color, detail);



    }

    #endregion

    #region TOOLS
    /// <summary>
    /// Rotates a vector point in space around pivot by angles
    /// </summary>
    /// <param name="point">The point in space to rotate</param>
    /// <param name="pivot">Pivot/center to rotate around</param>
    /// <param name="angle">Angle by which to rotate</param>
    /// <returns></returns>
    private static Vector3 RotatePoint(Vector3 point, Vector3 pivot, Vector3 angle)
    {
        var dir = point - pivot;
        dir = Quaternion.Euler(angle) * dir;
        point = dir + pivot;
        return point;
    }
    #endregion

    // Add:
    // *Diamond
    // *Dome
    // *Grid
    // *Frustrum
    // *Arrow
}