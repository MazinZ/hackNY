﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolfram.NETLink;
using System.IO;

namespace Mathematica
{
    class Program
    {
        static void Main(string[] args)
        {
            //for testing math
            //args = new string[] {"m", "-3dTest", "Sin[x+y]"};
            //for testing chem
            //args = new string[] { "c", "-3dTest", "Sin[x+y]" };
            //for testing bio
            //args = new string[] { "b", "-3dTest", "A2M" };

            //check to see how many arguments there are, if not 3 end program
            if(args.Count() != 3)
            {
                Console.WriteLine("Error, check arguments.");
                return;
            }
            else
            {
                Console.WriteLine("In else");
                //remove the dash character if its the first one
                string category = args[0];
                string nameOfModel = args[1][0] == '-' ? args[1].Substring(1) : args[1];
                string equation = args[2][0] == '-' ? args[2].Substring(1) : args[2];
                Console.WriteLine("Before switch");
                //call fucntion depending on the category
                switch (category)
                {
                    case "c":
                        getChemistry3dModel(nameOfModel, equation);
                        break;
                    case "m":
                        Console.WriteLine("in case");
                        getMath3dModel(nameOfModel, equation);
                        break;
                    case "b":
                        getBiology3dModel(nameOfModel, equation);
                        break;
                    default:
                        Console.WriteLine("Error");
                        return;
                }
            }
           
        }

        static void getMath3dModel(string nameOfModel, string equation)
        {
            IKernelLink ml = MathLinkFactory.CreateKernelLink();
            ml.WaitAndDiscardAnswer();
            //ml.Evaluate("gr1 = Plot3D[-(cos(x) + cos(y) + cos(z))=0, {x, -5, 5}, {y, -5, 5}, Mesh -> None]");
            ml.Evaluate("gr1 = Plot3D[" + equation + ", {x, -5, 5}, {y, -5, 5}, Mesh -> None]");
            ml.WaitAndDiscardAnswer();

            //ml.Evaluate("Export[\"3DPlot.obj\",  gr1]");

            if (!Directory.Exists("C:\\hackNY\\"))
            {
                Directory.CreateDirectory("C:\\hackNY\\");
            }
            ml.Evaluate(string.Format("Export[\"C:\\hackNY\\{0}.obj\",  gr1]", nameOfModel));
            ml.WaitAndDiscardAnswer();
            Console.WriteLine("hope it worked");
        }
        static void getChemistry3dModel(string nameOfModel, string moleculeName)
        {
            IKernelLink ml = MathLinkFactory.CreateKernelLink();
            ml.WaitAndDiscardAnswer();
            //ml.Evaluate("gr1 = Plot3D[-(cos(x) + cos(y) + cos(z))=0, {x, -5, 5}, {y, -5, 5}, Mesh -> None]");
            ml.Evaluate("gr1 = ChemicalData[\"Caffeine\", \"MoleculePlot\"]");
            ml.WaitAndDiscardAnswer();

            //ml.Evaluate("Export[\"3DPlot.obj\",  gr1]");

            if (!Directory.Exists("C:\\hackNY\\"))
            {
                Directory.CreateDirectory("C:\\hackNY\\");
            }
            ml.Evaluate(string.Format("Export[\"C:\\hackNY\\{0}.obj\",  gr1]", nameOfModel));
            ml.WaitAndDiscardAnswer();
            Console.WriteLine("hope it worked");
        }
        static void getBiology3dModel(string nameOfModel, string proteinName)
        {
            IKernelLink ml = MathLinkFactory.CreateKernelLink();
            ml.WaitAndDiscardAnswer();
            //ml.Evaluate("gr1 = Plot3D[-(cos(x) + cos(y) + cos(z))=0, {x, -5, 5}, {y, -5, 5}, Mesh -> None]");
            ml.Evaluate("gr1 = ProteinData[\"TPMT\", \"MoleculePlot\"]");
            ml.WaitAndDiscardAnswer();

            //ml.Evaluate("Export[\"3DPlot.obj\",  gr1]");

            if (!Directory.Exists("C:\\hackNY\\"))
            {
                Directory.CreateDirectory("C:\\hackNY\\");
            }
            ml.Evaluate(string.Format("Export[\"C:\\hackNY\\{0}.obj\",  gr1]", nameOfModel));
            ml.WaitAndDiscardAnswer();
            Console.WriteLine("hope it worked");
        }

    }
}