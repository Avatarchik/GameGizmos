using UnityEngine;
using System.Collections.Generic;

// GameGizmos.cs
// https://github.com/SlateNeon/GameGizmos
// Last Edit: 2015-03-05

public class GameGizmos : MonoBehaviour {
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
        _mRenderer.material = new Material(Shader.Find("Unlit/VertexColor"));
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
    public void Put(IEnumerable<Vector3> verts, IEnumerable<Color32> colors)
    {
        foreach (var v in verts)
        {
            _dVerts.Add(v);
            _dIndices.Add(_dIndices.Count);
        }
        foreach (var c in colors)
        {
            _dColors32.Add(c);
        }
    }

    #region Unity Callbacks
    void Awake()
    {
        Init();
    }
    void OnWillRenderObject()
    {
        PopulateMesh();
        ZeroTransform();
    }
    void OnRenderObject()
    {
        ClearEverything(); //clears data after render is complete
    }
    #endregion

    /// <summary>
    /// Rotates a point in space around pivot by angles
    /// </summary>
    /// <param name="point">Point's original location</param>
    /// <param name="pivot">Center pivot around which to rotate</param>
    /// <param name="angle">Rotation angles in XYZ</param>
    /// <returns>New position of a vector after rotation</returns>
    public static Vector3 RotatePoint(Vector3 point, Vector3 pivot, Vector3 angle)
    {
        var dir = point - pivot;
        dir = Quaternion.Euler(angle) * dir;
        point = dir + pivot;
        return point;
    }

    /// <summary>
    /// Draws a line "from" to "to" with default color
    /// </summary>
    /// <param name="from">Line's starting position</param>
    /// <param name="to">Line's ending position</param>
    public static void DrawLine(Vector3 from, Vector3 to)
    {
        DrawLine(from, to, I.DefaultColor);
    }
    /// <summary>
    /// Draws a line "from" to "to" with color
    /// </summary>
    /// <param name="from">Line's starting position</param>
    /// <param name="to">Line's ending position</param>
    /// <param name="color">Color of the line</param>
    public static void DrawLine(Vector3 from, Vector3 to, Color32 color)
    {
        I.PutListVerts.Clear(); I.PutListVerts.Add(from); I.PutListVerts.Add(to);
        I.PutListColors32.Clear(); I.PutListColors32.Add(color); I.PutListColors32.Add(color);
        I.Put(I.PutListVerts, I.PutListColors32);
    }
    
    /// <summary>
    /// Draws a rectangle at position with provided size
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="size">Size of the rectangle</param>
    public static void DrawRect(Vector3 pos, Vector3 size)
    {
        DrawRect(pos, size, I.DefaultColor, Vector3.zero);
    }
    /// <summary>
    /// Draws a rectangle at position with provided size and color
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="size">Size of the rectangle</param>
    /// <param name="color">Color</param>
    public static void DrawRect(Vector3 pos, Vector3 size, Color32 color)
    {
        DrawRect(pos, size, color, Vector3.zero);
    }
    /// <summary>
    /// Draws a rectangle at position with provided size, color and angle
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="size">Size of the rectangle</param>
    /// <param name="color">Color</param>
    /// <param name="angle">Angle</param>
    public static void DrawRect(Vector3 pos, Vector2 size, Color32 color, Vector3 angle)
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

        DrawLine(points[0], points[1], color);
        DrawLine(points[0], points[2], color);
        DrawLine(points[2], points[3], color);
        DrawLine(points[3], points[1], color);
    }

    /// <summary>
    /// Draws a cube at pos with provided size
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="size">Size</param>
    public static void DrawCube(Vector3 pos, Vector3 size)
    {
        DrawCube(pos, size, I.DefaultColor, Vector3.zero);
    }
    /// <summary>
    /// Draws a cube at pos with provided size and color
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="size">Size</param>
    /// <param name="color">Color</param>
    public static void DrawCube(Vector3 pos, Vector3 size, Color32 color)
    {
        DrawCube(pos, size, color, Vector3.zero);
    }
    /// <summary>
    /// Draws a cube at pos with provided size, color and angle
    /// </summary>
    /// <param name="pos">Position</param>
    /// <param name="size">Size</param>
    /// <param name="color">Color</param>
    /// <param name="angle">Angle</param>
    public static void DrawCube(Vector3 pos, Vector3 size, Color32 color, Vector3 angle)
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
        DrawLine(points[0], points[1], color);
        DrawLine(points[0], points[2], color);
        DrawLine(points[2], points[3], color);
        DrawLine(points[3], points[1], color);

        DrawLine(points[4], points[5], color);
        DrawLine(points[4], points[6], color);
        DrawLine(points[6], points[7], color);
        DrawLine(points[7], points[5], color);

        DrawLine(points[0], points[4], color);
        DrawLine(points[1], points[5], color);
        DrawLine(points[2], points[6], color);
        DrawLine(points[3], points[7], color);

    }

    /// <summary>
    /// Draws a circle that is lying flat on ground plane by default
    /// </summary>
    /// <param name="pos">Position in world space</param>
    /// <param name="diameter">Diameter of the circle</param>
    /// <param name="color">Color of the circle</param>
    /// <param name="angle">Euler angles rotation</param>
    /// <param name="detail">Amount of detail in the circle, more makes it smoother</param>
    public static void DrawCircle(Vector3 pos, float diameter, Color32 color, Vector3 angle, int detail = 8)
    {
        float step = 360f / detail;
        Vector3[] points = new Vector3[detail];
        for (var i = 0; i < detail; i++)
        {
            points[i] = RotatePoint(new Vector3(pos.x, pos.y, pos.z + (diameter/2)), pos, new Vector3(0, step * i, 0));
            points[i] = RotatePoint(points[i], pos, angle);
            if (i > 0 && i < detail - 1)
            {
                DrawLine(points[i - 1], points[i], color);
            }
            else if (i == detail - 1)
            {
                DrawLine(points[i], points[0], color);
                DrawLine(points[i-1], points[i], color);
            }

        }
    }

    /// <summary>
    /// Draws a sphere
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="diameter"></param>
    /// <param name="color"></param>
    /// <param name="angle"></param>
    /// <param name="detail"></param>
    public static void DrawSphere(Vector3 pos, float diameter, Color32 color, Vector3 angle, int detail = 8)
    {
        Vector3 n = Quaternion.Euler(angle) * Vector3.forward;  //normal direction of angle for 3rd axis
        DrawCircle(pos, diameter, color, angle, detail);
        DrawCircle(pos, diameter, color, new Vector3(angle.x, angle.y, angle.z+90), detail);
        DrawCircle(pos, diameter, color, Quaternion.FromToRotation(Vector3.up, n).eulerAngles, detail);

        

    }
}