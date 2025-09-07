using UnityEngine;

[ExecuteAlways]
public class RoomBuilder : MonoBehaviour
{
    [Header("Room Size (centered at origin)")]
    public Vector3 roomSize = new Vector3(20f, 10f, 20f); // width (x), height (y), depth (z)

    [Header("Thickness / Colliders")]
    public float wallThickness = 0.2f;

    [Header("Materials")]
    public Material floorMat;
    public Material floorOverlayMat; 
    public Material wallMat;
    public Material ceilingMat;

    [Header("Options")]
    public bool includeCeiling = true;
    public bool showGizmos = true;

    // children
    Transform floor, floorGrid, ceiling, wallN, wallS, wallE, wallW;

    void OnEnable() { Build(); }
    void OnValidate() { Build(); }

    public void Build()
    {
        // min room size
        if (roomSize.x < 10f) roomSize.x = 10f;
        if (roomSize.y < 10f) roomSize.y = 10f;
        if (roomSize.z < 10f) roomSize.z = 10f;

        floor   = CreateOrGet("Floor");
        floorGrid = CreateOrGet("Floor_Grid");
        ceiling = CreateOrGet("Ceiling");
        wallN   = CreateOrGet("Wall_North");
        wallS   = CreateOrGet("Wall_South");
        wallE   = CreateOrGet("Wall_East");
        wallW   = CreateOrGet("Wall_West");

        // Floor
        SetupPanel(floor, new Vector3(0, 0, 0), new Vector3(roomSize.x, wallThickness, roomSize.z), floorMat, true);
        if (floorOverlayMat) 
        {
            floorGrid.gameObject.SetActive(true);
            SetupPanel(floorGrid, new Vector3(0, (wallThickness/2)+0.001f, 0), new Vector3(roomSize.x, 0.0008f, roomSize.z), floorOverlayMat, false);
        } else
            floorGrid.gameObject.SetActive(false);

        // Ceiling
        ceiling.gameObject.SetActive(includeCeiling);
        if (includeCeiling)
            SetupPanel(ceiling, new Vector3(0, roomSize.y, 0), new Vector3(roomSize.x, wallThickness, roomSize.z), ceilingMat, true);

        // North & South walls (along X, at +/- Z/2)
        float halfZ = roomSize.z / 2f;
        SetupPanel(wallN, new Vector3(0, roomSize.y / 2f,  halfZ), new Vector3(roomSize.x, roomSize.y, wallThickness), wallMat, true);
        SetupPanel(wallS, new Vector3(0, roomSize.y / 2f, -halfZ), new Vector3(roomSize.x, roomSize.y, wallThickness), wallMat, true);

        // East & West walls (along Z, at +/- X/2)
        float halfX = roomSize.x / 2f;
        SetupPanel(wallE, new Vector3( halfX, roomSize.y / 2f, 0), new Vector3(wallThickness, roomSize.y, roomSize.z), wallMat, true);
        SetupPanel(wallW, new Vector3(-halfX, roomSize.y / 2f, 0), new Vector3(wallThickness, roomSize.y, roomSize.z), wallMat, true);
    }

    // Helper func to find or create a child obj for the boundaries of the room
    Transform CreateOrGet(string name)
    {
        var t = transform.Find(name);
        if (!t)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(transform, false);
            t = go.transform;
        }
        return t;
    }

    // Helper func to set up position, scale, material, and collider for a panel
    void SetupPanel(Transform t, Vector3 localPos, Vector3 scale, Material mat, bool collider)
    {
        t.SetLocalPositionAndRotation(localPos, Quaternion.identity);
        t.localScale = scale;

        var rend = t.GetComponent<MeshRenderer>();
        if (rend && mat) rend.sharedMaterial = mat;

        var col = t.GetComponent<BoxCollider>();
        if (!col && collider) col = t.gameObject.AddComponent<BoxCollider>();
        if (col) col.size = Vector3.one; // uses object scale

        // Remove MeshCollider (if primitive created one) — BoxCollider is cheaper
        var mc = t.GetComponent<MeshCollider>();
        if (mc) DestroyImmediate(mc);
    }

    // Draw a wireframe box in the editor to visualize the room size
    void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.1f);
        Gizmos.DrawCube(transform.position + new Vector3(0, roomSize.y/2f, 0), roomSize);
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 1f);
        Gizmos.DrawWireCube(transform.position + new Vector3(0, roomSize.y/2f, 0), roomSize);
    }
}
