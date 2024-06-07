using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLSV.Models;
using System.Security.Cryptography;
using System.Text;
namespace QLSV.Controllers
{
    public class DangNhapsController : Controller
    {
        private Model1 db = new Model1();
        public ActionResult Index()
        {
            return View(db.DangNhaps.ToList());
        }
        [HttpPost]
        public ActionResult Index(string TenDN, string MatKhau)
        {
            if (ModelState.IsValid) 
            {
                string tdn = TenDN.Trim();
                string mk = MatKhau.Trim();//TenDN được gán biến tdn hàm Trim() được sử dụng để loại bỏ các khoảng trắng ko cần thiết trong tên đăng nhập
                if (tdn == "" || mk == "")
                {
                    ViewBag.thongbao = "Vui lòng điển đầy đủ thông tin";

                }
                else
                {
                    var dtadmin = db.DangNhaps.Where(s => s.TenDN.Equals(tdn)).ToList();
                    var dtsv = db.SinhViens.Where(s => s.MaSV.Equals(tdn)).ToList();
                    var dtgv = db.GiangViens.Where(s => s.MaGV.Equals(tdn)).ToList();
                    if (dtadmin.Count > 0)
                    {
                        string mk1 = GetMD5(mk);
                        var kt = db.DangNhaps.Where(s => s.TenDN.Equals(tdn) && s.MatKhau.Equals(mk1)).ToList();
                        if (kt.Count > 0)
                        {
                            Session["TenDN"] = dtadmin.FirstOrDefault().TenDN;
                            Session["HoTen"] = dtadmin.FirstOrDefault().HoTen;
                            Session["HinhAnh"] = dtadmin.FirstOrDefault().HinhAnh;
                            Session["SDT"] = dtadmin.FirstOrDefault().SDT;
                            Session["DiaChi"] = dtadmin.FirstOrDefault().DiaChi;
                            Session["Email"] = dtadmin.FirstOrDefault().Email;
                            //ADMin
                            return RedirectToAction("Index", "Homes");
                        }
                        else
                        {
                            ViewBag.thongbao = "Sai tên đăng nhập hoặc mật khẩu";
                        }

                    }
                    else
                    {
                        if (dtsv.Count > 0)
                        {
                            var kt = db.SinhViens.Where(s => s.MaSV.Equals(tdn) && s.MatKhau.Equals(mk)).ToList();
                            if (kt.Count > 0)
                            {
                                Session["MaSV"] = dtsv.FirstOrDefault().MaSV;
                                Session["HinhAnh"] = dtsv.FirstOrDefault().HinhAnh;
                                // Sinh Viên
                                return RedirectToAction("SinhVien", "ThongTin");
                            }
                            else
                            {
                                ViewBag.thongbao = "Sai tên đăng nhập hoặc mật khẩu";
                            }
                        }
                        else
                        {
                            var kt = db.GiangViens.Where(s => s.MaGV.Equals(tdn) && s.MatKhau.Equals(mk)).ToList();
                            if (kt.Count > 0)
                            {
                                Session["MaGV"] = dtgv.FirstOrDefault().MaGV;
                                Session["HinhAnh"] = dtgv.FirstOrDefault().HinhAnh;
                                return RedirectToAction("GiangVien", "ThongTin");
                            }
                            else
                            {
                                ViewBag.thongbao = "Sai tên đăng nhập hoặc mật khẩu";
                            }
                        }
                    }
                }

            }
            return View();
        }
        public ActionResult Thoat()
        {
            Session.Clear();
            return RedirectToAction("Index", "DangNhaps");
        }
        //???
        //// GET: DangNhaps/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    DangNhap dangNhap = db.DangNhaps.Find(id);
        //    if (dangNhap == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(dangNhap);
        //}
        //// GET: DangNhaps/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}
        public ActionResult DangXuat()
        {
            Session.Abandon();
            return View("Index","DangNhaps");
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);//Chuyển đổi chuỗi str thành một mảng byte bằng cách sử dụng mã hóa UTF-8.
            byte[] targetData = md5.ComputeHash(fromData);//Sử dụng phương thức ComputeHash của đối tượng MD5 để tính toán mã MD5 của mảng byte đã chuyển đổi.
            string byte2String = null;
            //Chuyển đổi mảng byte kết quả thành một chuỗi hexa (byte2String) bằng cách duyệt qua từng phần tử trong mảng và chuyển đổi nó thành một chuỗi hexa.
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
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
