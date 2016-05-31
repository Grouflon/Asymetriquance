using UnityEngine;

public struct Chromosome
{
    public Chromosome(int _size)
    {
        m_size = _size;
        m_value = 0UL;
    }

    public void SetValue(ulong _value)
    {
        m_value = _value & ((1UL << m_size) - 1UL);
    }

    public ulong GetValue()
    {
        return m_value;
    }

    public float GetNormalizedValue()
    {
        return (float)m_value / (float)((1UL << m_size) - 1UL);
    }

    public void Mutate(float _mutationRate)
    {
        for (int i = 0; i < m_size; ++i)
        {
            if (Random.value < _mutationRate)
            {
                m_value ^= (1UL << i);
            }
        }
    }

    private int     m_size;
    private ulong    m_value;
}
