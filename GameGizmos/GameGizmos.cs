using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// GameGizmos.cs
// http://slateneon.github.io/
// 20150227

public class GameGizmos : MonoBehaviour {

    private static GameGizmos _I;
    public static GameGizmos I {
        get {
            if (_I == null) {
                _I = GameObject.FindObjectOfType<GameGizmos>();
                if (_I == null) {
                    _I = (new GameObject("GameGizmos", typeof(GameGizmos))).GetComponent<GameGizmos>();
                }
            }
            return _I;
        }
        private set { _I = value; }
    }

    public Color defaultColor = Color.white;

    #region Private
    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private List<Vector3> dVerts = new List<Vector3>();
    private List<int> dIndices = new List<int>();
    private List<Color32> dColors32 = new List<Color32>();
    #endregion

    #region MonoBehaviour
    void Awake() {
        Init();
    }
    void OnWillRenderObject() {
        PopulateMesh();
        ZeroTransform();
    }
    void OnRenderObject() {
        ClearEverything();
    }
    #endregion

    void Init() {
        transform.name = "GameGizmos";
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mesh = new Mesh() { name = "GameGizmos_Mesh" };
        meshFilter.mesh = mesh;
        mesh.MarkDynamic();
        meshRenderer.material = new Material(Shader.Find("Unlit/VertexColor"));
    }
    void PopulateMesh() {
        mesh.vertices = dVerts.ToArray();
        mesh.SetIndices(dIndices.ToArray(), MeshTopology.Lines, 0);
        mesh.colors32 = dColors32.ToArray();
    }
    void ClearEverything() {
        mesh.Clear();
        dVerts.Clear();
        dIndices.Clear();
        dColors32.Clear();
    }
    void ZeroTransform() {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
    void Put(IEnumerable<Vector3> verts, IEnumerable<Color32> colors)
    {
        dVerts = dVerts.Concat(verts).ToList();
        for (int i = 0; i < verts.Count(); i++)
        {
            dIndices.Add(dIndices.Count);
        }
        dColors32 = dColors32.Concat(colors).ToList();
    }
    static Vector3 RotatePoint(Vector3 point, Vector3 pivot, Vector3 angle)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angle) * dir;
        point = dir + pivot;
        return point;
    }

    #region Draw
    public static void DrawLine(Vector3 from, Vector3 to) {
        DrawLine(from, to, I.defaultColor);
    }
    public static void DrawLine(Vector3 from, Vector3 to, Color32 color) {
        I.Put(new List<Vector3>() { from, to }, new List<Color32>() {color, color });
    }

    public static void DrawRect(Vector3 pos, Vector3 size) {
        DrawRect(pos, size, I.defaultColor, Vector3.zero);
    }
    public static void DrawRect(Vector3 pos, Vector3 size, Color32 color) {
        DrawRect(pos, size, color, Vector3.zero);
    }
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

    public static void DrawCube(Vector3 pos, Vector3 size) {
        DrawCube(pos, size, I.defaultColor, Vector3.zero);
    }
    public static void DrawCube(Vector3 pos, Vector3 size, Color32 color) {
        DrawCube(pos, size, color, Vector3.zero);
    }
    public static void DrawCube(Vector3 pos, Vector3 size, Color32 color, Vector3 angle) {
        Vector3[] points = new Vector3[8];
        points[0] = new Vector3(pos.x + size.x / 2, pos.y + size.y / 2, pos.z + size.z / 2);
        points[1] = new Vector3(pos.x + size.x / 2, pos.y + size.y / 2, pos.z - size.z / 2);
        points[2] = new Vector3(pos.x - size.x / 2, pos.y + size.y / 2, pos.z + size.z / 2);
        points[3] = new Vector3(pos.x - size.x / 2, pos.y + size.y / 2, pos.z - size.z / 2);

        points[4] = new Vector3(pos.x + size.x / 2, pos.y - size.y / 2, pos.z + size.z / 2);
        points[5] = new Vector3(pos.x + size.x / 2, pos.y - size.y / 2, pos.z - size.z / 2);
        points[6] = new Vector3(pos.x - size.x / 2, pos.y - size.y / 2, pos.z + size.z / 2);
        points[7] = new Vector3(pos.x - size.x / 2, pos.y - size.y / 2, pos.z - size.z / 2);

        points[0] = RotatePoint(points[0], pos, angle);
        points[1] = RotatePoint(points[1], pos, angle);
        points[2] = RotatePoint(points[2], pos, angle);
        points[3] = RotatePoint(points[3], pos, angle);
        points[4] = RotatePoint(points[4], pos, angle);
        points[5] = RotatePoint(points[5], pos, angle);
        points[6] = RotatePoint(points[6], pos, angle);
        points[7] = RotatePoint(points[7], pos, angle);

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
    #endregion
}
