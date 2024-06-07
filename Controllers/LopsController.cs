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
    public class LopsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Lops
        public ActionResult Index()
        {
            ViewBag.MaNganh = new SelectList(db.Nganhs, "MaNganh", "TenNganh");
            var lops = db.Lops.Include(l => l.Nganh);
            return View(lops.ToList());
        }
        [HttpGet]
        public JsonResult Getdata2()
        {
            var lists = db.Nganhs.ToList();
            var list= JsonConvert.SerializeObject(lists);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public void AddData(string ml, string tl, string mn)
        {

            string malop = ml;
            string tenlop = tl;
            int trangthai = 1;
            string manganh = mn;
            

            try
            {
                if (malop.ToString() != "")
                {
                    var data = db.Lops.Where(s => s.MaLop.Equals(malop)).ToList();
                    if (data.Count > 0)
                    {
                        Response.Write("Trùng mã lớp");
                    }
                    else
                    {
                        Lop lop = new Lop();
                        lop.MaLop = malop;
                        lop.TenLop = tenlop;
                        lop.TrangThai = trangthai;
                       
                        lop.MaNganh = manganh;
                       
                        db.Lops.Add(lop);
                        db.SaveChanges();
                        Response.Write("Thêm thành công");

                    }
                }

                else
                {
                    Response.Write("Mã lớp không được để trống");
                }
            }
            catch
            {
                Response.Write("Thêm không thành công");
            }

        }
        [HttpPost]
        public void EditData(string ml, string tl, string mn)
        {

            string malop = ml;
            string tenlop = tl;
            string manganh = mn;
            try
            {
                Lop lop = new Lop();
                lop = db.Lops.Where(s => s.MaLop == malop).SingleOrDefault();
                lop.TenLop = tenlop;
                lop.MaNganh = manganh;
                db.SaveChanges();
                Response.Write("Sửa thành công");

            }
            catch (Exception e)
            {
                Response.Write("Sửa không thành công");
            }
        }
        [HttpGet]
        public JsonResult Getdata1(string id)
        {
            var list = (from l in db.Lops
                        from n in db.Nganhs
                        where l.MaNganh == n.MaNganh && l.MaLop.Equals(id)
                        select new
                        {
                            l.MaLop,
                            l.TenLop,
                            l.MaNganh,
                            n.TenNganh,
                        });
            var js = JsonConvert.SerializeObject(list);
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Getdata3(string id)
        {
            var list = (from l in db.Lops
                        from n in db.Nganhs
                        where l.MaNganh == n.MaNganh&& l.TenLop.Contains(id)
                        select new
                        {
                            l.MaLop,
                            l.TenLop,
                            l.MaNganh,
                            n.TenNganh,
                        });
            var js = JsonConvert.SerializeObject(list);
            return Json(js, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DelTrash(string id)
        {
            Lop lop = db.Lops.Find(id);
            lop.TrangThai = 0;
            //Cập nhật lại
            db.Entry(lop).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Trash()
        {
            int count = db.Lops.Count(m => m.TrangThai == 0);
            ViewBag.SoLuong = count;
            var list = db.Lops.Where(m => m.TrangThai == 0).ToList();
            return View("Trash", list);
        }
        public ActionResult ReTrash(string id)
        {
            Lop lop = db.Lops.Find(id);
            lop.TrangThai = 1;
            db.Entry(lop).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Trash", "Lops");
        }
        [HttpPost]
        public ActionResult XuatFile(string file)
        {
            if (file == "1")
            {
                var gv = new GridView();
                gv.DataSource = db.Lops.Where(l => l.TrangThai != 0).ToList();
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Lop.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter objStringWriter = new StringWriter();
                HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
                objHtmlTextWriter.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                gv.RenderControl(objHtmlTextWriter);
                Response.Output.Write(objStringWriter.ToString());
                Response.Flush();
                Response.End();
                return View();
            }
            else
            {
                if (file == "2")
                {
                    //sử dụng code https://techfunda.com/howto/309/export-data-into-ms-word-from-mvc
                    var data = db.Lops.Where(l => l.TrangThai != 0).ToList();
                    GridView gridview = new GridView();
                    gridview.DataSource = data;
                    gridview.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment;filename = Lop.doc");
                    Response.ContentType = "application/ms-word";
                    Response.Charset = "";
                    using (StringWriter sw = new StringWriter())
                    {
                        using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                        {
                            htw.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                            gridview.RenderControl(htw);
                            Response.Output.Write(sw.ToString());
                            Response.Flush();
                            Response.End();
                        }
                    }
                    return View();
                }
                else
                {
                    // sử dụng thư viện https://www.c-sharpcorner.com/article/export-pdf-from-html-in-mvc-net4/
                    var list = db.Lops.Where(l => l.TrangThai != 0).ToList();
                    return new PartialViewAsPdf("PrintPDF", list)
                    {
                        FileName = "Lop.pdf"
                    };
                }
            }
        }
        [HttpPost]
        public JsonResult Getdatathongke()
        {
            var query = from s in db.SinhViens
                        from l in db.Lops
                        where l.MaLop == s.Malop
                        group l by l.TenLop into b
                        orderby b.Count() descending
                        let TenLop = b.Key
                        let SoLuong = b.Count()
                        select new
                        {
                            TenLop,
                            SoLuong,
                        };
            var js = JsonConvert.SerializeObject(query);
            return Json(js, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
      
        public ActionResult TimDrop(string MaNganh)
        {
            int count = 0;
            var list = db.Lops.Where(s => s.TrangThai != 0).ToList();
            ViewBag.MaNganh = new SelectList(db.Nganhs, "MaNganh", "TenNganh");
            if (MaNganh == "")
            {
                count = db.Lops.Count(m => m.TrangThai != 0);
                ViewBag.SoLuong = count;
                list = db.Lops.Where(s => s.TrangThai != 0).ToList();
            }
            else
            {
                count = db.Lops.Count(m => m.MaNganh.Equals(MaNganh) && m.TrangThai != 0);
                ViewBag.SoLuong = count;
                list = db.Lops.Where(m => m.MaNganh.Equals(MaNganh) && m.TrangThai != 0).ToList();
            }
            return View("Index", list.ToPagedList(1, 5));
        }
        [HttpGet]
        public ActionResult Index(int? page)
        {
            ViewBag.MaNganh = new SelectList(db.Nganhs, "MaNganh", "TenNganh");
            int count = db.Lops.Count(m => m.TrangThai != 0);
            ViewBag.SoLuong = count;
            var list = db.Lops.Where(m => m.TrangThai != 0).ToList();
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            //Drop tham khảo https://sethphat.com/sp-372/razor-view-asp-net-mvc-dropdownlist
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public void Delete(string Id)
        {
            try
            {
                Lop lop = db.Lops.Find(Id);
                db.Lops.Remove(lop);
                db.SaveChanges();
                Response.Write("Xóa thành công");
            }
            catch (Exception e)
            {
                Response.Write("Xóa thất bại");
            }
        }
        // SV
        public ActionResult sv_Lop_Index()
        {
            var lops = db.Lops.Include(l => l.Nganh);
            return View(lops.ToList());
        }
        //GV
        public ActionResult gv_Lop_Index()
        {
            var lops = db.Lops.Include(l => l.Nganh);
            return View(lops.ToList());
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
