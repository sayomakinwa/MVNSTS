using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvnsts
{
    class Individual : IEquatable<Individual>
    {
        public int dimensionality;
        public Requirement[] elements;
        private double score = 0; //the bigger, the better
        private double cost = 0; //the smaller, the better
        private double scoreCost = 0; //the bigger, the better: score/cost
        public Individual(Requirement[] elems)
        {
            dimensionality = elems.Length;
            elements = new Requirement[dimensionality];
            for (int i = 0; i < dimensionality; i++)
            {
                elements[i] = new Requirement();
                elements[i].Copy(elems[i]);
            }
        }

        public Individual(Requirement[] elems, bool initialize, double rate, Random rnd)
        {
            dimensionality = elems.Length;
            elements = new Requirement[dimensionality];
            for (int i = 0; i < dimensionality; i++)
            {
                elements[i] = new Requirement();
                elements[i].name = elems[i].name;
                elements[i].cost = elems[i].cost;
                elements[i].score = elems[i].score;

                double temp = rnd.NextDouble();
                elements[i].decision = Convert.ToByte((temp >= rate));
            }
        }
        public void reGenerate(double rate, Random rnd)
        {
            for (int i = 0; i < dimensionality; i++)
            {
                double temp = rnd.NextDouble();
                elements[i].decision = Convert.ToByte((temp >= rate));
            }
        }

        public double[] getFitness()
        {
            score = cost = 0;
            for (int i = 0; i < dimensionality; i++)
            {
                score += elements[i].score * elements[i].decision;
                cost += elements[i].cost * elements[i].decision;
            }
            scoreCost = score / cost;
            return new double[] {score, cost, scoreCost};
        }

        public bool Equals(Individual indiv)
        {
            for (int i = 0; i < dimensionality; i++)
            {
                if (indiv.elements[i].decision != this.elements[i].decision) return false;
            }
            return true;
        }

        public void Copy(Individual indiv)
        {
            this.dimensionality = indiv.dimensionality;
            for (int i = 0; i < dimensionality; i++)
            {
                this.elements[i].Copy(indiv.elements[i]);
            }
        }

        public void Display()
        {
            double[] fitness = getFitness();
            Console.WriteLine("Individual:");
            Console.WriteLine("Score: "+fitness[0].ToString()+", Cost: "+fitness[1].ToString()+", Ratio: "+fitness[2].ToString());
            Console.WriteLine("Requirements selection: ");
            foreach (Requirement req in elements)
            {
                req.Display();
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //@Override
        /*public String toString()
        {
            String geneString = "";
            for (int i = 0; i < size(); i++)
            {
                geneString += getGene(i);
            }
            return geneString;
        }*/
    }
}
