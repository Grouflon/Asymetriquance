using UnityEngine;
using System.Collections.Generic;
using System;

public class GeneShaker : MonoBehaviour
{

    public float minCubeSize = 0.1f;
    public float maxCubeSize = 10.0f;
    public float minExpandRatio = 0.0f;
    public float maxExpandRatio = 1.0f;
    public float minPeriod = 0.1f;
    public float maxPeriod = 2.0f;

    public float cubeSizeMutationRate = 0.5f;
    public float expandRatioMutationRate = 0.5f;
    public float periodMutationRate = 0.5f;
    public float cubesMutationRate = 0.5f;

    public GeneWorm geneWormPrefab;
    public float raceDuration = 10.0f;
    public int generationSize = 10;

    Genome m_baseGenome;
    float m_timer = 0.0f;
    int m_generationCount = 0;

    struct Subject
    {
        public Genome genome;
        public GeneWorm worm;
    }
    Subject[] m_currentGeneration;

	// Use this for initialization
	void Start ()
    {
        m_baseGenome = new Genome();
        int int16Max = (1 << 16) - 1;

        Chromosome cubeSize = new Chromosome(16);
        cubeSize.SetValue((ulong)((1.0f / (maxCubeSize - minCubeSize) * int16Max)));
        m_baseGenome.chromosomes.Add("cubeSize", cubeSize);

        Chromosome expandRatio = new Chromosome(16);
        expandRatio.SetValue((ulong)((0.25f / (maxExpandRatio - minExpandRatio) * int16Max)));
        m_baseGenome.chromosomes.Add("expandRatio", expandRatio);

        Chromosome period = new Chromosome(16);
        period.SetValue((ulong)((0.5f / (maxPeriod - minPeriod) * int16Max)));
        m_baseGenome.chromosomes.Add("period", period);

        Chromosome cubes = new Chromosome(40);
        cubes.SetValue(1UL | (1UL << 4) | (1UL << 4));
        m_baseGenome.chromosomes.Add("cubes", cubes);

       /* GeneWorm worm = CreateWormFromGenome(m_baseGenome);
        worm.transform.position = new Vector3(0.0f, 1.0f, 0.0f);*/


        m_currentGeneration = new Subject[generationSize];
        for (int i = 0; i < generationSize; ++i)
        {
            Subject subject = new Subject();
            subject.genome = m_baseGenome.Copy();
            subject.genome.chromosomes["cubeSize"].Mutate(cubeSizeMutationRate);
            subject.genome.chromosomes["expandRatio"].Mutate(expandRatioMutationRate);
            subject.genome.chromosomes["period"].Mutate(periodMutationRate);
            subject.genome.chromosomes["cubes"].Mutate(cubesMutationRate);

            subject.worm = CreateWormFromGenome(subject.genome);
            subject.worm.transform.position = new Vector3(0.0f, 1.0f, maxCubeSize * 3 * i);
            m_currentGeneration[i] = subject;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // TODO: have our own simulation time in order to have deterministic simulations

        m_timer += Time.deltaTime;

	    if (m_timer > raceDuration)
        {
            m_timer = 0.0f;
            ++m_generationCount;

            Array.Sort(m_currentGeneration, delegate (Subject subjec1, Subject subject2) {
                return subjec1.worm.transform.position.x.CompareTo(subject2.worm.transform.position.x);
            });

            for (int i = 2; i < 2 + (generationSize-2) / 2; ++i)
            {
                m_currentGeneration[i].genome = m_currentGeneration[0].genome.Copy();
                m_currentGeneration[i].genome.chromosomes["cubeSize"].Mutate(cubeSizeMutationRate);
                m_currentGeneration[i].genome.chromosomes["expandRatio"].Mutate(expandRatioMutationRate);
                m_currentGeneration[i].genome.chromosomes["period"].Mutate(periodMutationRate);
                m_currentGeneration[i].genome.chromosomes["cubes"].Mutate(cubesMutationRate);
            }

            for (int i = 2 + ((generationSize - 2) / 2); i < generationSize; ++i)
            {
                m_currentGeneration[i].genome = m_currentGeneration[1].genome.Copy();
                m_currentGeneration[i].genome.chromosomes["cubeSize"].Mutate(cubeSizeMutationRate);
                m_currentGeneration[i].genome.chromosomes["expandRatio"].Mutate(expandRatioMutationRate);
                m_currentGeneration[i].genome.chromosomes["period"].Mutate(periodMutationRate);
                m_currentGeneration[i].genome.chromosomes["cubes"].Mutate(cubesMutationRate);
            }

            for (int i = 0; i < generationSize; ++i)
            {
                GameObject.Destroy(m_currentGeneration[i].worm.gameObject);
                m_currentGeneration[i].worm = CreateWormFromGenome(m_currentGeneration[i].genome);
                m_currentGeneration[i].worm.transform.position = new Vector3(0.0f, maxCubeSize, maxCubeSize * 3 * i);
            }
        }
	}

    GeneWorm CreateWormFromGenome(Genome _genome)
    {
        GeneWorm worm = Instantiate(geneWormPrefab);

        worm.cubeSize = (maxCubeSize - minCubeSize) * _genome.chromosomes["cubeSize"].GetNormalizedValue();
        worm.expansionRatio = (maxExpandRatio - minExpandRatio) * _genome.chromosomes["expandRatio"].GetNormalizedValue();
        worm.expansionPeriod = (maxPeriod - minPeriod) * _genome.chromosomes["expandRatio"].GetNormalizedValue();

        ulong cubesValue = _genome.chromosomes["cubes"].GetValue();

        List<CubeType> cubes = new List<CubeType>();
        for (int i = 0; i < 10; ++i)
        {
            ulong mask = (1 << 4) - 1;
            ulong cubeValue = (cubesValue >> (i * 4)) & mask;
            if (cubeValue > 0UL && cubeValue < (ulong)CubeType.COUNT)
            {
                cubes.Add((CubeType)cubeValue);
            }
        }
        worm.cubes = new CubeType[cubes.Count];
        for (int i = 0; i < cubes.Count; ++i)
        {
            worm.cubes[i] = cubes[i];
        }

        return worm;
    }
}
