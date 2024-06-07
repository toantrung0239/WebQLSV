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
using Rotativa;
using Rotativa.MVC;

namespace projectCK.Controllers
{
    public class MonHocsController : Controller
    {
        private Model1 db = new Model1();

        [HttpGet]
        public ActionResult Index(int? page)
        {
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            int count = db.MonHocs.Count();
            ViewBag.SoLuong = count;
            var list = db.MonHocs.ToList();
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            //Drop tham khảo https://sethphat.com/sp-372/razor-view-asp-net-mvc-dropdownlist
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public JsonResult GetKhoa()
        {
            var list = db.Khoas.ToList();
            var a = JsonConvert.SerializeObject(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Getdata3(string id)
        {

            var list = (from l in db.MonHocs
                        from c in db.Khoas
                        where l.MaKhoa == c.MaKhoa && l.TenMH.Contains(id)
                        select new
                        {
                            l.MaMH,
                            l.TenMH,
                            l.MaKhoa,
                            c.TenKhoa,
                            l.SoTietLT,
                            l.SoTietTH,
                            l.STC,
                        });
            var a = JsonConvert.SerializeObject(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
       
        [HttpGet]
        public JsonResult Getdata1(string id)
        {
            var list = (from l in db.MonHocs
                        from c in db.Khoas
                        where l.MaKhoa == c.MaKhoa && l.MaMH.Equals(id)
                        select new
                        {
                            l.MaMH,
                            l.TenMH,
                            l.SoTietLT,
                            l.MaKhoa,
                            c.TenKhoa,
                            l.SoTietTH,
                            l.STC
                        });
            var a = JsonConvert.SerializeObject(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TimDrop(string MaKhoa)
        {
            int count = 0;
            var list = db.MonHocs.ToList();
            ViewBag.MaKhoa = new SelectList(db.Khoas, "MaKhoa", "TenKhoa");
            if (MaKhoa == "")
            {
                count = db.MonHocs.Count();
                ViewBag.SoLuong = count;
                list = db.MonHocs.ToList();
            }
            else
            {
                count = db.MonHocs.Count(m => m.MaKhoa.Equals(MaKhoa));
                ViewBag.SoLuong = count;
                list = db.MonHocs.Where(m => m.MaKhoa.Equals(MaKhoa)).ToList();
            }
            return View("Index", list.ToPagedList(1, 5));
        }
    
        public ActionResult sv_MonHoc_Index()
        {
            var monHocs = db.MonHocs.Include(m => m.Khoa);
            return View(monHocs.ToList());
        }
        //GV
        public ActionResult gv_MonHoc_Index()
        {
            var monHocs = db.MonHocs.Include(m => m.Khoa);
            return View(monHocs.ToList());
        }

    }
}