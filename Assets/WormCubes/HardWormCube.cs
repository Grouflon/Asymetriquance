using UnityEngine;
using System.Collections;

public class HardWormCube : WormCube
{
    public override void CalculateVertices(float _size, float _period, float _amplitude, float _t, out Vector3[] _vertices)
    {
        float halfSize = _size * 0.5f;
        _vertices = new Vector3[8];
        _vertices[0] = new Vector3(0.0f, halfSize, halfSize);
        _vertices[1] = new Vector3(0.0f, -halfSize, halfSize);
        _vertices[2] = new Vector3(0.0f, -halfSize, -halfSize);
        _vertices[3] = new Vector3(0.0f, halfSize, -halfSize);
        _vertices[4] = new Vector3(_size, halfSize, halfSize);
        _vertices[5] = new Vector3(_size, -halfSize, halfSize);
        _vertices[6] = new Vector3(_size, -halfSize, -halfSize);
        _vertices[7] = new Vector3(_size, halfSize, -halfSize);
    }

    public override DeformationType GetDeformationType()
    {
        return DeformationType.Master;
    }
}
