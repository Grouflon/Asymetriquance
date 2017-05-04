using UnityEngine;
using System.Collections;

public class TopBendWormCube : WormCube
{

    public override void CalculateVertices(float _size, float _period, float _amplitude, float _t, out Vector3[] _vertices)
    {
        float halfSize = _size * 0.5f;
        float angle = Mathf.Rad2Deg * ((Mathf.Sin(((_t / _period) * Mathf.PI * 2.0f) + (m_invertedPhase ? Mathf.PI : 0.0f)) + 1.0f) / 2.0f) * Mathf.Acos(Mathf.Sqrt(3) / 2.0f);

        _vertices = new Vector3[8];
        _vertices[0] = new Vector3(0.0f, halfSize, halfSize);
        _vertices[1] = new Vector3(0.0f, -halfSize, halfSize);
        _vertices[2] = new Vector3(0.0f, -halfSize, -halfSize);
        _vertices[3] = new Vector3(0.0f, halfSize, -halfSize);

        Quaternion rotation = Quaternion.AngleAxis(-angle, _vertices[2] - _vertices[1]);

        _vertices[5] = _vertices[1] + rotation * new Vector3(_size, 0.0f, 0.0f);
        _vertices[6] = _vertices[2] + rotation * new Vector3(_size, 0.0f, 0.0f);

        _vertices[4] = _vertices[5] + rotation * rotation * new Vector3(0.0f, _size, 0.0f);
        _vertices[7] = _vertices[6] + rotation * rotation * new Vector3(0.0f, _size, 0.0f);
    }

    public override DeformationType GetDeformationType()
    {
        return DeformationType.Mix;
    }
}
