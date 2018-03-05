using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvnsts
{
    class Program
    {
        static Random r = new Random();
        static void Main(string[] args)
        {
            InputOutputHandler input = new InputOutputHandler();
            Requirement[] req = input.GetInputs();
            
            //int k = 15;
            int n = 2;
            double rate = 0.5;

            List<Individual> front = new List<Individual>();
            List<Individual> tabu = new List<Individual>();

            Individual[] neigbourhood = new Individual[n];
            /*Console.WriteLine("Initialisation:");
            for (int i = 0; i < n; i++)
            {
                neigbourhood[i] = new Individual(req, true, rate, r);
                //front.Add(neigbourhood[i]);
                neigbourhood[i].Display();
                Console.WriteLine("Sane? " + IsSane(neigbourhood[i]).ToString() + " Generated? " + InArray(neigbourhood, neigbourhood[i]).ToString());
                //Console.WriteLine(i);
            }*/

            
            
            /*
            //Unit testing
            neigbourhood[2] = new Individual(req);
            Individual test = new Individual(req);
            Individual test2 = new Individual(req, true, rate, r);
            for (int i = 0; i < req.Length; i++)
            {
                neigbourhood[2].elements[i].decision = Convert.ToByte(i%2);
                test.elements[i].decision = Convert.ToByte(i % 2);
            }

            front.Add(neigbourhood[2]);

            foreach (Individual neigh in front)
            {
                neigh.Display();
            }
            Console.WriteLine("==================");
            test.Display();
            Console.WriteLine(front.Contains(test));

            test2.Display();

            Console.WriteLine(front.Contains(test2));
            //test.Display();
            //unit testing
            */


            

            Console.WriteLine("Neighbourhood: ");
            for (int i = 0; i < n; i++)
            {
                Individual temp = new Individual(req, true, rate, r);
                while (!IsSane(temp) || InArray(neigbourhood, temp))
                    temp.reGenerate(rate, r);
                neigbourhood[i] = temp;
                neigbourhood[i].Display();
                Individual localOpt = neigbourhood[i];
                //Individual localOpt = new Individual(req);
                //localOpt.Copy(neigbourhood[i]);
                List<Individual> individualList = GenerateIndiv(tabu, neigbourhood[i]);
                Console.WriteLine("Options: ");
                foreach (Individual indiv in individualList)
                {
                    //indiv.Display();
                    
                    if ((indiv.getFitness()[1] <= neigbourhood[i].getFitness()[1] && indiv.getFitness()[0] > neigbourhood[i].getFitness()[0]) && !front.Contains(indiv))
                    {
                        front.Add(indiv);
                        localOpt = indiv;
                    }
                    else tabu.Add(indiv);
                }
                if (localOpt.Equals(neigbourhood[i]) && !front.Contains(neigbourhood[i]))
                    front.Add(neigbourhood[i]);
            }

            Console.WriteLine("Fronts: ");
            foreach (Individual ind in front)
            {
                ind.Display();
            }
            Console.ReadLine();

        }
        static bool IsSane(Individual ind)
        {
            bool sane = false;
            foreach (Requirement element in ind.elements)
            {
                if (element.decision == 1) sane = true;
            }
            return sane;
        }

        static bool InArray(Individual[] array, Individual elem)
        {
            bool toReturn = false;
            foreach (Individual arr in array)
            {
                try
                {
                    toReturn = true;
                    for (int i = 0; i < arr.dimensionality; i++)
                    {
                        if (arr.elements[i].decision != elem.elements[i].decision) toReturn = false;
                    }
                    if (toReturn) return toReturn;
                }
                catch (NullReferenceException e)
                {
                    return false;
                }
            }
            return toReturn;
        }

        static Individual LoadOrder(RequirementWrapper[] first, RequirementWrapper[] second, RequirementWrapper[] third, Individual pivot)
        {
            Requirement[] tempReqScore = new Requirement[pivot.dimensionality];
            //initializing the requirements array
            for (int k = 0; k < tempReqScore.Length; k++)
            {
                tempReqScore[k] = new Requirement();
                tempReqScore[k].Copy(pivot.elements[k]);
                tempReqScore[k].decision = 0;
            }

            double maxCost = pivot.getFitness()[1];

            //Console.WriteLine("maxCost: " + maxCost.ToString());


            for (int i = 0; i < first.Length; i++)
            {
                if ((maxCost - first[i].element.cost) >= 0)
                {
                    tempReqScore[first[i].index].decision = 1;
                    maxCost = maxCost - first[i].element.cost;
                }
            }
            if (maxCost > 0)
            {
                for (int i = 0; i < pivot.dimensionality; i++)
                {
                    if ((maxCost - second[i].element.cost) >= 0)
                    {
                        if (tempReqScore[second[i].index].decision != 1)
                        {
                            tempReqScore[second[i].index].decision = 1;
                            maxCost -= second[i].element.cost;
                        }
                    }
                }
            }
            if (maxCost > 0)
            {
                for (int i = 0; i < pivot.dimensionality; i++)
                {
                    if ((maxCost - third[i].element.cost) >= 0)
                    {
                        if (tempReqScore[third[i].index].decision != 1)
                        {
                            tempReqScore[third[i].index].decision = 1;
                            maxCost -= third[i].element.cost;
                        }
                    }
                }
            }
            //Console.WriteLine("maxCost: " + maxCost.ToString());
            Individual tempIndiv = new Individual(tempReqScore);
            return tempIndiv;
        }
        static List<Individual> GenerateIndiv(List<Individual> tabuList, Individual pivot)
        {
            RequirementWrapper[] score_sort = new RequirementWrapper[pivot.dimensionality];
            RequirementWrapper[] cost_sort = new RequirementWrapper[pivot.dimensionality];
            RequirementWrapper[] score_cost_sort = new RequirementWrapper[pivot.dimensionality];

            for (int i = 0; i < pivot.dimensionality; i++)
            {
                score_sort[i] = new RequirementWrapper();
                cost_sort[i] = new RequirementWrapper();
                score_cost_sort[i] = new RequirementWrapper();

                score_sort[i].element.Copy(pivot.elements[i]);
                cost_sort[i].element.Copy(pivot.elements[i]);
                score_cost_sort[i].element.Copy(pivot.elements[i]);
                
                score_sort[i].index = cost_sort[i].index = score_cost_sort[i].index = i; 
            }

            //sort: all sorts are in decending order of importance
            Requirement tempReq = new Requirement();
            int tempIndex;
            for (int s = 0; s < pivot.dimensionality; s++)
            {
                for (int x = s + 1; x < pivot.dimensionality; x++)
                {
                    if (score_sort[s].element.score < score_sort[x].element.score) //score sort
                    {
                        tempReq.Copy(score_sort[s].element);
                        tempIndex = score_sort[s].index;

                        score_sort[s].element.Copy(score_sort[x].element);
                        score_sort[s].index = score_sort[x].index;

                        score_sort[x].element.Copy(tempReq);
                        score_sort[x].index = tempIndex;
                    }

                    if (cost_sort[s].element.cost > cost_sort[x].element.cost) //cost sort
                    {
                        tempReq.Copy(cost_sort[s].element);
                        tempIndex = cost_sort[s].index;

                        cost_sort[s].element.Copy(cost_sort[x].element);
                        cost_sort[s].index = cost_sort[x].index;

                        cost_sort[x].element.Copy(tempReq);
                        cost_sort[x].index = tempIndex;
                    }

                    if ((score_cost_sort[s].element.score / score_cost_sort[s].element.cost) < (score_cost_sort[x].element.score / score_cost_sort[x].element.cost)) //score_cost sort
                    {
                        tempReq.Copy(score_cost_sort[s].element);
                        tempIndex = score_cost_sort[s].index;

                        score_cost_sort[s].element.Copy(score_cost_sort[x].element);
                        score_cost_sort[s].index = score_cost_sort[x].index;

                        score_cost_sort[x].element.Copy(tempReq);
                        score_cost_sort[x].index = tempIndex;
                    }
                }
            }

            List<Individual> toReturn = new List<Individual>();
            Individual temp = new Individual(pivot.elements);

            temp = LoadOrder(score_sort, cost_sort, score_cost_sort, pivot);
            if (!toReturn.Contains(temp)) toReturn.Add(temp);

            temp = LoadOrder(score_sort, score_cost_sort, cost_sort, pivot);
            if (!toReturn.Contains(temp)) toReturn.Add(temp);

            temp = LoadOrder(cost_sort, score_sort, score_cost_sort, pivot);
            if (!toReturn.Contains(temp)) toReturn.Add(temp);

            temp = LoadOrder(cost_sort, score_cost_sort, score_sort, pivot);
            if (!toReturn.Contains(temp)) toReturn.Add(temp);

            temp = LoadOrder(score_cost_sort, score_sort, cost_sort, pivot);
            if (!toReturn.Contains(temp)) toReturn.Add(temp);

            temp = LoadOrder(score_cost_sort, cost_sort, score_sort, pivot);
            if (!toReturn.Contains(temp)) toReturn.Add(temp);

            /*Console.WriteLine("\n\nScore sort:");
            foreach (RequirementWrapper rw in score_sort)
            {
                Console.WriteLine(rw.index);
                rw.element.Display();
            }

            Console.WriteLine("\n\nCost sort:");
            foreach (RequirementWrapper rw in cost_sort)
            {
                Console.WriteLine(rw.index);
                rw.element.Display();
            }

            Console.WriteLine("\n\nScore Cost sort:");
            foreach (RequirementWrapper rw in score_cost_sort)
            {
                Console.WriteLine(rw.index);
                rw.element.Display();
            }*/


            return toReturn;
        }

    }
}
