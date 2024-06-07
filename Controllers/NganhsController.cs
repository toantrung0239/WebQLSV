using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using PagedList;
using QLSV.Models;
using Rotativa.MVC;

namespace QLSV.Controllers
{
    public class NganhsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Nganhs
        public ActionResult Index()
        {
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            var nganhs = db.Nganhs.Include(n => n.Khoa);
            return View(nganhs.ToList());
        }

        // GET: Nganhs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nganh nganh = db.Nganhs.Find(id);
            if (nganh == null)
            {
                return HttpNotFound();
            }
            return View(nganh);
        }

        // GET: Nganhs/Create
        public ActionResult Create()
        {
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            return View();
        }
        [HttpGet]
        public JsonResult Getdata2()
        {
            var lists = db.Khoas.ToList();
            var a = JsonConvert.SerializeObject(lists);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Getdata1(string id)
        {
            var list = (from l in db.Nganhs
                        from c in db.Khoas
                        where l.MaKhoa==c.MaKhoa && l.MaNganh.Equals(id)
                        select new
                        {
                            l.MaNganh,
                            l.TenNganh,
                           l.MaKhoa,
                           c.TenKhoa,
                           l.TongSTC,
                        });
            var a = JsonConvert.SerializeObject(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Getdata3(string id)
        {
            var list = (from l in db.Nganhs
                        from c in db.Khoas
                        where l.MaKhoa == c.MaKhoa && l.TenNganh.Contains(id)
                        select new
                        {
                            l.MaNganh,
                            l.TenNganh,
                            l.MaKhoa,
                            c.TenKhoa,
                            l.TongSTC,
                        });
            var a = JsonConvert.SerializeObject(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TimDrop(string Makhoa)
        {
            int count = 0;
            var list = db.Nganhs.ToList();
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            if (Makhoa == "")
            {
                count = db.Nganhs.Count();
                ViewBag.SoLuong = count;
                list = db.Nganhs.ToList();
            }
            else
            {
                count = db.Nganhs.Count(m => m.MaKhoa.Equals(Makhoa));
                ViewBag.SoLuong = count;
                list = db.Nganhs.Where(m => m.MaKhoa.Equals(Makhoa)).ToList();
            }
            return View("Index", list.ToPagedList(1, 5));
        }
        [HttpGet]
        public ActionResult Index(int? page)
        {
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            int count = db.Nganhs.Count();
            ViewBag.SoLuong = count;
            var list = db.Nganhs.ToList();
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
       //Drop tham khảo https://sethphat.com/sp-372/razor-view-asp-net-mvc-dropdownlist
            return View(list.ToPagedList(pageNumber, pageSize));
        }
       
        public ActionResult sv_Nganh_Index()
        {
            var nganhs = db.Nganhs.Include(n => n.Khoa);
            return View(nganhs.ToList());
        }
        //GV
        public ActionResult gv_Nganh_Index()
        {
             var nganhs = db.Nganhs.Include(n => n.Khoa);
            return View(nganhs.ToList());
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
