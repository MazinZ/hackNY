using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.IO;

namespace hackNYWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult getData(string category, string name, string equation)
        {

            //call executable and pass arguments
            Process myProcess = new Process();
            myProcess.StartInfo.FileName = @"C:\Users\Trekele\Documents\GitHub\hackNY\Mathematica\Mathematica\bin\Debug\Mathematica.exe";
            myProcess.StartInfo.Arguments = string.Format("{0} -{0}{1} -{2}", category, name, equation);
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.RedirectStandardOutput = true;
            myProcess.Start();
            StreamReader myStreamReader = myProcess.StandardOutput;
            string myString = "";
            while(!myProcess.HasExited)
            {
                myString += myStreamReader.ReadLine() + "\n";
            }
            // Read the standard output of the spawned process.
            Console.WriteLine(myString);
            return View("Index");
        }
    }
}