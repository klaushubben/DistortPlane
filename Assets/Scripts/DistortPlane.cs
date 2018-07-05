using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class DistortPlane : MonoBehaviour
{
    public Vector3[] Corners
    {
        get { return m_Corners; }
        set { m_Corners = value; }
    }

    [SerializeField]
    Vector3[] m_Corners = new Vector3[4];

    [Range(1, 20)]
    public int hSegments = 2;
    [Range(1, 20)]
    public int vSegments = 2;

    public bool UseTextureForInitialPosition;
    public bool ResetCorners = true;
    public bool ShowBounds = true;

    Mesh mesh;
    int _hSeg;
    int _vSeg;
    Vector3[] normPoints;
    Vector3[] distortPoints;
    bool bMeshInited;

    void CalculateMesh()
    {
        int hCount2 = _hSeg + 1;
        int vCount2 = _vSeg + 1;
        int numTri = _hSeg * _vSeg * 6;
        int numVert = hCount2 * vCount2;

        // create "normalized" points: x,y between 0 -> 1
        normPoints = new Vector3[numVert];
        Vector2[] uvs = new Vector2[numVert];
        Vector4[] tangents = new Vector4[numVert];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        int index = 0;
        float scaleX = 1.0f / _hSeg;
        float scaleY = 1.0f / _vSeg;
        for (float y = 0.0f; y < vCount2; y++)
        {
            for (float x = 0.0f; x < hCount2; x++)
            {
                normPoints[index] = new Vector3(x * scaleX, y * scaleY, 0.0f);
                tangents[index] = tangent;
                uvs[index++] = new Vector2(x * scaleX, y * scaleY);
            }
        }

        // Triangles
        index = 0;
        int[] tri = new int[numTri];
        for (int y = 0; y < _vSeg; y++)
        {
            for (int x = 0; x < _hSeg; x++)
            {
                tri[index] = (y * hCount2) + x;
                tri[index + 1] = ((y + 1) * hCount2) + x;
                tri[index + 2] = (y * hCount2) + x + 1;

                tri[index + 3] = ((y + 1) * hCount2) + x;
                tri[index + 4] = ((y + 1) * hCount2) + x + 1;
                tri[index + 5] = (y * hCount2) + x + 1;
                index += 6;
            }
        }

        mesh = new Mesh
        {
            name = "DistortPlane_Mesh",
            vertices = normPoints, // temporarily
            triangles = tri,
            uv = uvs,
            tangents = tangents,
        };
        mesh.RecalculateNormals();

        bMeshInited = true;
    }

    void Distort()
    {
        if (!bMeshInited) CalculateMesh();

        distortPoints = new Vector3[normPoints.Length];

        float c0, c1;
        Vector3 temp = Vector3.zero;

        //corner order: BotL, BotR, TopL, TopR
        for (int i = 0; i < normPoints.Length; i++)
        {
            /* TO LERP OR NOT TO LERP, WHAT IS THE DIFF? ;) */
            // X
            c0 = m_Corners[0].x + (normPoints[i].x * (m_Corners[1].x - m_Corners[0].x));
            c1 = m_Corners[2].x + (normPoints[i].x * (m_Corners[3].x - m_Corners[2].x));
            distortPoints[i].x = Mathf.Lerp(c0, c1, normPoints[i].y);

            // Y
            c0 = m_Corners[0].y + (normPoints[i].y * (m_Corners[2].y - m_Corners[0].y));
            c1 = m_Corners[1].y + (normPoints[i].y * (m_Corners[3].y - m_Corners[1].y));
            distortPoints[i].y = Mathf.Lerp(c0, c1, normPoints[i].x);

            // Z
            c0 = m_Corners[0].z + (normPoints[i].x * (m_Corners[1].z - m_Corners[0].z));
            c1 = m_Corners[2].z + (normPoints[i].x * (m_Corners[3].z - m_Corners[2].z));
            distortPoints[i].z = Mathf.Lerp(c0, c1, normPoints[i].y);
        }

        mesh.vertices = distortPoints;
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.RecalculateBounds();
    }


    void ResetCornerPosition()
    {
        float halfW = 0.5f;
        float halfH = 0.5f;

        if (UseTextureForInitialPosition)
        {
            Texture texture = GetComponent<MeshRenderer>().sharedMaterial.GetTexture("_MainTex");
            if (texture != null)
            {
                float w = texture.width;
                float h = texture.height;

                if (w > h)
                {
                    halfW = 0.5f;
                    halfH = (h / w) * 0.5f;
                }
                else
                {
                    halfH = 0.5f;
                    halfW = (texture.width / texture.height) * 0.5f;
                }
            }
        }

        m_Corners[0] = new Vector3(-halfW, -halfH, 0f);
        m_Corners[1] = new Vector3(halfW, -halfH, 0f);
        m_Corners[2] = new Vector3(-halfW, halfH, 0f);
        m_Corners[3] = new Vector3(halfW, halfH, 0f);
    }

    public virtual void UpdateCorners()
    {
        Distort();
    }

    void Update()
    { 
        if (ResetCorners)
        {
            ResetCornerPosition();
            Distort();
            ResetCorners = false;
        }

        if (_hSeg != hSegments || _vSeg != vSegments)
        {
            _hSeg = hSegments;
            _vSeg = vSegments;
            CalculateMesh();
            Distort();
        }
    }

    void OnDrawGizmos()
    {
        if (ShowBounds)
        {
            Renderer r = GetComponent<Renderer>();
            if (r)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(r.bounds.center, r.bounds.size);
            }
        } 
    } 
}
