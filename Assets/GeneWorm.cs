using UnityEngine;
using System.Collections;

public enum CubeType
{
    Hard = 1,
    Soft,
    Pulse,
    AntiPulse,
    BottomBend,
    AntiBottomBend,
    TopBend,
    AntiTopBend,
    COUNT
}

/* IDEAS:
    - could use cube count as Rigidbody mass
*/
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GeneWorm : MonoBehaviour
{
    public CubeType[] cubes;
    public float cubeSize = 1.0f;
    public float expansionRatio = 0.5f;
    public float expansionPeriod = 0.5f;

    WormCube[] wormCubes;
    Vector3[] m_vertices;
    float m_timer;
    Mesh m_mesh;
    Mesh[] m_colliderMeshes;
    MeshCollider[] m_meshColliders;
    MeshFilter m_meshFilter;

    // Use this for initialization
    void Start()
    {
        GetComponent<Rigidbody>().mass = cubes.Length;

        wormCubes = new WormCube[cubes.Length];
        for (int i = 0; i < cubes.Length; ++i)
        {
            switch (cubes[i])
            {
                case CubeType.Hard: wormCubes[i] = new HardWormCube(); break;
                case CubeType.Soft: wormCubes[i] = new SoftWormCube(); break;
                case CubeType.Pulse: wormCubes[i] = new PulseWormCube(); break;
                case CubeType.AntiPulse:
                    {
                        wormCubes[i] = new PulseWormCube();
                        wormCubes[i].SetInvertedPhase(true);
                    }
                    break;
                case CubeType.BottomBend: wormCubes[i] = new BottomBendWormCube(); break;
                case CubeType.AntiBottomBend:
                    {
                        wormCubes[i] = new BottomBendWormCube();
                        wormCubes[i].SetInvertedPhase(true);
                    }
                    break;
                case CubeType.TopBend: wormCubes[i] = new TopBendWormCube(); break;
                case CubeType.AntiTopBend:
                    {
                        wormCubes[i] = new TopBendWormCube();
                        wormCubes[i].SetInvertedPhase(true);
                    }
                    break;
            }
        }

        m_vertices = new Vector3[4 * (cubes.Length + 1)];
        m_timer = 0.0f;
        m_mesh = new Mesh();
        m_colliderMeshes = new Mesh[cubes.Length];
        m_meshColliders = new MeshCollider[cubes.Length];
        for (int i = 0; i < cubes.Length; ++i)
        {
            m_colliderMeshes[i] = new Mesh();
            m_meshColliders[i] = gameObject.AddComponent<MeshCollider>();
        }
        m_meshFilter = GetComponent<MeshFilter>();

        // VERTICES
        for (int i = 0; i < cubes.Length + 1; ++i)
        {
            m_vertices[i * 4 + 0] = new Vector3(i * cubeSize, cubeSize * 0.5f, cubeSize * 0.5f);
            m_vertices[i * 4 + 1] = new Vector3(i * cubeSize, -cubeSize * 0.5f, cubeSize * 0.5f);
            m_vertices[i * 4 + 2] = new Vector3(i * cubeSize, -cubeSize * 0.5f, -cubeSize * 0.5f);
            m_vertices[i * 4 + 3] = new Vector3(i * cubeSize, cubeSize * 0.5f, -cubeSize * 0.5f);
        }
        m_mesh.vertices = m_vertices;

        // INDICES
        int[] indices = new int[6 * (4 * (cubes.Length + 2))];
        // FRONT
        indices[0] = 0; indices[1] = 2; indices[2] = 1;
        indices[3] = 0; indices[4] = 3; indices[5] = 2;
        // BACK
        indices[6 * (4 * (cubes.Length + 1)) + 0] = cubes.Length * 4 + 0; indices[6 * (4 * (cubes.Length + 1)) + 1] = cubes.Length * 4 + 1; indices[6 * (4 * (cubes.Length + 1)) + 2] = cubes.Length * 4 + 2;
        indices[6 * (4 * (cubes.Length + 1)) + 3] = cubes.Length * 4 + 0; indices[6 * (4 * (cubes.Length + 1)) + 4] = cubes.Length * 4 + 2; indices[6 * (4 * (cubes.Length + 1)) + 5] = cubes.Length * 4 + 3;
        for (int i = 0; i < cubes.Length; ++i)
        {
            int indicesStart = 6 * (1 + (i * 4));
            int firstRowIndex = i * 4;
            int secondRowIndex = (i + 1) * 4;
            // TOP
            indices[indicesStart + 0] = firstRowIndex + 0; indices[indicesStart + 1] = secondRowIndex + 0; indices[indicesStart + 2] = secondRowIndex + 3;
            indices[indicesStart + 3] = firstRowIndex + 0; indices[indicesStart + 4] = secondRowIndex + 3; indices[indicesStart + 5] = firstRowIndex + 3;
            // LEFT
            indices[indicesStart + 6] = firstRowIndex + 0; indices[indicesStart + 7] = secondRowIndex + 1; indices[indicesStart + 8] = secondRowIndex + 0;
            indices[indicesStart + 9] = firstRowIndex + 0; indices[indicesStart + 10] = firstRowIndex + 1; indices[indicesStart + 11] = secondRowIndex + 1;
            // RIGHT
            indices[indicesStart + 12] = firstRowIndex + 3; indices[indicesStart + 13] = secondRowIndex + 3; indices[indicesStart + 14] = secondRowIndex + 2;
            indices[indicesStart + 15] = firstRowIndex + 3; indices[indicesStart + 16] = secondRowIndex + 2; indices[indicesStart + 17] = firstRowIndex + 2;
            // BOTTOM
            indices[indicesStart + 18] = firstRowIndex + 1; indices[indicesStart + 19] = secondRowIndex + 2; indices[indicesStart + 20] = secondRowIndex + 1;
            indices[indicesStart + 21] = firstRowIndex + 1; indices[indicesStart + 22] = firstRowIndex + 2; indices[indicesStart + 23] = secondRowIndex + 2;
        }
        m_mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        // COLLIDERS MESHES
        for (int i = 0; i < cubes.Length; ++i)
        {
            Vector3[] vertices = new Vector3[8];
            for (int j = 0; j < 8; ++j)
            {
                vertices[j] = m_vertices[i * 4 + j];
            }

            m_colliderMeshes[i].vertices = vertices;
            int[] colliderIndices = new int[36];

            // TOP
            colliderIndices[0] = 0; colliderIndices[1] = 4; colliderIndices[2] = 7;
            colliderIndices[3] = 0; colliderIndices[4] = 7; colliderIndices[5] = 3;
            // LEFT
            colliderIndices[6] = 0; colliderIndices[7] = 5; colliderIndices[8] = 4;
            colliderIndices[9] = 0; colliderIndices[10] = 1; colliderIndices[11] = 5;
            // RIGHT
            colliderIndices[12] = 3; colliderIndices[13] = 7; colliderIndices[14] = 6;
            colliderIndices[15] = 3; colliderIndices[16] = 6; colliderIndices[17] = 2;
            // BOTTOM
            colliderIndices[18] = 1; colliderIndices[19] = 6; colliderIndices[20] = 5;
            colliderIndices[21] = 1; colliderIndices[22] = 2; colliderIndices[23] = 6;
            // FRONT
            colliderIndices[24] = 0; colliderIndices[25] = 2; colliderIndices[26] = 1;
            colliderIndices[27] = 0; colliderIndices[28] = 3; colliderIndices[29] = 2;
            // BACK
            colliderIndices[30] = 4; colliderIndices[31] = 5; colliderIndices[32] = 6;
            colliderIndices[33] = 4; colliderIndices[34] = 6; colliderIndices[35] = 7;

            m_colliderMeshes[i].SetIndices(colliderIndices, MeshTopology.Triangles, 0);
            m_meshColliders[i].sharedMesh = m_colliderMeshes[i];
            m_meshColliders[i].convex = true;
        }

        m_mesh.name = "Bonjour";
        m_meshFilter.mesh = m_mesh;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < wormCubes.Length; ++i)
        {
            // We assume that a face of the cube is always a plane.
            Plane basePlane = new Plane(
                m_vertices[(i * 4) + 0],
                m_vertices[(i * 4) + 1],
                m_vertices[(i * 4) + 2]
            );
            Vector3 origin = (m_vertices[(i * 4) + 0] + m_vertices[(i * 4) + 1] + m_vertices[(i * 4) + 2] + m_vertices[(i * 4) + 3]) / 4.0f;
            Quaternion rotation = Quaternion.FromToRotation(new Vector3(1.0f, 0.0f, 0.0f), basePlane.normal);
            Vector3[] outVertices;
            wormCubes[i].CalculateVertices(cubeSize, expansionPeriod, cubeSize * 0.5f * expansionRatio, m_timer, out outVertices);
            for (int j = 0; j < 8; ++j)
            {
                outVertices[j] = (rotation * outVertices[j]) + origin;
            }

            WormCube.DeformationType currentDeformation = wormCubes[i].GetDeformationType();
            WormCube.DeformationType previousDeformation = WormCube.DeformationType.Slave;
            if (i > 0) previousDeformation = wormCubes[i - 1].GetDeformationType();

            if (
                (i == 0) ||
                (currentDeformation == WormCube.DeformationType.Master && previousDeformation != WormCube.DeformationType.Master) ||
                (previousDeformation == WormCube.DeformationType.Slave && currentDeformation != WormCube.DeformationType.Slave)
            )
            {
                for (int j = 0; j < 8; ++j)
                {
                    m_vertices[(i * 4) + j] = outVertices[j];
                }
            }
            else if (
                (previousDeformation == WormCube.DeformationType.Master && currentDeformation != WormCube.DeformationType.Master) ||
                (currentDeformation == WormCube.DeformationType.Slave && previousDeformation != WormCube.DeformationType.Slave)
            )
            {
                for (int j = 4; j < 8; ++j)
                {
                    m_vertices[(i * 4) + j] = outVertices[j];
                }
            }
            else
            {
                for (int j = 0; j < 4; ++j)
                {
                    m_vertices[(i * 4) + j] = (m_vertices[(i * 4) + j] + outVertices[j]) / 2.0f;
                }
                for (int j = 4; j < 8; ++j)
                {
                    m_vertices[(i * 4) + j] = outVertices[j];
                }
            }
        }

        m_mesh.vertices = m_vertices;
        for (int i = 0; i < cubes.Length; ++i)
        {
            Vector3[] vertices = new Vector3[8];
            for (int j = 0; j < 8; ++j)
            {
                vertices[j] = m_vertices[i * 4 + j];
            }

            m_colliderMeshes[i].vertices = vertices;
            m_meshColliders[i].sharedMesh = m_colliderMeshes[i];
        }

        m_timer += Time.fixedDeltaTime;
    }
}
