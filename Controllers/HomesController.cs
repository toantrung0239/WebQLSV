using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QLSV.Models;
namespace QLSV.Controllers
{
    public class HomesController : Controller
    {
        private Model1 db = new Model1();
        // GET: Homes
        public ActionResult Index()
        {
            return View();
        }
        
    }
}