using UnityEngine;
using System.Collections;

public abstract class WormCube {

    public enum DeformationType
    {
        Slave,
        Mix,
        Master,
    }

    public abstract void CalculateVertices(float _size, float _period, float _amplitude, float _t, out Vector3[] _vertices);
    public abstract DeformationType GetDeformationType();

    public void SetInvertedPhase(bool _value)
    {
        m_invertedPhase = _value;
    }

    protected bool m_invertedPhase = false;
}
