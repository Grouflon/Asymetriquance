using System.Collections.Generic;

public class Genome {

    public Genome()
    {
        chromosomes = new Dictionary<string, Chromosome>();
    }

    public Genome Copy()
    {
        Genome genome = new Genome();
        genome.chromosomes = new Dictionary<string, Chromosome>(chromosomes);
        return genome;
    }

    public Dictionary<string, Chromosome> chromosomes;
}
