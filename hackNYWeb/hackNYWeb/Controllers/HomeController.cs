using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using hackNYWeb.Models;

namespace hackNYWeb.Controllers
{
    public class HomeController : Controller
    {
        private Picture p;
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult getData(string category, string name, string equation, HttpPostedFileBase file)
        {
            string fileName = category + name;
            string path = "";
            string pathOfFolder = "";
            p = new Picture(category, name, equation, file);
            Vuforia v;
            if (file.ContentLength > 0)
            {
                
                path = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                pathOfFolder = Server.MapPath("~/Content/images") + "\\";
                file.SaveAs(path);
            }

            SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
            f.OpenPdf(path);

            if (f.PageCount > 0)
            {
                f.ImageOptions.Dpi = 300;
                f.ImageOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;

                for (int page = 1; page <= f.PageCount; page++)
                {
                    f.ToImage(pathOfFolder + "MyPDFImage.jpg", page);
                }
            }

            return View("TargetSelection");
            //v = new Vuforia(fileName, path);
            //v.postTarget();
        }

        //    //call executable and pass arguments
        //    Process myProcess = new Process();
        //    myProcess.StartInfo.FileName = @"C:\Users\Trekele\Documents\GitHub\hackNY\Mathematica\Mathematica\bin\Debug\Mathematica.exe";
        //    myProcess.StartInfo.Arguments = string.Format("{0} {1} \"{2}\"", category, fileName, equation);
        //    myProcess.StartInfo.UseShellExecute = false;
        //    myProcess.StartInfo.RedirectStandardOutput = true;
        //    myProcess.Start();
        //    StreamReader myStreamReader = myProcess.StandardOutput;
        //    string myString = "";
        //    while(!myProcess.HasExited)
        //    {
        //        myString += myStreamReader.ReadLine() + "\n";
        //    }
        //    // Read the standard output of the spawned process.
        //    Console.WriteLine(myString);
        //    return View("Index");
        //}
        
    }
}