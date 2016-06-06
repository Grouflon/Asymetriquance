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
        foreach (KeyValuePair<string, Chromosome> entry in chromosomes)
        {
            Chromosome chromosome = new Chromosome(entry.Value.GetSize());
            chromosome.SetValue(entry.Value.GetValue());
            genome.chromosomes[entry.Key] = chromosome;
        }
        
        return genome;
    }

    public Dictionary<string, Chromosome> chromosomes;
}
