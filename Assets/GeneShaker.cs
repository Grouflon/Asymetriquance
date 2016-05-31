using UnityEngine;
using System.Collections.Generic;

public class GeneShaker : MonoBehaviour
{

    public float minCubeSize = 0.1f;
    public float maxCubeSize = 10.0f;
    public float minExpandRatio = 0.0f;
    public float maxExpandRatio = 1.0f;
    public float minPeriod = 0.1f;
    public float maxPeriod = 2.0f;

    public GeneWorm geneWormPrefab;

    Genome baseGenome;

	// Use this for initialization
	void Start ()
    {
        baseGenome = new Genome();
        int int16Max = (1 << 16) - 1;

        Chromosome cubeSize = new Chromosome(16);
        cubeSize.SetValue((ulong)((1.0f / (maxCubeSize - minCubeSize) * int16Max)));
        baseGenome.chromosomes.Add("cubeSize", cubeSize);

        Chromosome expandRatio = new Chromosome(16);
        expandRatio.SetValue((ulong)((0.25f / (maxExpandRatio - minExpandRatio) * int16Max)));
        baseGenome.chromosomes.Add("expandRatio", expandRatio);

        Chromosome period = new Chromosome(16);
        period.SetValue((ulong)((0.5f / (maxPeriod - minPeriod) * int16Max)));
        baseGenome.chromosomes.Add("period", period);

        Chromosome cubes = new Chromosome(30);
        cubes.SetValue(1UL | (1UL << 3) | (1UL << 6));
        baseGenome.chromosomes.Add("cubes", cubes);

        GeneWorm worm = CreateWormFromGenome(baseGenome);
        worm.transform.position = new Vector3(0.0f, 1.0f, 0.0f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    GeneWorm CreateWormFromGenome(Genome _genome)
    {
        GeneWorm worm = Instantiate(geneWormPrefab);

        worm.cubeSize = (maxCubeSize - minCubeSize) * _genome.chromosomes["cubeSize"].GetNormalizedValue();
        worm.expansionRatio = (maxExpandRatio - minExpandRatio) * _genome.chromosomes["expandRatio"].GetNormalizedValue();
        worm.expansionPeriod = (maxPeriod - minPeriod) * _genome.chromosomes["expandRatio"].GetNormalizedValue();

        ulong cubesValue = _genome.chromosomes["cubes"].GetValue();
        Debug.Log(cubesValue);

        List<CubeType> cubes = new List<CubeType>();
        for (int i = 0; i < 10; ++i)
        {
            ulong mask = (1 << 3) - 1;
            ulong cubeValue = (cubesValue >> (i * 3)) & mask;
            Debug.Log(cubeValue);
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
