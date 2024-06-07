using QLSV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data.SqlClient;
using PagedList;
using Rotativa;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using Rotativa.MVC;

namespace QLSV.Controllers
{
    public class KhoasController : Controller
    {
        private Model1 db = new Model1();

        // GET: Khoas
        [HttpGet]
        public JsonResult Getdata(string id)
        {
            var list = db.Khoas.Where(p => p.MaKhoa.Equals(id));
            var a = JsonConvert.SerializeObject(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Getdata1(string id)
        {
            var list = db.Khoas.Where(p => p.TenKhoa.Contains(id));
            var a = JsonConvert.SerializeObject(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

       
        public ActionResult Index(int? page)
        {
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            int count = db.Khoas.Count();
            ViewBag.SoLuong = count;
            var list = db.Khoas.ToList();
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Search(FormCollection f,int? page)
        {
            string b = (f["tenkhoa"]).Trim();
            var list = db.Khoas.Where(p => p.TenKhoa.Contains(b)).ToList();
            int count = list.Count();
            ViewBag.SoLuong = count;
            return View("Index", list.ToPagedList(1, 3));
        }
       
        public ActionResult sv_Khoa_Index()
        {
            var khoa = db.Khoas;
            return View(khoa.ToList());
        }
        //GV
        public ActionResult gv_Khoa_Index()
        {
            var khoa = db.Khoas;
            return View(khoa.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
