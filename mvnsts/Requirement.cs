using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvnsts
{
    class Requirement
    {
        public string name;
        public double cost;
        public double score;
        public byte decision;
        public void Display()
        {
            Console.Write("Req: " + name + ", Cost: " + cost.ToString() + ", Score: " + score.ToString() + ", Decision: " + decision.ToString() + (Convert.ToBoolean(decision) ? " Chosen" : " Declined"));
        }
        public void Copy(Requirement other)
        {
            this.name = other.name;
            this.cost = other.cost;
            this.score = other.score;
            this.decision = other.decision;
        }
    }
}
