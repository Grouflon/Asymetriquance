using UnityEngine;
using System.Collections;

public class PulseWormCube : WormCube {

    public override void CalculateVertices(float _size, float _period, float _amplitude, float _t, out Vector3[] _vertices)
    {
        float halfSize = _size * 0.5f;
        float expansion = Mathf.Sin((_t / _period) * Mathf.PI * 2.0f) * _amplitude * (m_invertedPhase ? -1.0f : 1.0f);

        _vertices = new Vector3[8];
        _vertices[0] = new Vector3(0.0f,  halfSize + expansion,  halfSize + expansion);
        _vertices[1] = new Vector3(0.0f, -halfSize - expansion,  halfSize + expansion);
        _vertices[2] = new Vector3(0.0f, -halfSize - expansion, -halfSize - expansion);
        _vertices[3] = new Vector3(0.0f,  halfSize + expansion, -halfSize - expansion);
        _vertices[4] = new Vector3(_size + 2*expansion,  halfSize + expansion,  halfSize + expansion);
        _vertices[5] = new Vector3(_size + 2*expansion, -halfSize - expansion,  halfSize + expansion);
        _vertices[6] = new Vector3(_size + 2*expansion, -halfSize - expansion, -halfSize - expansion);
        _vertices[7] = new Vector3(_size + 2*expansion,  halfSize + expansion, -halfSize - expansion);
    }

    public override DeformationType GetDeformationType()
    {
        return DeformationType.Mix;
    }
}
