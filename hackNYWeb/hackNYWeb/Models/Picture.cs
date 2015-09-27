using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace hackNYWeb.Models
{
    public class Picture
    {
        public string category{ get; set; }
        public string name { get; set; }
        public string equation { get; set; }
        public HttpPostedFileBase file { get; set; }
        public Picture(string category, string name, string equation, HttpPostedFileBase file)
        {
            this.category = category;
            this.name = name;
            this.equation = equation;
            this.file = file;
        }
    }
}
