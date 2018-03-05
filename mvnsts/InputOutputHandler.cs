using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mvnsts
{
    class InputOutputHandler
    {
        int dim, numRecs;
        string header, inputDir, outputDir, filename;
        public InputOutputHandler(int records = 132)
        {
            this.numRecs = records;
            this.inputDir = "data/input/";
            this.outputDir = "data/output/";
            this.filename = "req.csv";
        }
        public Requirement[] GetInputs()
        {
            string input = System.IO.File.ReadAllText(this.inputDir + this.filename);

            string[] lines = input.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Requirement[] vals = new Requirement[this.numRecs];

            for (int r = 0; r < this.numRecs; r++)
            {
                
                string[] lineR = lines[r+1].Split(',');
                vals[r] = new Requirement();
                Console.WriteLine(r + " - " + lineR[1]);
                vals[r].name = lineR[0];
                vals[r].score = Convert.ToDouble(lineR[1]);
                vals[r].cost = Convert.ToDouble(lineR[2]);
            }

            return vals;
        }

        public void ShowVector(string[] v, bool nl)
        {
            for (int i = 0; i < v.Length; ++i)
                Console.Write(v[i] + " ");
            if (nl == true)
                Console.WriteLine("");
        }

        public void WriteOutputs(List<Individual> front, double rate)
        {
            int xAxis = front.Count + 3;
            int yAxis = this.numRecs + 5;
            string[][] csvData = new string[yAxis][];
            for (int s = 0; s < yAxis; s++)
            {
                csvData[s] = new string[xAxis];
                for (int d = 0; d < xAxis; d++)
                {
                    csvData[s][d] = " ";
                }
            }

            csvData[0][0] = "Rate: "+rate;
            csvData[1][3] = "Pareto Decisions";
            csvData[2][0] = "Req";
            csvData[2][1] = "Score";
            csvData[2][2] = "Cost";

            for (int i = 3; i < xAxis; i++)
            {
                csvData[2][i] = "Patero "+(i-2).ToString();
            }
            for (int i = 3; i < this.numRecs+3; i++)
            {
                csvData[i][0] = front[0].elements[i-3].name;
                csvData[i][1] = front[0].elements[i-3].score+"";
                csvData[i][2] = front[0].elements[i-3].cost + "";
            }

            for (int i = 3; i < this.numRecs+3; i++)
            {
                for (int k = 3; k < xAxis; k++)
                {
                    csvData[i][k] = front[k - 3].elements[i-3].decision+"";
                }
            }

            csvData[yAxis - 2][2] = "Total Score:";
            csvData[yAxis - 1][2] = "Total Cost:";

            for (int i = 3; i < xAxis; i++)
            {
                double[] fitness = front[i - 3].getFitness();
                csvData[yAxis - 2][i] = fitness[0] + "";
                csvData[yAxis - 1][i] = fitness[1] + "";
            }

            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < csvData.Length; index++)
            {
                sb.AppendLine(string.Join(",", csvData[index]));
            }
            System.IO.File.WriteAllText(this.outputDir + this.filename, sb.ToString());
            
        }
    }
}
